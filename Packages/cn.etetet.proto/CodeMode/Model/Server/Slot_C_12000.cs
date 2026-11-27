using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    [MemoryPackable]
    [Message(Opcode.SlotGrantProto)]
    public partial class SlotGrantProto : MessageObject
    {
        public static SlotGrantProto Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<SlotGrantProto>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int ItemId { get; set; }
        [MemoryPackOrder(1)]
        public long Count { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.SlotCellProto)]
    public partial class SlotCellProto : MessageObject
    {
        public static SlotCellProto Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<SlotCellProto>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int Row { get; set; }
        [MemoryPackOrder(1)]
        public int Column { get; set; }
        [MemoryPackOrder(2)]
        public int SymbolId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.SlotWinProto)]
    public partial class SlotWinProto : MessageObject
    {
        public static SlotWinProto Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<SlotWinProto>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int SymbolId { get; set; }
        [MemoryPackOrder(1)]
        public int Count { get; set; }
        [MemoryPackOrder(2)]
        public long Amount { get; set; }
        [MemoryPackOrder(3)]
        public List<SlotCellProto> Cells { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.SlotStepProto)]
    public partial class SlotStepProto : MessageObject
    {
        public static SlotStepProto Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<SlotStepProto>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int Index { get; set; }
        [MemoryPackOrder(1)]
        public List<int> Symbols { get; set; } = new();

        [MemoryPackOrder(2)]
        public List<SlotWinProto> Wins { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_EnterSlot)]
    [ResponseType(nameof(G2C_EnterSlot))]
    public partial class C2G_EnterSlot : MessageObject, ISessionRequest
    {
        public static C2G_EnterSlot Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_EnterSlot>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_EnterSlot)]
    public partial class G2C_EnterSlot : MessageObject, ISessionResponse
    {
        public static G2C_EnterSlot Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_EnterSlot>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long PlayerId { get; set; }
        [MemoryPackOrder(4)]
        public List<SlotGrantProto> Wallet { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_Spin)]
    [ResponseType(nameof(G2C_Spin))]
    public partial class C2G_Spin : MessageObject, ISessionRequest
    {
        public static C2G_Spin Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_Spin>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int RoomId { get; set; }
        [MemoryPackOrder(2)]
        public int StakeId { get; set; }
        [MemoryPackOrder(3)]
        public int LineCount { get; set; }
        [MemoryPackOrder(4)]
        public string ClientSpinId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_Spin)]
    public partial class G2C_Spin : MessageObject, ISessionResponse
    {
        public static G2C_Spin Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_Spin>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long Payout { get; set; }
        [MemoryPackOrder(4)]
        public long Damage { get; set; }
        [MemoryPackOrder(5)]
        public int StageId { get; set; }
        [MemoryPackOrder(6)]
        public long BossHp { get; set; }
        [MemoryPackOrder(7)]
        public int FreeSpins { get; set; }
        [MemoryPackOrder(8)]
        public long Seed { get; set; }
        [MemoryPackOrder(9)]
        public List<SlotStepProto> Steps { get; set; } = new();

        [MemoryPackOrder(10)]
        public List<SlotGrantProto> Wallet { get; set; } = new();

        [MemoryPackOrder(11)]
        public List<SlotGrantProto> Grants { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.SlotStakeProto)]
    public partial class SlotStakeProto : MessageObject
    {
        public static SlotStakeProto Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<SlotStakeProto>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int StakeId { get; set; }
        [MemoryPackOrder(1)]
        public long BetGold { get; set; }
        [MemoryPackOrder(2)]
        public int VipLevel { get; set; }
        [MemoryPackOrder(3)]
        public long TotalCost { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_GetProfile)]
    [ResponseType(nameof(G2C_GetProfile))]
    public partial class C2G_GetProfile : MessageObject, ISessionRequest
    {
        public static C2G_GetProfile Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_GetProfile>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_GetProfile)]
    public partial class G2C_GetProfile : MessageObject, ISessionResponse
    {
        public static G2C_GetProfile Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_GetProfile>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long PlayerId { get; set; }
        [MemoryPackOrder(4)]
        public string Account { get; set; }
        [MemoryPackOrder(5)]
        public string Nickname { get; set; }
        [MemoryPackOrder(6)]
        public string Avatar { get; set; }
        [MemoryPackOrder(7)]
        public int VipExp { get; set; }
        [MemoryPackOrder(8)]
        public int VipLevel { get; set; }
        [MemoryPackOrder(9)]
        public int RealNameVerified { get; set; }
        [MemoryPackOrder(10)]
        public int PhoneBound { get; set; }
        [MemoryPackOrder(11)]
        public int UnreadMail { get; set; }
        [MemoryPackOrder(12)]
        public int ClaimableTask { get; set; }
        [MemoryPackOrder(13)]
        public int Maintenance { get; set; }
        [MemoryPackOrder(14)]
        public List<SlotGrantProto> Wallet { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_Rename)]
    [ResponseType(nameof(G2C_Rename))]
    public partial class C2G_Rename : MessageObject, ISessionRequest
    {
        public static C2G_Rename Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_Rename>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string Nickname { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_Rename)]
    public partial class G2C_Rename : MessageObject, ISessionResponse
    {
        public static G2C_Rename Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_Rename>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public string Nickname { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_EnterRoom)]
    [ResponseType(nameof(G2C_EnterRoom))]
    public partial class C2G_EnterRoom : MessageObject, ISessionRequest
    {
        public static C2G_EnterRoom Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_EnterRoom>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int RoomId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_EnterRoom)]
    public partial class G2C_EnterRoom : MessageObject, ISessionResponse
    {
        public static G2C_EnterRoom Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_EnterRoom>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public int RoomId { get; set; }
        [MemoryPackOrder(4)]
        public int StageId { get; set; }
        [MemoryPackOrder(5)]
        public long BossHp { get; set; }
        [MemoryPackOrder(6)]
        public long BossHpMax { get; set; }
        [MemoryPackOrder(7)]
        public int FreeSpins { get; set; }
        [MemoryPackOrder(8)]
        public long PublicStock { get; set; }
        [MemoryPackOrder(9)]
        public long RankPool { get; set; }
        [MemoryPackOrder(10)]
        public long JackpotPool { get; set; }
        [MemoryPackOrder(11)]
        public List<SlotStakeProto> Stakes { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_GetRoomSnapshot)]
    [ResponseType(nameof(G2C_GetRoomSnapshot))]
    public partial class C2G_GetRoomSnapshot : MessageObject, ISessionRequest
    {
        public static C2G_GetRoomSnapshot Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_GetRoomSnapshot>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int RoomId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_GetRoomSnapshot)]
    public partial class G2C_GetRoomSnapshot : MessageObject, ISessionResponse
    {
        public static G2C_GetRoomSnapshot Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_GetRoomSnapshot>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public int RoomId { get; set; }
        [MemoryPackOrder(4)]
        public int StageId { get; set; }
        [MemoryPackOrder(5)]
        public long BossHp { get; set; }
        [MemoryPackOrder(6)]
        public long BossHpMax { get; set; }
        [MemoryPackOrder(7)]
        public int FreeSpins { get; set; }
        [MemoryPackOrder(8)]
        public long PublicStock { get; set; }
        [MemoryPackOrder(9)]
        public long RankPool { get; set; }
        [MemoryPackOrder(10)]
        public long JackpotPool { get; set; }
        [MemoryPackOrder(11)]
        public List<SlotStakeProto> Stakes { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_PushWallet)]
    public partial class G2C_PushWallet : MessageObject, IMessage
    {
        public static G2C_PushWallet Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_PushWallet>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public List<SlotGrantProto> Wallet { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_PushProgress)]
    public partial class G2C_PushProgress : MessageObject, IMessage
    {
        public static G2C_PushProgress Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_PushProgress>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RoomId { get; set; }
        [MemoryPackOrder(1)]
        public int StageId { get; set; }
        [MemoryPackOrder(2)]
        public long BossHp { get; set; }
        [MemoryPackOrder(3)]
        public long BossHpMax { get; set; }
        [MemoryPackOrder(4)]
        public int FreeSpins { get; set; }
        [MemoryPackOrder(5)]
        public long TotalDamage { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_PushMail)]
    public partial class G2C_PushMail : MessageObject, IMessage
    {
        public static G2C_PushMail Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_PushMail>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public long MailId { get; set; }
        [MemoryPackOrder(1)]
        public string Title { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_PushNotice)]
    public partial class G2C_PushNotice : MessageObject, IMessage
    {
        public static G2C_PushNotice Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_PushNotice>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int Kind { get; set; }
        [MemoryPackOrder(1)]
        public string Title { get; set; }
        [MemoryPackOrder(2)]
        public string Body { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_PushKick)]
    public partial class G2C_PushKick : MessageObject, IMessage
    {
        public static G2C_PushKick Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_PushKick>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int Reason { get; set; }
        [MemoryPackOrder(1)]
        public string Message { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    public static partial class Opcode
    {
        public const ushort SlotGrantProto = 12001;
        public const ushort SlotCellProto = 12002;
        public const ushort SlotWinProto = 12003;
        public const ushort SlotStepProto = 12004;
        public const ushort C2G_EnterSlot = 12005;
        public const ushort G2C_EnterSlot = 12006;
        public const ushort C2G_Spin = 12007;
        public const ushort G2C_Spin = 12008;
        public const ushort SlotStakeProto = 12009;
        public const ushort C2G_GetProfile = 12010;
        public const ushort G2C_GetProfile = 12011;
        public const ushort C2G_Rename = 12012;
        public const ushort G2C_Rename = 12013;
        public const ushort C2G_EnterRoom = 12014;
        public const ushort G2C_EnterRoom = 12015;
        public const ushort C2G_GetRoomSnapshot = 12016;
        public const ushort G2C_GetRoomSnapshot = 12017;
        public const ushort G2C_PushWallet = 12018;
        public const ushort G2C_PushProgress = 12019;
        public const ushort G2C_PushMail = 12020;
        public const ushort G2C_PushNotice = 12021;
        public const ushort G2C_PushKick = 12022;
    }
}