namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_GetRoomSnapshotHandler : MessageSessionHandler<C2G_GetRoomSnapshot, G2C_GetRoomSnapshot>
    {
        protected override async ETTask Run(Session session, C2G_GetRoomSnapshot request, G2C_GetRoomSnapshot response)
        {
            G2C_EnterRoom room = G2C_EnterRoom.Create();
            await SlotGateCall.Room(session, request.RoomId, room);
            response.Error = room.Error;
            response.Message = room.Message;
            response.RoomId = room.RoomId;
            response.StageId = room.StageId;
            response.BossHp = room.BossHp;
            response.BossHpMax = room.BossHpMax;
            response.FreeSpins = room.FreeSpins;
            response.PublicStock = room.PublicStock;
            response.RankPool = room.RankPool;
            response.JackpotPool = room.JackpotPool;
            response.Stakes.AddRange(room.Stakes);
        }
    }
}
