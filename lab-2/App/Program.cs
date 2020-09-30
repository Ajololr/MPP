using System;
using System.Collections.Generic;
using ClassLibrary1;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            Foo foo;
            Faker faker = new Faker();
            foo = faker.Create<Foo>();
            Console.WriteLine(foo);
        }
    }

    class StrGenerator
    {
        
    }
    
    class Foo
    {
        private int _integerValue;
        private float _floatValue;
        public List<DateTime> list;
        public string StingValue;
        private long _longValue;
        private double _doubleValue;
        public Bar bar;
        public bool b;

        public Foo()
        {
        }
        
        public Foo(int integerValue, List<DateTime> list)
        {
            IntegerValue = integerValue;
            this.list = list;
        }
        
        public Foo(int integerValue, float floatValue, List<DateTime> list, decimal dec)
        {
            IntegerValue = integerValue;
            _floatValue = floatValue;
            this.list = list;
        }

        public int IntegerValue
        {
            set => _integerValue = value;
        }

        public long LongValue
        {
            get => _longValue;
            set => _longValue = value;
        }

        public double DoubleValue
        {
            get => _doubleValue;
            set => _doubleValue = value;
        }
    }

    class Bar
    {
        public int IntValue;
        public string StringValue;
        public Other _other;
        public Dictionary<int,int> Dictionary;

        public Bar()
        {
            
        }
        
        public Bar(int intValue)
        {
            IntValue = intValue;
        }
    }

    class Other
    {
        public DateTime DateTime;

    }
}