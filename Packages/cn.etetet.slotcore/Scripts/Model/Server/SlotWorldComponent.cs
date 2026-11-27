namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public sealed class SlotWorldComponent : Entity, IAwake
    {
        public SlotCatalog Catalog = new();
        public SlotRoomLedgerBook Ledgers = new();
        public SlotSpinStore Spins = new();
        public bool Maintenance;
    }
}
