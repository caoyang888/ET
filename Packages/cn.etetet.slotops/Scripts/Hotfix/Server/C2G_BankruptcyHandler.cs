namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_BankruptcyHandler : MessageSessionHandler<C2G_Bankruptcy, G2C_Bankruptcy>
    {
        protected override async ETTask Run(Session session, C2G_Bankruptcy request, G2C_Bankruptcy response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 11, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
