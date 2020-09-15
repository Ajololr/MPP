﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Tracer
{
    
    public class TracerImpl : ITracer
    {
        public readonly TraceResult Result = new TraceResult();
        private ConcurrentDictionary<int, Stack<MethodInfo>> _methodsListMap = new ConcurrentDictionary<int, Stack<MethodInfo>>();
        private ConcurrentStack<Stopwatch> _watch = new ConcurrentStack<Stopwatch>();
        public void StartTrace()
        {
            string className = "";
            string methodName = "";
            int threadId = -1;

            GetMethodData(out methodName, out className, out threadId);
            MethodInfo methodInfo = new MethodInfo(methodName, className);

            ThreadInfo threadInfo = new ThreadInfo(threadId);

            LinkedListNode<ThreadInfo> node = Result.Threads.Find(threadInfo);
            
            if (node == null)
            {
                node = Result.Threads.AddLast(threadInfo);
                node.Value.Methods.AddLast(methodInfo);
                _methodsListMap[threadId] = new Stack<MethodInfo>();
            }
            else
            {
                if (_methodsListMap[threadId].Count != 0)
                {
                    MethodInfo mf;
                    if (_methodsListMap[threadId].TryPeek(out mf))
                    {
                        mf.Methods.AddLast(methodInfo);
                    }
                }
                else
                {
                    node.Value.Methods.AddLast(methodInfo);
                }
            }
            
            _methodsListMap[threadId].Push(methodInfo);
            _watch.Push(System.Diagnostics.Stopwatch.StartNew());
        }

        public void StopTrace()
        {
            Stopwatch stopwatch;
            if (_watch.TryPop(out stopwatch))
            {
                stopwatch.Stop();
            }
            long elapsedMs = stopwatch.ElapsedMilliseconds;

            int threadId = GetThreadId();
            MethodInfo mf;
            if (_methodsListMap[threadId].TryPop(out mf))
            {
                mf.ElapsedMs = elapsedMs;
            }
            
            ThreadInfo threadInfo = new ThreadInfo(threadId);
            LinkedListNode<ThreadInfo> node = Result.Threads.Find(threadInfo);
            node.Value.ElapsedMs += elapsedMs;
        }

        public TraceResult GetTraceResult()
        {
            return Result;
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GetMethodData(out string methodName, out string className, out int threadId)
        {
            const int index = 2;
            
            var st = new StackTrace();
            var sf = st.GetFrame(index);
            
            MethodBase method = sf.GetMethod();
            
            className = method.ReflectedType.Name;
            methodName = method.Name;
            threadId = GetThreadId();
        }

        private int GetThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }
    }
}