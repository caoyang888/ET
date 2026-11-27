using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public static class SlotRoundSettler
    {
        public static SlotRoundResult Settle(SlotCatalog catalog, SlotPlayerState player, SlotSpinRequest request, SlotRoomLedgerBook ledgers)
        {
            SlotRoundResult result = new SlotRoundResult();
            result.PlayerId = player.PlayerId;
            result.ClientSpinId = request.ClientSpinId;
            result.RoomId = request.RoomId;
            result.StakeId = request.StakeId;
            result.LineCount = request.LineCount;
            result.Seed = request.Seed;

            SlotRoomDef room;
            SlotStakeDef stake;
            SlotMachineDef machine;
            if (!catalog.Rooms.TryGetValue(request.RoomId, out room) || !catalog.Machines.TryGetValue(room.MachineId, out machine))
            {
                result.Error = SlotFail.RoomMissing;
                return result;
            }

            if (!catalog.Stakes.TryGetValue(request.StakeId, out stake) || stake.RoomId != room.RoomId)
            {
                result.Error = SlotFail.StakeMissing;
                return result;
            }

            int lineCount = machine.Evaluator == SlotEvaluatorKind.Cascade ? 0 : request.LineCount;
            if (machine.Evaluator == SlotEvaluatorKind.Payline && (lineCount < 1 || lineCount > room.MaxLines))
            {
                result.Error = SlotFail.LineCount;
                return result;
            }

            SlotProgressState progress = Progress(catalog, player, room.ProgressGroup);
            if (progress.Cleared)
            {
                result.Error = SlotFail.StageCleared;
                return result;
            }

            bool free = progress.FreeSpins > 0;
            if (free && (progress.LockedStakeId != stake.StakeId || progress.LockedLineCount != lineCount))
            {
                result.Error = SlotFail.FreeStakeLocked;
                return result;
            }

            long totalStake = free ? 0 : machine.Evaluator == SlotEvaluatorKind.Cascade ? stake.BetGold : stake.BetGold * lineCount;
            if (!free && player.Get(catalog.Rule.GoldItemId) < totalStake)
            {
                result.Error = SlotFail.NotEnoughGold;
                return result;
            }

            SlotStageDef stage;
            if (!catalog.Stages.TryGetValue(progress.StageId, out stage))
            {
                result.Error = SlotFail.MachineMissing;
                return result;
            }

            if (!free)
            {
                player.Add(catalog.Rule.GoldItemId, -totalStake);
                player.TotalBet += totalStake;
                player.TotalCost += totalStake;
            }
            else
            {
                progress.FreeSpins -= 1;
            }

            result.UsedFreeSpin = free;
            result.StakeGold = totalStake;
            SlotRng rng = new SlotRng(request.Seed);
            SlotRoomLedger stock = ledgers.GetOrCreate(room.RoomId, room.InitialPublicStock);
            double chance = SlotRtpComposer.Compose(catalog, player, room, lineCount, stock.PublicStock, rng);
            result.HitChance = chance;
            bool forced = false;
            bool win;
            if (progress.PityCooldown > 0)
            {
                progress.PityCooldown -= 1;
                win = rng.NextDouble() < chance;
            }
            else if (progress.MissStreak >= catalog.Pity.MissThreshold)
            {
                win = true;
                forced = true;
                progress.MissStreak = 0;
                progress.PityCooldown = catalog.Pity.CooldownSpins;
            }
            else
            {
                win = rng.NextDouble() < chance;
            }

            result.ForcedByPity = forced;
            if (!win)
            {
                progress.MissStreak += 1;
            }
            else if (!forced)
            {
                progress.MissStreak = 0;
            }

            int[] grid = SlotBoardGenerator.Generate(catalog, machine, win, lineCount <= 0 ? 1 : lineCount, rng);
            List<SlotCascadeStep> steps;
            if (machine.Evaluator == SlotEvaluatorKind.Cascade)
            {
                steps = SlotCascadeEvaluator.Evaluate(catalog, machine, grid, rng);
            }
            else
            {
                SlotCascadeStep step = new SlotCascadeStep();
                for (int i = 0; i < grid.Length; ++i)
                {
                    step.Cells.Add(grid[i]);
                }

                SlotPaylineEvaluator.Evaluate(catalog, machine, grid, lineCount, step.Wins, step.Features);
                steps = new List<SlotCascadeStep> { step };
            }

            result.Steps = steps;
            long milliSum = 0;
            int multiplierFace = 0;
            int jackpotSymbol = 0;
            int jackpotCount = 0;
            int freeGrant = 0;
            foreach (SlotCascadeStep step in steps)
            {
                foreach (SlotWinLine lineWin in step.Wins)
                {
                    milliSum += lineWin.MilliMultiplier;
                    lineWin.Amount = stake.BetGold * lineWin.MilliMultiplier / 1000;
                    result.Payout += lineWin.Amount;
                }

                foreach (SlotFeatureAward feature in step.Features)
                {
                    if (feature.Kind == SlotSymbolKind.LineFeature && feature.FaceMilli > 0)
                    {
                        feature.Amount = stake.BetGold * feature.FaceMilli / 1000;
                        result.Payout += feature.Amount;
                    }

                    if (feature.Kind == SlotSymbolKind.FreeSpin)
                    {
                        int grant;
                        if (machine.FreeSpinsByCount.TryGetValue(feature.Count, out grant) && grant > freeGrant)
                        {
                            freeGrant = grant;
                        }
                    }

                    if (feature.Kind == SlotSymbolKind.JackpotCounter || feature.Kind == SlotSymbolKind.JackpotTrigger)
                    {
                        if (feature.Count > jackpotCount)
                        {
                            jackpotCount = feature.Count;
                            jackpotSymbol = feature.SymbolId;
                        }
                    }

                    if (feature.Kind == SlotSymbolKind.CascadeMultiplier && step.Wins.Count > 0)
                    {
                        for (int n = 0; n < feature.Count; ++n)
                        {
                            multiplierFace += RollFace(catalog, rng);
                        }
                    }
                }
            }

            if (multiplierFace > 0 && result.Payout > 0)
            {
                result.Payout = result.Payout * multiplierFace;
            }

            if (freeGrant > 0)
            {
                progress.FreeSpins = freeGrant;
                progress.LockedStakeId = stake.StakeId;
                progress.LockedLineCount = lineCount;
            }

            ledgers.Apply(room, totalStake, result.Payout, result);
            int rate = JackpotRate(catalog, room.RoomId, jackpotSymbol, jackpotCount, stake.BetGold);
            if (rate > 0)
            {
                long taken = ledgers.TryTakeJackpot(room.RoomId, rate);
                result.JackpotTaken = taken;
                result.Payout += taken;
                result.JackpotDelta -= taken;
            }

            if (catalog.Rule.PiggyMilli > 0 && totalStake > 0)
            {
                long piggy = totalStake * catalog.Rule.PiggyMilli / 1000;
                if (piggy > 0)
                {
                    result.Grants.Add(new SlotItemGrant(90001, piggy));
                }
            }

            player.Add(catalog.Rule.GoldItemId, result.Payout);
            player.TotalWin += result.Payout;
            long damage = SlotDamageResolver.Resolve(catalog, stake, stage, result.Payout, totalStake == 0 ? stake.BetGold : totalStake, free);
            if (damage > progress.BossHp)
            {
                damage = progress.BossHp;
            }

            result.Damage = damage;
            progress.BossHp -= damage;
            player.TotalDamage += damage;
            result.StageId = progress.StageId;
            result.BossHp = progress.BossHp;
            if (progress.BossHp <= 0)
            {
                result.StageCleared = true;
                Grant(player, result, stage.ClearReward);
                if (stage.NextStageId <= 0 || !catalog.Stages.ContainsKey(stage.NextStageId))
                {
                    progress.Cleared = true;
                    progress.BossHp = 0;
                    Grant(player, result, machine.ClearReward);
                }
                else
                {
                    SlotStageDef next = catalog.Stages[stage.NextStageId];
                    progress.StageId = next.StageId;
                    progress.BossHp = next.HpMax;
                    result.StageId = next.StageId;
                    result.BossHp = next.HpMax;
                }
            }

            result.FreeSpinsLeft = progress.FreeSpins;
            return result;
        }

        private static int JackpotRate(SlotCatalog catalog, int roomId, int symbolId, int count, long bet)
        {
            int rate = 0;
            foreach (SlotJackpotRule rule in catalog.Jackpots)
            {
                if (rule.RoomId != roomId || rule.SymbolId != symbolId || count < rule.MinCount || bet < rule.MinBet)
                {
                    continue;
                }

                if (rule.Count == 0 || rule.Count == count)
                {
                    if (rule.RateMilli > rate)
                    {
                        rate = rule.RateMilli;
                    }
                }
            }

            return rate;
        }

        private static int RollFace(SlotCatalog catalog, SlotRng rng)
        {
            if (catalog.Rule.MultiplierFaces.Count == 0)
            {
                return 1;
            }

            List<int> weights = new List<int>();
            for (int i = 0; i < catalog.Rule.MultiplierFaces.Count; ++i)
            {
                weights.Add(catalog.Rule.MultiplierFaces[i].Weight);
            }

            return catalog.Rule.MultiplierFaces[rng.Pick(weights)].Face;
        }

        private static SlotProgressState Progress(SlotCatalog catalog, SlotPlayerState player, int group)
        {
            SlotProgressState progress;
            if (player.Progress.TryGetValue(group, out progress))
            {
                return progress;
            }

            SlotStageDef first = catalog.FirstStage(group);
            progress = new SlotProgressState();
            progress.StageId = first == null ? 0 : first.StageId;
            progress.BossHp = first == null ? 0 : first.HpMax;
            player.Progress[group] = progress;
            return progress;
        }

        private static void Grant(SlotPlayerState player, SlotRoundResult result, List<SlotItemGrant> grants)
        {
            for (int i = 0; i < grants.Count; ++i)
            {
                player.Add(grants[i].ItemId, grants[i].Count);
                result.Grants.Add(new SlotItemGrant(grants[i].ItemId, grants[i].Count));
            }
        }
    }
}
