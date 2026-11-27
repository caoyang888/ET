namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_DevPayHandler : MessageSessionHandler<C2G_DevPay, G2C_DevPay>
    {
        protected override async ETTask Run(Session session, C2G_DevPay request, G2C_DevPay response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 3, 0, 0, 0, request.OrderId, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.VipLevel = reply.Value;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
