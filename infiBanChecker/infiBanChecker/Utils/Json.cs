using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace infiBanChecker.Utils
{
    public struct JsonData
    {
        [JsonProperty("InfistarToken")]
        public string InfistarAccessToken { get; set; }
    }

    public class Json
    { 
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
            return File.Exists(Helpers._config);
        }
        #endregion
    }
}
