using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    [MemoryPackable]
    [Message(Opcode.G2Slot_Enter)]
    [ResponseType(nameof(Slot2G_Enter))]
    public partial class G2Slot_Enter : MessageObject, IRequest
    {
        public static G2Slot_Enter Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2Slot_Enter>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }
        [MemoryPackOrder(2)]
        public string Account { get; set; }
        [MemoryPackOrder(3)]
        public string Nickname { get; set; }
        [MemoryPackOrder(4)]
        public string GateIp { get; set; }
        [MemoryPackOrder(5)]
        public int GatePort { get; set; }
        [MemoryPackOrder(6)]
        public long GateFiber { get; set; }
        [MemoryPackOrder(7)]
        public long GateInstance { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.Slot2G_Enter)]
    public partial class Slot2G_Enter : MessageObject, IResponse
    {
        public static Slot2G_Enter Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<Slot2G_Enter>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public List<SlotGrantProto> Wallet { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2Slot_Room)]
    [ResponseType(nameof(Slot2G_Room))]
    public partial class G2Slot_Room : MessageObject, ILocationRequest
    {
        public static G2Slot_Room Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2Slot_Room>(isFromPool);
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
    [Message(Opcode.Slot2G_Room)]
    public partial class Slot2G_Room : MessageObject, ILocationResponse
    {
        public static Slot2G_Room Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<Slot2G_Room>(isFromPool);
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
    [Message(Opcode.G2Slot_Spin)]
    [ResponseType(nameof(Slot2G_Spin))]
    public partial class G2Slot_Spin : MessageObject, ILocationRequest
    {
        public static G2Slot_Spin Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2Slot_Spin>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }
        [MemoryPackOrder(2)]
        public int RoomId { get; set; }
        [MemoryPackOrder(3)]
        public int StakeId { get; set; }
        [MemoryPackOrder(4)]
        public int LineCount { get; set; }
        [MemoryPackOrder(5)]
        public string ClientSpinId { get; set; }
        [MemoryPackOrder(6)]
        public long Seed { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.Slot2G_Spin)]
    public partial class Slot2G_Spin : MessageObject, ILocationResponse
    {
        public static Slot2G_Spin Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<Slot2G_Spin>(isFromPool);
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
    [Message(Opcode.G2Slot_Maintenance)]
    [ResponseType(nameof(Slot2G_Maintenance))]
    public partial class G2Slot_Maintenance : MessageObject, IRequest
    {
        public static G2Slot_Maintenance Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2Slot_Maintenance>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Enabled { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.Slot2G_Maintenance)]
    public partial class Slot2G_Maintenance : MessageObject, IResponse
    {
        public static Slot2G_Maintenance Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<Slot2G_Maintenance>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2Slot_Ops)]
    [ResponseType(nameof(Slot2G_Ops))]
    public partial class G2Slot_Ops : MessageObject, ILocationRequest
    {
        public static G2Slot_Ops Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2Slot_Ops>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Action { get; set; }
        [MemoryPackOrder(2)]
        public int Arg1 { get; set; }
        [MemoryPackOrder(3)]
        public int Arg2 { get; set; }
        [MemoryPackOrder(4)]
        public long Arg3 { get; set; }
        [MemoryPackOrder(5)]
        public string Text { get; set; }
        [MemoryPackOrder(6)]
        public string Text2 { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.Slot2G_Ops)]
    public partial class Slot2G_Ops : MessageObject, ILocationResponse
    {
        public static Slot2G_Ops Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<Slot2G_Ops>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public int Value { get; set; }
        [MemoryPackOrder(4)]
        public long Amount { get; set; }
        [MemoryPackOrder(5)]
        public List<SlotGrantProto> Wallet { get; set; } = new();

        [MemoryPackOrder(6)]
        public string Text { get; set; }
        [MemoryPackOrder(7)]
        public int Value2 { get; set; }
        [MemoryPackOrder(8)]
        public long Amount2 { get; set; }
        [MemoryPackOrder(9)]
        public List<SlotTaskProto> Tasks { get; set; } = new();

        [MemoryPackOrder(10)]
        public List<SlotProductProto> Products { get; set; } = new();

        [MemoryPackOrder(11)]
        public List<SlotMailProto> Mails { get; set; } = new();

        [MemoryPackOrder(12)]
        public List<SlotOrderProto> Orders { get; set; } = new();

        [MemoryPackOrder(13)]
        public List<SlotRankProto> Ranks { get; set; } = new();

        [MemoryPackOrder(14)]
        public int Value3 { get; set; }
        [MemoryPackOrder(15)]
        public int Value4 { get; set; }
        [MemoryPackOrder(16)]
        public string Text2 { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    public static partial class Opcode
    {
        public const ushort G2Slot_Enter = 22001;
        public const ushort Slot2G_Enter = 22002;
        public const ushort G2Slot_Room = 22003;
        public const ushort Slot2G_Room = 22004;
        public const ushort G2Slot_Spin = 22005;
        public const ushort Slot2G_Spin = 22006;
        public const ushort G2Slot_Maintenance = 22007;
        public const ushort Slot2G_Maintenance = 22008;
        public const ushort G2Slot_Ops = 22009;
        public const ushort Slot2G_Ops = 22010;
    }
}