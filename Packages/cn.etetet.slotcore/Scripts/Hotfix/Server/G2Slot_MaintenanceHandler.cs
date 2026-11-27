namespace ET.Server
{
    [MessageHandler(SceneType.Slot)]
    public class G2Slot_MaintenanceHandler : MessageHandler<Scene, G2Slot_Maintenance, Slot2G_Maintenance>
    {
        protected override async ETTask Run(Scene scene, G2Slot_Maintenance request, Slot2G_Maintenance response)
        {
            SlotWorldComponent world = scene.GetComponent<SlotWorldComponent>();
            world.Maintenance = request.Enabled != 0;
            if (world.Database != null)
            {
                world.Database.SetMaintenance(world.Maintenance);
            }

            SlotPlayerComponent players = scene.GetComponent<SlotPlayerComponent>();
            foreach (EntityRef<SlotPlayer> slotRef in players.Players.Values)
            {
                SlotPlayer slotPlayer = slotRef;
                if (slotPlayer == null)
                {
                    continue;
                }

                SlotPush.Notice(slotPlayer, 2, "维护", world.Maintenance ? "服务器维护中" : "维护结束");
            }

            await ETTask.CompletedTask;
        }
    }
}
