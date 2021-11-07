//using Microsoft.EntityFrameworkCore.ChangeTracking;

using System.Diagnostics.CodeAnalysis;

namespace ConcurrencyValueGeneratorTest
{
    public interface IMyLongValueGenerator
    {
        long Next();
    }
}
