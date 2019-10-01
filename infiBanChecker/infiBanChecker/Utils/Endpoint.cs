// ReSharper disable ArrangeAccessorOwnerBody
namespace infiBanChecker.Utils
{
    internal abstract class Endpoint
    {
        private protected string _infiToken;
        private protected string _infiSteam64;

        public virtual string infiToken
        {
            get => _infiToken;
            set => _infiToken = value;
        }
        public virtual string steamID
        {
            get => _infiSteam64;
            set => _infiSteam64 = value;
        }
        public virtual string endpoint
        {
            get => "https://api.infistar.de/arma/getGlobalBan";
        }
        public virtual string uri
        {
            get => $"{endpoint}?license_token={infiToken}";
        }
    }
}
