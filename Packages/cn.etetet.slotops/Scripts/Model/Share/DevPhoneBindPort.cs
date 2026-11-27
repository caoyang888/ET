namespace ET
{
    [EnableClass]
    public sealed class DevPhoneBindPort : IPhoneBindPort
    {
        public bool Bind(string number, string code)
        {
            return number != null && number.Length >= 6 && code == "000000";
        }
    }
}
