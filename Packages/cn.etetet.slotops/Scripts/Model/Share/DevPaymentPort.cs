namespace ET
{
    [EnableClass]
    public sealed class DevPaymentPort : IPaymentPort
    {
        public bool MarkPaid(OpsOrder order)
        {
            order.Paid = true;
            return true;
        }
    }
}
