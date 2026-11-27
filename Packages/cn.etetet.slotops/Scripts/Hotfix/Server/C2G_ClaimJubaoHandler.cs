namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ClaimJubaoHandler : MessageSessionHandler<C2G_ClaimJubao, G2C_ClaimJubao>
    {
        protected override async ETTask Run(Session session, C2G_ClaimJubao request, G2C_ClaimJubao response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 4, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
