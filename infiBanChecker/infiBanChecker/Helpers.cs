 
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace infiBanChecker
{
    internal sealed class Helpers
    {
        #region Reference Data Types 
        internal static Assembly _assembly = Assembly.GetExecutingAssembly();
        internal const string endPoint = "https://api.infistar.de/arma/getGlobalBan";
        internal static readonly string _config = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{ _assembly.GetName().Name}.json");
        #endregion

        #region Read Steam64 from Console
        internal static string getID
        {
            get
            {
                Console.Clear();
                Console.WriteLine("Enter The SteamID You Want To Check Ban Status For\n");
                Console.WriteLine("SteamID64: ");
                return Console.ReadLine();
            }
        }
        #endregion

        #region Read Token Value From Json
        internal static object getJsonValue(string token)
        {
            var jRead = File.OpenText(_config);
            JObject jfile;
            using (var jreader = new JsonTextReader(jRead))
            {
                jfile = JToken.ReadFrom(jreader) as JObject;
                if (jfile == null || !jfile.ContainsKey(token))
                {
                    Console.WriteLine($"Unable to find Json Token:( '{token}' )");
                    Console.WriteLine("Press `ANY Key` To Exit");
                    Console.ReadKey();
                    Thread.SpinWait(10);
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
    }
}
