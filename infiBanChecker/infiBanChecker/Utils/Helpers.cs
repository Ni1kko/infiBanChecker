﻿using System;
using System.IO; 
using System.Net.Http; 
using System.Runtime.InteropServices; 
using System.Text.RegularExpressions;
using System.Threading.Tasks;  
using Newtonsoft.Json.Linq;

namespace infiBanChecker.Utils
{
    internal sealed class Helpers
    {
        #region Reference Data Types  
        internal static System.Reflection.Assembly _assembly = typeof(Helpers).Assembly;  
        internal static API api; 
        internal static readonly string _config = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{ _assembly.GetName().Name}.json");
        private static string APIErrorMessage;
        private static bool isUrlParametersValid, isGlobalBanned, isSteam64Error, isTokenOk = false;
        private static HttpResponseMessage APIresponse;
        #endregion

        #region Resolve Embedded Assemblies
        internal static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var stream = _assembly.GetManifestResourceStream("infiBanChecker.EmbeddedAssemblies.Newtonsoft.Json.dll");
            var assemblyData = new Byte[stream.Length];
            stream.Read(assemblyData, 0, assemblyData.Length);
            return System.Reflection.Assembly.Load(assemblyData);
        }
        #endregion

        #region Read Steam64 from Console
        private static string readSid64FromConsole
        {
            get
            { 
                Console.Clear();
                Console.WriteLine("{0}\n", Properties.Localization.EnterSteam64Message);
                Console.WriteLine("{0}: ", Properties.Localization.Steam64);
                return Console.ReadLine();
            }
        }
        #endregion

        #region Read Token Value From Json
        internal static object jsonContainsKey(string token)
        {
            var jRead = File.OpenText(_config);
            JObject jfile;
            using (var jreader = new Newtonsoft.Json.JsonTextReader(jRead))
            {
                jfile = JToken.ReadFrom(jreader) as JObject;
                if (jfile == null || !jfile.ContainsKey(token))
                { 
                    Console.WriteLine("{0}:( '{1}' )", Properties.Localization.JsonTokenMissingMessage, token);
                    Console.WriteLine(Properties.Localization.AnyKeyToExitMessage);
                    Console.ReadKey();
                    Task.Delay(timeSeconds(2));
                    Environment.Exit(0);
                }
                return jfile.GetValue(token).ToString();
            } 
        }
        #endregion

        #region setupConsole 

        #region Parameters
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;
        #endregion

        #region Import c++ Libs  
        [DllImport("user32.dll")] private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")] private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)] private static extern IntPtr GetConsoleWindow();
        #endregion

        internal static void setupConsole(string title, int h, int w, ConsoleColor col_bg = ConsoleColor.Black, ConsoleColor col_txt = ConsoleColor.White)
        {
            //Slightly increase console width if localization in not english or danish
            w = (Program.localization == "en-EN" || Program.localization == "da-DK") ? w : (w + 15);

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

        #region stringContainsInteger 
        internal static object[] stringContainsInteger(string stringInteger) => new object[] { (Regex.Matches(stringInteger, @"[a-zA-Z]").Count > 0), stringInteger };
        #endregion

        #region Webclient Connect 
        private static HttpClient connectToEndpoint(API api)
        {
            #region Setup Http Instance
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(api.endpoint);
            #endregion

            #region Add accept header for JSON 
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
            );
            #endregion  

            return client;
        }
        #endregion

        #region Webclient Response Result 
        private static HttpResponseMessage getUriResponseResult(string apiQuery, HttpClient webClient) => webClient.GetAsync(apiQuery).Result;
        #endregion

        #region CheckSteamID 
        internal static async Task CheckSteam64(API api)
        {
            #region Check SteamID is valid SteamID number  
            api.steamID = "";
            isSteam64Error = false;
            while (api.steamID.Length < 17 || api.steamID.Length > 17 || isSteam64Error)
            {
                var stringCheck = stringContainsInteger(readSid64FromConsole);
                isSteam64Error = (bool)stringCheck[0];
                api.steamID = (string)stringCheck[1];
                if (api.steamID.Length == 17 && !isSteam64Error)
                {
                    Console.Clear(); 
                    Console.WriteLine("{0}: `{1}` {2}\n", Properties.Localization.ValidatingSteam64Message, api.steamID, Properties.Localization.ValidatingWithAPIMessage);
                    break;
                }
            }
            #endregion

            #region Connect to api and get response   
            HttpClient webClientInstance = connectToEndpoint(api);
            APIresponse = getUriResponseResult( // Blocking call! Code will wait here until a response is received or a timeout occurs.
                webClient: webClientInstance,
                apiQuery: (api.uri)
                );
            #endregion

            #region Parse the response
            dynamic data = JObject.Parse(APIresponse.Content.ReadAsStringAsync().Result);
            isUrlParametersValid = (data.status == "success");
            #endregion

            #region Check response
            if (APIresponse.IsSuccessStatusCode)
            {
                #region Check infistars API responded with success    
                if (isUrlParametersValid)
                {
                    isGlobalBanned = (data.message.state == "1");
                    Console.Clear(); 
                    Console.WriteLine("{0}: {1}\r\n", Properties.Localization.ResultFoundMessage, data.message.uid);

                    if (isGlobalBanned) 
                        Console.WriteLine("{0} | {1}\r\n", Properties.Localization.BanGlobal, data.message.bandate);
                    else 
                        Console.WriteLine("{0}\r\n", Properties.Localization.BanClean);
                }
                #endregion
            }
            else
            {
                APIErrorMessage = data.message;
                APIErrorMessage = APIErrorMessage.Length < 1 ? APIresponse.ReasonPhrase : data.message;
                Console.WriteLine("{0} ({1})\r\n", (int)APIresponse.StatusCode, APIErrorMessage);
            }
            #endregion

            #region Wait for user input
            Console.WriteLine(Properties.Localization.AnyKeyToSearchAnotherMessage);
            Console.ReadKey();
            #endregion

            #region Cleanup 
            Console.Clear();
            webClientInstance.Dispose();//HttpClient instance is disposed automatically when the application terminates so the following call is to dispose old client for searching another steamid.
            await Task.Delay(400);
            #endregion
        }
        #endregion

        #region Seconds To MilSeconds 
        internal static int timeSeconds(int seconds)
        {
            return (seconds * 1000);
        }
        #endregion

        #region Check InfiStar Token 
        internal static async Task<bool> checkInfiStarToken(string tokenFromJson)
        {
            if (isTokenOk) return true;
            isTokenOk = !isTokenOk;
           
            if (tokenFromJson == "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
            { 
                Console.WriteLine("{0} \n{1}\r\n\r\n{2}\r\n\r\n", Properties.Localization.EditDefaultConfigMessage, Properties.Localization.ConfigCanBeFoundMessage, _config);
                await exitConsole();
            }
            else
            {
                // infistar license to short
                if (tokenFromJson.Length< 30)
                { 
                       Console.WriteLine("{0}\r\n{1}\r\n\r\n", Properties.Localization.InfiLicenseTooShortMessage, Properties.Localization.CheckAndTryAgainMessage);
                    await exitConsole();
                }
                else
                {
                    // infistar license invalid format
                    if (!(bool) stringContainsInteger(tokenFromJson)[0])
                    {

                        Console.WriteLine("{0}\r\n{1}\r\n\r\n", Properties.Localization.InfiLicenseInvalidMessage, Properties.Localization.CheckAndTryAgainMessage);
                        await exitConsole();
                    }
                    else
                    {
                        //Set infistar token
                        api.infiToken = tokenFromJson;

                        //Append infistar token to the console title
                        Console.Title += $" | {api.infiToken}";
                    } 
                } 
            }
            return true;
        }
        #endregion

        #region config
        /// temp
        /// Todo: Use jsonWriter 
        /// <summary>
        /// writes token & value too Json
        /// </summary>
        private static async Task<bool> configWriter(string[] arr)
        {
            var _jsonOut = new System.Text.StringBuilder(arr.Length);
            
            foreach (var a in arr)
            {
                var aSplit = a.Split(':');
                _jsonOut.Append($"\t\"{aSplit[0]}\":\"{aSplit[1]}\",\r\n"); 
            }

            if (_jsonOut.Length > 1)
            {
                File.WriteAllText(
                    contents: "{\r\n\r\n" + 
                                _jsonOut +
                              "\r\n}", 
                    path: _config 
               );  
            }

            return true;
        }

        /// <summary>
        /// create default config
        /// </summary>
        private static async Task<bool> createConfig()
        {  
            bool mismatch = false; 
            string[] jsonData = {
                "infiStarLic:xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            };

            if (!File.Exists(_config))
            {
                configWriter(jsonData).Wait(); 
                using (var jreader = new Newtonsoft.Json.JsonTextReader(File.OpenText(_config)))
                {
                    JObject jfile = JToken.ReadFrom(jreader) as JObject;
                    foreach (var data in jsonData)
                    {
                        if (mismatch) break;
                        if (jfile == null || !jfile.ContainsKey((string)jsonContainsKey(data.Split(':')[0])))
                        {
                            mismatch = !mismatch;
                        }
                    }
                }
            } 
            return mismatch;
        }

        /// <summary>
        /// checks config exists if not try create default config
        /// </summary> 
        internal static async Task<bool> configExists()
        { 
            if (!File.Exists(_config))
            {
                if (!await createConfig())
                {
                    Console.WriteLine("unable to create config\r\nplease make sure app is in a folder where it has permissions and try again\r\n\r\n");
                    await exitConsole();
                }
            }
            return File.Exists(_config);
        }

        #endregion

        #region exitConsole
        internal static async Task exitConsole(int timeout = 10)
        {
            Console.WriteLine(Properties.Localization.AnyKeyToExitMessage);
            Console.ReadKey();
            while (timeout > 0)
            {
                timeout -= 1;
                if (timeout < 1) break;
                else
                {
                    Console.Clear();
                    Console.WriteLine("{0} {1} {2}...", Properties.Localization.ExitingInMessage, timeout, Properties.Localization.Seconds);
                    await Task.Delay(Helpers.timeSeconds(1));
                }
            }
            Environment.Exit(0);
        } 
        #endregion
    }
}
