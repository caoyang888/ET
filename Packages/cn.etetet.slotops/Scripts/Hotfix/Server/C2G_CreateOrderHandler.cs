namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_CreateOrderHandler : MessageSessionHandler<C2G_CreateOrder, G2C_CreateOrder>
    {
        protected override async ETTask Run(Session session, C2G_CreateOrder request, G2C_CreateOrder response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 2, request.ProductId, 0, 0, request.OrderId, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.OrderId = request.OrderId;
        }
    }
}
