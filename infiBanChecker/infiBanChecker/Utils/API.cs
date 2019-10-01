 namespace infiBanChecker.Utils
{
    internal class API : Endpoint
    {
        public override string uri => $"{base.uri}&uid={base.steamID}";
    }
}
