using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 命中率 = 基础命中率 + 各命名策略的加值，最后夹紧到 [0,1]。
    /// </summary>
    [EnableClass]
    public static class SlotRtpComposer
    {
        public static double Compose(SlotCatalog catalog, SlotPlayerState player, SlotRoomDef room, int lineCount, long publicStock, SlotRng rng)
        {
            double chance = BaseRate(catalog, room.MachineId, lineCount);
            long gold = player.Get(catalog.Rule.GoldItemId);
            double rtp = player.TotalBet <= 0 ? 0 : (double)player.TotalWin / player.TotalBet;
            chance += Sum(catalog, "NewBalance", gold, player.TotalDamage, rng);
            chance += Sum(catalog, "PersonalRtp", Milli(rtp), 0, rng);
            chance += Sum(catalog, "Recharge", player.RechargeCents, 0, rng);
            chance += Sum(catalog, "PublicStock", publicStock, 0, rng);
            long ratioMilli = player.TotalBet <= 0 ? 0 : player.TotalDamage * 1000 / player.TotalBet;
            chance += Sum(catalog, "Overflow", player.RechargeCents, ratioMilli, rng);
            if (chance < 0)
            {
                return 0;
            }

            if (chance > 1)
            {
                return 1;
            }

            return chance;
        }

        public static double BaseRate(SlotCatalog catalog, int machineId, int lineCount)
        {
            SlotHitChanceDef found = null;
            foreach (SlotHitChanceDef row in catalog.HitChances)
            {
                if (row.MachineId != machineId)
                {
                    continue;
                }

                if (row.LineCount == lineCount)
                {
                    return row.WinRate;
                }

                if (found == null || row.LineCount < lineCount)
                {
                    found = row;
                }
            }

            return found == null ? 0 : found.WinRate;
        }

        private static double Sum(SlotCatalog catalog, string policy, long axis, long band, SlotRng rng)
        {
            double total = 0;
            foreach (SlotPolicyBand row in catalog.Policies)
            {
                if (row.Policy != policy)
                {
                    continue;
                }

                if (!Contains(row.AxisMin, row.AxisMax, axis))
                {
                    continue;
                }

                if (row.BandMax != 0 || row.BandMin != 0)
                {
                    if (!Contains(row.BandMin, row.BandMax, band))
                    {
                        continue;
                    }
                }

                if (row.GateMax != 0 || row.GateMin != 0)
                {
                    int roll = rng.NextInt(10) + 1;
                    total += roll >= row.GateMin && roll <= row.GateMax ? row.Delta : row.AlternateDelta;
                    continue;
                }

                total += row.Delta;
            }

            return total;
        }

        private static bool Contains(long min, long max, long value)
        {
            if (min >= 0 && value < min)
            {
                return false;
            }

            if (max >= 0 && value > max)
            {
                return false;
            }

            return true;
        }

        private static long Milli(double value)
        {
            if (value < 0)
            {
                return 0;
            }

            return (long)(value * 1000);
        }
    }
}
