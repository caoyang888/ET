namespace ET.Server
{
    public static class SlotRoomView
    {
        public static int FillRoom(SlotPlayer player, int roomId, Slot2G_Room response)
        {
            SlotWorldComponent world = player.Root().GetComponent<SlotWorldComponent>();
            SlotRoomDef room;
            if (!world.Catalog.Rooms.TryGetValue(roomId, out room))
            {
                return ErrorCode.ERR_SlotRoomMissing;
            }

            SlotProgressState progress;
            if (!player.Spin.Progress.TryGetValue(room.ProgressGroup, out progress))
            {
                SlotStageDef first = world.Catalog.FirstStage(room.ProgressGroup);
                progress = new SlotProgressState
                {
                    StageId = first == null ? 0 : first.StageId,
                    BossHp = first == null ? 0 : first.HpMax,
                };
                player.Spin.Progress[room.ProgressGroup] = progress;
            }

            SlotStageDef stage = null;
            world.Catalog.Stages.TryGetValue(progress.StageId, out stage);
            SlotRoomLedger ledger = world.Ledgers.Snapshot(roomId);
            response.RoomId = roomId;
            response.StageId = progress.StageId;
            response.BossHp = progress.BossHp;
            response.BossHpMax = stage == null ? 0 : stage.HpMax;
            response.FreeSpins = progress.FreeSpins;
            response.PublicStock = ledger.PublicStock;
            response.RankPool = ledger.RankPool;
            response.JackpotPool = ledger.Jackpot;
            foreach (SlotStakeDef stake in world.Catalog.Stakes.Values)
            {
                if (stake.RoomId != roomId)
                {
                    continue;
                }

                SlotStakeProto proto = SlotStakeProto.Create();
                proto.StakeId = stake.StakeId;
                proto.BetGold = stake.BetGold;
                proto.VipLevel = stake.VipLevel;
                proto.TotalCost = stake.TotalCost;
                response.Stakes.Add(proto);
            }

            return ErrorCode.ERR_Success;
        }
    }
}
