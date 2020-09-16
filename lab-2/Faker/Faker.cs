using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ClassLibrary1
{
    public class Faker
    {
        private readonly string pluginPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
        private List<IPlugin> plugins = new List<IPlugin>();

        private static Dictionary<Type, Func<Object>> @switch = new Dictionary<Type, Func<Object>> {
            { typeof(int), () => GenerateInt32() },
            { typeof(float), () => GenerateFloat() },
            { typeof(long), () => GenerateInt64() },
            { typeof(double), () => GenerateDouble() },
            { typeof(char), () => GenerateChar() },
            { typeof(bool), () => GenerateBool() },
            { typeof(byte), () => GenerateByte() },
            { typeof(List<int>), () => GenerateIntList()},
        };
            
        private HashSet<Type> _usedClasses = new HashSet<Type>();
        private static Random random = new Random();

        public Faker()
        {
            RefreshPlugins();
        }
        
        public T Create<T>() where T : class
        {
            Type type = typeof(T);
            return (T)CreateInner(type);
        }

        private Object CreateInner(Type type)
        {
            if (_usedClasses.Contains(type)) throw new Exception("Recursive DTO");
            
            _usedClasses.Add(type);
            ConstructorInfo constructor = selectConstructor(type);
            if (constructor == null) return null;
            Object[] param = GenerateParams(constructor);
            Object obj = constructor.Invoke(param);
            
            SetFields(obj,type);
            SetProperties(obj,type);
            
            return obj;
        }

        private void SetFields(Object obj, Type type)
        {
            foreach (FieldInfo info in type.GetFields())
            {
                Type fieldType = info.FieldType;
                
                if (!@switch.ContainsKey(fieldType))
                {
                    if (IsDto(fieldType))
                    {
                        info.SetValue(obj, CreateInner(fieldType));
                    }
                }
                else
                {
                    info.SetValue(obj, @switch[fieldType]());
                }
            }
        }

        private void SetProperties(Object obj, Type type)
        {
            foreach (PropertyInfo info in type.GetProperties())
            {
                if (!info.CanWrite) continue;
                
                Type propertyType = info.PropertyType;
                
                if (!@switch.ContainsKey(propertyType))
                {
                    if (IsDto(propertyType))
                    {
                        info.SetValue(obj, CreateInner(propertyType));
                    }
                }
                else
                {
                    info.SetValue(obj, @switch[info.PropertyType]());
                }
            }
        }
        
        public static bool IsDto(Type type)
        {
            return type.IsClass && !type.IsValueType && !type.IsGenericType;
        }

        private ConstructorInfo selectConstructor(Type type)
        {
            Dictionary<ConstructorInfo, int> constructorMap = new Dictionary<ConstructorInfo, int>();
            
            ConstructorInfo[] constructors = type.GetConstructors();

            foreach (ConstructorInfo info in constructors)
            {
                if (info.GetParameters().All(param => @switch.ContainsKey(param.ParameterType) || IsDto(param.ParameterType)))
                    constructorMap.Add(info, info.GetParameters().Length);
            }

            int[] values = constructorMap.Values.ToArray();
            if (values.Length == 0)
                return null;
            int maxValue = values.Max();
            int maxIndex = values.ToList().IndexOf(maxValue);

            return constructors[maxIndex];
        }

        private Object[] GenerateParams(ConstructorInfo constructor)
        {
            var result = new LinkedList<object>();
            
            foreach (ParameterInfo info in constructor.GetParameters())
            {
                Type parameterType = info.ParameterType;
                
                if (!@switch.ContainsKey(parameterType))
                {
                    if (IsDto(parameterType))
                    {
                        result.AddLast(CreateInner(parameterType));
                    }
                }
                else
                {
                    result.AddLast(@switch[parameterType]());
                }
            }

            return result.ToArray();
        }
        
        private void RefreshPlugins()
        {
            plugins.Clear();

            DirectoryInfo pluginDirectory = new DirectoryInfo(pluginPath);
            if (!pluginDirectory.Exists)
                pluginDirectory.Create();
        
            var pluginFiles = Directory.GetFiles(pluginPath, "*.dll");
            foreach (var file in pluginFiles)
            {
                Assembly asm = Assembly.LoadFrom(file);
                var types = asm.GetTypes().
                    Where(t => t.GetInterfaces(). Where(i => i.FullName == typeof(IPlugin).FullName).Any());

                foreach (var type in types)
                {           
                    var plugin = asm.CreateInstance(type.FullName) as IPlugin;
                    @switch.Add(plugin.GetGeneratorType(), () => plugin.Generate());
                }
            }
        }

        public static int GenerateInt32()
        {
            return random.Next(Int32.MaxValue);
        }
        
        public static long GenerateInt64()
        {
            byte[] buf = new byte[8];
            random.NextBytes(buf);
            return BitConverter.ToInt64(buf, 0);
        }
        
        public static float GenerateFloat()
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }

        public static char GenerateChar()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZadcdefghijklmnopqrstuvwxyz0123456789";
            
            return chars[random.Next(chars.Length - 1)];
        }
        
        public static byte GenerateByte()
        {
            byte[] buf = new byte[1];
            random.NextBytes(buf);
            return buf[0];
        }
        
        public static double GenerateDouble()
        {
            return random.NextDouble();
        }
        
        public static bool GenerateBool()
        {
            return random.Next(100) <= 50;
        }
        
        public static List<int> GenerateIntList()
        {
            const int length = 5;
            var res = new List<int>(); 
            
            for (int i = 0; i < length; i++)
            {
                res.Add((int)@switch[typeof(int)]());
            }

            return res;
        }
    }
}
