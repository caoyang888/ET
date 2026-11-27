namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_SlotRoomMissing = ErrorCode.ERR_WithoutException + PackageType.SlotCore * 1000 + 1;
        public const int ERR_SlotStakeMissing = ErrorCode.ERR_WithoutException + PackageType.SlotCore * 1000 + 2;
        public const int ERR_SlotLineCount = ErrorCode.ERR_WithoutException + PackageType.SlotCore * 1000 + 3;
        public const int ERR_SlotStageCleared = ErrorCode.ERR_WithoutException + PackageType.SlotCore * 1000 + 4;
        public const int ERR_SlotNotEnoughGold = ErrorCode.ERR_WithoutException + PackageType.SlotCore * 1000 + 5;
        public const int ERR_SlotFreeLocked = ErrorCode.ERR_WithoutException + PackageType.SlotCore * 1000 + 6;
        public const int ERR_SlotMaintenance = ErrorCode.ERR_WithoutException + PackageType.SlotCore * 1000 + 7;
        public const int ERR_SlotPlayerMissing = ErrorCode.ERR_WithoutException + PackageType.SlotCore * 1000 + 8;
    }
}
