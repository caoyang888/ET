namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ClaimPiggyHandler : MessageSessionHandler<C2G_ClaimPiggy, G2C_ClaimPiggy>
    {
        protected override async ETTask Run(Session session, C2G_ClaimPiggy request, G2C_ClaimPiggy response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 10, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
