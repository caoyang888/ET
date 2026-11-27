namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_EnterSlotHandler : MessageSessionHandler<C2G_EnterSlot, G2C_EnterSlot>
    {
        protected override async ETTask Run(Session session, C2G_EnterSlot request, G2C_EnterSlot response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            long playerId = player.Id;
            string account = player.Account ?? string.Empty;
            ActorId gateActor = player.GetComponent<PlayerSessionComponent>().GetActorId();
            Fiber fiber = session.Fiber();
            StartSceneConfig slot = fiber.GetSingleton<StartSceneConfigCategory>().GetOneBySceneType(session.Zone(), SceneType.Slot);
            G2Slot_Enter enter = G2Slot_Enter.Create();
            enter.PlayerId = playerId;
            enter.Account = account;
            enter.Nickname = account;
            enter.GateIp = gateActor.Address.IP ?? string.Empty;
            enter.GatePort = gateActor.Address.Port;
            enter.GateFiber = gateActor.FiberInstanceId.Fiber;
            enter.GateInstance = gateActor.FiberInstanceId.InstanceId;
            Slot2G_Enter reply = (Slot2G_Enter)await session.Root().GetComponent<MessageSender>().Call(slot.GetActorId(fiber), enter);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.PlayerId = playerId;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
