using System; 
using System.Threading.Tasks;
namespace infiBanChecker
{
    internal sealed class Program
    { 
        #region Version information
        //      Major Version
        //      Minor Version
        //      Build Number 
        internal const string version = "2.1.1";
        #endregion
         
        #region EntryPoint
        private static async Task Main()
        {  
            #region Subscibe AssemblyResolve to resolve embedded assemblies
            AppDomain.CurrentDomain.AssemblyResolve += Utils.Helpers.CurrentDomain_AssemblyResolve; 
            #endregion

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
