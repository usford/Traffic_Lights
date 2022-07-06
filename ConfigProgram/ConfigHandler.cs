using System;
using System.IO;
using Newtonsoft.Json;

namespace Traffic_Lights.ConfigProgram {
    public class ConfigHandler {
        public ConfigJson ConfigJson { get; set; }
        //Для разработки
        public readonly string PathToDirectory = new DirectoryInfo(@"..\..\..").FullName;
        //Для установки
        //public readonly string PathToDirectory = new DirectoryInfo(@"..").FullName + @"Traffic_Lights";
        public readonly string PathToSvgElements;
        public readonly string PathToExcelFiles;
        private readonly string _pathToConfig;
        public ConfigHandler() {
            PathToSvgElements = $@"{PathToDirectory}\Элементы схемы";
            PathToExcelFiles = $@"{PathToDirectory}\Excel файлы";
            _pathToConfig = $@"{PathToDirectory}\ConfigProgram\config.json";

            ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(File.ReadAllText(_pathToConfig))
                ?? throw new ArgumentNullException(nameof(ConfigJson));
        }
        public void Update() {
            string output = JsonConvert.SerializeObject(ConfigJson, Formatting.Indented);
            File.WriteAllText(_pathToConfig, output);
        }
    }
}
