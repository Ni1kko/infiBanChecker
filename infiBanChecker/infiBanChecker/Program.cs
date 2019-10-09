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
        internal const string version = "3.0.0";
        #endregion
         
        #region EntryPoint
        private static async Task Main()
        {
            #region Subscribe Assembly Resolver
            AppDomain.CurrentDomain.AssemblyResolve += Utils.Helpers.getInstance().AssemblyResolver;
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
            var infiBanCheck = new InfiBanCheck();
            await infiBanCheck.Run();
            #endregion 
        }
        #endregion
        
    }
}
