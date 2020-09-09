using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Tracer
{
    public class XmlSerializer : ISerializer<TraceResult>
    {
        private const string FILE_PATH = "XML output.txt";
        private readonly XDocument xdoc = new XDocument();

        public string stringify(TraceResult traceResult)
        {
            XElement root = new XElement("root");

            foreach (ThreadInfo threadInfo in traceResult.Threads)
            {
                XElement thread = new XElement("thread");
                
                XAttribute threadId = new XAttribute("id", threadInfo.ThreadId);
                XAttribute threadTimeMs = new XAttribute("timeMs", threadInfo.ElapsedMs);
                thread.Add(threadId, threadTimeMs);
                
                AddMethods(thread, threadInfo.Methods);
                
                root.Add(thread);
            }
            
            xdoc.Add(root);

            return xdoc.ToString();
        }
        
        private static void AddMethods(XElement element, LinkedList<MethodInfo> list)
        {
            foreach (MethodInfo methodInfo in list)
            {
                XElement method = new XElement("method");
                
                XAttribute methodName = new XAttribute("name", methodInfo.MethodName);
                XAttribute methodClass = new XAttribute("class", methodInfo.ClassName);
                XAttribute methodTimeMs = new XAttribute("timeMs", methodInfo.ElapsedMs);
                
                method.Add(methodName, methodClass, methodTimeMs);
                
                AddMethods(method, methodInfo.Methods);
                
                element.Add(method);
            }
        }

        public void saveToFile()
        {
            using FileStream fileStream = File.Create(FILE_PATH);
            xdoc.Save(fileStream);
        }

        public void writeToConsole()
        {
            Console.WriteLine(xdoc.ToString());
        }
    }
}