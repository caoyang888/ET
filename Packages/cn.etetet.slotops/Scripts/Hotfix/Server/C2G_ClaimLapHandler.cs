namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ClaimLapHandler : MessageSessionHandler<C2G_ClaimLap, G2C_ClaimLap>
    {
        protected override async ETTask Run(Session session, C2G_ClaimLap request, G2C_ClaimLap response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 6, request.Laps, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
