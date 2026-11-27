using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 房间账本。公共库存和排行资金按押注计提，大奖池按赢分计提。锁保证并发入账不会丢更新。
    /// </summary>
    [EnableClass]
    public sealed class SlotRoomLedgerBook
    {
        private readonly object gate = new object();
        private readonly Dictionary<int, SlotRoomLedger> rooms = new Dictionary<int, SlotRoomLedger>();

        public SlotRoomLedger GetOrCreate(int roomId, long initialStock)
        {
            lock (this.gate)
            {
                SlotRoomLedger ledger;
                if (!this.rooms.TryGetValue(roomId, out ledger))
                {
                    ledger = new SlotRoomLedger();
                    ledger.RoomId = roomId;
                    ledger.PublicStock = initialStock;
                    this.rooms[roomId] = ledger;
                }

                return Copy(ledger);
            }
        }

        public void Apply(SlotRoomDef room, long bet, long win, SlotRoundResult result)
        {
            lock (this.gate)
            {
                SlotRoomLedger ledger = this.Mutable(room);
                long stock = Amount(room.PublicStockBase, room.PublicStockMilli, bet, win);
                long jackpot = Amount(room.JackpotBase, room.JackpotMilli, bet, win);
                long rank = Amount(room.RankPoolBase, room.RankPoolMilli, bet, win);
                ledger.PublicStock += stock;
                ledger.Jackpot += jackpot;
                ledger.RankPool += rank;
                ledger.Version += 1;
                result.PublicStockDelta = stock;
                result.JackpotDelta = jackpot;
                result.RankPoolDelta = rank;
            }
        }

        public long TryTakeJackpot(int roomId, int rateMilli)
        {
            lock (this.gate)
            {
                SlotRoomLedger ledger;
                if (!this.rooms.TryGetValue(roomId, out ledger) || rateMilli <= 0 || ledger.Jackpot <= 0)
                {
                    return 0;
                }

                long amount = ledger.Jackpot * rateMilli / 1000;
                if (amount > ledger.Jackpot)
                {
                    amount = ledger.Jackpot;
                }

                ledger.Jackpot -= amount;
                ledger.Version += 1;
                return amount;
            }
        }

        public SlotRoomLedger Snapshot(int roomId)
        {
            lock (this.gate)
            {
                SlotRoomLedger ledger;
                if (!this.rooms.TryGetValue(roomId, out ledger))
                {
                    return new SlotRoomLedger { RoomId = roomId };
                }

                return Copy(ledger);
            }
        }

        private SlotRoomLedger Mutable(SlotRoomDef room)
        {
            SlotRoomLedger ledger;
            if (!this.rooms.TryGetValue(room.RoomId, out ledger))
            {
                ledger = new SlotRoomLedger();
                ledger.RoomId = room.RoomId;
                ledger.PublicStock = room.InitialPublicStock;
                this.rooms[room.RoomId] = ledger;
            }

            return ledger;
        }

        private static long Amount(SlotLedgerBaseKind basis, int milli, long bet, long win)
        {
            long baseAmount = basis == SlotLedgerBaseKind.Win ? win : bet;
            if (milli <= 0 || baseAmount <= 0)
            {
                return 0;
            }

            return baseAmount * milli / 1000;
        }

        private static SlotRoomLedger Copy(SlotRoomLedger ledger)
        {
            return new SlotRoomLedger
            {
                RoomId = ledger.RoomId,
                PublicStock = ledger.PublicStock,
                Jackpot = ledger.Jackpot,
                RankPool = ledger.RankPool,
                Version = ledger.Version,
            };
        }
    }
}
