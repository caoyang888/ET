using ET;

namespace ET.Test
{
    public class Slotops_Flows_Test : ATestHandler
    {
        public override async ETTask<int> Handle(TestContext context)
        {
            int code = SlotOpsChecks.Flows();
            await ETTask.CompletedTask;
            return code;
        }
    }
}
