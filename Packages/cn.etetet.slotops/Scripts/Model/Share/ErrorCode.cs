namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_OpsMissing = ErrorCode.ERR_WithoutException + PackageType.SlotOps * 1000 + 1;
        public const int ERR_OpsAlready = ErrorCode.ERR_WithoutException + PackageType.SlotOps * 1000 + 2;
        public const int ERR_OpsNotReady = ErrorCode.ERR_WithoutException + PackageType.SlotOps * 1000 + 3;
        public const int ERR_OpsNotEnough = ErrorCode.ERR_WithoutException + PackageType.SlotOps * 1000 + 4;
        public const int ERR_OpsClosed = ErrorCode.ERR_WithoutException + PackageType.SlotOps * 1000 + 5;
        public const int ERR_OpsQuota = ErrorCode.ERR_WithoutException + PackageType.SlotOps * 1000 + 6;
        public const int ERR_OpsLimit = ErrorCode.ERR_WithoutException + PackageType.SlotOps * 1000 + 7;
    }
}
