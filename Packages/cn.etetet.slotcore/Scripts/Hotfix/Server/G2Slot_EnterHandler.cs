namespace ET.Server
{
    [MessageHandler(SceneType.Slot)]
    public class G2Slot_EnterHandler : MessageHandler<Scene, G2Slot_Enter, Slot2G_Enter>
    {
        protected override async ETTask Run(Scene scene, G2Slot_Enter request, Slot2G_Enter response)
        {
            EntityRef<Scene> sceneRef = scene;
            SlotWorldComponent world = scene.GetComponent<SlotWorldComponent>();
            if (world.Maintenance)
            {
                response.Error = ErrorCode.ERR_SlotMaintenance;
                return;
            }

            await SlotPlayerFactory.GetOrCreate(scene, request.PlayerId);
            scene = sceneRef;
            SlotPlayer player = scene.GetComponent<SlotPlayerComponent>().Get(request.PlayerId);
            if (player == null)
            {
                response.Error = ErrorCode.ERR_SlotPlayerMissing;
                return;
            }

            SlotProtoMap.FillWallet(response.Wallet, player.Spin);
        }
    }
}
