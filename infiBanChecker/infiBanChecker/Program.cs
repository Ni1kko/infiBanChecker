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
        public static Assembly _assembly = Assembly.GetExecutingAssembly();
        private const string endPoint = "https://api.infistar.de/arma/getGlobalBan";
        private static string steamID, infiToken, APIErrorMessage;
        private static readonly string config = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{ _assembly.GetName().Name}.json");
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
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;

        #endregion

        #region Import c++ Helper Libs  
        [DllImport("user32.dll")] private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")] private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)] private static extern IntPtr GetConsoleWindow();
        #endregion

        #region Helpers   
        private static void setupConsole(string title, int h, int w, ConsoleColor col_bg = ConsoleColor.Black, ConsoleColor col_txt = ConsoleColor.White)
        {
            Console.Title = title;
            Console.BackgroundColor = col_bg;
            Console.ForegroundColor = col_txt;
            Console.WindowHeight = h;
            Console.BufferHeight = h;
            Console.WindowWidth = w;
            Console.BufferWidth = w;

            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
        }
        #endregion

        #region EntryPoint
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

            setupConsole(
                $"InfiBanCheck | {infiToken}",
                w: 55, h: 20,
                col_bg: ConsoleColor.White,
                col_txt: ConsoleColor.Black
            );

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
        #endregion
    }
}
