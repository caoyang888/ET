namespace ET.Server
{
    [MessageHandler(SceneType.Slot)]
    public class G2Slot_MaintenanceHandler : MessageHandler<Scene, G2Slot_Maintenance, Slot2G_Maintenance>
    {
        protected override async ETTask Run(Scene scene, G2Slot_Maintenance request, Slot2G_Maintenance response)
        {
            scene.GetComponent<SlotWorldComponent>().Maintenance = request.Enabled != 0;
            await ETTask.CompletedTask;
        }
    }
}
