namespace ET.Server
{
    [MessageHandler(SceneType.Slot)]
    public class G2Slot_RoomHandler : MessageLocationHandler<SlotPlayer, G2Slot_Room, Slot2G_Room>
    {
        protected override async ETTask Run(SlotPlayer player, G2Slot_Room request, Slot2G_Room response)
        {
            response.Error = SlotRoomView.FillRoom(player, request.RoomId, response);
            await ETTask.CompletedTask;
        }
    }
}
