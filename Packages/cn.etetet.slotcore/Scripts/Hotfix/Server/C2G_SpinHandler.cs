namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_SpinHandler : MessageSessionHandler<C2G_Spin, G2C_Spin>
    {
        protected override async ETTask Run(Session session, C2G_Spin request, G2C_Spin response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            Scene root = session.Root();
            G2Slot_Spin spin = G2Slot_Spin.Create();
            spin.PlayerId = player.Id;
            spin.RoomId = request.RoomId;
            spin.StakeId = request.StakeId;
            spin.LineCount = request.LineCount;
            spin.ClientSpinId = request.ClientSpinId;
            int rpcId = request.RpcId;
            Slot2G_Spin reply = (Slot2G_Spin)await root.GetComponent<MessageLocationSenderComponent>().Get(LocationType.SlotPlayer).Call(player.Id, spin);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Payout = reply.Payout;
            response.Damage = reply.Damage;
            response.StageId = reply.StageId;
            response.BossHp = reply.BossHp;
            response.FreeSpins = reply.FreeSpins;
            response.Seed = reply.Seed;
            response.Steps.AddRange(reply.Steps);
            response.Wallet.AddRange(reply.Wallet);
            response.Grants.AddRange(reply.Grants);
            response.RpcId = rpcId;
        }
    }
}
