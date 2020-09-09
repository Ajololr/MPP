using System.Collections.Generic;

namespace Tracer
{
    public class TraceResult
    {
        private readonly LinkedList<ThreadInfo> _threads = new LinkedList<ThreadInfo>();

        public LinkedList<ThreadInfo> Threads => _threads;
    }
    
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

        public override int GetHashCode()
        {
            return ThreadId;
        }
    }

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
    }
}