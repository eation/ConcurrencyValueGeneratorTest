using System;

namespace ConcurrencyValueGeneratorTest
{
    public interface ISequentialIdGenerator<out T>:ISequentialIdGenerator where T: struct
    {
        new T NextId();
    }
}
