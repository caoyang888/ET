namespace ET.Server
{
    public static class SlotPlayerFactory
    {
        public static async ETTask<SlotPlayer> GetOrCreate(Scene scene, long playerId)
        {
            SlotPlayerComponent players = scene.GetComponent<SlotPlayerComponent>();
            SlotPlayer current = players.Get(playerId);
            if (current != null)
            {
                return current;
            }

            SlotPlayer created = players.AddChildWithId<SlotPlayer, long>(playerId, playerId);
            created.AddComponent<MailBoxComponent, int>(MailBoxType.OrderedMessage);
            players.Players.Add(playerId, created);
            EntityRef<SlotPlayer> createdRef = created;
            await scene.GetComponent<LocationProxyComponent>().Add(LocationType.SlotPlayer, playerId, created.GetActorId());
            return createdRef;
        }
    }
}
