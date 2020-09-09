using System;
using System.IO;
using Newtonsoft.Json;

namespace Tracer
{
    public class JsonSerializer : ISerializer<TraceResult>
    {
        private string _json;

        public string Json
        {
            get => _json;
            set => _json = value;
        }

        public string stringify(TraceResult obj)
        {
            _json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return _json;
        }

        public void saveToFile()
        {
            using ( FileStream fileStream = File.Create("Json output.txt"))
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