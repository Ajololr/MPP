using System;
using System.Linq;
using ClassLibrary1;

namespace StringGeneratorLib
{
    public class StringGenerator : IPlugin
    {
        private Random _random = new Random();
        public object Generate()
        {
            const int length = 8;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZadcdefghijklmnopqrstuvwxyz0123456789";
            
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public Type GetGeneratorType()
        {
            return typeof(string);
        }
    }
}