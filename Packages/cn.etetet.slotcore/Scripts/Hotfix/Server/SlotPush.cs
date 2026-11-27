namespace ET.Server
{
    public static class SlotPush
    {
        public static void Wallet(SlotPlayer player)
        {
            G2C_PushWallet message = G2C_PushWallet.Create();
            SlotProtoMap.FillWallet(message.Wallet, player.Spin);
            Send(player, message);
        }

        public static void Progress(SlotPlayer player, int roomId, int stageId, long bossHp, long bossHpMax, int freeSpins)
        {
            G2C_PushProgress message = G2C_PushProgress.Create();
            message.RoomId = roomId;
            message.StageId = stageId;
            message.BossHp = bossHp;
            message.BossHpMax = bossHpMax;
            message.FreeSpins = freeSpins;
            message.TotalDamage = player.Spin.TotalDamage;
            Send(player, message);
        }

        public static void Mail(SlotPlayer player, long mailId, string title)
        {
            G2C_PushMail message = G2C_PushMail.Create();
            message.MailId = mailId;
            message.Title = title ?? string.Empty;
            Send(player, message);
        }

        public static void Notice(SlotPlayer player, int kind, string title, string body)
        {
            G2C_PushNotice message = G2C_PushNotice.Create();
            message.Kind = kind;
            message.Title = title ?? string.Empty;
            message.Body = body ?? string.Empty;
            Send(player, message);
        }

        public static void Kick(SlotPlayer player, int reason, string text)
        {
            G2C_PushKick message = G2C_PushKick.Create();
            message.Reason = reason;
            message.Message = text ?? string.Empty;
            Send(player, message);
        }

        private static void Send(SlotPlayer player, IMessage message)
        {
            if (string.IsNullOrEmpty(player.GateIp) || player.GateInstance == 0)
            {
                return;
            }

            ActorId actorId = new ActorId(
                new Address(player.GateIp, player.GatePort),
                new FiberInstanceId { Fiber = player.GateFiber, InstanceId = player.GateInstance });
            player.Root().GetComponent<MessageSender>().Send(actorId, message);
        }
    }
}
