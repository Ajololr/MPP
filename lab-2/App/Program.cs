using System;
using System.Collections.Generic;
using ClassLibrary1;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            Faker faker = new Faker();
            Foo foo = faker.Create<Foo>();
            Console.WriteLine(foo);
        }
    }

    class Foo
    {
        private int _integerValue;
        private float _floatValue;
        public string StingValue;
        private long _longValue;
        private double _doubleValue;
        public Bar bar;
        public List<int> list;
        public bool b;

        public Foo()
        {
        }
        
        public Foo(int integerValue, float floatValue, List<int> list)
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

        public Other()
        {
            
        }
    }
}