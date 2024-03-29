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
        private static bool isUrlParametersValid, isGlobalBanned, isTokenOk = false;
        #endregion

        #region Resolve Embedded Assemblies
        internal Assembly AssemblyResolver(object sender, ResolveEventArgs args)
        {
            var askedAssembly = new AssemblyName(args.Name);

            lock (this)
            { 
                var stream = _assembly.GetManifestResourceStream($"infiBanChecker.EmbeddedAssemblies.{askedAssembly.Name}.dll");
                if (stream == null) return null;

                Assembly assembly = null; 
                try
                {
                    var assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    assembly = Assembly.Load(assemblyData);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Loading embedded assembly: {1}{0}Has thrown a unhandled exception: {2}", Environment.NewLine, askedAssembly.Name, e);
                    _ = exitConsole(10);//unsure on this i've never used discards before.
                }
                finally
                {
                    if(assembly != null)
                        Console.WriteLine("Loaded embedded assembly: {0}", askedAssembly.Name); 
                }
                return assembly;
            }
        }
        #endregion
         
        #region Setup Console 

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

        #region Seconds To MilSeconds 
        internal static int timeSeconds(int seconds)
        {
            return (seconds * 1000);
        }
        #endregion

        #region Read SteamID64 from Console
        private static string readSid64FromConsole
        {
            get
            {
                Console.Clear();
                Console.WriteLine("{0}" + Environment.NewLine, EnterSteam64Message);
                Console.WriteLine("{0}: ", Steam64);
                return Console.ReadLine();
            }
        }
        #endregion

        #region CheckSteamID64
        internal static async Task CheckSteam64(API api)
        {
            #region Check SteamID is valid SteamID number 
            
            var ci = readSid64FromConsole;  
            while (Regex.Matches(ci, @"[a-zA-Z]").Count > 0)
            {
                ci = readSid64FromConsole;
                if (Regex.Matches(ci, @"[a-zA-Z]").Count < 0) break; 
            }
             
            api.steamID = ulong.Parse(ci); 
             
            if (api.steamID.IsValid)
            {
                Console.Clear();
                Console.WriteLine("{0}: `{1}` {2}" + Environment.NewLine, ValidatingSteam64Message, api.steamID, ValidatingWithAPIMessage);
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
                    Console.WriteLine("{0}: {1}" + Environment.NewLine, ResultFoundMessage, data?.message?.uid);

                    if (isGlobalBanned) 
                        Console.WriteLine("{0} | {1}" + Environment.NewLine, BanGlobal, data?.message?.bandate);
                    else 
                        Console.WriteLine("{0}" + Environment.NewLine, BanClean);
                }
                #endregion
            }
            else
            {
                _api.statusCodeMessage = data?.message ?? api?.response.ReasonPhrase;
                Console.WriteLine("{0} ({1})" + Environment.NewLine, (int)api?.response?.StatusCode, _api.statusCodeMessage);
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
          
        #region Check InfiStar Token 
        internal static async Task<bool> checkInfiStarToken(string tokenFromJson)
        {
            if (isTokenOk) return true;
            isTokenOk = !isTokenOk;
           
            if (Json.getInstance().Generated || tokenFromJson == "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
            {
                Console.Clear();
                Console.WriteLine("{1} {0}{0}{2}{0}{3}{0}", Environment.NewLine, EditDefaultConfigMessage, ConfigCanBeFoundMessage, _config);
                await exitConsole();
            }
            else
            {
                // infistar license to short
                if (tokenFromJson.Length < 32)
                {
                    Console.Clear();
                    Console.WriteLine("{1} {0}{0}{2}{0}", Environment.NewLine, InfiLicenseTooShortMessage, CheckAndTryAgainMessage);
                    await exitConsole();
                }
                else
                { 
                    // infistar license invalid format 
                    if (!string.IsNullOrEmpty(tokenFromJson) && !Regex.IsMatch(tokenFromJson, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled))
                    {
                        Console.Clear();
                        Console.WriteLine("{1} {0}{0}{2}{0}", Environment.NewLine, InfiLicenseInvalidMessage, CheckAndTryAgainMessage);
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
            Console.WriteLine(AnyKeyToExitMessage);
            Console.ReadKey();
            while (timeout > 0)
            {
                timeout -= 1;
                if (timeout < 1) break;
                else
                {
                    Console.Clear();
                    Console.WriteLine("{0} {1} {2}...", ExitingInMessage, timeout, Seconds);
                    await Task.Delay(Helpers.timeSeconds(1));
                }
            }
            Environment.Exit(0);
        } 
        #endregion
    }
}
