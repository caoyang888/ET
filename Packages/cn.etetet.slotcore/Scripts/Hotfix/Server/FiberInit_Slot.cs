namespace ET.Server
{
    [Invoke(SceneType.Slot)]
    public class FiberInit_Slot : AInvokeHandler<FiberInit, ETTask>
    {
        public override async ETTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, int>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ProcessInnerSender>();
            root.AddComponent<MessageSender>();
            root.AddComponent<LocationProxyComponent>();
            root.AddComponent<MessageLocationSenderComponent>();
            root.AddComponent<SlotPlayerComponent>();
            SlotWorldComponent world = root.AddComponent<SlotWorldComponent>();
            world.Catalog = SlotCatalogLoader.Load(root.Fiber());
            world.Ledgers = new SlotRoomLedgerBook();
            SlotPersistence.Open(world);
            foreach (SlotRoomDef room in world.Catalog.Rooms.Values)
            {
                world.Ledgers.GetOrCreate(room.RoomId, room.InitialPublicStock);
            }

            await ETTask.CompletedTask;
        }
    }
}
