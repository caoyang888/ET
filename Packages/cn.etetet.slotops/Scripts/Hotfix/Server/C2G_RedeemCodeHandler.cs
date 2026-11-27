namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_RedeemCodeHandler : MessageSessionHandler<C2G_RedeemCode, G2C_RedeemCode>
    {
        protected override async ETTask Run(Session session, C2G_RedeemCode request, G2C_RedeemCode response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 9, 0, 0, 0, request.Code, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
