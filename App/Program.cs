using System.Threading;
using Tracer;

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
            Thread instanceCaller = new Thread(
                new ThreadStart(other.OtherMethod));
            instanceCaller.Start();
            
            Bar bar = new Bar(tracerImpl);
            bar.InnerMethod();

            instanceCaller.Join();
                
            TraceResult traceResult = tracerImpl.GetTraceResult();
            
            ISerializer<TraceResult> serializer = new JsonSerializer();
            serializer.stringify(traceResult);
            serializer.writeToConsole();
            serializer.saveToFile();
            
            serializer = new XmlSerializer();
            serializer.stringify(traceResult);
            serializer.writeToConsole();
            serializer.saveToFile();
        }
    }
    
    public class Foo
    {
        private Bar _bar;
        private ITracer _tracer;

        public Foo(ITracer tracer)
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

        public Bar(ITracer tracer)
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

        public Other(ITracer tracer)
        {
            _tracer = tracer;
        }
    
        public void OtherMethod()
        {
            _tracer.StartTrace();

            Thread.Sleep(100);
            
            _tracer.StopTrace();
        }
    }
}