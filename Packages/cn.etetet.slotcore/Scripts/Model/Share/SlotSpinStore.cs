using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public sealed class SlotSpinStore
    {
        private readonly object gate = new object();
        private readonly Dictionary<string, SlotRoundResult> rounds = new Dictionary<string, SlotRoundResult>();

        public bool TryGet(long playerId, string clientSpinId, out SlotRoundResult result)
        {
            lock (this.gate)
            {
                return this.rounds.TryGetValue(Key(playerId, clientSpinId), out result);
            }
        }

        public void Save(SlotRoundResult result)
        {
            lock (this.gate)
            {
                this.rounds[Key(result.PlayerId, result.ClientSpinId)] = result;
            }
        }

        private static string Key(long playerId, string clientSpinId)
        {
            return playerId + ":" + clientSpinId;
        }
    }
}
