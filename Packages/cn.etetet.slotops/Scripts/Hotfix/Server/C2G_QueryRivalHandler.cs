namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_QueryRivalHandler : MessageSessionHandler<C2G_QueryRival, G2C_QueryRival>
    {
        protected override async ETTask Run(Session session, C2G_QueryRival request, G2C_QueryRival response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 15, request.Period, request.Place, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.RivalDamage = reply.Amount;
        }
    }
}
