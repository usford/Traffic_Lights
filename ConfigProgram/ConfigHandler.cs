using System;
using System.IO;
using Newtonsoft.Json;
using Traffic_Lights.Interfaces;

namespace Traffic_Lights.ConfigProgram {
    public class ConfigHandler : IConfigHandler {
        public ConfigJson ConfigJson { get; set; }      
        public string PathToDirectory { get; }  
        public string PathToSvgElements { get; }
        public string PathToExcelFiles { get; }
        public string PathToConfig { get; }
        public ConfigHandler() {
            bool isDevelopment = true;

            PathToDirectory = isDevelopment
                ? new DirectoryInfo(@"..\..\..").FullName
                : new DirectoryInfo(@"..").FullName + @"Traffic_Lights";

            PathToSvgElements = $@"{PathToDirectory}\Элементы схемы";
            PathToExcelFiles = $@"{PathToDirectory}\Excel файлы";
            PathToConfig = $@"{PathToDirectory}\ConfigProgram\config.json";

            ConfigJson = Initialize(); 
        }
        public ConfigJson Initialize() {
            var configJson = JsonConvert.DeserializeObject<ConfigJson>(File.ReadAllText(PathToConfig))
                ?? throw new ArgumentNullException(nameof(ConfigJson));
            return configJson;
        }
        public void Update() {
            string output = JsonConvert.SerializeObject(ConfigJson, Formatting.Indented);
            File.WriteAllText(PathToConfig, output);
        }
    }
}
