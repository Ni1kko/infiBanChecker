﻿using System;
using System.IO; 
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices; 
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using static infiBanChecker.Localization.Language;
using static System.Threading.Thread;

namespace infiBanChecker.Utils
{
    internal sealed class Helpers
    {
        internal static Helpers getInstance() { return new Helpers(); }
        
        #region Reference Data Types  
        internal static API _api;
        internal static Assembly _assembly = typeof(Helpers).Assembly;   
        internal static readonly string _config = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{ _assembly.GetName().Name}.json");
        private static bool isUrlParametersValid, isGlobalBanned, isSteam64Error, isTokenOk = false;
        #endregion

        #region Resolve Embedded Assemblies
        internal Assembly AssemblyResolver(object sender, ResolveEventArgs args)
        {
            AssemblyName askedAssembly = new AssemblyName(args.Name);

            lock (this)
            {
                Assembly assembly = null;
                Stream stream = _assembly.GetManifestResourceStream($"infiBanChecker.EmbeddedAssemblies.{askedAssembly.Name}.dll");

                if (stream != null)
                {
                    var assemblyData = new byte[stream.Length];
                    try
                    {
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        assembly = Assembly.Load(assemblyData);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Loading embedded assembly: {0}\r\nHas thrown a unhandled exception: {1}", askedAssembly.Name, e);
                        //Console.ReadKey();
                    }
                    finally
                    {
                        if(assembly != null)
                            Console.WriteLine("Loaded embedded assembly: {0}", askedAssembly.Name);
                    }
                }
                return assembly;
            }
        }
        #endregion

        #region Read Steam64 from Console
        private static string readSid64FromConsole
        {
            get
            { 
                Console.Clear();
                Console.WriteLine("{0}\n", Localization.Language.EnterSteam64Message);
                Console.WriteLine("{0}: ", Localization.Language.Steam64);
                return Console.ReadLine();
            }
        }
        #endregion
  
        #region SetupConsole 

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
            #region Slightly increase console width depending on the users language 
            if(CurrentCultureCode == GetCultureCode(CultureCode.English))
                w += 0;
            else 
            if(CurrentCultureCode == GetCultureCode(CultureCode.Danish))
                w += 0;
            else 
            if(CurrentCultureCode == GetCultureCode(CultureCode.German))
                w += 20;
            else
            if (CurrentCultureCode == GetCultureCode(CultureCode.French))
                w += 10;
            else
            if (CurrentCultureCode == GetCultureCode(CultureCode.Russian))
                w += 25;
            else
            if (CurrentCultureCode == GetCultureCode(CultureCode.Polish))
                w += 10;
            else
                w += 30;
            #endregion
             
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
            HttpClient client = new HttpClient();

            #region Attempt connection
            try
            { 
                client.BaseAddress = new Uri(api.endpoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with connection with {api.endpoint} : {ex.Message} -> {ex.InnerException?.Message}");
            }
            finally
            {
                #region Add accept header for JSON 
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );
                #endregion  
            }
            #endregion

            return client;
        }
        #endregion

        #region Webclient Response Result
        
        private static HttpResponseMessage getUriResponseResult(string apiQuery, HttpClient webClient)
        {
            HttpResponseMessage response = null;

            #region Attempt Communication
            try
            {
                response = webClient.GetAsync(apiQuery).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error communicating with {_api.endpoint} : {ex.Message} -> {ex.InnerException?.Message}");
            }
            #endregion

            return response;
        } 
        
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
                    Console.WriteLine("{0}: `{1}` {2}\n", Localization.Language.ValidatingSteam64Message, api.steamID, Localization.Language.ValidatingWithAPIMessage);
                    break;
                }
            }
            #endregion

            #region Connect to api and get response   
            HttpClient webClientInstance = connectToEndpoint(api);
            api.response = getUriResponseResult( // Blocking call! Code will wait here until a response is received or a timeout occurs.
                webClient: webClientInstance,
                apiQuery: (api.uri)
                );
            #endregion

            #region Parse the response
            dynamic data = null;
            try
            {
                data = JObject.Parse(api?.response?.Content.ReadAsStringAsync()?.Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing response from {api.endpoint}: {ex.Message} -> {ex.InnerException?.Message}");
            }
            #endregion

            #region Check response
            if (api.response.IsSuccessStatusCode)
            {
                #region Check infistars API responded with success 

                isUrlParametersValid = (data?.status == "success");

                if (isUrlParametersValid)
                {
                    isGlobalBanned = (data?.message.state == "1");
                    Console.Clear(); 
                    Console.WriteLine("{0}: {1}\r\n", ResultFoundMessage, data?.message?.uid);

                    if (isGlobalBanned) 
                        Console.WriteLine("{0} | {1}\r\n", BanGlobal, data?.message?.bandate);
                    else 
                        Console.WriteLine("{0}\r\n", BanClean);
                }
                #endregion
            }
            else
            {
                _api.statusCodeMessage = (data?.message.Length < 1) ?  api?.response.ReasonPhrase : data?.message;
                Console.WriteLine("{0} ({1})\r\n", (int)api?.response?.StatusCode, _api.statusCodeMessage);
            }
            #endregion

            #region Wait for user input
            Console.WriteLine(AnyKeyToSearchAnotherMessage);
            Console.ReadKey();
            #endregion

            #region Cleanup  
            try
            {
                Console.Clear();
                webClientInstance.Dispose();//HttpClient instance is disposed automatically when the application terminates so the following call is to dispose old client for searching another steamid.
                await Task.Delay(400);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disposing connection with {api.endpoint}: {ex.Message} -> {ex.InnerException?.Message}");
            } 
            #endregion
        }
        #endregion

        #region Seconds To MilSeconds 
        internal static int timeSeconds(int seconds)
        {
            return (seconds * 1000);
        }
        #endregion

        #region Culture Code
        internal enum CultureCode
        {
            English,
            Danish,
            German,
            French,
            Russian,
            Polish
        }

        internal static string CurrentCultureCode => CurrentThread.CurrentUICulture.Name;

        internal static string GetCultureCode(CultureCode cc)
        {
            switch (cc)
            {
                case CultureCode.English:
                    return "en-EN";
                case CultureCode.Danish:
                    return "da-DK";
                case CultureCode.German:
                    return "da-DK";
                case CultureCode.French:
                    return "br-FR";
                case CultureCode.Russian:
                    return "ru-RU";
                case CultureCode.Polish:
                    return "pl-PL";
                default:
                    return "";
            }
        }
   

    #endregion

        #region Check InfiStar Token 
        internal static async Task<bool> checkInfiStarToken(string tokenFromJson)
        {
            if (isTokenOk) return true;
            isTokenOk = !isTokenOk;
           
            if (Json.getInstance().Generated || tokenFromJson == "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
            { 
                Console.WriteLine("{0} \n{1}\r\n\r\n{2}\r\n\r\n", EditDefaultConfigMessage, ConfigCanBeFoundMessage, _config);
                await exitConsole();
            }
            else
            {
                // infistar license to short
                if (tokenFromJson.Length< 30)
                { 
                       Console.WriteLine("{0}\r\n{1}\r\n\r\n", InfiLicenseTooShortMessage, CheckAndTryAgainMessage);
                    await exitConsole();
                }
                else
                {
                    // infistar license invalid format
                    if (!(bool) stringContainsInteger(tokenFromJson)[0])
                    {
                        Console.WriteLine("{0}\r\n{1}\r\n\r\n", InfiLicenseInvalidMessage, CheckAndTryAgainMessage);
                        await exitConsole();
                    }
                    else
                    {
                        //Set infistar token
                        _api.infiToken = tokenFromJson;

                        //Append infistar token to the console title
                        Console.Title += $" | {_api.infiToken}";
                    } 
                } 
            }
            return true;
        }
        #endregion
         
        #region ExitConsole
        internal static async Task exitConsole(int timeout = 10)
        {
            Console.WriteLine(Localization.Language.AnyKeyToExitMessage);
            Console.ReadKey();
            while (timeout > 0)
            {
                timeout -= 1;
                if (timeout < 1) break;
                else
                {
                    Console.Clear();
                    Console.WriteLine("{0} {1} {2}...", Localization.Language.ExitingInMessage, timeout, Localization.Language.Seconds);
                    await Task.Delay(Helpers.timeSeconds(1));
                }
            }
            Environment.Exit(0);
        } 
        #endregion
    }
}
