// ReSharper disable ArrangeAccessorOwnerBody
using System.Net.Http;
using static infiBanChecker.Utils.Helpers;

namespace infiBanChecker.Utils
{ 
    internal abstract class ApiData
    {
        private protected string _infiToken, _infiSteam64;
        private protected const string _endpoint = "https://api.infistar.de/";
        private protected HttpResponseMessage _apiResponse;
        private static string _statusCodeMessage;

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
