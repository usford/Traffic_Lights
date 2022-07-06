using System;
using Newtonsoft.Json;
using System.IO;

namespace Traffic_Lights.ConfigProgram {
    public class ConfigJson {
        public bool isSetup { get; set; } //Установлена ли программа
        public bool dropDatabase { get; set; } //Удалить ли базу данных
    }
}
