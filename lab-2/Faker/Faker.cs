using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Faker
{
    public class Faker
    {
        private Dictionary<Type, Func<Object>> @switch = new Dictionary<Type, Func<Object>> {
            { typeof(int), () => GenerateInt32() },
            { typeof(string), () => GenerateString() },
            { typeof(float), () => GenerateFloat() },
            { typeof(long), () => GenerateInt64() },
            { typeof(double), () => GenerateDouble() },
            { typeof(DateTime), () => GenerateDateTime() },
        };
        private static Random random = new Random();
        
        public T Create<T>() where T : class
        {
            Type type = typeof(T);

            ConstructorInfo constructor = selectConstructor(type);

            Object[] param = GenerateParams(constructor);

            T obj = (T) constructor.Invoke(param);

            foreach (FieldInfo info in type.GetFields())
            {
                info.SetValue(obj, @switch[info.FieldType]());
            }

            foreach (PropertyInfo info in type.GetProperties())
            {
                if (info.CanWrite) info.SetValue(obj,@switch[info.PropertyType]());
            }
            
            return obj;
        }

        private ConstructorInfo selectConstructor(Type type)
        {
            Dictionary<ConstructorInfo, int> constructorMap = new Dictionary<ConstructorInfo, int>();
            
            ConstructorInfo[] constructors = type.GetConstructors();

            foreach (ConstructorInfo info in constructors)
            {
                constructorMap.Add(info, info.GetParameters().Length);
            }

            int[] values = constructorMap.Values.ToArray();
            int maxValue = values.Max();
            int maxIndex = values.ToList().IndexOf(maxValue);

            return constructors[maxIndex];
        }

        private Object[] GenerateParams(ConstructorInfo constructor)
        {
            LinkedList<Object> result = new LinkedList<object>();
            
            foreach (ParameterInfo info in constructor.GetParameters())
            {
                Type type = info.ParameterType;
                result.AddLast(@switch[type]());
            }

            return result.ToArray();
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

        public static string GenerateString()
        {
            const int length = 8;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static double GenerateDouble()
        {
            return random.NextDouble();
        }
        
        public static DateTime GenerateDateTime()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;           
            return start.AddDays(random.Next(range));
        }
    }
}
