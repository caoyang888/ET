namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_RollDiceHandler : MessageSessionHandler<C2G_RollDice, G2C_RollDice>
    {
        protected override async ETTask Run(Session session, C2G_RollDice request, G2C_RollDice response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 5, request.Steps, request.Lucky, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Cell = reply.Value;
            response.Laps = (int)reply.Amount;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
