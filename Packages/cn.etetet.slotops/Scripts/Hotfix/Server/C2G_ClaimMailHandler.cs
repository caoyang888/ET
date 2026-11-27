namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ClaimMailHandler : MessageSessionHandler<C2G_ClaimMail, G2C_ClaimMail>
    {
        protected override async ETTask Run(Session session, C2G_ClaimMail request, G2C_ClaimMail response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 8, 0, 0, request.MailId, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
