using System;
using Traffic_Lights.ConfigProgram;

namespace Traffic_Lights.Interfaces {
    public interface IConfigHandler {
        public ConfigJson ConfigJson { get; set; }
        public string PathToDirectory { get; }
        public string PathToExcelFiles { get; }
        public string PathToConfig { get; }
        public string PathToSvgElements { get; }
        public ConfigJson Initialize();
        public void Update();
    }
}
