using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Tracer
{
    public interface ITracer 
    {
        void StartTrace();

        void StopTrace();

        TraceResult GetTraceResult();
    }
    
    [Serializable]
    public class TraceResult
    {
        public LinkedList<ThreadInfo> Threads = new LinkedList<ThreadInfo>();
        
        public TraceResult(){}
    }
    
    [Serializable]
    public class ThreadInfo
    {
        public int ThreadId = -1;
        public long ElapsedMs = 0;

        public LinkedList<MethodInfo> Methods = new LinkedList<MethodInfo>();

        public ThreadInfo(int threadId)
        {
            ThreadId = threadId;
        }

        public override bool Equals(object obj)
        {
            if (obj is ThreadInfo)
            {
                return ((ThreadInfo)obj).ThreadId == ThreadId;
            }

            return false;
        }
        
        public ThreadInfo(){}
    }
    
    [Serializable]
    public class MethodInfo
    {
        public string MethodName = "";
        public string ClassName = "";
        public long ElapsedMs = 0;
        public LinkedList<MethodInfo> Methods = new LinkedList<MethodInfo>();
        
        public MethodInfo(string methodName, string className)
        {
            MethodName = methodName;
            ClassName = className;
        }
        
        public MethodInfo(){}
    }
}