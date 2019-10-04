using static infiBanChecker.Utils.Helpers;

namespace infiBanChecker
{
    internal sealed class InfiBanCheck
    {
        internal InfiBanCheck() => _api = new Utils.API();

        #region Run Infistar GlobalBan Checker 
        internal async System.Threading.Tasks.Task Run()
        {
            #region check config is present
            if (!await configExists()) await exitConsole(timeSeconds(2));
            #endregion

            #region loop the function for checking steam64's so we can search another 
            // loop is only active if the config does not contain the default infistar license token
            // change `while` to `if` for the app to close after you have got your result
            while (await checkInfiStarToken((string)jsonContainsKey("infiStarLic")))
            {
                await CheckSteam64(api: _api);
            }
            #endregion 
        }
        #endregion
    }
}
