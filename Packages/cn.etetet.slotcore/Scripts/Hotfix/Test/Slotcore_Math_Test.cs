using ET;

namespace ET.Test
{
    public class Slotcore_Math_Test : ATestHandler
    {
        public override async ETTask<int> Handle(TestContext context)
        {
            int code = SlotMathChecks.Payline();
            if (code != 0)
            {
                return code;
            }

            code = SlotMathChecks.FreeSpinAndFeatures();
            if (code != 0)
            {
                return code;
            }

            code = SlotMathChecks.CascadeCoordinates();
            if (code != 0)
            {
                return code;
            }

            code = SlotMathChecks.RtpAndPity();
            if (code != 0)
            {
                return code;
            }

            code = SlotMathChecks.DamageCap();
            if (code != 0)
            {
                return code;
            }

            code = SlotMathChecks.IdempotentSpin();
            if (code != 0)
            {
                return code;
            }

            code = SlotMathChecks.LedgerAndJackpot();
            await ETTask.CompletedTask;
            return code;
        }
    }
}
