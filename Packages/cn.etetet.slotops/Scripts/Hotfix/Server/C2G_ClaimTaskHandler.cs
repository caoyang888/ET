namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ClaimTaskHandler : MessageSessionHandler<C2G_ClaimTask, G2C_ClaimTask>
    {
        protected override async ETTask Run(Session session, C2G_ClaimTask request, G2C_ClaimTask response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 1, request.TaskId, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Wallet.AddRange(reply.Wallet);
        }
    }
}
