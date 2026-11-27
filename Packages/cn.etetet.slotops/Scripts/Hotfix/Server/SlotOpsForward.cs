namespace ET.Server
{
    public static class SlotOpsForward
    {
        public static async ETTask<Slot2G_Ops> Call(Session session, int action, int arg1, int arg2, long arg3, string text, string text2)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            G2Slot_Ops request = G2Slot_Ops.Create();
            request.Action = action;
            request.Arg1 = arg1;
            request.Arg2 = arg2;
            request.Arg3 = arg3;
            request.Text = text ?? string.Empty;
            request.Text2 = text2 ?? string.Empty;
            return (Slot2G_Ops)await session.Root().GetComponent<MessageLocationSenderComponent>().Get(LocationType.SlotPlayer).Call(player.Id, request);
        }
    }
}
