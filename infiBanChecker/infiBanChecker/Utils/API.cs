// ReSharper disable ArrangeAccessorOwnerBody
using System.Net.Http;
using SteamKit2;
using static infiBanChecker.Utils.Helpers;

namespace infiBanChecker.Utils
{ 
    internal abstract class ApiData
    {
        private protected SteamID _Steam64;
        private protected string _infiToken;
        private protected const string _endpoint = "https://api.infistar.de/";
        private protected HttpResponseMessage _apiResponse;
        private static string _statusCodeMessage;

        public virtual string infiToken
        {
            get => _infiToken;
            set => _infiToken = value;
        }
        public virtual SteamID steamID
        {
            get => _Steam64;
            set => _Steam64 = value;
        }
        public virtual HttpResponseMessage response
        {
            get => _apiResponse;
            set => _apiResponse = value;
        }
        public virtual string statusCodeMessage
        {
            get => _statusCodeMessage;
            set => _statusCodeMessage = value;
        }
        public virtual string endpoint => _endpoint;
        public virtual string uri => $"{_api.endpoint}?license_token={infiToken}";
    }

    internal class API : ApiData
    {
        public override string endpoint => $"{base.endpoint}arma/getGlobalBan";
        public override string uri => $"{base.uri}&uid={base.steamID}";
    }
}
