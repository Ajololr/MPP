using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dependency_Injection_Library
{
    public class ImplementationInfo
    {
        public Type Type;
        public bool IsSingleton;
        private Object _value;

        public ImplementationInfo(Type type, bool isSingleton)
        {
            Type = type;
            IsSingleton = isSingleton;
        }

        public Object GetValue()
        {
           return _value; 
        }

        internal void SetValue(Object value)
        {
            _value = value;
        }
    }
    
    public class DependenciesConfiguration
    {
        private Dictionary<Type, List<ImplementationInfo> > _dependencies = new Dictionary<Type, List<ImplementationInfo>>();

        public void Register(Type dependency, Type implementation, bool isSingleton = false)
        {
            if (!_dependencies.ContainsKey(dependency))
            { 
                _dependencies[dependency] = new List<ImplementationInfo>();
            }
            _dependencies[dependency].Add(new ImplementationInfo(implementation, isSingleton));
        }
        
        public void Register<TDependency, TImplementation>(bool isSingleton = false) where TDependency: class where TImplementation: TDependency
        {
            if (!_dependencies.ContainsKey(typeof(TDependency)))
            { 
                _dependencies[typeof(TDependency)] = new List<ImplementationInfo>();
            }
            _dependencies[typeof(TDependency)].Add(new ImplementationInfo(typeof(TImplementation), isSingleton));
        }

        public int Count()
        {
            return _dependencies.Count;
        }

        public ImplementationInfo GetFirstImplementationType(Type key)
        {
            return _dependencies[key].First();
        }
        
        public List<ImplementationInfo> GetAllImplementationTypes(Type key)
        {
            return _dependencies[key];
        }

        public bool Has(Type key)
        {
            return _dependencies.ContainsKey(key);
        }
    }

    public class DependencyProvider
    {
        private DependenciesConfiguration _configuration;
        
        public DependencyProvider(DependenciesConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Object Resolve<TDependency>() where TDependency : class
        {
            Type dependencyType = typeof(TDependency);
            if (typeof(IEnumerable).IsAssignableFrom(dependencyType))
            { 
                Type genericType = dependencyType.GetGenericArguments()[0];
                List<Object> list = new List<object>();
                foreach (var implementationInfo in _configuration.GetAllImplementationTypes(genericType))
                {    
                    list.Add(Resolve(implementationInfo));
                }

                return list.AsEnumerable();
            }
            
            if (_configuration.Has(dependencyType))
            {
                ImplementationInfo implementationInfo = _configuration.GetFirstImplementationType(dependencyType);
                return Resolve(implementationInfo) as TDependency;
            }

            if (dependencyType.IsGenericType)
            {
                Type genericBase = dependencyType.GetGenericTypeDefinition();
                Type genericArgument = dependencyType.GetGenericArguments()[0];
                if (_configuration.Has(genericBase) && _configuration.Has(genericArgument))
                {
                    return ResolveOpenGeneric(_configuration.GetFirstImplementationType(genericBase).Type, genericArgument) as TDependency;
                }
            }
            
            throw new Exception("Not supported type");
        }

        private Object CreateObject(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();

            foreach (var constructorInfo in constructors)
            {
                ParameterInfo[] parameters = constructorInfo.GetParameters();
                bool isSuitable = parameters.All(info => _configuration.Has(info.ParameterType) || typeof(IEnumerable).IsAssignableFrom(info.ParameterType));

                if (isSuitable)
                {
                    Object value = constructorInfo.Invoke(parameters.Select(info =>
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(info.ParameterType))
                        {
                            Type genericType = info.ParameterType.GetGenericArguments()[0];
                            ArrayList arrayList = new ArrayList();
                            foreach (var implementationType in _configuration.GetAllImplementationTypes(genericType))
                            {    
                                arrayList.Add(Resolve(implementationType));
                            }

                            ConstructorInfo[] constructorInfo =
                                typeof(List<>).MakeGenericType(genericType).GetConstructors();
                            Object list = constructorInfo[2].Invoke(new Object[] {arrayList as Object});

                            return list;
                        }
                        return Resolve(_configuration.GetFirstImplementationType(info.ParameterType));
                    }).ToArray());
                    return value;
                }
            }
            
            throw new Exception("No suitable constructor");
        }

        private Object ResolveOpenGeneric(Type baseType, Type argumentType)
        {
            return CreateObject(baseType.MakeGenericType(argumentType));
        }

        private Object Resolve(ImplementationInfo implementationInfo)
        {
            Type type = implementationInfo.Type;
            if (implementationInfo.IsSingleton && implementationInfo.GetValue() != null)
            {
                return implementationInfo.GetValue();
            }

            Object value = CreateObject(type);
            
            if (implementationInfo.IsSingleton) implementationInfo.SetValue(value);

            return value;
        }
    }
}
