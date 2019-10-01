using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers; 
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 

namespace infiBanChecker
{ 
    sealed class Program
    {
        #region Reference Data Types 
        private static string steamID, infiToken, APIErrorMessage;
        #endregion

        #region EntryPoint
        static void Main()
        { 
            if (!File.Exists(Helpers._config))
            {
                int timeout = 10;//in seconds
                
                Console.WriteLine($"Unable to find file:( {Helpers._config} )");
                Console.Beep(3700, 500);//Im soo fucking sorry :crying-with-laughter: 
                Console.WriteLine("Press `ANY Key` To Exit");
                Console.ReadKey();
                while (timeout > 0)
                { 
                    timeout -= 1;
                    if (timeout < 1) break;
                    else
                    { 
                        Console.Clear();
                        Console.WriteLine($"Exiting in {timeout} Seconds...");
                        var twoSeconds = (2 * 60);
                        Thread.Sleep(twoSeconds);
                    } 
                } 
                Environment.Exit(0);
            }

            infiToken = (string)Helpers.getJsonValue("infiStarLic");

            Helpers.setupConsole(
                $"{Helpers._assembly.GetName().Name} | {infiToken}",
                w: 55, h: 20,
                col_bg: ConsoleColor.White,
                col_txt: ConsoleColor.Black
            );

            steamID = Helpers.getID;
            
          
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Helpers.endPoint); 
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string uri = $"{Helpers.endPoint}?license_token={infiToken}&uid={steamID}";
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
        #endregion
    }
}
