using System;
using Dependency_Injection_Library;
using Xunit;

namespace Dependency_Injection_Test_Project
{
    interface IService {}

    class Service1 : IService
    {
    };

    abstract class AbstractService : IService
    {
    };
    
    class Service2 : AbstractService
    {
    };
    
    class ServiceImpl : IService
    {
        public ServiceImpl(IRepository repository) // ServiceImpl зависит от IRepository
        {
        }
    }
    
    interface IRepository{}
    
    class RepositoryImpl : IRepository
    {
        public RepositoryImpl(){} // может иметь свои зависимости, опустим для простоты
    }
    
    public class UnitTest1
    {
        [Fact]
        public void DependenciesConfigurationRegister()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, Service1>();
            dependencies.Register<AbstractService, Service2>();
            Assert.Equal(2, dependencies.Count());
        }
        
        [Fact]
        public void asd()
        {
        }
    }
}
