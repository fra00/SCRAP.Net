using Newtonsoft.Json;
using Robot.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot
{
    public static class LogFileForEmulator
    {
        public static void WriteCommand(string command,string fileName) {
            // Serializza l'array modificato in una nuova stringa json
            string newJson = JsonConvert.SerializeObject(new             {
                Command=command
            });
            // Scrivi la nuova stringa json su un file di testo
            File.WriteAllText(fileName, newJson);

        }

        public static void Write(string textJson, string fileName)
        {
            // Scrivi la nuova stringa json su un file di testo
            File.WriteAllText(fileName, textJson);
        }
    }
}
