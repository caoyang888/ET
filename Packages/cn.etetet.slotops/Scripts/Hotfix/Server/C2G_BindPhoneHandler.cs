namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_BindPhoneHandler : MessageSessionHandler<C2G_BindPhone, G2C_BindPhone>
    {
        protected override async ETTask Run(Session session, C2G_BindPhone request, G2C_BindPhone response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 13, 0, 0, 0, request.Number, request.Code);
            response.Error = reply.Error;
            response.Message = reply.Message;
        }
    }
}
