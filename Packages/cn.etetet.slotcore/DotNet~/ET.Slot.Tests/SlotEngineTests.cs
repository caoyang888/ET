using ET.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ET.SlotTests
{
    [TestClass]
    public class SlotEngineTests
    {
        [TestMethod]
        public void Payline_LeftToRight_ReportsCells()
        {
            Assert.AreEqual(0, SlotMathChecks.Payline());
        }

        [TestMethod]
        public void FreeSpin_AndSpecialSymbols_AreDetected()
        {
            Assert.AreEqual(0, SlotMathChecks.FreeSpinAndFeatures());
        }

        [TestMethod]
        public void Cascade_ReturnsCoordinates()
        {
            Assert.AreEqual(0, SlotMathChecks.CascadeCoordinates());
        }

        [TestMethod]
        public void RtpPolicy_AndPity_ForceAWin()
        {
            Assert.AreEqual(0, SlotMathChecks.RtpAndPity());
        }

        [TestMethod]
        public void Damage_RespectsFloorAndCap()
        {
            Assert.AreEqual(0, SlotMathChecks.DamageCap());
        }

        [TestMethod]
        public void Spin_IsIdempotent()
        {
            Assert.AreEqual(0, SlotMathChecks.IdempotentSpin());
        }

        [TestMethod]
        public void Ledger_IsConcurrent_AndJackpotDoesNotOverdraw()
        {
            Assert.AreEqual(0, SlotMathChecks.LedgerAndJackpot());
        }

        [TestMethod]
        public void Ops_TaskShopVipBoardMailRank()
        {
            Assert.AreEqual(0, SlotOpsChecks.Flows());
        }
    }
}
