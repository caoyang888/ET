using System;
using System.Collections.Generic;
using System.IO;
using ET.Slot.Postgres;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ET.SlotTests
{
    [TestClass]
    public class SlotPostgresTests
    {
        [TestMethod]
        public void Schema_Save_Reload_AndSpinIdempotency_SurviveNewConnection()
        {
            string root = FindRepoRoot();
            string connection = SlotPostgresSettings.Resolve(root);
            string schema = Path.Combine(root, "Config", "Sql", "slot_postgres_schema.sql");
            SlotPostgresDatabase database = new SlotPostgresDatabase(connection, schema);
            database.EnsureSchema();
            string account = "it-" + Guid.NewGuid().ToString("N");
            SlotSnapshot snapshot = new SlotSnapshot();
            snapshot.Account = new SlotAccountRow
            {
                Account = account,
                PlayerId = 42,
                Nickname = "pilot",
                Avatar = "a",
                WalletJson = "{\"2\":1000}",
                VipExp = 30,
            };
            snapshot.Progress.Add(new SlotProgressRow
            {
                ProgressGroup = 1,
                StageId = 1001,
                BossHp = 40000,
                TotalBet = 300,
                TotalDamage = 9,
            });
            snapshot.Ops = new SlotOpsRow
            {
                JubaoLevel = 1,
                DiceCell = 4,
                Piggy = 15,
                TaskJson = "{\"1\":9}",
                ClaimJson = "[]",
                KingJson = "[]",
                LapJson = "[]",
                RedeemJson = "[]",
            };
            SlotSpinRow spin = new SlotSpinRow
            {
                ClientSpinId = "spin-1",
                PlayerId = 42,
                RoomId = 101,
                StakeId = 10101,
                Bet = 300,
                Win = 0,
                Damage = 9,
                Seed = 11,
                ResultJson = "{\"payout\":0}",
                CreatedUnix = 1,
            };
            database.Save(snapshot, spin, new List<SlotLedgerRow>
            {
                new SlotLedgerRow { RoomId = 101, PublicStock = 270, RankPool = 12, JackpotPool = 0, Version = 1 },
            }, new Dictionary<int, int>(), false);

            SlotPostgresDatabase restarted = new SlotPostgresDatabase(connection, schema);
            SlotSnapshot loaded = restarted.Load(account);
            Assert.AreEqual("pilot", loaded.Account.Nickname);
            Assert.AreEqual(30, loaded.Account.VipExp);
            Assert.AreEqual(1001, loaded.Progress[0].StageId);
            Assert.AreEqual(15, loaded.Ops.Piggy);
            SlotSpinRow again = restarted.FindSpin(account, "spin-1");
            Assert.AreEqual(300, again.Bet);
            Assert.IsTrue(again.ResultJson.Contains("payout"));
            List<SlotLedgerRow> ledgers = restarted.LoadLedgers();
            bool found = false;
            foreach (SlotLedgerRow row in ledgers)
            {
                if (row.RoomId == 101)
                {
                    found = true;
                    Assert.AreEqual(270, row.PublicStock);
                    Assert.AreEqual(12, row.RankPool);
                }
            }

            Assert.IsTrue(found);
        }

        private static string FindRepoRoot()
        {
            DirectoryInfo dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "Config", "postgres.json")))
                {
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            return Directory.GetCurrentDirectory();
        }
    }
}
