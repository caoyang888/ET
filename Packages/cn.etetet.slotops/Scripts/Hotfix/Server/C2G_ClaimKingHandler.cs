namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ClaimKingHandler : MessageSessionHandler<C2G_ClaimKing, G2C_ClaimKing>
    {
        protected override async ETTask Run(Session session, C2G_ClaimKing request, G2C_ClaimKing response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 7, request.TierId, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
