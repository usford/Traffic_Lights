using System;
using System.Diagnostics;
using System.IO;

namespace Traffic_Lights
{
    public class DebugLogger
    {
        private Stopwatch sw;
        
        public DebugLogger()
        {
            sw = new Stopwatch();
        }

        public void Start()
        {
            sw.Start();
        }
        public void Write(string msg)
        {
            using (var file = new StreamWriter("DebugTL.txt", true))
            {
                file.WriteAsync($"Время: {DateTime.Now}\n");
                file.WriteAsync($"{msg} {sw.Elapsed}\n\n");
            }
        }
        public void Stop()
        {
            sw.Stop();
        }
    }
}
