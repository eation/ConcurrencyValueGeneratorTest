using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ConcurrencyValueGeneratorTest
{
    public class InMemoryTestDbContext : DbContext
    {
        public InMemoryTestDbContext() { }

        public InMemoryTestDbContext(DbContextOptions<InMemoryTestDbContext> options)
               : base(options)
        {
        }

        public DbSet<TestClass3>? TClass3 { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TestClass3>().Property(m => m.Id).HasValueGenerator<MyLongValueGenerator>();

            var token = modelBuilder.Entity<TestClass3>().Property(m => m.CuToken).HasValueGenerator<MyLongValueGenerator>().ValueGeneratedOnAddOrUpdate();
            //token.HasValueGenerator<MyLongValueGenerator>();
            token.IsConcurrencyToken();
            token.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);
            token.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

            var t1 = modelBuilder.Entity<TestClass3>().Property(m => m.TestValue).HasValueGenerator<MyLongValueGenerator>().ValueGeneratedOnAddOrUpdate();
            t1.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);
            t1.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
        }
    }

    public class TestClass3
    {
        public long Id{get;set;}

        public string? Remark { get; set; } = null;

        public long? TestValue { get; set; }

        public long CuToken { get; set; }
    }
}
