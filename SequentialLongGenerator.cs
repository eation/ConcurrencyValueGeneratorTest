using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrencyValueGeneratorTest
{
    /// <summary>
    /// 
    /// </summary>
    public class SequentialLongGenerator : ISequentialIdGenerator<long>
    {
        public SequentialLongGeneratorConfig idGenConfig = SequentialLongGeneratorConfig.Default;
      
        public SnowflakeConfig snowflakeIdConfig = SnowflakeConfig.Default;
       
        public SequentialLongGenerator()
        {
            ValidateGenConfig();
        }
       
        public SequentialLongGenerator(long workerId, long datacenterId, long sequence = 0L)
        {
            idGenConfig = new SequentialLongGeneratorConfig { WorkerId = workerId, DatacenterId = datacenterId, Sequence = sequence };
            ValidateGenConfig();
        }
       
        public SequentialLongGenerator(SequentialLongGeneratorConfig _idGenConfig)
        {
            this.idGenConfig = _idGenConfig;
            ValidateGenConfig();
        }
      
        public SequentialLongGenerator(SnowflakeConfig _sequenceIdConfig, SequentialLongGeneratorConfig _idGenConfig)
        {
            this.snowflakeIdConfig = _sequenceIdConfig;
            this.idGenConfig = _idGenConfig;
            ValidateGenConfig();
        }

        public void ValidateGenConfig()
        {
           
            if (idGenConfig.WorkerId > snowflakeIdConfig.MaxWorkers || idGenConfig.WorkerId < 0)
            {
                throw new ArgumentException(string.Format("worker Id must >= 0，and <= MaxWorkerId： {0}", snowflakeIdConfig.MaxWorkers));
            }

            if (idGenConfig.DatacenterId > snowflakeIdConfig.MaxDatacenters || idGenConfig.DatacenterId < 0)
            {
                throw new ArgumentException(string.Format("datacenter Id >=0，and <= MaxDatacenterId： {0}", snowflakeIdConfig.MaxDatacenters));
            }
        }

        
        readonly object _lock = new();
        //private static SpinLock _lock = new SpinLock();

      
        public virtual long NextId()
        {
            //        Console.WriteLine("worker starting. timestamp left shift {0}, datacenter id bits {1}, worker id bits {2}, sequence bits {3}, workerid {4}",
            //sequenceIdConfig.TimestampLeftShift, sequenceIdConfig.DatacenterIdBits, sequenceIdConfig.WorkerIdBits, sequenceIdConfig.SequenceBits, idGenConfig.WorkerId);

            lock (_lock)
            {
                var timestamp = TimeGen();
                if (timestamp < idGenConfig._lastTimestamp)
                {
                    throw new Exception(string.Format("timestamp must >= _lastTimestamp.", idGenConfig._lastTimestamp - timestamp));
                }
                
                if (idGenConfig._lastTimestamp == timestamp)
                {
                    idGenConfig._sequence = (idGenConfig._sequence + 1) & snowflakeIdConfig.MaxSequenceIds;
                    if (idGenConfig._sequence == 0)
                    {
                        timestamp = TilNextMillis(idGenConfig._lastTimestamp);
                    }
                }
                else
                {
                    idGenConfig._sequence = 0;//new Random().Next(10);
                }

                idGenConfig._lastTimestamp = timestamp;
                return ((timestamp - snowflakeIdConfig.Twepoch) << snowflakeIdConfig.TimestampLeftShift) | (idGenConfig.DatacenterId << snowflakeIdConfig.DatacenterIdShift) | (idGenConfig.WorkerId << snowflakeIdConfig.WorkerIdShift) | idGenConfig._sequence;
            }
        }

        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        protected virtual long TimeGen()
        {
            return (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalMilliseconds;
        }

        object ISequentialIdGenerator.NextId()
        {
            throw new NotImplementedException();
        }
    }
}
