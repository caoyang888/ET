namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_GetProfileHandler : MessageSessionHandler<C2G_GetProfile, G2C_GetProfile>
    {
        protected override async ETTask Run(Session session, C2G_GetProfile request, G2C_GetProfile response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            long playerId = player.Id;
            string account = player.Account ?? string.Empty;
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 35, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.PlayerId = playerId;
            response.Account = account;
            response.Nickname = reply.Text ?? string.Empty;
            response.Avatar = reply.Text2 ?? string.Empty;
            response.VipLevel = reply.Value;
            response.VipExp = reply.Value2;
            response.RealNameVerified = reply.Value3;
            response.PhoneBound = reply.Value4;
            response.UnreadMail = (int)reply.Amount;
            response.ClaimableTask = (int)reply.Amount2;
            response.Maintenance = reply.Message == "1" ? 1 : 0;
            response.Message = string.Empty;
            response.Wallet.AddRange(reply.Wallet);
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_RenameHandler : MessageSessionHandler<C2G_Rename, G2C_Rename>
    {
        protected override async ETTask Run(Session session, C2G_Rename request, G2C_Rename response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 36, 0, 0, 0, request.Nickname, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
            response.Nickname = reply.Text ?? string.Empty;
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ListTasksHandler : MessageSessionHandler<C2G_ListTasks, G2C_ListTasks>
    {
        protected override async ETTask Run(Session session, C2G_ListTasks request, G2C_ListTasks response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 20, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Tasks.AddRange(reply.Tasks);
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_GetJubaoHandler : MessageSessionHandler<C2G_GetJubao, G2C_GetJubao>
    {
        protected override async ETTask Run(Session session, C2G_GetJubao request, G2C_GetJubao response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 21, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Level = reply.Value;
            response.ReadyUnix = reply.Amount;
            response.Gold = reply.Amount2;
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_GetDiceBoardHandler : MessageSessionHandler<C2G_GetDiceBoard, G2C_GetDiceBoard>
    {
        protected override async ETTask Run(Session session, C2G_GetDiceBoard request, G2C_GetDiceBoard response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 22, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Cell = reply.Value;
            response.Laps = reply.Value2;
            response.CellCount = reply.Value3;
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_GetKingHandler : MessageSessionHandler<C2G_GetKing, G2C_GetKing>
    {
        protected override async ETTask Run(Session session, C2G_GetKing request, G2C_GetKing response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 23, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Damage = reply.Amount;
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_GetPiggyHandler : MessageSessionHandler<C2G_GetPiggy, G2C_GetPiggy>
    {
        protected override async ETTask Run(Session session, C2G_GetPiggy request, G2C_GetPiggy response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 24, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Piggy = reply.Amount;
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_GetBankruptcyHandler : MessageSessionHandler<C2G_GetBankruptcy, G2C_GetBankruptcy>
    {
        protected override async ETTask Run(Session session, C2G_GetBankruptcy request, G2C_GetBankruptcy response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 25, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Claims = reply.Value;
            response.Limit = reply.Value2;
            response.Gold = reply.Amount;
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ListProductsHandler : MessageSessionHandler<C2G_ListProducts, G2C_ListProducts>
    {
        protected override async ETTask Run(Session session, C2G_ListProducts request, G2C_ListProducts response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 26, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Products.AddRange(reply.Products);
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_GetVipInfoHandler : MessageSessionHandler<C2G_GetVipInfo, G2C_GetVipInfo>
    {
        protected override async ETTask Run(Session session, C2G_GetVipInfo request, G2C_GetVipInfo response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 27, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Level = reply.Value;
            response.Exp = reply.Value2;
            response.CanRename = reply.Value3;
            response.CanIdle = reply.Value4;
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ListOrdersHandler : MessageSessionHandler<C2G_ListOrders, G2C_ListOrders>
    {
        protected override async ETTask Run(Session session, C2G_ListOrders request, G2C_ListOrders response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 28, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Orders.AddRange(reply.Orders);
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ListMailsHandler : MessageSessionHandler<C2G_ListMails, G2C_ListMails>
    {
        protected override async ETTask Run(Session session, C2G_ListMails request, G2C_ListMails response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 29, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Mails.AddRange(reply.Mails);
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ReadMailHandler : MessageSessionHandler<C2G_ReadMail, G2C_ReadMail>
    {
        protected override async ETTask Run(Session session, C2G_ReadMail request, G2C_ReadMail response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 30, 0, 0, request.MailId, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Message = reply.Message;
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ClaimAllMailsHandler : MessageSessionHandler<C2G_ClaimAllMails, G2C_ClaimAllMails>
    {
        protected override async ETTask Run(Session session, C2G_ClaimAllMails request, G2C_ClaimAllMails response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 31, 0, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Wallet.AddRange(reply.Wallet);
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_DeleteMailHandler : MessageSessionHandler<C2G_DeleteMail, G2C_DeleteMail>
    {
        protected override async ETTask Run(Session session, C2G_DeleteMail request, G2C_DeleteMail response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 32, 0, 0, request.MailId, string.Empty, string.Empty);
            response.Error = reply.Error;
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_DevSendMailHandler : MessageSessionHandler<C2G_DevSendMail, G2C_DevSendMail>
    {
        protected override async ETTask Run(Session session, C2G_DevSendMail request, G2C_DevSendMail response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 33, request.ItemId, 0, request.Count, request.Title, request.Body);
            response.Error = reply.Error;
            response.MailId = reply.Amount;
        }
    }

    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_ListRankHandler : MessageSessionHandler<C2G_ListRank, G2C_ListRank>
    {
        protected override async ETTask Run(Session session, C2G_ListRank request, G2C_ListRank response)
        {
            Slot2G_Ops reply = await SlotOpsForward.Call(session, 34, request.Period, 0, 0, string.Empty, string.Empty);
            response.Error = reply.Error;
            response.Rows.AddRange(reply.Ranks);
        }
    }
}
