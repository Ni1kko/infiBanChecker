using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace infiBanChecker.Utils
{
    public struct JsonData
    { 
        [JsonProperty("InfistarToken")]
        public string InfistarAccessToken { get; set; }
    }

    public class Json
    {
        public string[] keys = { "InfistarToken" }; 
        public JsonData config;
        public bool Generated = false;

        #region Class constructor
        private Json()
        {
            if (exists())
            #region Load config 
            {
                LoadConfig();
            }
            #endregion
            else
            #region Create & Load config 
            { 
                if (File.Exists(Helpers._config)) File.Delete(Helpers._config);

                Generated = true;

                using (var writer = new StreamWriter(File.Create(Helpers._config)))
                {
                    var data = new JsonData
                    {
                        // default infistar token 
                        InfistarAccessToken = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
                    };

                    writer.Write(JsonConvert.SerializeObject(data, Formatting.Indented));
                }

                LoadConfig();
            }
            #endregion
        }
        public static Json getInstance() { return new Json(); }
        #endregion

        #region Load config. 
        private void LoadConfig()
        {
            using (var reader = new StreamReader(File.OpenRead(Helpers._config)))
            {
                config = JsonConvert.DeserializeObject<JsonData>(reader.ReadToEnd());
            }
        }
        #endregion
         
        #region Checks config exists.
        private bool exists()
        {
            if (File.Exists(Helpers._config))
            {
                var jfile = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(Helpers._config)));

                foreach (string key in keys)
                {  
                    if (jfile == null || !jfile.ContainsKey(key))
                    {
                        Console.WriteLine("{0}:( '{1}' )", Localization.Language.JsonKeyMissingMessage, key);
                        Console.WriteLine(Localization.Language.AnyKeyToExitMessage);
                        Console.ReadKey();
                        Task.Delay(Helpers.timeSeconds(2));
                        Environment.Exit(0);
                    }
                }

                return true;
            } 

            return false; 
        }
        #endregion

         
    }
}
