namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_SetMaintenanceHandler : MessageSessionHandler<C2G_SetMaintenance, G2C_SetMaintenance>
    {
        protected override async ETTask Run(Session session, C2G_SetMaintenance request, G2C_SetMaintenance response)
        {
            Fiber fiber = session.Fiber();
            StartSceneConfig slot = fiber.GetSingleton<StartSceneConfigCategory>().GetOneBySceneType(session.Zone(), SceneType.Slot);
            G2Slot_Maintenance inner = G2Slot_Maintenance.Create();
            inner.Enabled = request.Enabled;
            Slot2G_Maintenance reply = (Slot2G_Maintenance)await session.Root().GetComponent<MessageSender>().Call(slot.GetActorId(fiber), inner);
            response.Error = reply.Error;
            response.Message = reply.Message;
        }
    }
}
