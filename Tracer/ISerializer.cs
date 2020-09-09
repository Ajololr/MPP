using System.IO;

namespace Tracer
{
    public interface ISerializer<T>
    {
        string stringify(T obj);
        void saveToFile();
        void writeToConsole();
    }
}