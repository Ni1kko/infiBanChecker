﻿using System;
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
            if (await Helpers.configExists())
            {
                // loop if infistar license token is not default
                while (await Helpers.checkInfiStarToken((string)Helpers.jsonContainsKey("infiStarLic")))
                {
                    await Helpers.CheckSteam64(api: Helpers.api);
                }
            }
        }
        #endregion
    }
}
