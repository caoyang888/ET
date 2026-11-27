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

            if (player.GateInstance != 0 && player.GateInstance != request.GateInstance)
            {
                SlotPush.Kick(player, 1, "账号在其他地方登录");
            }

            player.Account = string.IsNullOrEmpty(request.Account) ? player.Id.ToString() : request.Account;
            player.GateIp = request.GateIp ?? string.Empty;
            player.GatePort = request.GatePort;
            player.GateFiber = request.GateFiber;
            player.GateInstance = request.GateInstance;
            SlotWorldComponent slotWorld = scene.GetComponent<SlotWorldComponent>();
            if (player.Spin.Wallet.Count == 0)
            {
                SlotPersistence.LoadOrCreate(slotWorld, player, slotWorld.Catalog.Rule.NewPlayerJade);
            }

            if (string.IsNullOrEmpty(player.Nickname))
            {
                player.Nickname = string.IsNullOrEmpty(request.Nickname) ? player.Account : request.Nickname;
            }

            SlotProtoMap.FillWallet(response.Wallet, player.Spin);
        }
    }
}
