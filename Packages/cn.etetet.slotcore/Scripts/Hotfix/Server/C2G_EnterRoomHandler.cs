namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_EnterRoomHandler : MessageSessionHandler<C2G_EnterRoom, G2C_EnterRoom>
    {
        protected override async ETTask Run(Session session, C2G_EnterRoom request, G2C_EnterRoom response)
        {
            await SlotGateCall.Room(session, request.RoomId, response);
        }
    }
}
