using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Linq;

namespace ConcurrencyValueGeneratorTest
{
    internal class Program
    {
        private static IServiceCollection? Services;
        private static IServiceProvider? ServiceProvider;

        static void Main(string[] args)
        {
            Services = new ServiceCollection();

            Services.AddEntityFrameworkSqlServer();
            Services.AddDbContext<SQLTestDbContext>(options =>
            {
                options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ConcurrencyValueGeneratorTest;Integrated Security=True");
                //options.EnableSensitiveDataLogging();
                //options.LogTo(Console.WriteLine);
                //options.ReplaceService<IValueGeneratorSelector, MyValueGeneratorSelector>();
            });

            //Services.AddEntityFrameworkInMemoryDatabase();
            //Services.AddDbContext<ConcurrencyTest2DbContext>(options =>
            //            {
            //                //options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ConcurrencyValue2GeneratorTest;Integrated Security=True");
            //                options.UseInMemoryDatabase("ConcurrencyValue2GeneratorTest");
            //                options.EnableSensitiveDataLogging();
            //                options.LogTo(Console.WriteLine);
            //                //options.ReplaceService<IValueGeneratorSelector, MyValueGeneratorSelector>();
            //            });

            Services.TryAddScoped<IMyLongValueGenerator, MyLongValueGenerator>();
            ServiceProvider = Services.BuildServiceProvider();

            Test1_1();
            Test1_2();
            Test1_3();
            Test1_4();
            //Test2();

            Console.ReadLine();
        }

        /// <summary>
        /// not set Entry(T).State, not use Update to set EntityState
        /// </summary>
        public static void Test1_1()
        {
            var db = ServiceProvider!.CreateScope().ServiceProvider.GetRequiredService<SQLTestDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var entity = new TestClass1 { Remark = Guid.NewGuid().ToString() };
            db.TClass1!.Add(entity);
            //db.TClass1!.Attach(entity);//Bug,no tracked any entity,ChangingCount or ChangedCount is 0
            //db.TClass2!.Add(new TestClass2 { Remark = "t2222" });

            db.SaveChanges();

            var ft1 = db.TClass1.OrderByDescending(m => m.Id).FirstOrDefault();
            //var ft2 = db.TClass2.FirstOrDefault();
            var cu1_1 = ft1!.CuToken;
            //var cu2_1 = ft2!.CuToken;

            Console.WriteLine($"{Environment.NewLine}{nameof(Test1_1)} Firt Saved! {nameof(ft1.Id)}:{ft1.Id}  {nameof(ft1.TestValue)}:{ft1.TestValue}  {nameof(ft1.CuToken)}:{ft1.CuToken}  {nameof(ft1.Remark)}:{ft1.Remark}{Environment.NewLine}");

            ft1.Remark = $"{nameof(Test1_1)},update";//case 1 not set EntityState, not use Update
            db.SaveChanges();


            var ft3 = db.TClass1.OrderByDescending(m => m.Id).FirstOrDefault();
            var cu1_2 = ft3!.CuToken;

            if (entity.Id != ft3.Id || cu1_1 == cu1_2)
            {
                Console.WriteLine($"Failed {nameof(Test1_1)}!  {nameof(ft3.Id)}:{ft3.Id}  {nameof(ft3.TestValue)}:{ft3.TestValue}  {nameof(ft3.CuToken)}:{ft3.CuToken}  {nameof(ft3.Remark)}:{ft3.Remark}{Environment.NewLine}");
            }
            else
            {
                Console.WriteLine($"{Environment.NewLine}Hello World,{nameof(Test1_1)}!  {nameof(ft3.Id)}:{ft3.Id}  {nameof(ft3.TestValue)}:{ft3.TestValue}  {nameof(ft3.CuToken)}:{ft3.CuToken}  {nameof(ft3.Remark)}:{ft3.Remark}{Environment.NewLine}");
            }
        }
        /// <summary>
        /// set Entry(T).State
        /// </summary>
        public static void Test1_2()
        {
            var db = ServiceProvider!.CreateScope().ServiceProvider.GetRequiredService<SQLTestDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var entity = new TestClass1 { Remark = Guid.NewGuid().ToString() };
            db.TClass1!.Add(entity);

            db.SaveChanges();

            var ft1 = db.TClass1.OrderByDescending(m => m.Id).FirstOrDefault();
            var cu1_1 = ft1!.CuToken;

            Console.WriteLine($"{Environment.NewLine}{nameof(Test1_2)} Firt Saved! {nameof(ft1.Id)}:{ft1.Id}  {nameof(ft1.TestValue)}:{ft1.TestValue}  {nameof(ft1.CuToken)}:{ft1.CuToken}  {nameof(ft1.Remark)}:{ft1.Remark}{Environment.NewLine}");

            ft1.Remark = $"{nameof(Test1_2)},update";//case 1 no set EntityState, no use Update

            db.Entry(ft1).State = EntityState.Modified;//case 2
            db.SaveChanges();


            var ft3 = db.TClass1.OrderByDescending(m => m.Id).FirstOrDefault();

            var cu1_2 = ft3!.CuToken;

            if (entity.Id != ft3.Id || cu1_1 == cu1_2)
            {
                Console.WriteLine($"Failed {nameof(Test1_2)}! {nameof(ft3.Id)}:{ft3.Id}  {nameof(ft3.TestValue)}:{ft3.TestValue}  {nameof(ft3.CuToken)}:{ft3.CuToken}  {nameof(ft3.Remark)}:{ft3.Remark}{Environment.NewLine}");
            }
            else
            {
                Console.WriteLine($"{Environment.NewLine}Hello World,{nameof(Test1_2)}! {nameof(ft3.Id)}:{ft3.Id}  {nameof(ft3.TestValue)}:{ft3.TestValue}  {nameof(ft3.CuToken)}:{ft3.CuToken}  {nameof(ft3.Remark)}:{ft3.Remark}{Environment.NewLine}");
            }
        }
        /// <summary>
        /// not set Entry(T).State,use Update to set EntityState
        /// </summary>
        public static void Test1_3()
        {
            var db = ServiceProvider!.CreateScope().ServiceProvider.GetRequiredService<SQLTestDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var entity = new TestClass1 { Remark = Guid.NewGuid().ToString() };
            db.TClass1!.Add(entity);
            //db.TClass1!.Attach(entity);//Bug
            //db.TClass2!.Add(new TestClass2 { Remark = "t2222" });

            db.SaveChanges();

            var ft1 = db.TClass1.OrderByDescending(m => m.Id).FirstOrDefault();
            var cu1_1 = ft1!.CuToken;

            Console.WriteLine($"{Environment.NewLine}{nameof(Test1_3)} Firt Saved! {nameof(ft1.Id)}:{ft1.Id}  {nameof(ft1.TestValue)}:{ft1.TestValue}  {nameof(ft1.CuToken)}:{ft1.CuToken}  {nameof(ft1.Remark)}:{ft1.Remark}{Environment.NewLine}");

            ft1.Remark = $"{nameof(Test1_3)},update";
            db.TClass1!.Update(entity);//case 3
            db.SaveChanges();


            var ft3 = db.TClass1.OrderByDescending(m => m.Id).FirstOrDefault();

            var cu1_2 = ft3!.CuToken;


            if (entity.Id != ft3.Id || cu1_1 == cu1_2)
            {
                Console.WriteLine($"Failed {nameof(Test1_3)}! {nameof(ft3.Id)}:{ft3.Id}  {nameof(ft3.TestValue)}:{ft3.TestValue}  {nameof(ft3.CuToken)}:{ft3.CuToken}  {nameof(ft3.Remark)}:{ft3.Remark}{Environment.NewLine}");
            }
            else
            {
                Console.WriteLine($"{Environment.NewLine}Hello World,{nameof(Test1_3)}! {nameof(ft3.Id)}:{ft3.Id}  {nameof(ft3.TestValue)}:{ft3.TestValue}  {nameof(ft3.CuToken)}:{ft3.CuToken}  {nameof(ft3.Remark)}:{ft3.Remark}{Environment.NewLine}");
            }
        }

        /// <summary>
        /// set Entry(T).State,and use Update to set EntityState
        /// </summary>
        public static void Test1_4()
        {
            var db = ServiceProvider!.CreateScope().ServiceProvider.GetRequiredService<SQLTestDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var entity = new TestClass1 { Remark = Guid.NewGuid().ToString() };
            db.TClass1!.Add(entity);
            //db.TClass1!.Attach(entity);//Bug
            //db.TClass2!.Add(new TestClass2 { Remark = "t2222" });

            db.SaveChanges();

            var ft1 = db.TClass1.OrderByDescending(m => m.Id).FirstOrDefault();
            var cu1_1 = ft1!.CuToken;

            Console.WriteLine($"{Environment.NewLine}{nameof(Test1_4)} Firt Saved! {nameof(ft1.Id)}:{ft1.Id}  {nameof(ft1.TestValue)}:{ft1.TestValue}  {nameof(ft1.CuToken)}:{ft1.CuToken}  {nameof(ft1.Remark)}:{ft1.Remark}{Environment.NewLine}");

            ft1.Remark = $"{nameof(Test1_4)},update";
            db.Entry(ft1).State = EntityState.Modified;//case 4
            db.TClass1!.Update(entity);//case 4
            db.SaveChanges();


            var ft3 = db.TClass1.OrderByDescending(m => m.Id).FirstOrDefault();

            var cu1_2 = ft3!.CuToken;


            if (entity.Id != ft3.Id || cu1_1 == cu1_2)
            {
                Console.WriteLine($"Failed {nameof(Test1_4)}! {nameof(ft3.Id)}:{ft3.Id}  {nameof(ft3.TestValue)}:{ft3.TestValue}  {nameof(ft3.CuToken)}:{ft3.CuToken}  {nameof(ft3.Remark)}:{ft3.Remark}{Environment.NewLine}");
            }
            else
            {
                Console.WriteLine($"{Environment.NewLine}Hello World,{nameof(Test1_4)}! {nameof(ft3.Id)}:{ft3.Id}  {nameof(ft3.TestValue)}:{ft3.TestValue}  {nameof(ft3.CuToken)}:{ft3.CuToken}  {nameof(ft3.Remark)}:{ft3.Remark}{Environment.NewLine}");
            }
        }

        public static void Test2()
        {
            var db = ServiceProvider!.GetRequiredService<InMemoryTestDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            var entity = new TestClass3 { Id = MyLongValueGenerator.NextID(), Remark = "t33" };
            db.TClass3!.Add(entity);

            db.SaveChanges();

            var ft1 = db.TClass3.OrderByDescending(m => m.Id).FirstOrDefault();
            var cu1_1 = ft1!.CuToken;
            Console.WriteLine($"{Environment.NewLine}Firt Saved! {nameof(ft1.Id)}:{ft1.Id}  {nameof(ft1.TestValue)}:{ft1.TestValue}  {nameof(ft1.CuToken)}:{ft1.CuToken}  {nameof(ft1.Remark)}:{ft1.Remark}{Environment.NewLine}{Environment.NewLine}");

            ft1.Remark = "t33,uu";
            db.Entry(ft1).State = EntityState.Modified;
            db.SaveChanges();


            var ft3 = db.TClass3.OrderByDescending(m => m.Id).FirstOrDefault();

            var cu1_2 = ft3!.CuToken;

            if (cu1_1 == cu1_2 || entity.Id != ft3.Id)
            {
                Console.WriteLine($"Failed {nameof(Test2)}! {nameof(ft3.Id)}:{ft3.Id}  {nameof(ft3.TestValue)}:{ft3.TestValue}  {nameof(ft3.CuToken)}:{ft3.CuToken}  {nameof(ft3.Remark)}:{ft3.Remark}{Environment.NewLine}");
            }
            else
            {
            Console.WriteLine($"{Environment.NewLine}Hello World,{nameof(Test2)}! {nameof(ft3.Id)}:{ft3.Id}  {nameof(ft3.TestValue)}:{ft3.TestValue}  {nameof(ft3.CuToken)}:{ft3.CuToken}  {nameof(ft3.Remark)}:{ft3.Remark}{Environment.NewLine}");
            }
        }
    }
}
