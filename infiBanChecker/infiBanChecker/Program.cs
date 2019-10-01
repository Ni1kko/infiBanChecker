using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Newtonsoft.Json.Linq.JObject;

namespace infiBanChecker
{
    
    class Program
    {
        private protected const string endPoint = "https://api.infistar.de/arma/getGlobalBan"; 
        private protected static string steamID;
        private protected static string infiToken;
        
        static void Main(string[] args)
        {
            steamID = "76561198050103064";
            infiToken = "someTokenHere"; 
            string uri = $"{endPoint}?license_token={infiToken}&uid={steamID}";

            //Console.WriteLine(uri);
            //Console.ReadKey();
             
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

            Console.ReadKey();
        }
    }
}
