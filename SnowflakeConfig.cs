using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrencyValueGeneratorTest
{
    public class SnowflakeConfig
    {
        public int WorkerIdBits { get; private set; }

        public int DatacenterIdBits { get; private set; }

        public int SequenceBits { get; private set; }

        public int TotalBits
        {
            get
            {
                return WorkerIdBits + DatacenterIdBits + SequenceBits;
            }
        }

        public long MaxDatacenters
        {
            get
            {
                return -1L ^ (-1L << DatacenterIdBits);
            }
        }

        public long MaxWorkers
        {
            get
            {
                return -1L ^ (-1L << WorkerIdBits);
            }
        }
        public long MaxSequenceIds
        {
            get
            {
                return -1L ^ (-1L << SequenceBits);
            }
        }
        //public long SequenceMask
        //{
        //    get
        //    {
        //        return -1L ^ (-1L << SequenceBits);
        //    }
        //}

        
        public int TimestampBits { get; private set; }
       
        public long MaxIntervals { get { return (1L << TimestampBits); } }

       
        internal int WorkerIdShift
        {
            get
            {
                return SequenceBits;
            }
        }
        
        internal int DatacenterIdShift
        {
            get
            {
                return SequenceBits + WorkerIdBits;
            }
        }
        
        internal int TimestampLeftShift
        {
            get
            {
                return SequenceBits + WorkerIdBits + DatacenterIdBits;
            }
        }

        
        public DateTime? BaseLineTime=null;

       
        public long Twepoch = DateTime.UnixEpoch.Ticks/10000;
        //Twepoch = BaseLineTime.HasValue? (long) (BaseLineTime!.Value - DateTime.UnixEpoch).TotalMilliseconds : DateTime.UnixEpoch.Ticks / 10000;

        public static readonly SnowflakeConfig Default = new(3, 3, 14,new DateTime(2020,12,31,23,59,59,DateTimeKind.Utc));

        //public SequenceIdConfig(byte workerIdBits, byte datacenterIdBits)
        //{
        //    WorkerIdBits = workerIdBits;
        //    DatacenterIdBits = datacenterIdBits;
        //    SequenceBits = 63 - Math.Abs(WorkerIdBits) - Math.Abs(DatacenterIdBits);

        //    ValidateSequenceConfig();
        //}

        public SnowflakeConfig()
        {
            WorkerIdBits = 3;
            DatacenterIdBits = 3;
            SequenceBits = 4;
            TimestampBits = 63 - Math.Abs(WorkerIdBits) - Math.Abs(DatacenterIdBits) - Math.Abs(SequenceBits);
            ValidateSequenceConfig();
        }
        public SnowflakeConfig(byte workerIdBits, byte datacenterIdBits, byte sequenceBits)
        {
            WorkerIdBits = workerIdBits;
            DatacenterIdBits = datacenterIdBits;
            SequenceBits = sequenceBits;
            TimestampBits = 63 - Math.Abs(WorkerIdBits) - Math.Abs(DatacenterIdBits) - Math.Abs(SequenceBits);
            ValidateSequenceConfig();
        }
        public SnowflakeConfig(byte workerIdBits, byte datacenterIdBits, byte sequenceBits, DateTime baseLineTime)
        {
            WorkerIdBits = workerIdBits;
            DatacenterIdBits = datacenterIdBits;
            SequenceBits = sequenceBits;
            TimestampBits = 63 - Math.Abs(WorkerIdBits) - Math.Abs(DatacenterIdBits) - Math.Abs(SequenceBits);
            BaseLineTime = baseLineTime;
            ValidateSequenceConfig();
        }

        public void ValidateSequenceConfig()
        {
            //if (TotalBits != 63)
            //    throw new InvalidOperationException("Number of bits used to generate Id's is not equal to 63");

            if (WorkerIdBits + DatacenterIdBits > 31)
            { 
                throw new ArgumentOutOfRangeException("GeneratorId cannot have more than 31 bits");
            }

            if (SequenceBits > 48)
            { 
                throw new ArgumentOutOfRangeException("Sequence cannot have more than 48 bits");
            }
            if (BaseLineTime.HasValue)
            {
                Twepoch = BaseLineTime!.Value.Ticks / 10000;
            }
            else
            {
                Twepoch = DateTime.UnixEpoch.Ticks / 10000;
            } 
        }
       
        public DateTime AvailableDate()
        {
                if (MaxIntervals <= ((DateTime.MaxValue.Ticks/10000)- Twepoch))
                {
                    return DateTime.MinValue.AddMilliseconds(Twepoch+MaxIntervals);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("MaxIntervals Out Of Max DateTime's Milliseconds!");
                }
        }
        public DateTime AvailableFromDate(DateTime epoch)
        {
            if (MaxIntervals <= ((DateTime.MaxValue.Ticks / 10000) - Twepoch))
            {
                return DateTime.MinValue.Add(DateTime.MinValue.AddMilliseconds(Twepoch + MaxIntervals) - epoch);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(epoch),"MaxIntervals Out Of Max DateTime's Milliseconds!");
            }
        }
    }
}
