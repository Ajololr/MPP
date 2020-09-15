using System;
using Faker;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            Faker.Faker faker = new Faker.Faker();
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

        public Foo()
        {
            
        }

        public Foo(int integerValue, float floatValue)
        {
            IntegerValue = integerValue;
            _floatValue = floatValue;
        }

        private Foo(int integerValue)
        {
            
        }

        public int IntegerValue
        {
            set => _integerValue = value;
        }

        public float FloatValue => _floatValue;

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
}