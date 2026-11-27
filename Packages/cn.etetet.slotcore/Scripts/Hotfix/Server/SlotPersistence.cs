using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ET.Slot.Postgres;

namespace ET.Server
{
    public static class SlotPersistence
    {
        public static void Open(SlotWorldComponent world)
        {
            string connection = SlotPostgresSettings.Resolve(Environment.CurrentDirectory);
            string schema = Path.Combine(Environment.CurrentDirectory, "Config", "Sql", "slot_postgres_schema.sql");
            if (string.IsNullOrEmpty(connection) || !File.Exists(schema))
            {
                Log.Warning("PostgreSQL 未配置，Slot 不会落库");
                return;
            }

            world.Database = new SlotPostgresDatabase(connection, schema);
            world.Database.EnsureSchema();
            world.Maintenance = world.Database.LoadMaintenance();
            foreach (SlotLedgerRow row in world.Database.LoadLedgers())
            {
                world.Ledgers.Restore(row.RoomId, row.PublicStock, row.RankPool, row.JackpotPool, row.Version);
            }
        }

        public static void LoadOrCreate(SlotWorldComponent world, SlotPlayer player, long jade)
        {
            if (world.Database == null)
            {
                player.Spin.Add(world.Catalog.Rule.JadeItemId, jade);
                player.Nickname = player.Account;
                return;
            }

            SlotSnapshot snapshot = world.Database.Load(player.Account);
            if (snapshot == null || snapshot.Account == null)
            {
                player.Spin.Add(world.Catalog.Rule.JadeItemId, jade);
                player.Nickname = player.Account;
                Save(world, player, null, null);
                return;
            }

            player.Nickname = snapshot.Account.Nickname;
            player.Avatar = snapshot.Account.Avatar;
            player.Spin.Wallet.Clear();
            foreach (KeyValuePair<int, long> pair in ReadWallet(snapshot.Account.WalletJson))
            {
                player.Spin.Wallet[pair.Key] = pair.Value;
            }

            bool totals = false;
            foreach (SlotProgressRow row in snapshot.Progress)
            {
                player.Spin.Progress[row.ProgressGroup] = new SlotProgressState
                {
                    StageId = row.StageId,
                    BossHp = row.BossHp,
                    Cleared = row.Cleared,
                    FreeSpins = row.FreeSpins,
                    LockedStakeId = row.LockedStakeId,
                    LockedLineCount = row.LockedLineCount,
                    MissStreak = row.MissStreak,
                    PityCooldown = row.PityCooldown,
                };
                if (!totals)
                {
                    player.Spin.TotalBet = row.TotalBet;
                    player.Spin.TotalWin = row.TotalWin;
                    player.Spin.TotalDamage = row.TotalDamage;
                    player.Spin.RechargeCents = row.RechargeCents;
                    player.Spin.TotalCost = row.TotalCost;
                    totals = true;
                }
            }
        }

        public static SlotRoundResult FindSpin(SlotWorldComponent world, string account, string spinId)
        {
            if (world.Database == null || string.IsNullOrEmpty(spinId))
            {
                return null;
            }

            SlotSpinRow row = world.Database.FindSpin(account, spinId);
            if (row == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<SlotRoundResult>(row.ResultJson, SlotJson.Options());
        }

        public static void Save(SlotWorldComponent world, SlotPlayer player, SlotRoundResult result, Dictionary<int, int> kingQuota)
        {
            if (world.Database == null)
            {
                return;
            }

            SlotSnapshot snapshot = new SlotSnapshot();
            snapshot.Account = new SlotAccountRow
            {
                Account = player.Account,
                PlayerId = player.Id,
                Nickname = string.IsNullOrEmpty(player.Nickname) ? player.Account : player.Nickname,
                Avatar = player.Avatar ?? string.Empty,
                WalletJson = JsonSerializer.Serialize(player.Spin.Wallet, SlotJson.Options()),
                VipExp = 0,
            };
            foreach (KeyValuePair<int, SlotProgressState> pair in player.Spin.Progress)
            {
                snapshot.Progress.Add(new SlotProgressRow
                {
                    ProgressGroup = pair.Key,
                    StageId = pair.Value.StageId,
                    BossHp = pair.Value.BossHp,
                    Cleared = pair.Value.Cleared,
                    FreeSpins = pair.Value.FreeSpins,
                    LockedStakeId = pair.Value.LockedStakeId,
                    LockedLineCount = pair.Value.LockedLineCount,
                    MissStreak = pair.Value.MissStreak,
                    PityCooldown = pair.Value.PityCooldown,
                    TotalBet = player.Spin.TotalBet,
                    TotalWin = player.Spin.TotalWin,
                    TotalDamage = player.Spin.TotalDamage,
                    RechargeCents = player.Spin.RechargeCents,
                    TotalCost = player.Spin.TotalCost,
                    ClearCount = pair.Value.Cleared ? 1 : 0,
                });
            }

            SlotSpinRow spin = null;
            if (result != null && result.Error == SlotFail.Ok && !string.IsNullOrEmpty(result.ClientSpinId))
            {
                spin = new SlotSpinRow
                {
                    ClientSpinId = result.ClientSpinId,
                    PlayerId = player.Id,
                    RoomId = result.RoomId,
                    StakeId = result.StakeId,
                    Bet = result.StakeGold,
                    Win = result.Payout,
                    Damage = result.Damage,
                    Seed = result.Seed,
                    ResultJson = JsonSerializer.Serialize(result, SlotJson.Options()),
                    CreatedUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                };
            }

            List<SlotLedgerRow> ledgers = new List<SlotLedgerRow>();
            foreach (SlotRoomDef room in world.Catalog.Rooms.Values)
            {
                SlotRoomLedger ledger = world.Ledgers.Snapshot(room.RoomId);
                ledgers.Add(new SlotLedgerRow
                {
                    RoomId = room.RoomId,
                    PublicStock = ledger.PublicStock,
                    RankPool = ledger.RankPool,
                    JackpotPool = ledger.Jackpot,
                    Version = ledger.Version,
                });
            }

            world.Database.Save(snapshot, spin, ledgers, kingQuota ?? new Dictionary<int, int>(), world.Maintenance);
        }

        private static Dictionary<int, long> ReadWallet(string json)
        {
            Dictionary<int, long> wallet = new Dictionary<int, long>();
            if (string.IsNullOrEmpty(json))
            {
                return wallet;
            }

            Dictionary<string, long> raw = JsonSerializer.Deserialize<Dictionary<string, long>>(json, SlotJson.Options());
            if (raw == null)
            {
                return wallet;
            }

            foreach (KeyValuePair<string, long> pair in raw)
            {
                wallet[int.Parse(pair.Key)] = pair.Value;
            }

            return wallet;
        }
    }
}
