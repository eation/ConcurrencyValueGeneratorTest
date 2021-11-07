using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyValueGeneratorTest
{
    public class SQLTestDbContext : DbContext
    {
        public SQLTestDbContext() { }

        public SQLTestDbContext(DbContextOptions<SQLTestDbContext> options)
               : base(options)
        {
        }

        public DbSet<TestClass1>? TClass1 { get; set; }
        public DbSet<TestClass2>? TClass2 { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer(
                //                @"Server=(localdb)\mssqllocaldb;Database=EFCore5CustomValueGeneratorV1;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestClass1>().Property(m => m.Id).HasValueGenerator<MyLongValueGenerator>();

            var token = modelBuilder.Entity<TestClass1>().Property(m => m.CuToken).HasValueGenerator<MyLongValueGenerator>().ValueGeneratedOnAddOrUpdate();
            token.IsConcurrencyToken();
            token.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);
            token.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

            var t1 = modelBuilder.Entity<TestClass1>().Property(m => m.TestValue).HasValueGenerator<MyLongValueGenerator>().ValueGeneratedOnAddOrUpdate();
            t1.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);
            t1.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

            modelBuilder.Entity<TestClass2>().Property(m => m.Id).HasValueGenerator<MyLongValueGenerator>();
        }
    }
}
