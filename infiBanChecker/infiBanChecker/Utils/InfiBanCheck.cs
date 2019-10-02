namespace infiBanChecker.Utils
{
    internal sealed class InfiBanCheck
    {
        internal InfiBanCheck() => Helpers.api = new API();

        #region Run Infistar GlobalBan Checker 
        internal async System.Threading.Tasks.Task Run()
        {
            #region check config is present
            if (!await Helpers.configExists()) return;
            #endregion

            #region loop the function for checking steam64's so we can search another 
            // loop is only active if the config does not contain the default infistar license token
            // change `while` to `if` for the app to close after you have got your result
            while (await Helpers.checkInfiStarToken((string)Helpers.jsonContainsKey("infiStarLic")))
            {
                await Helpers.CheckSteam64(api: Helpers.api);
            }
            #endregion 
        }
        #endregion
    }
}
