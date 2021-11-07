
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyValueGeneratorTest
{ 
    public class MyLongValueGenerator : ValueGenerator<long>, IMyLongValueGenerator
    {
        private static readonly SequentialLongGenerator worker = new(1, 1);//For Test
        //private bool v;

        public MyLongValueGenerator()
        {
            
        }

        /// <summary>
        ///     Gets a value indicating whether the values generated are temporary or permanent. This implementation
        ///     always returns false, meaning the generated values will be saved to the database.
        /// </summary>
        public override bool GeneratesTemporaryValues => false;


        public long Next()
        {
            return worker.NextId();
        }

        public static long NextID() 
        {
            return worker.NextId(); 
        }

        //
        // Summary:
        //     Template method to be overridden by implementations to perform value generation.
        //
        // Parameters:
        //   entry:
        //     The change tracking entry of the entity for which the value is being generated.
        //
        // Returns:
        //     The generated value.
        public override long Next([NotNull] EntityEntry entry)
        {
            return worker.NextId();
        }
        //
        // Summary:
        //     Template method to be overridden by implementations to perform value generation.
        //
        // Parameters:
        //   entry:
        //     The change tracking entry of the entity for which the value is being generated.
        //
        //   cancellationToken:
        //     A System.Threading.CancellationToken to observe while waiting for the task to
        //     complete.
        //
        // Returns:
        //     The generated value.
        public new virtual ValueTask<long> NextAsync([NotNull] EntityEntry entry, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new ValueTask<long>(Next(entry));
        }

        //
        // Summary:
        //     Gets a value to be assigned to a property.
        //
        // Parameters:
        //   entry:
        //     The change tracking entry of the entity for which the value is being generated.
        //
        // Returns:
        //     The value to be assigned to a property.
        protected override object NextValue(EntityEntry entry)
        {
            return Next(entry);
        }

        //
        // Summary:
        //     Gets a value to be assigned to a property.
        //
        // Parameters:
        //   entry:
        //     The change tracking entry of the entity for which the value is being generated.
        //
        //   cancellationToken:
        //     A System.Threading.CancellationToken to observe while waiting for the task to
        //     complete.
        //
        // Returns:
        //     The value to be assigned to a property.
        protected override ValueTask<object> NextValueAsync(EntityEntry entry, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new ValueTask<object>(NextValue(entry));
        }
    }


}
