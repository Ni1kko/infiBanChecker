using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 

namespace infiBanChecker
{ 
    class Program
    { 
        private protected const string endPoint = "https://api.infistar.de/arma/getGlobalBan";
        private protected static string config = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
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
        private static object getJsonValue(string token)
        { 
            var jRead = File.OpenText(config);
            var jreader = new JsonTextReader(jRead);
            var jfile = (JObject)JToken.ReadFrom(jreader);
             
            if (!jfile.ContainsKey(token))
            {
                Console.WriteLine($"Unable to find Json Token:( '{token}' )");
                Console.Beep(1300, 200); 
                Console.WriteLine("Press `ANY Key` To Exit");
                Console.ReadKey(); 
                Environment.Exit(0);
            } 

            return jfile.GetValue(token).ToString();
        }

        static void Main()
        {
            if (!File.Exists(config))
            {
                Console.WriteLine($"Unable to find file:( {config} )");
                Console.Beep(3700, 500);//Im soo fucking sorry :crying-with-laughter: 
                Console.WriteLine("Exiting...");
                Thread.Sleep(400);
                Environment.Exit(0);
            }

            steamID = getID; 
            infiToken = (string)getJsonValue("infiStarLic"); 
            string uri = $"{endPoint}?license_token={infiToken}&uid={steamID}";
  
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(endPoint); 
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
            HttpResponseMessage APIresponse = client.GetAsync(uri).Result; 
            dynamic data = JObject.Parse(APIresponse.Content.ReadAsStringAsync().Result);
     
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
