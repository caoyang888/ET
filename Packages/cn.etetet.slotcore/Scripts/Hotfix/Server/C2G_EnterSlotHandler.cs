namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_EnterSlotHandler : MessageSessionHandler<C2G_EnterSlot, G2C_EnterSlot>
    {
        protected override async ETTask Run(Session session, C2G_EnterSlot request, G2C_EnterSlot response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            long playerId = player.Id;
            Fiber fiber = session.Fiber();
            StartSceneConfig slot = fiber.GetSingleton<StartSceneConfigCategory>().GetOneBySceneType(session.Zone(), SceneType.Slot);
            G2Slot_Enter enter = G2Slot_Enter.Create();
            enter.PlayerId = playerId;
            Slot2G_Enter reply = (Slot2G_Enter)await session.Root().GetComponent<MessageSender>().Call(slot.GetActorId(fiber), enter);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.PlayerId = playerId;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
