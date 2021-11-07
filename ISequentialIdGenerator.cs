using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyValueGeneratorTest
{
    public interface ISequentialIdGenerator
    {
        object NextId();
        void ValidateGenConfig();
    }
}
