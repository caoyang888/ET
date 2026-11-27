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
    }
}