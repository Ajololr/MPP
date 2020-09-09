using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Tracer;
using XmlSerializer = Tracer.XmlSerializer;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            TracerImpl tracerImpl = new TracerImpl();
            
            Foo foo = new Foo(tracerImpl);
            foo.MyMethod();
            foo.AnotherMethod();
            
            Other other = new Other(tracerImpl);
            Thread InstanceCaller = new Thread(
                new ThreadStart(other.OtherMethod));
            InstanceCaller.Start();
            
            Bar bar = new Bar(tracerImpl);
            bar.InnerMethod();

            InstanceCaller.Join();
                
            TraceResult traceResult = tracerImpl.GetTraceResult();
            
            ISerializer<TraceResult> serializer = new XmlSerializer();
            serializer.stringify(traceResult);
            serializer.writeToConsole();
            serializer.saveToFile();
        }
    }
    
    public class Foo
    {
        private Bar _bar;
        private ITracer _tracer;

        internal Foo(ITracer tracer)
        {
            _tracer = tracer;
            _bar = new Bar(_tracer);
        }
    
        public void MyMethod()
        {
            _tracer.StartTrace();
            
            _bar.InnerMethod();

            _tracer.StopTrace();
        }
        
        public void AnotherMethod()
        {
            _tracer.StartTrace();
            
            Other other =new Other(_tracer);
            other.OtherMethod();

            _tracer.StopTrace();
        }
    }

    public class Bar
    {
        private ITracer _tracer;

        internal Bar(ITracer tracer)
        {
            _tracer = tracer;
        }
    
        public void InnerMethod()
        {
            _tracer.StartTrace();

            for (int i = 0; i < 10000000; i++)
            {
                
            }
            
            _tracer.StopTrace();
        }
    }
    
    public class Other
    {
        private ITracer _tracer;

        internal Other(ITracer tracer)
        {
            _tracer = tracer;
        }
    
        public void OtherMethod()
        {
            _tracer.StartTrace();

            Thread.Sleep(20);
            
            _tracer.StopTrace();
        }
    }
}