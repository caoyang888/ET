namespace ET
{
    [EnableClass]
    public sealed class DevAdProofPort : IAdProofPort
    {
        public bool Verify(string proof)
        {
            return proof == "dev-ad";
        }
    }
}
