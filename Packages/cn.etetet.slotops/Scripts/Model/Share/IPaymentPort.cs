namespace ET
{
    public interface IPaymentPort
    {
        bool MarkPaid(OpsOrder order);
    }
}
