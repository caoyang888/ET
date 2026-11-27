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

    [MemoryPackable]
    [Message(Opcode.SlotTaskProto)]
    public partial class SlotTaskProto : MessageObject
    {
        public static SlotTaskProto Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<SlotTaskProto>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int TaskId { get; set; }
        [MemoryPackOrder(1)]
        public int TaskType { get; set; }
        [MemoryPackOrder(2)]
        public int Condition { get; set; }
        [MemoryPackOrder(3)]
        public long Progress { get; set; }
        [MemoryPackOrder(4)]
        public long Target { get; set; }
        [MemoryPackOrder(5)]
        public int Claimed { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_ListTasks)]
    [ResponseType(nameof(G2C_ListTasks))]
    public partial class C2G_ListTasks : MessageObject, ISessionRequest
    {
        public static C2G_ListTasks Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ListTasks>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ListTasks)]
    public partial class G2C_ListTasks : MessageObject, ISessionResponse
    {
        public static G2C_ListTasks Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ListTasks>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public List<SlotTaskProto> Tasks { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_GetJubao)]
    [ResponseType(nameof(G2C_GetJubao))]
    public partial class C2G_GetJubao : MessageObject, ISessionRequest
    {
        public static C2G_GetJubao Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_GetJubao>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_GetJubao)]
    public partial class G2C_GetJubao : MessageObject, ISessionResponse
    {
        public static G2C_GetJubao Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_GetJubao>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public int Level { get; set; }
        [MemoryPackOrder(4)]
        public long ReadyUnix { get; set; }
        [MemoryPackOrder(5)]
        public long Gold { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_GetDiceBoard)]
    [ResponseType(nameof(G2C_GetDiceBoard))]
    public partial class C2G_GetDiceBoard : MessageObject, ISessionRequest
    {
        public static C2G_GetDiceBoard Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_GetDiceBoard>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_GetDiceBoard)]
    public partial class G2C_GetDiceBoard : MessageObject, ISessionResponse
    {
        public static G2C_GetDiceBoard Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_GetDiceBoard>(isFromPool);
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
        public int CellCount { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_GetKing)]
    [ResponseType(nameof(G2C_GetKing))]
    public partial class C2G_GetKing : MessageObject, ISessionRequest
    {
        public static C2G_GetKing Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_GetKing>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_GetKing)]
    public partial class G2C_GetKing : MessageObject, ISessionResponse
    {
        public static G2C_GetKing Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_GetKing>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long Damage { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_GetPiggy)]
    [ResponseType(nameof(G2C_GetPiggy))]
    public partial class C2G_GetPiggy : MessageObject, ISessionRequest
    {
        public static C2G_GetPiggy Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_GetPiggy>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_GetPiggy)]
    public partial class G2C_GetPiggy : MessageObject, ISessionResponse
    {
        public static G2C_GetPiggy Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_GetPiggy>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long Piggy { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_GetBankruptcy)]
    [ResponseType(nameof(G2C_GetBankruptcy))]
    public partial class C2G_GetBankruptcy : MessageObject, ISessionRequest
    {
        public static C2G_GetBankruptcy Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_GetBankruptcy>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_GetBankruptcy)]
    public partial class G2C_GetBankruptcy : MessageObject, ISessionResponse
    {
        public static G2C_GetBankruptcy Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_GetBankruptcy>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public int Claims { get; set; }
        [MemoryPackOrder(4)]
        public int Limit { get; set; }
        [MemoryPackOrder(5)]
        public long Gold { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.SlotProductProto)]
    public partial class SlotProductProto : MessageObject
    {
        public static SlotProductProto Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<SlotProductProto>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int ProductId { get; set; }
        [MemoryPackOrder(1)]
        public int Category { get; set; }
        [MemoryPackOrder(2)]
        public long PriceCents { get; set; }
        [MemoryPackOrder(3)]
        public int LimitCount { get; set; }
        [MemoryPackOrder(4)]
        public int Bought { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_ListProducts)]
    [ResponseType(nameof(G2C_ListProducts))]
    public partial class C2G_ListProducts : MessageObject, ISessionRequest
    {
        public static C2G_ListProducts Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ListProducts>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ListProducts)]
    public partial class G2C_ListProducts : MessageObject, ISessionResponse
    {
        public static G2C_ListProducts Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ListProducts>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public List<SlotProductProto> Products { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_GetVipInfo)]
    [ResponseType(nameof(G2C_GetVipInfo))]
    public partial class C2G_GetVipInfo : MessageObject, ISessionRequest
    {
        public static C2G_GetVipInfo Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_GetVipInfo>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_GetVipInfo)]
    public partial class G2C_GetVipInfo : MessageObject, ISessionResponse
    {
        public static G2C_GetVipInfo Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_GetVipInfo>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public int Level { get; set; }
        [MemoryPackOrder(4)]
        public int Exp { get; set; }
        [MemoryPackOrder(5)]
        public int CanRename { get; set; }
        [MemoryPackOrder(6)]
        public int CanIdle { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.SlotOrderProto)]
    public partial class SlotOrderProto : MessageObject
    {
        public static SlotOrderProto Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<SlotOrderProto>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public string OrderId { get; set; }
        [MemoryPackOrder(1)]
        public int ProductId { get; set; }
        [MemoryPackOrder(2)]
        public int Paid { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_ListOrders)]
    [ResponseType(nameof(G2C_ListOrders))]
    public partial class C2G_ListOrders : MessageObject, ISessionRequest
    {
        public static C2G_ListOrders Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ListOrders>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ListOrders)]
    public partial class G2C_ListOrders : MessageObject, ISessionResponse
    {
        public static G2C_ListOrders Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ListOrders>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public List<SlotOrderProto> Orders { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.SlotMailProto)]
    public partial class SlotMailProto : MessageObject
    {
        public static SlotMailProto Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<SlotMailProto>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public long MailId { get; set; }
        [MemoryPackOrder(1)]
        public string Title { get; set; }
        [MemoryPackOrder(2)]
        public string Body { get; set; }
        [MemoryPackOrder(3)]
        public int Read { get; set; }
        [MemoryPackOrder(4)]
        public int Claimed { get; set; }
        [MemoryPackOrder(5)]
        public List<SlotGrantProto> Rewards { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_ListMails)]
    [ResponseType(nameof(G2C_ListMails))]
    public partial class C2G_ListMails : MessageObject, ISessionRequest
    {
        public static C2G_ListMails Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ListMails>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ListMails)]
    public partial class G2C_ListMails : MessageObject, ISessionResponse
    {
        public static G2C_ListMails Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ListMails>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public List<SlotMailProto> Mails { get; set; } = new();

        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_ReadMail)]
    [ResponseType(nameof(G2C_ReadMail))]
    public partial class C2G_ReadMail : MessageObject, ISessionRequest
    {
        public static C2G_ReadMail Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ReadMail>(isFromPool);
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
    [Message(Opcode.G2C_ReadMail)]
    public partial class G2C_ReadMail : MessageObject, ISessionResponse
    {
        public static G2C_ReadMail Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ReadMail>(isFromPool);
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
    [Message(Opcode.C2G_ClaimAllMails)]
    [ResponseType(nameof(G2C_ClaimAllMails))]
    public partial class C2G_ClaimAllMails : MessageObject, ISessionRequest
    {
        public static C2G_ClaimAllMails Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ClaimAllMails>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ClaimAllMails)]
    public partial class G2C_ClaimAllMails : MessageObject, ISessionResponse
    {
        public static G2C_ClaimAllMails Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ClaimAllMails>(isFromPool);
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
    [Message(Opcode.C2G_DeleteMail)]
    [ResponseType(nameof(G2C_DeleteMail))]
    public partial class C2G_DeleteMail : MessageObject, ISessionRequest
    {
        public static C2G_DeleteMail Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_DeleteMail>(isFromPool);
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
    [Message(Opcode.G2C_DeleteMail)]
    public partial class G2C_DeleteMail : MessageObject, ISessionResponse
    {
        public static G2C_DeleteMail Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_DeleteMail>(isFromPool);
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
    [Message(Opcode.C2G_DevSendMail)]
    [ResponseType(nameof(G2C_DevSendMail))]
    public partial class C2G_DevSendMail : MessageObject, ISessionRequest
    {
        public static C2G_DevSendMail Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_DevSendMail>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string Title { get; set; }
        [MemoryPackOrder(2)]
        public string Body { get; set; }
        [MemoryPackOrder(3)]
        public int ItemId { get; set; }
        [MemoryPackOrder(4)]
        public long Count { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_DevSendMail)]
    public partial class G2C_DevSendMail : MessageObject, ISessionResponse
    {
        public static G2C_DevSendMail Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_DevSendMail>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long MailId { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.SlotRankProto)]
    public partial class SlotRankProto : MessageObject
    {
        public static SlotRankProto Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<SlotRankProto>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int Place { get; set; }
        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }
        [MemoryPackOrder(2)]
        public string Nickname { get; set; }
        [MemoryPackOrder(3)]
        public long Damage { get; set; }
        [MemoryPackOrder(4)]
        public int Rival { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.C2G_ListRank)]
    [ResponseType(nameof(G2C_ListRank))]
    public partial class C2G_ListRank : MessageObject, ISessionRequest
    {
        public static C2G_ListRank Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<C2G_ListRank>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Period { get; set; }
        public override void Dispose()
        {
            ObjectPool.Recycle(this);
        }
    }

    [MemoryPackable]
    [Message(Opcode.G2C_ListRank)]
    public partial class G2C_ListRank : MessageObject, ISessionResponse
    {
        public static G2C_ListRank Create(bool isFromPool = false)
        {
            return ObjectPool.Fetch<G2C_ListRank>(isFromPool);
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public List<SlotRankProto> Rows { get; set; } = new();

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
        public const ushort SlotTaskProto = 13031;
        public const ushort C2G_ListTasks = 13032;
        public const ushort G2C_ListTasks = 13033;
        public const ushort C2G_GetJubao = 13034;
        public const ushort G2C_GetJubao = 13035;
        public const ushort C2G_GetDiceBoard = 13036;
        public const ushort G2C_GetDiceBoard = 13037;
        public const ushort C2G_GetKing = 13038;
        public const ushort G2C_GetKing = 13039;
        public const ushort C2G_GetPiggy = 13040;
        public const ushort G2C_GetPiggy = 13041;
        public const ushort C2G_GetBankruptcy = 13042;
        public const ushort G2C_GetBankruptcy = 13043;
        public const ushort SlotProductProto = 13044;
        public const ushort C2G_ListProducts = 13045;
        public const ushort G2C_ListProducts = 13046;
        public const ushort C2G_GetVipInfo = 13047;
        public const ushort G2C_GetVipInfo = 13048;
        public const ushort SlotOrderProto = 13049;
        public const ushort C2G_ListOrders = 13050;
        public const ushort G2C_ListOrders = 13051;
        public const ushort SlotMailProto = 13052;
        public const ushort C2G_ListMails = 13053;
        public const ushort G2C_ListMails = 13054;
        public const ushort C2G_ReadMail = 13055;
        public const ushort G2C_ReadMail = 13056;
        public const ushort C2G_ClaimAllMails = 13057;
        public const ushort G2C_ClaimAllMails = 13058;
        public const ushort C2G_DeleteMail = 13059;
        public const ushort G2C_DeleteMail = 13060;
        public const ushort C2G_DevSendMail = 13061;
        public const ushort G2C_DevSendMail = 13062;
        public const ushort SlotRankProto = 13063;
        public const ushort C2G_ListRank = 13064;
        public const ushort G2C_ListRank = 13065;
    }
}