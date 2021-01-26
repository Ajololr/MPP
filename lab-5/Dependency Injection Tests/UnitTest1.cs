using System;
using System.Collections.Generic;
using Dependency_Injection_Library;
using NUnit.Framework;

namespace TestProject1
{
    interface IService {}

    interface IOtherService
    {
    }

    class ServiceImpl1 : IService
    {
    };

    class ServiceImpl2 : IService
    {
    };
    
    class NoConstructorImpl : IService
    {
        private NoConstructorImpl()
        {
        }
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
    
    interface IService<TRepository> where TRepository : IRepository
    {
    }

    class ServiceImpl<TRepository> : IService<TRepository> 
        where TRepository : IRepository
    {
        public TRepository Repository;
        public ServiceImpl(TRepository repository)
        {
            Repository = repository;
        }
    }

    class MySqlRepository : IRepository
    {
        
    }

    [TestFixture]
    public class DependenciesConfigurationClass
    {
        [Test]
        public void Count()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl1>();
            dependencies.Register<AbstractService, Service2>();
            Assert.AreEqual(2, dependencies.Count());
        }

        [Test]
        public void AsSelf()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<ServiceImpl1, ServiceImpl1>();
            
            Assert.AreEqual(1, dependencies.Count());
        }
    }

    [TestFixture]
    public class DependencyProviderClass
    {
        [Test]
        public void ResolveBasic()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl1>();
            dependencies.Register<AbstractService, Service2>();
            
            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IService>();

            Assert.AreEqual( typeof(ServiceImpl1), service1.GetType());
        }
        
        [Test]
        public void NoImplementationError()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl1>();
            dependencies.Register<AbstractService, Service2>();
            
            var provider = new DependencyProvider(dependencies);

            Assert.Throws(typeof(Exception), () => provider.Resolve<IOtherService>());
        }
        
        [Test]
        public void NoConstructorError()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, NoConstructorImpl>();
            
            var provider = new DependencyProvider(dependencies);

            Assert.Throws(typeof(Exception), () => provider.Resolve<IService>());
        }

        
        [Test]
        public void ResolveRecursive()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl>();
            dependencies.Register<IRepository, RepositoryImpl>();
            
            var provider = new DependencyProvider(dependencies);
            var service = provider.Resolve<IService>();

            Assert.AreEqual( typeof(ServiceImpl), service.GetType());
        }
        
        
        [Test]
        public void ResolveSingleton()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl1>(isSingleton: true);
            
            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IService>();
            var service2 = provider.Resolve<IService>();

            Assert.AreSame( service1, service2);
        }
        
        [Test]
        public void ResolveEnumerable()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl1>();
            dependencies.Register<IService, ServiceImpl2>();
            
            var provider = new DependencyProvider(dependencies);
            var services = provider.Resolve<IEnumerable<IService>>();
        
            Assert.AreEqual(2, (services as List<object>)?.Count);    
        }
        
        [Test]
        public void ResolveGenericDependency()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, MySqlRepository>();
            dependencies.Register<IService<IRepository>, ServiceImpl<IRepository>>();
            
            var provider = new DependencyProvider(dependencies);
            var service = provider.Resolve<IService<IRepository>>();
        
            Assert.NotNull((service as ServiceImpl<IRepository>)?.Repository);    
        }
        
        [Test]
        public void ResolveOpenGenericsDependency()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IRepository, MySqlRepository>();
            dependencies.Register(typeof(IService<>), typeof(ServiceImpl<>));
            
            var provider = new DependencyProvider(dependencies);
            var service = provider.Resolve<IService<IRepository>>();
        
            Assert.NotNull((service as ServiceImpl<IRepository>)?.Repository);    
        }
    }
}