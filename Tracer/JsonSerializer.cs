using System;
using System.IO;
using Newtonsoft.Json;

namespace Tracer
{
    public class JsonSerializer : ISerializer<TraceResult>
    {
        private const string FILE_PATH = "Json output.txt";
        private string _json;
        public string Json => _json;

        public string stringify(TraceResult obj)
        {
            _json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return _json;
        }

        public void saveToFile()
        {
            using ( FileStream fileStream = File.Create(FILE_PATH))
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(_json);
                fileStream.Write(array);
            }
        }

        public void writeToConsole()
        {
            Console.WriteLine(_json);
        }
    }
}