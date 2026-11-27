namespace ET.Server
{
    public static class SlotGateCall
    {
        public static async ETTask Room(Session session, int roomId, G2C_EnterRoom response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            long playerId = player.Id;
            G2Slot_Room inner = G2Slot_Room.Create();
            inner.RoomId = roomId;
            Slot2G_Room reply = (Slot2G_Room)await session.Root().GetComponent<MessageLocationSenderComponent>().Get(LocationType.SlotPlayer).Call(playerId, inner);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.RoomId = reply.RoomId;
            response.StageId = reply.StageId;
            response.BossHp = reply.BossHp;
            response.BossHpMax = reply.BossHpMax;
            response.FreeSpins = reply.FreeSpins;
            response.PublicStock = reply.PublicStock;
            response.RankPool = reply.RankPool;
            response.JackpotPool = reply.JackpotPool;
            response.Stakes.AddRange(reply.Stakes);
        }
    }
}
