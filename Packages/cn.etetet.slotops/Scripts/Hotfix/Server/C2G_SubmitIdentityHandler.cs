namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_SubmitIdentityHandler : MessageSessionHandler<C2G_SubmitIdentity, G2C_SubmitIdentity>
    {
        protected override async ETTask Run(Session session, C2G_SubmitIdentity request, G2C_SubmitIdentity response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 12, 0, 0, 0, request.Name, request.Number);
            response.Error = reply.Error;
            response.Message = reply.Message;
        }
    }
}
