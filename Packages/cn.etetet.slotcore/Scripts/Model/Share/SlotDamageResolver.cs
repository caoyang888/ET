namespace ET
{
    [EnableClass]
    public static class SlotDamageResolver
    {
        public static long Resolve(SlotCatalog catalog, SlotStakeDef stake, SlotStageDef stage, long payout, long totalStake, bool freeSpin)
        {
            long raw = payout * (stake.WinFactor <= 0 ? 1 : stake.WinFactor) / 100;
            if (payout < totalStake && raw < stake.MinDamage)
            {
                raw = stake.MinDamage;
            }

            long cap = long.MaxValue;
            int micro = freeSpin && stage.FreeDamageCapMicro > 0 ? stage.FreeDamageCapMicro : stage.DamageCapMicro;
            if (micro > 0 && stage.HpMax > 0)
            {
                long hpCap = stage.HpMax * micro / 1000000;
                if (hpCap < cap)
                {
                    cap = hpCap;
                }
            }

            if (catalog.Rule.LimitDamageRate > 0 && totalStake > 0)
            {
                long stakeCap = totalStake * catalog.Rule.LimitDamageRate;
                if (stakeCap < cap)
                {
                    cap = stakeCap;
                }
            }

            if (raw > cap)
            {
                raw = cap;
            }

            if (raw < 0)
            {
                return 0;
            }

            return raw;
        }
    }
}
