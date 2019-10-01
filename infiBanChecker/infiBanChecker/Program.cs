using System;
using System.IO;
using System.Threading.Tasks; 
using infiBanChecker.Utils;

namespace infiBanChecker
{ 
   
    sealed class Program
    { 
        #region EntryPoint
        private static async Task Main()
        {
            // new instance of the api class
            Helpers.api = new API();

            // setup console window
            Helpers.setupConsole(
                $"{Helpers._assembly.GetName().Name}",
                w: 65, h: 15,
                col_bg: ConsoleColor.White,
                col_txt: ConsoleColor.Black
            );
             
            // check config is present
            if (!File.Exists(Helpers._config))
            {
                //Todo: Create default blank config if its not present
                Console.WriteLine($"Unable to find file\r\n\r\n{Helpers._config}\r\n\r\n");
                Console.Beep(3700, 500);//Im soo fucking sorry :crying-with-laughter: 
                await Helpers.exitConsole(30);
            }
             
            // loop if infistar license token is not default
            while (await Helpers.checkInfiStarToken((string)Helpers.getJsonValue("infiStarLic")))
            {
                await Helpers.CheckSteam64(api: Helpers.api);
            } 
        }
        #endregion
    }
}
