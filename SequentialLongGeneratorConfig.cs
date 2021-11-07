using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrencyValueGeneratorTest
{
    public class SequentialLongGeneratorConfig
    {
        public long _sequence = 0L;
        public long _lastTimestamp = -1L;

        public static readonly SequentialLongGeneratorConfig Default = new() { WorkerId = 0, DatacenterId = 0 };

        public long WorkerId { get; set; }
        public long DatacenterId { get; set; }

        public long Sequence
        {
            get { return _sequence; }

            set { _sequence = value; }
        }
        public SequentialLongGeneratorConfig()
        {

        }
    }
}
