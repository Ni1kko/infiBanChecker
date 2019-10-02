using System.Threading.Tasks; 

namespace infiBanChecker
{
    internal sealed class Program
    {
        #region EntryPoint
        private static async Task Main()
        {
            #region Setup the console window parameters
            Utils.Helpers.setupConsole(
                $"{Utils.Helpers._assembly.GetName().Name}",
                w: 65, h: 15,
                col_bg: System.ConsoleColor.White,
                col_txt: System.ConsoleColor.Black
            );
            #endregion

            #region Start BanChecker
            var infiBanCheck = new Utils.InfiBanCheck();
            await infiBanCheck.Run();
            #endregion 
        }
        #endregion 
    } 
}
