using System.Threading;
using App;
using Tracer;
using Xunit;

namespace Tests
{
    public class TraceResultTests
    {
        [Fact]
        public void InitialThreadCountShouldBeZero()
        {
            ITracer tracer = new TracerImpl();
            TraceResult result = tracer.GetTraceResult();
            
            Assert.Empty(result.Threads);
        }
        
        [Fact]
        public void MethodCountShouldBeTwo()
        {
            ITracer tracer = new TracerImpl();
            Bar bar = new Bar(tracer);
            bar.InnerMethod();
            bar.InnerMethod();
            
            TraceResult result = tracer.GetTraceResult();
            
            Assert.Equal(2, result.Threads.Last?.Value.Methods.Count);
        }
        
        [Fact]
        public void OtherMethodExecutionTimeShouldBeMoreOrEqual100()
        {
            ITracer tracer = new TracerImpl();
            Other other = new Other(tracer);
            other.OtherMethod();
            
            TraceResult result = tracer.GetTraceResult();
            
            Assert.True( result.Threads.Last?.Value.ElapsedMs >= 100);
        }
        
        [Fact]
        public void MethodNameShouldBeCorrect()
        {
            ITracer tracer = new TracerImpl();
            Foo foo = new Foo(tracer);
            foo.MyMethod();
            
            TraceResult result = tracer.GetTraceResult();
            
            Assert.Equal("MyMethod", result.Threads.Last?.Value.Methods.Last?.Value.MethodName);
        }
        
        [Fact]
        public void ClassNameShouldBeCorrect()
        {
            ITracer tracer = new TracerImpl();
            Foo foo = new Foo(tracer);
            foo.MyMethod();
            
            TraceResult result = tracer.GetTraceResult();
            
            Assert.Equal("Foo", result.Threads.Last?.Value.Methods.Last?.Value.ClassName);
        }
        
        [Fact]
        public void MethodShouldBeNested()
        {
            ITracer tracer = new TracerImpl();
            Foo foo = new Foo(tracer);
            foo.MyMethod();
            
            TraceResult result = tracer.GetTraceResult();
            
            Assert.True(result.Threads.Last?.Value.Methods.Last?.Value.Methods.Count != 0);
        }
        
        [Fact]
        public void ThreadCountShouldBeTwo()
        {
            ITracer tracer = new TracerImpl();
            
            Foo foo = new Foo(tracer);
            foo.MyMethod();
            
            Other other = new Other(tracer);
            Thread instanceCaller = new Thread(
                new ThreadStart(other.OtherMethod));
            instanceCaller.Start();

            instanceCaller.Join();
            
            TraceResult result = tracer.GetTraceResult();
            
            Assert.True(result.Threads.Count == 2);
        }
    }
}