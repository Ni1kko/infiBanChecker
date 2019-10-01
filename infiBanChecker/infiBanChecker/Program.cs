using System;
using System.Net.Http;
using System.Net.Http.Headers;
using static Newtonsoft.Json.Linq.JObject;

namespace infiBanChecker
{ 
    class Program
    {
        private protected const string endPoint = "https://api.infistar.de/arma/getGlobalBan"; 
        private protected static string steamID;
        private protected static string infiToken;
        public static string APIErrorMessage;
        private static string getID
        {
            get
            {
                Console.Clear();
                Console.WriteLine("Enter The SteamID You Want To Check Ban Status For\n");
                Console.WriteLine("SteamID64: ");
                return Console.ReadLine();
            }
        }

        static void Main()
        {
            steamID = getID; 
            infiToken = "someTokenHere"; 
            string uri = $"{endPoint}?license_token={infiToken}&uid={steamID}";
  
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(endPoint); 
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
            HttpResponseMessage APIresponse = client.GetAsync(uri).Result; 
            dynamic data = Parse(APIresponse.Content.ReadAsStringAsync().Result);
     
            if (APIresponse.IsSuccessStatusCode)
            { 
                if (data.status == "success")
                { 
                    Console.Clear();
                    Console.WriteLine($"Found Result For SteamID: {data.message.uid}\r\n");
                    Console.WriteLine(
                        $"{((data.message.state == "1") ? ("GlobalBan | " + data.message.bandate + "\r\n") : "Clean\r\n")}");
                } 
            }
            else
            {
                APIErrorMessage = data.message;
                APIErrorMessage = APIErrorMessage.Length < 1 ? APIresponse.ReasonPhrase : data.message;
                Console.WriteLine("{0} ({1})\r\n", (int)APIresponse.StatusCode, APIErrorMessage); 
            }

            Console.ReadKey();
        }
    }
}
