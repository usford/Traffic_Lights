﻿using System;
using System.IO;
using Newtonsoft.Json;
using Traffic_Lights.Interfaces;
using Traffic_Lights.Views;

namespace Traffic_Lights.ConfigProgram {
    public class ConfigHandler : IConfigHandler {
        public ConfigJson ConfigJson { get; set; }      
        public string PathToDirectory { get; }  
        public string PathToSvgElements { get; }
        public string PathToExcelFiles { get; }
        public string PathToConfig { get; }
        public string PathToProject { get; }
        public ConfigHandler() {
            bool isDevelopment = false;

            PathToDirectory = isDevelopment
                ? new DirectoryInfo(@"..\..\..").FullName
                : new DirectoryInfo(@"..").FullName + @"Светофор\Traffic_Lights";

            PathToSvgElements = $@"{PathToDirectory}\Элементы схемы";
            PathToExcelFiles = $@"{PathToDirectory}\Excel файлы";
            PathToConfig = $@"{PathToDirectory}\ConfigProgram\config.json";
            PathToProject = new DirectoryInfo(@"..").FullName + @"Светофор";
            //task1
            //Для разработки
            //PathToProject = @"E:\";
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
