namespace ET.Server
{
    public static class SlotErrorMap
    {
        public static int FromSpin(int fail)
        {
            switch (fail)
            {
                case SlotFail.Ok:
                    return ErrorCode.ERR_Success;
                case SlotFail.RoomMissing:
                    return ErrorCode.ERR_SlotRoomMissing;
                case SlotFail.StakeMissing:
                    return ErrorCode.ERR_SlotStakeMissing;
                case SlotFail.LineCount:
                    return ErrorCode.ERR_SlotLineCount;
                case SlotFail.StageCleared:
                    return ErrorCode.ERR_SlotStageCleared;
                case SlotFail.NotEnoughGold:
                    return ErrorCode.ERR_SlotNotEnoughGold;
                case SlotFail.FreeStakeLocked:
                    return ErrorCode.ERR_SlotFreeLocked;
                default:
                    return ErrorCode.ERR_SlotPlayerMissing;
            }
        }

        public static int FromOps(int fail)
        {
            switch (fail)
            {
                case OpsServices.Ok:
                    return ErrorCode.ERR_Success;
                case OpsServices.Already:
                    return ErrorCode.ERR_OpsAlready;
                case OpsServices.NotReady:
                    return ErrorCode.ERR_OpsNotReady;
                case OpsServices.NotEnough:
                    return ErrorCode.ERR_OpsNotEnough;
                case OpsServices.Closed:
                    return ErrorCode.ERR_OpsClosed;
                case OpsServices.Quota:
                    return ErrorCode.ERR_OpsQuota;
                case OpsServices.Limit:
                    return ErrorCode.ERR_OpsLimit;
                default:
                    return ErrorCode.ERR_OpsMissing;
            }
        }
    }
}
