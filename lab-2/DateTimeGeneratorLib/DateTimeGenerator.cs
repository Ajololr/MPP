using System;
using ClassLibrary1;

namespace DateTimeGeneratorLib
{
    public class DateTimeGeneratorIPlugin : IPlugin
    {
        private Random _random = new Random();

        public object Generate()
        {
            DateTime start = new DateTime(1995, 1, 1, 1,1,1);
            int rangeSeconds = (DateTime.Today - start).Seconds;  
            int rangeMinutes = (DateTime.Today - start).Minutes;  
            int rangeHours = (DateTime.Today - start).Hours;  
            int rangeDays = (DateTime.Today - start).Days;
            return start.AddSeconds(_random.Next(rangeSeconds))
                .AddMinutes(_random.Next(rangeMinutes))
                .AddHours(_random.Next(rangeHours))
                .AddDays(_random.Next(rangeDays));
        }

        public Type GetGeneratorType()
        {
            return typeof(DateTime);
        }
    }
}