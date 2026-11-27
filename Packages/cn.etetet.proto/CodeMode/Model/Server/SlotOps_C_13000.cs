using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    [MemoryPackable]
    [Message(Opcode.C2G_ClaimTask)]
    [ResponseType(nameof(G2C_ClaimTask))]
    public partial class C2G_ClaimTask : MessageObject, ISessionRequest
    {
        public static C2G_ClaimTask Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ClaimTask>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int TaskId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ClaimTask)]
    public partial class G2C_ClaimTask : MessageObject, ISessionResponse
    {
        public static G2C_ClaimTask Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ClaimTask>(isFromPool);
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
    [Message(Opcode.C2G_CreateOrder)]
    [ResponseType(nameof(G2C_CreateOrder))]
    public partial class C2G_CreateOrder : MessageObject, ISessionRequest
    {
        public static C2G_CreateOrder Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_CreateOrder>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int ProductId { get; set; }
        [MemoryPackOrder(2)]
        public string OrderId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_CreateOrder)]
    public partial class G2C_CreateOrder : MessageObject, ISessionResponse
    {
        public static G2C_CreateOrder Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_CreateOrder>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public string OrderId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_DevPay)]
    [ResponseType(nameof(G2C_DevPay))]
    public partial class C2G_DevPay : MessageObject, ISessionRequest
    {
        public static C2G_DevPay Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_DevPay>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string OrderId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_DevPay)]
    public partial class G2C_DevPay : MessageObject, ISessionResponse
    {
        public static G2C_DevPay Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_DevPay>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public int VipLevel { get; set; }
        [MemoryPackOrder(4)]
        public List<SlotGrantProto> Wallet { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_ClaimJubao)]
    [ResponseType(nameof(G2C_ClaimJubao))]
    public partial class C2G_ClaimJubao : MessageObject, ISessionRequest
    {
        public static C2G_ClaimJubao Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ClaimJubao>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ClaimJubao)]
    public partial class G2C_ClaimJubao : MessageObject, ISessionResponse
    {
        public static G2C_ClaimJubao Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ClaimJubao>(isFromPool);
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
    [Message(Opcode.C2G_RollDice)]
    [ResponseType(nameof(G2C_RollDice))]
    public partial class C2G_RollDice : MessageObject, ISessionRequest
    {
        public static C2G_RollDice Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_RollDice>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Steps { get; set; }
        [MemoryPackOrder(2)]
        public int Lucky { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_RollDice)]
    public partial class G2C_RollDice : MessageObject, ISessionResponse
    {
        public static G2C_RollDice Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_RollDice>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public int Cell { get; set; }
        [MemoryPackOrder(4)]
        public int Laps { get; set; }
        [MemoryPackOrder(5)]
        public List<SlotGrantProto> Wallet { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_ClaimLap)]
    [ResponseType(nameof(G2C_ClaimLap))]
    public partial class C2G_ClaimLap : MessageObject, ISessionRequest
    {
        public static C2G_ClaimLap Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ClaimLap>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Laps { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ClaimLap)]
    public partial class G2C_ClaimLap : MessageObject, ISessionResponse
    {
        public static G2C_ClaimLap Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ClaimLap>(isFromPool);
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
    [Message(Opcode.C2G_ClaimKing)]
    [ResponseType(nameof(G2C_ClaimKing))]
    public partial class C2G_ClaimKing : MessageObject, ISessionRequest
    {
        public static C2G_ClaimKing Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ClaimKing>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int TierId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ClaimKing)]
    public partial class G2C_ClaimKing : MessageObject, ISessionResponse
    {
        public static G2C_ClaimKing Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ClaimKing>(isFromPool);
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
    [Message(Opcode.C2G_ClaimMail)]
    [ResponseType(nameof(G2C_ClaimMail))]
    public partial class C2G_ClaimMail : MessageObject, ISessionRequest
    {
        public static C2G_ClaimMail Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ClaimMail>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public long MailId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ClaimMail)]
    public partial class G2C_ClaimMail : MessageObject, ISessionResponse
    {
        public static G2C_ClaimMail Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ClaimMail>(isFromPool);
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
    [Message(Opcode.C2G_RedeemCode)]
    [ResponseType(nameof(G2C_RedeemCode))]
    public partial class C2G_RedeemCode : MessageObject, ISessionRequest
    {
        public static C2G_RedeemCode Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_RedeemCode>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string Code { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_RedeemCode)]
    public partial class G2C_RedeemCode : MessageObject, ISessionResponse
    {
        public static G2C_RedeemCode Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_RedeemCode>(isFromPool);
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
    [Message(Opcode.C2G_ClaimPiggy)]
    [ResponseType(nameof(G2C_ClaimPiggy))]
    public partial class C2G_ClaimPiggy : MessageObject, ISessionRequest
    {
        public static C2G_ClaimPiggy Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ClaimPiggy>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ClaimPiggy)]
    public partial class G2C_ClaimPiggy : MessageObject, ISessionResponse
    {
        public static G2C_ClaimPiggy Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ClaimPiggy>(isFromPool);
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
    [Message(Opcode.C2G_Bankruptcy)]
    [ResponseType(nameof(G2C_Bankruptcy))]
    public partial class C2G_Bankruptcy : MessageObject, ISessionRequest
    {
        public static C2G_Bankruptcy Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_Bankruptcy>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_Bankruptcy)]
    public partial class G2C_Bankruptcy : MessageObject, ISessionResponse
    {
        public static G2C_Bankruptcy Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_Bankruptcy>(isFromPool);
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
    [Message(Opcode.C2G_SubmitIdentity)]
    [ResponseType(nameof(G2C_SubmitIdentity))]
    public partial class C2G_SubmitIdentity : MessageObject, ISessionRequest
    {
        public static C2G_SubmitIdentity Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_SubmitIdentity>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string Name { get; set; }
        [MemoryPackOrder(2)]
        public string Number { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_SubmitIdentity)]
    public partial class G2C_SubmitIdentity : MessageObject, ISessionResponse
    {
        public static G2C_SubmitIdentity Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_SubmitIdentity>(isFromPool);
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
    [Message(Opcode.C2G_BindPhone)]
    [ResponseType(nameof(G2C_BindPhone))]
    public partial class C2G_BindPhone : MessageObject, ISessionRequest
    {
        public static C2G_BindPhone Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_BindPhone>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string Number { get; set; }
        [MemoryPackOrder(2)]
        public string Code { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_BindPhone)]
    public partial class G2C_BindPhone : MessageObject, ISessionResponse
    {
        public static G2C_BindPhone Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_BindPhone>(isFromPool);
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
    [Message(Opcode.C2G_QueryRival)]
    [ResponseType(nameof(G2C_QueryRival))]
    public partial class C2G_QueryRival : MessageObject, ISessionRequest
    {
        public static C2G_QueryRival Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_QueryRival>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Period { get; set; }
        [MemoryPackOrder(2)]
        public int Place { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_QueryRival)]
    public partial class G2C_QueryRival : MessageObject, ISessionResponse
    {
        public static G2C_QueryRival Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_QueryRival>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long RivalDamage { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_SetMaintenance)]
    [ResponseType(nameof(G2C_SetMaintenance))]
    public partial class C2G_SetMaintenance : MessageObject, ISessionRequest
    {
        public static C2G_SetMaintenance Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_SetMaintenance>(isFromPool);
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
    [Message(Opcode.G2C_SetMaintenance)]
    public partial class G2C_SetMaintenance : MessageObject, ISessionResponse
    {
        public static G2C_SetMaintenance Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_SetMaintenance>(isFromPool);
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

    public static partial class Opcode
    {
        public const ushort C2G_ClaimTask = 13001;
        public const ushort G2C_ClaimTask = 13002;
        public const ushort C2G_CreateOrder = 13003;
        public const ushort G2C_CreateOrder = 13004;
        public const ushort C2G_DevPay = 13005;
        public const ushort G2C_DevPay = 13006;
        public const ushort C2G_ClaimJubao = 13007;
        public const ushort G2C_ClaimJubao = 13008;
        public const ushort C2G_RollDice = 13009;
        public const ushort G2C_RollDice = 13010;
        public const ushort C2G_ClaimLap = 13011;
        public const ushort G2C_ClaimLap = 13012;
        public const ushort C2G_ClaimKing = 13013;
        public const ushort G2C_ClaimKing = 13014;
        public const ushort C2G_ClaimMail = 13015;
        public const ushort G2C_ClaimMail = 13016;
        public const ushort C2G_RedeemCode = 13017;
        public const ushort G2C_RedeemCode = 13018;
        public const ushort C2G_ClaimPiggy = 13019;
        public const ushort G2C_ClaimPiggy = 13020;
        public const ushort C2G_Bankruptcy = 13021;
        public const ushort G2C_Bankruptcy = 13022;
        public const ushort C2G_SubmitIdentity = 13023;
        public const ushort G2C_SubmitIdentity = 13024;
        public const ushort C2G_BindPhone = 13025;
        public const ushort G2C_BindPhone = 13026;
        public const ushort C2G_QueryRival = 13027;
        public const ushort G2C_QueryRival = 13028;
        public const ushort C2G_SetMaintenance = 13029;
        public const ushort G2C_SetMaintenance = 13030;
    }
}