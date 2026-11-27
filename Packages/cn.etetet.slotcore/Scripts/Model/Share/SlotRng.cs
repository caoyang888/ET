using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotRng
    {
        private ulong state;

        public SlotRng(long seed)
        {
            this.state = seed == 0 ? 0x9E3779B97F4A7C15UL : (ulong)seed;
        }

        public int NextInt(int exclusiveMax)
        {
            if (exclusiveMax <= 1)
            {
                return 0;
            }

            return (int)(this.NextUlong() % (ulong)exclusiveMax);
        }

        public double NextDouble()
        {
            return (this.NextUlong() >> 11) * (1.0 / (1UL << 53));
        }

        public int Pick(IReadOnlyList<int> weights)
        {
            int total = 0;
            for (int i = 0; i < weights.Count; ++i)
            {
                if (weights[i] > 0)
                {
                    total += weights[i];
                }
            }

            if (total <= 0)
            {
                return 0;
            }

            int roll = this.NextInt(total);
            int cursor = 0;
            for (int i = 0; i < weights.Count; ++i)
            {
                if (weights[i] <= 0)
                {
                    continue;
                }

                cursor += weights[i];
                if (roll < cursor)
                {
                    return i;
                }
            }

            return weights.Count - 1;
        }

        private ulong NextUlong()
        {
            ulong x = this.state;
            x ^= x >> 12;
            x ^= x << 25;
            x ^= x >> 27;
            this.state = x;
            return x * 0x2545F4914F6CDD1DUL;
        }
    }
}
