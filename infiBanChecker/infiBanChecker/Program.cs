using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks; 
using infiBanChecker.Utils;

namespace infiBanChecker
{ 
    sealed class Program
    { 
        #region EntryPoint
        private static async Task Main()
        {
            //Setup Console Window
            Helpers.setupConsole(
                $"{Helpers._assembly.GetName().Name}",
                w: 65, h: 15,
                col_bg: ConsoleColor.White,
                col_txt: ConsoleColor.Black
            );

            //Check Config Is Present
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
                        await Task.Delay(twoSeconds);
                    } 
                } 
                Environment.Exit(0);
            }
         
            //Setup api info
            API api = new API();
            api.infiToken = (string)Helpers.getJsonValue("infiStarLic"); 
      
            //Append Infistar Token To Title
            Console.Title += $" | {api.infiToken}";

            //Loop
            while (true)
            {
                await Helpers.CheckSteam64(api: api);
            }
             
        }
        #endregion
    }
}
