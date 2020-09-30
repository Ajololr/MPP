using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ClassLibrary1
{
    public class Faker
    {
        private FakerConfig _config;
        private readonly string pluginPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
        private List<IPlugin> plugins = new List<IPlugin>();

        private static Dictionary<Type, Func<Object>> _switch = new Dictionary<Type, Func<Object>> {
            { typeof(int), () => GenerateInt32() },
            { typeof(float), () => GenerateFloat() },
            { typeof(long), () => GenerateInt64() },
            { typeof(double), () => GenerateDouble() },
            { typeof(char), () => GenerateChar() },
            { typeof(bool), () => GenerateBool() },
            { typeof(byte), () => GenerateByte() },
        };
            
        private HashSet<Type> _usedClasses = new HashSet<Type>();
        private static Random random = new Random();

        public Faker(FakerConfig config) : this()
        {
            _config = config;
        }

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
            ConstructorInfo constructor = SelectConstructor(type);
            if (constructor == null) return null;
            Object[] param = GenerateParams(constructor);
            Object obj = constructor.Invoke(param);

            
            SetFields(obj,type);
            SetProperties(obj,type);
            
            return obj;
        }

        private bool isValidList(Type fieldType)
        {
            return fieldType.IsGenericType 
                   && fieldType.GetGenericTypeDefinition() == typeof(List<>)
                   && _switch.ContainsKey(fieldType.GetGenericArguments()[0]);
        }

        private void SetFields(Object obj, Type classType)
        {
            foreach (FieldInfo info in classType.GetFields())
            {
                Type fieldType = info.FieldType;

                if (!_switch.ContainsKey(fieldType))
                {
                    if ( fieldType.IsGenericType 
                         && fieldType.GetGenericTypeDefinition() == typeof(List<>)
                         && _switch.ContainsKey(fieldType.GetGenericArguments()[0]))
                    {
                        Type innerType = fieldType.GetGenericArguments()[0];
                        
                        List<Object> list = GenerateList(innerType);

                        if (list[0] is int)
                        {
                            info.SetValue(obj, list.Cast<int>().ToList());
                        } else if (list[0] is bool)
                        {
                            info.SetValue(obj, list.Cast<bool>().ToList());
                        } else if (list[0] is DateTime)
                        {
                            info.SetValue(obj, list.Cast<DateTime>().ToList());
                        } else if (list[0] is char)
                        {
                            info.SetValue(obj, list.Cast<char>().ToList());
                        }
                    }
                    else if (IsDto(fieldType))
                    {
                        info.SetValue(obj, CreateInner(fieldType));
                    }
                }
                else
                {
                    info.SetValue(obj, _switch[fieldType]());
                }
            }
        }

        private void SetProperties(Object obj, Type type)
        {
            foreach (PropertyInfo info in type.GetProperties())
            {
                if (!info.CanWrite) continue;
                
                Type propertyType = info.PropertyType;
                
                if (!_switch.ContainsKey(propertyType))
                {
                    if ( propertyType.IsGenericType 
                         && propertyType.GetGenericTypeDefinition() == typeof(List<>)
                         && _switch.ContainsKey(propertyType.GetGenericArguments()[0]))
                    {
                        Type innerType = propertyType.GetGenericArguments()[0];
                        
                        List<Object> list = GenerateList(innerType);

                        if (list[0] is int)
                        {
                            info.SetValue(obj, list.Cast<int>().ToList());
                        } else if (list[0] is bool)
                        {
                            info.SetValue(obj, list.Cast<bool>().ToList());
                        } else if (list[0] is DateTime)
                        {
                            info.SetValue(obj, list.Cast<DateTime>().ToList());
                        } else if (list[0] is char)
                        {
                            info.SetValue(obj, list.Cast<char>().ToList());
                        }
                    }
                    else if (IsDto(propertyType))
                    {
                        info.SetValue(obj, CreateInner(propertyType));
                    }
                }
                else
                {
                    info.SetValue(obj, _switch[info.PropertyType]());
                }
            }
        }
        
        public static bool IsDto(Type type)
        {
            return type.IsClass && !type.IsValueType && !type.IsGenericType;
        }

        private ConstructorInfo SelectConstructor(Type type)
        {
            List<ConstructorInfo> constructors = new List<ConstructorInfo>();
            List<int> lengthList = new List<int>();
            
            foreach (ConstructorInfo info in type.GetConstructors())
            {
                if (info.GetParameters().All(param =>
                    _switch.ContainsKey(param.ParameterType)
                    || isValidList(param.ParameterType)
                    || IsDto(param.ParameterType)))
                {
                    constructors.Add(info);
                    lengthList.Add(info.GetParameters().Length);
                }
            }
            

            int[] values = lengthList.ToArray();
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
                
                if (!_switch.ContainsKey(parameterType))
                {
                    if ( parameterType.IsGenericType 
                         && parameterType.GetGenericTypeDefinition() == typeof(List<>)
                         && _switch.ContainsKey(parameterType.GetGenericArguments()[0]))
                    {
                        Type innerType = parameterType.GetGenericArguments()[0];
                        
                        List<Object> list = GenerateList(innerType);

                        if (list[0] is int)
                        {
                            result.AddLast(list.Cast<int>().ToList());
                        } else if (list[0] is bool)
                        {
                            result.AddLast(list.Cast<bool>().ToList());
                        } else if (list[0] is DateTime)
                        {
                            result.AddLast(list.Cast<DateTime>().ToList());
                        } else if (list[0] is char)
                        {
                            result.AddLast(list.Cast<char>().ToList());
                        }
                    }
                    else if (IsDto(parameterType))
                    {
                        result.AddLast(CreateInner(parameterType));
                    }
                }
                else
                {
                    result.AddLast(_switch[parameterType]());
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
                    _switch.Add(plugin.GetGeneratorType(), () => plugin.Generate());
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
        
        public static List<Object> GenerateList(Type innerType)
        {
            int length = random.Next(1, 10);
            List<Object> res = null;

            if (_switch.ContainsKey(innerType))
            {
                res = new List<Object>();
                for (int i = 0; i < length; i++)
                {
                    res.Add(_switch[innerType]());
                }
            }
            
            return res;
        }
    }
}
