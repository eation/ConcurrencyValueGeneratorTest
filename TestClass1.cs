using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrencyValueGeneratorTest
{
    public class TestClass1
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]//Avoid Set Identity Insert OFF Error
        public long Id
        {
            get;
            set; 
        }

        [StringLength(128)]
        public string? Remark { get; set; } = null;

        public DateTime? Date { get; set; } = DateTime.Now;

        public long? TestValue { get; set; }
        //public long TestValue2 { get; set; } = MyLongValueGenerator.NextID();
        //public long? TestValue3 { get; set; }
        //public long TestValue4 { get; set; } = 8660224005361369088;

        //[Timestamp]
        //[ConcurrencyCheck]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public long CuToken { get; set; }

        [ForeignKey(nameof(T2))]
        public long? T2Id { get; set; }
        public virtual TestClass2? T2{ get; set; }
    }

    public class TestClass2
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [StringLength(128)]
        public string? Remark { get; set; } = null;

        public DateTime? Date { get; set; } = DateTime.Now;

        [Timestamp]
        public byte[] CuToken { get; set; }= null!;
    }
}
