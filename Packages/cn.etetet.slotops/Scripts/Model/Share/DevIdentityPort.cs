namespace ET
{
    [EnableClass]
    public sealed class DevIdentityPort : IIdentityPort
    {
        public bool Verify(string name, string number)
        {
            return !string.IsNullOrEmpty(name) && number != null && number.Length >= 6;
        }
    }
}
