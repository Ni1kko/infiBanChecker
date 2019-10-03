namespace infiBanChecker.Localization { 
    using static System.Threading.Thread;

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Language {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Language() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("infiBanChecker.Localization.Language", typeof(Language).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
         
        /// <summary>
        ///   Looks up a localized string similar to Press `ANY Key` To Exit.
        /// </summary>
        internal static string AnyKeyToExitMessage {
            get {
                return ResourceManager.GetString("AnyKeyToExitMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Press `ANY Key` To Lookup another ID.
        /// </summary>
        internal static string AnyKeyToSearchAnotherMessage {
            get {
                return ResourceManager.GetString("AnyKeyToSearchAnotherMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Clean.
        /// </summary>
        internal static string BanClean {
            get {
                return ResourceManager.GetString("BanClean", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GlobalBan.
        /// </summary>
        internal static string BanGlobal {
            get {
                return ResourceManager.GetString("BanGlobal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to please check and try again.
        /// </summary>
        internal static string CheckAndTryAgainMessage {
            get {
                return ResourceManager.GetString("CheckAndTryAgainMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to config file can be found here.
        /// </summary>
        internal static string ConfigCanBeFoundMessage {
            get {
                return ResourceManager.GetString("ConfigCanBeFoundMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must edit `infiStarLic` and add you license token.
        /// </summary>
        internal static string EditDefaultConfigMessage {
            get {
                return ResourceManager.GetString("EditDefaultConfigMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter The SteamID You Want To Check GlobalBan Status For.
        /// </summary>
        internal static string EnterSteam64Message {
            get {
                return ResourceManager.GetString("EnterSteam64Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exiting in.
        /// </summary>
        internal static string ExitingInMessage {
            get {
                return ResourceManager.GetString("ExitingInMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to infiStarLic license token format invalid.
        /// </summary>
        internal static string InfiLicenseInvalidMessage {
            get {
                return ResourceManager.GetString("InfiLicenseInvalidMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to infiStarLic license token is too short.
        /// </summary>
        internal static string InfiLicenseTooShortMessage {
            get {
                return ResourceManager.GetString("InfiLicenseTooShortMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find Json Token.
        /// </summary>
        internal static string JsonTokenMissingMessage {
            get {
                return ResourceManager.GetString("JsonTokenMissingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found Result For SteamID.
        /// </summary>
        internal static string ResultFoundMessage {
            get {
                return ResourceManager.GetString("ResultFoundMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Seconds.
        /// </summary>
        internal static string Seconds {
            get {
                return ResourceManager.GetString("Seconds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SteamID64.
        /// </summary>
        internal static string Steam64 {
            get {
                return ResourceManager.GetString("Steam64", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validating SteamID.
        /// </summary>
        internal static string ValidatingSteam64Message {
            get {
                return ResourceManager.GetString("ValidatingSteam64Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to With Infistar&apos;s API.
        /// </summary>
        internal static string ValidatingWithAPIMessage {
            get {
                return ResourceManager.GetString("ValidatingWithAPIMessage", resourceCulture);
            }
        }

        /// <summary>
        /// Culture Codes
        /// </summary>
        internal enum CultureCode
        {
            English,
            Danish,
            German,
            French,
            Russian,
            Polish
        }

        /// <summary>
        /// Defines current culture code
        /// </summary>
        internal static string CurrentCultureCode
        {
            get
            {
                return CurrentThread.CurrentUICulture.Name;
            }
        }

        /// <summary>
        /// Returns Culture code enum as string
        /// </summary>
        /// <param name="cc">Culture code enum</param>
        /// <returns></returns>
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
    }
}
