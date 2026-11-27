using System;
using System.Collections.Generic;
using System.IO;
using Npgsql;

namespace ET.Slot.Postgres
{
    public sealed class SlotPostgresDatabase
    {
        private readonly string connectionString;
        private readonly string schemaSql;

        public SlotPostgresDatabase(string connectionString, string schemaPath)
        {
            this.connectionString = connectionString;
            this.schemaSql = File.ReadAllText(schemaPath);
        }

        public void EnsureSchema()
        {
            using NpgsqlConnection connection = this.Open();
            using NpgsqlCommand command = new NpgsqlCommand(this.schemaSql, connection);
            command.ExecuteNonQuery();
        }

        public SlotSnapshot Load(string account)
        {
            using NpgsqlConnection connection = this.Open();
            SlotSnapshot snapshot = new SlotSnapshot();
            using (NpgsqlCommand command = new NpgsqlCommand("SELECT player_id, nickname, avatar, wallet_json::text, vip_exp, real_name_verified, phone_bound FROM slot_account WHERE account = @account", connection))
            {
                command.Parameters.AddWithValue("account", account);
                using NpgsqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                snapshot.Account = new SlotAccountRow
                {
                    Account = account,
                    PlayerId = reader.GetInt64(0),
                    Nickname = reader.GetString(1),
                    Avatar = reader.GetString(2),
                    WalletJson = reader.GetString(3),
                    VipExp = reader.GetInt32(4),
                    RealNameVerified = reader.GetBoolean(5),
                    PhoneBound = reader.GetBoolean(6),
                };
            }

            using (NpgsqlCommand command = new NpgsqlCommand("SELECT progress_group, stage_id, boss_hp, cleared, free_spins, locked_stake_id, locked_line_count, miss_streak, pity_cooldown, total_bet, total_win, total_damage, recharge_cents, total_cost, clear_count FROM slot_progress WHERE account = @account", connection))
            {
                command.Parameters.AddWithValue("account", account);
                using NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    snapshot.Progress.Add(new SlotProgressRow
                    {
                        ProgressGroup = reader.GetInt32(0),
                        StageId = reader.GetInt32(1),
                        BossHp = reader.GetInt64(2),
                        Cleared = reader.GetBoolean(3),
                        FreeSpins = reader.GetInt32(4),
                        LockedStakeId = reader.GetInt32(5),
                        LockedLineCount = reader.GetInt32(6),
                        MissStreak = reader.GetInt32(7),
                        PityCooldown = reader.GetInt32(8),
                        TotalBet = reader.GetInt64(9),
                        TotalWin = reader.GetInt64(10),
                        TotalDamage = reader.GetInt64(11),
                        RechargeCents = reader.GetInt32(12),
                        TotalCost = reader.GetInt64(13),
                        ClearCount = reader.GetInt32(14),
                    });
                }
            }

            snapshot.Ops = this.ReadOps(connection, account);
            snapshot.Orders = this.ReadOrders(connection, account);
            snapshot.Mails = this.ReadMails(connection, account);
            snapshot.Ranks = this.ReadRanks(connection, account);
            return snapshot;
        }

        public void Save(SlotSnapshot snapshot, SlotSpinRow spin, List<SlotLedgerRow> ledgers, Dictionary<int, int> kingQuota, bool? maintenance)
        {
            using NpgsqlConnection connection = this.Open();
            using NpgsqlTransaction tx = connection.BeginTransaction();
            this.UpsertAccount(connection, tx, snapshot.Account);
            foreach (SlotProgressRow row in snapshot.Progress)
            {
                this.UpsertProgress(connection, tx, snapshot.Account.Account, row);
            }

            this.UpsertOps(connection, tx, snapshot.Account.Account, snapshot.Ops);
            foreach (SlotOrderRow order in snapshot.Orders)
            {
                this.UpsertOrder(connection, tx, snapshot.Account.Account, order);
            }

            foreach (SlotRankRow rank in snapshot.Ranks)
            {
                this.UpsertRank(connection, tx, snapshot.Account.Account, rank);
            }

            if (spin != null)
            {
                this.InsertSpin(connection, tx, snapshot.Account.Account, spin);
            }

            foreach (SlotLedgerRow ledger in ledgers)
            {
                this.UpsertLedger(connection, tx, ledger);
            }

            foreach (KeyValuePair<int, int> pair in kingQuota)
            {
                using NpgsqlCommand command = new NpgsqlCommand("INSERT INTO slot_king_quota (tier_id, taken) VALUES (@tier, @taken) ON CONFLICT (tier_id) DO UPDATE SET taken = EXCLUDED.taken", connection, tx);
                command.Parameters.AddWithValue("tier", pair.Key);
                command.Parameters.AddWithValue("taken", pair.Value);
                command.ExecuteNonQuery();
            }

            if (maintenance.HasValue)
            {
                using NpgsqlCommand command = new NpgsqlCommand("INSERT INTO slot_maintenance (id, enabled) VALUES (1, @enabled) ON CONFLICT (id) DO UPDATE SET enabled = EXCLUDED.enabled", connection, tx);
                command.Parameters.AddWithValue("enabled", maintenance.Value);
                command.ExecuteNonQuery();
            }

            tx.Commit();
        }

        public SlotSpinRow FindSpin(string account, string clientSpinId)
        {
            using NpgsqlConnection connection = this.Open();
            using NpgsqlCommand command = new NpgsqlCommand("SELECT player_id, room_id, stake_id, bet, win, damage, seed, result_json::text, created_unix FROM slot_spin WHERE account = @account AND client_spin_id = @spin", connection);
            command.Parameters.AddWithValue("account", account);
            command.Parameters.AddWithValue("spin", clientSpinId);
            using NpgsqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new SlotSpinRow
            {
                ClientSpinId = clientSpinId,
                PlayerId = reader.GetInt64(0),
                RoomId = reader.GetInt32(1),
                StakeId = reader.GetInt32(2),
                Bet = reader.GetInt64(3),
                Win = reader.GetInt64(4),
                Damage = reader.GetInt64(5),
                Seed = reader.GetInt64(6),
                ResultJson = reader.GetString(7),
                CreatedUnix = reader.GetInt64(8),
            };
        }

        public List<SlotLedgerRow> LoadLedgers()
        {
            List<SlotLedgerRow> rows = new List<SlotLedgerRow>();
            using NpgsqlConnection connection = this.Open();
            using NpgsqlCommand command = new NpgsqlCommand("SELECT room_id, public_stock, rank_pool, jackpot_pool, version FROM slot_room_ledger", connection);
            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new SlotLedgerRow
                {
                    RoomId = reader.GetInt32(0),
                    PublicStock = reader.GetInt64(1),
                    RankPool = reader.GetInt64(2),
                    JackpotPool = reader.GetInt64(3),
                    Version = reader.GetInt64(4),
                });
            }

            return rows;
        }

        public Dictionary<int, int> LoadKingQuota()
        {
            Dictionary<int, int> quota = new Dictionary<int, int>();
            using NpgsqlConnection connection = this.Open();
            using NpgsqlCommand command = new NpgsqlCommand("SELECT tier_id, taken FROM slot_king_quota", connection);
            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                quota[reader.GetInt32(0)] = reader.GetInt32(1);
            }

            return quota;
        }

        public void SetMaintenance(bool enabled)
        {
            using NpgsqlConnection connection = this.Open();
            using NpgsqlCommand command = new NpgsqlCommand("INSERT INTO slot_maintenance (id, enabled) VALUES (1, @enabled) ON CONFLICT (id) DO UPDATE SET enabled = EXCLUDED.enabled", connection);
            command.Parameters.AddWithValue("enabled", enabled);
            command.ExecuteNonQuery();
        }

        public bool LoadMaintenance()
        {
            using NpgsqlConnection connection = this.Open();
            using NpgsqlCommand command = new NpgsqlCommand("SELECT enabled FROM slot_maintenance WHERE id = 1", connection);
            object value = command.ExecuteScalar();
            return value is bool enabled && enabled;
        }

        public void UpdateMail(string account, SlotMailRow mail)
        {
            using NpgsqlConnection connection = this.Open();
            using NpgsqlCommand command = new NpgsqlCommand("UPDATE slot_mail SET is_read = @read, is_claimed = @claimed, is_deleted = @deleted, attachment_json = @attachment::jsonb WHERE account = @account AND mail_id = @id", connection);
            command.Parameters.AddWithValue("read", mail.IsRead);
            command.Parameters.AddWithValue("claimed", mail.IsClaimed);
            command.Parameters.AddWithValue("deleted", mail.IsDeleted);
            command.Parameters.AddWithValue("attachment", string.IsNullOrEmpty(mail.AttachmentJson) ? "[]" : mail.AttachmentJson);
            command.Parameters.AddWithValue("account", account);
            command.Parameters.AddWithValue("id", mail.MailId);
            command.ExecuteNonQuery();
        }

        public long InsertMail(string account, SlotMailRow mail)
        {
            using NpgsqlConnection connection = this.Open();
            using NpgsqlCommand command = new NpgsqlCommand("INSERT INTO slot_mail (account, player_id, title, body, attachment_json, is_read, is_claimed, is_deleted, expire_unix) VALUES (@account, @player, @title, @body, @attachment::jsonb, @read, @claimed, FALSE, @expire) RETURNING mail_id", connection);
            command.Parameters.AddWithValue("account", account);
            command.Parameters.AddWithValue("player", mail.PlayerId);
            command.Parameters.AddWithValue("title", mail.Title ?? string.Empty);
            command.Parameters.AddWithValue("body", mail.Body ?? string.Empty);
            command.Parameters.AddWithValue("attachment", string.IsNullOrEmpty(mail.AttachmentJson) ? "[]" : mail.AttachmentJson);
            command.Parameters.AddWithValue("read", mail.IsRead);
            command.Parameters.AddWithValue("claimed", mail.IsClaimed);
            command.Parameters.AddWithValue("expire", mail.ExpireUnix);
            return (long)command.ExecuteScalar();
        }

        public List<SlotRankRow> ListRank(int period, int limit)
        {
            List<SlotRankRow> rows = new List<SlotRankRow>();
            using NpgsqlConnection connection = this.Open();
            using NpgsqlCommand command = new NpgsqlCommand("SELECT player_id, nickname, damage FROM slot_rank WHERE period = @period ORDER BY damage DESC LIMIT @limit", connection);
            command.Parameters.AddWithValue("period", period);
            command.Parameters.AddWithValue("limit", limit);
            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new SlotRankRow
                {
                    Period = period,
                    PlayerId = reader.GetInt64(0),
                    Nickname = reader.GetString(1),
                    Damage = reader.GetInt64(2),
                });
            }

            return rows;
        }

        private NpgsqlConnection Open()
        {
            NpgsqlConnection connection = new NpgsqlConnection(this.connectionString);
            connection.Open();
            return connection;
        }

        private void UpsertAccount(NpgsqlConnection connection, NpgsqlTransaction tx, SlotAccountRow row)
        {
            using NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO slot_account (account, player_id, nickname, avatar, wallet_json, vip_exp, real_name_verified, phone_bound, created_unix, updated_unix)
VALUES (@account, @player, @nickname, @avatar, @wallet::jsonb, @vip, @real, @phone, @now, @now)
ON CONFLICT (account) DO UPDATE SET player_id = EXCLUDED.player_id, nickname = EXCLUDED.nickname, avatar = EXCLUDED.avatar, wallet_json = EXCLUDED.wallet_json, vip_exp = EXCLUDED.vip_exp, real_name_verified = EXCLUDED.real_name_verified, phone_bound = EXCLUDED.phone_bound, updated_unix = EXCLUDED.updated_unix", connection, tx);
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            command.Parameters.AddWithValue("account", row.Account);
            command.Parameters.AddWithValue("player", row.PlayerId);
            command.Parameters.AddWithValue("nickname", row.Nickname ?? row.Account);
            command.Parameters.AddWithValue("avatar", row.Avatar ?? string.Empty);
            command.Parameters.AddWithValue("wallet", string.IsNullOrEmpty(row.WalletJson) ? "{}" : row.WalletJson);
            command.Parameters.AddWithValue("vip", row.VipExp);
            command.Parameters.AddWithValue("real", row.RealNameVerified);
            command.Parameters.AddWithValue("phone", row.PhoneBound);
            command.Parameters.AddWithValue("now", now);
            command.ExecuteNonQuery();
        }

        private void UpsertProgress(NpgsqlConnection connection, NpgsqlTransaction tx, string account, SlotProgressRow row)
        {
            using NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO slot_progress (account, progress_group, stage_id, boss_hp, cleared, free_spins, locked_stake_id, locked_line_count, miss_streak, pity_cooldown, total_bet, total_win, total_damage, recharge_cents, total_cost, clear_count)
VALUES (@account, @group, @stage, @hp, @cleared, @free, @stake, @lines, @miss, @pity, @bet, @win, @damage, @recharge, @cost, @clears)
ON CONFLICT (account, progress_group) DO UPDATE SET stage_id = EXCLUDED.stage_id, boss_hp = EXCLUDED.boss_hp, cleared = EXCLUDED.cleared, free_spins = EXCLUDED.free_spins, locked_stake_id = EXCLUDED.locked_stake_id, locked_line_count = EXCLUDED.locked_line_count, miss_streak = EXCLUDED.miss_streak, pity_cooldown = EXCLUDED.pity_cooldown, total_bet = EXCLUDED.total_bet, total_win = EXCLUDED.total_win, total_damage = EXCLUDED.total_damage, recharge_cents = EXCLUDED.recharge_cents, total_cost = EXCLUDED.total_cost, clear_count = EXCLUDED.clear_count", connection, tx);
            command.Parameters.AddWithValue("account", account);
            command.Parameters.AddWithValue("group", row.ProgressGroup);
            command.Parameters.AddWithValue("stage", row.StageId);
            command.Parameters.AddWithValue("hp", row.BossHp);
            command.Parameters.AddWithValue("cleared", row.Cleared);
            command.Parameters.AddWithValue("free", row.FreeSpins);
            command.Parameters.AddWithValue("stake", row.LockedStakeId);
            command.Parameters.AddWithValue("lines", row.LockedLineCount);
            command.Parameters.AddWithValue("miss", row.MissStreak);
            command.Parameters.AddWithValue("pity", row.PityCooldown);
            command.Parameters.AddWithValue("bet", row.TotalBet);
            command.Parameters.AddWithValue("win", row.TotalWin);
            command.Parameters.AddWithValue("damage", row.TotalDamage);
            command.Parameters.AddWithValue("recharge", row.RechargeCents);
            command.Parameters.AddWithValue("cost", row.TotalCost);
            command.Parameters.AddWithValue("clears", row.ClearCount);
            command.ExecuteNonQuery();
        }

        private void UpsertOps(NpgsqlConnection connection, NpgsqlTransaction tx, string account, SlotOpsRow row)
        {
            if (row == null)
            {
                return;
            }

            using NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO slot_ops (account, jubao_level, jubao_ready_unix, dice_cell, dice_laps, piggy, bankruptcy_claims, task_json, claim_json, king_json, lap_json, redeem_json, order_json)
VALUES (@account, @jubao, @ready, @cell, @laps, @piggy, @bank, @task::jsonb, @claim::jsonb, @king::jsonb, @lap::jsonb, @redeem::jsonb, '{}'::jsonb)
ON CONFLICT (account) DO UPDATE SET jubao_level = EXCLUDED.jubao_level, jubao_ready_unix = EXCLUDED.jubao_ready_unix, dice_cell = EXCLUDED.dice_cell, dice_laps = EXCLUDED.dice_laps, piggy = EXCLUDED.piggy, bankruptcy_claims = EXCLUDED.bankruptcy_claims, task_json = EXCLUDED.task_json, claim_json = EXCLUDED.claim_json, king_json = EXCLUDED.king_json, lap_json = EXCLUDED.lap_json, redeem_json = EXCLUDED.redeem_json", connection, tx);
            command.Parameters.AddWithValue("account", account);
            command.Parameters.AddWithValue("jubao", row.JubaoLevel);
            command.Parameters.AddWithValue("ready", row.JubaoReadyUnix);
            command.Parameters.AddWithValue("cell", row.DiceCell);
            command.Parameters.AddWithValue("laps", row.DiceLaps);
            command.Parameters.AddWithValue("piggy", row.Piggy);
            command.Parameters.AddWithValue("bank", row.BankruptcyClaims);
            command.Parameters.AddWithValue("task", string.IsNullOrEmpty(row.TaskJson) ? "{}" : row.TaskJson);
            command.Parameters.AddWithValue("claim", Empty(row.ClaimJson));
            command.Parameters.AddWithValue("king", Empty(row.KingJson));
            command.Parameters.AddWithValue("lap", Empty(row.LapJson));
            command.Parameters.AddWithValue("redeem", Empty(row.RedeemJson));
            command.ExecuteNonQuery();
        }

        private void UpsertOrder(NpgsqlConnection connection, NpgsqlTransaction tx, string account, SlotOrderRow row)
        {
            using NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO slot_order (order_id, account, player_id, product_id, amount, status, receipt, created_unix)
VALUES (@id, @account, @player, @product, @amount, @status, @receipt, @created)
ON CONFLICT (order_id) DO UPDATE SET status = EXCLUDED.status, receipt = EXCLUDED.receipt, amount = EXCLUDED.amount", connection, tx);
            command.Parameters.AddWithValue("id", row.OrderId);
            command.Parameters.AddWithValue("account", account);
            command.Parameters.AddWithValue("player", row.PlayerId);
            command.Parameters.AddWithValue("product", row.ProductId);
            command.Parameters.AddWithValue("amount", row.Amount);
            command.Parameters.AddWithValue("status", row.Status ?? "created");
            command.Parameters.AddWithValue("receipt", row.Receipt ?? string.Empty);
            command.Parameters.AddWithValue("created", row.CreatedUnix);
            command.ExecuteNonQuery();
        }

        private void UpsertRank(NpgsqlConnection connection, NpgsqlTransaction tx, string account, SlotRankRow row)
        {
            using NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO slot_rank (account, period, player_id, nickname, damage) VALUES (@account, @period, @player, @nickname, @damage)
ON CONFLICT (account, period) DO UPDATE SET player_id = EXCLUDED.player_id, nickname = EXCLUDED.nickname, damage = EXCLUDED.damage", connection, tx);
            command.Parameters.AddWithValue("account", account);
            command.Parameters.AddWithValue("period", row.Period);
            command.Parameters.AddWithValue("player", row.PlayerId);
            command.Parameters.AddWithValue("nickname", row.Nickname ?? account);
            command.Parameters.AddWithValue("damage", row.Damage);
            command.ExecuteNonQuery();
        }

        private void InsertSpin(NpgsqlConnection connection, NpgsqlTransaction tx, string account, SlotSpinRow row)
        {
            using NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO slot_spin (account, client_spin_id, player_id, room_id, stake_id, bet, win, damage, seed, result_json, created_unix)
VALUES (@account, @spin, @player, @room, @stake, @bet, @win, @damage, @seed, @result::jsonb, @created)
ON CONFLICT (account, client_spin_id) DO NOTHING", connection, tx);
            command.Parameters.AddWithValue("account", account);
            command.Parameters.AddWithValue("spin", row.ClientSpinId);
            command.Parameters.AddWithValue("player", row.PlayerId);
            command.Parameters.AddWithValue("room", row.RoomId);
            command.Parameters.AddWithValue("stake", row.StakeId);
            command.Parameters.AddWithValue("bet", row.Bet);
            command.Parameters.AddWithValue("win", row.Win);
            command.Parameters.AddWithValue("damage", row.Damage);
            command.Parameters.AddWithValue("seed", row.Seed);
            command.Parameters.AddWithValue("result", string.IsNullOrEmpty(row.ResultJson) ? "{}" : row.ResultJson);
            command.Parameters.AddWithValue("created", row.CreatedUnix);
            command.ExecuteNonQuery();
        }

        private void UpsertLedger(NpgsqlConnection connection, NpgsqlTransaction tx, SlotLedgerRow row)
        {
            using NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO slot_room_ledger (room_id, public_stock, rank_pool, jackpot_pool, version)
VALUES (@room, @stock, @rank, @jackpot, @version)
ON CONFLICT (room_id) DO UPDATE SET public_stock = EXCLUDED.public_stock, rank_pool = EXCLUDED.rank_pool, jackpot_pool = EXCLUDED.jackpot_pool, version = EXCLUDED.version", connection, tx);
            command.Parameters.AddWithValue("room", row.RoomId);
            command.Parameters.AddWithValue("stock", row.PublicStock);
            command.Parameters.AddWithValue("rank", row.RankPool);
            command.Parameters.AddWithValue("jackpot", row.JackpotPool);
            command.Parameters.AddWithValue("version", row.Version);
            command.ExecuteNonQuery();
        }

        private SlotOpsRow ReadOps(NpgsqlConnection connection, string account)
        {
            using NpgsqlCommand command = new NpgsqlCommand("SELECT jubao_level, jubao_ready_unix, dice_cell, dice_laps, piggy, bankruptcy_claims, task_json::text, claim_json::text, king_json::text, lap_json::text, redeem_json::text FROM slot_ops WHERE account = @account", connection);
            command.Parameters.AddWithValue("account", account);
            using NpgsqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return new SlotOpsRow
                {
                    JubaoLevel = 1,
                    DiceCell = 1,
                    TaskJson = "{}",
                    ClaimJson = "[]",
                    KingJson = "[]",
                    LapJson = "[]",
                    RedeemJson = "[]",
                };
            }

            return new SlotOpsRow
            {
                JubaoLevel = reader.GetInt32(0),
                JubaoReadyUnix = reader.GetInt64(1),
                DiceCell = reader.GetInt32(2),
                DiceLaps = reader.GetInt32(3),
                Piggy = reader.GetInt64(4),
                BankruptcyClaims = reader.GetInt32(5),
                TaskJson = reader.GetString(6),
                ClaimJson = reader.GetString(7),
                KingJson = reader.GetString(8),
                LapJson = reader.GetString(9),
                RedeemJson = reader.GetString(10),
            };
        }

        private List<SlotOrderRow> ReadOrders(NpgsqlConnection connection, string account)
        {
            List<SlotOrderRow> rows = new List<SlotOrderRow>();
            using NpgsqlCommand command = new NpgsqlCommand("SELECT order_id, player_id, product_id, amount, status, receipt, created_unix FROM slot_order WHERE account = @account ORDER BY created_unix", connection);
            command.Parameters.AddWithValue("account", account);
            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new SlotOrderRow
                {
                    OrderId = reader.GetString(0),
                    PlayerId = reader.GetInt64(1),
                    ProductId = reader.GetInt32(2),
                    Amount = reader.GetInt64(3),
                    Status = reader.GetString(4),
                    Receipt = reader.GetString(5),
                    CreatedUnix = reader.GetInt64(6),
                });
            }

            return rows;
        }

        private List<SlotMailRow> ReadMails(NpgsqlConnection connection, string account)
        {
            List<SlotMailRow> rows = new List<SlotMailRow>();
            using NpgsqlCommand command = new NpgsqlCommand("SELECT mail_id, player_id, title, body, attachment_json::text, is_read, is_claimed, is_deleted, expire_unix FROM slot_mail WHERE account = @account AND is_deleted = FALSE ORDER BY mail_id", connection);
            command.Parameters.AddWithValue("account", account);
            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new SlotMailRow
                {
                    MailId = reader.GetInt64(0),
                    PlayerId = reader.GetInt64(1),
                    Title = reader.GetString(2),
                    Body = reader.GetString(3),
                    AttachmentJson = reader.GetString(4),
                    IsRead = reader.GetBoolean(5),
                    IsClaimed = reader.GetBoolean(6),
                    IsDeleted = reader.GetBoolean(7),
                    ExpireUnix = reader.GetInt64(8),
                });
            }

            return rows;
        }

        private List<SlotRankRow> ReadRanks(NpgsqlConnection connection, string account)
        {
            List<SlotRankRow> rows = new List<SlotRankRow>();
            using NpgsqlCommand command = new NpgsqlCommand("SELECT period, player_id, nickname, damage FROM slot_rank WHERE account = @account", connection);
            command.Parameters.AddWithValue("account", account);
            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new SlotRankRow
                {
                    Period = reader.GetInt32(0),
                    PlayerId = reader.GetInt64(1),
                    Nickname = reader.GetString(2),
                    Damage = reader.GetInt64(3),
                });
            }

            return rows;
        }

        private static string Empty(string json)
        {
            return string.IsNullOrEmpty(json) ? "[]" : json;
        }
    }
}
