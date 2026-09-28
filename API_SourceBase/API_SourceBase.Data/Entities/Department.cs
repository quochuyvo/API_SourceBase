using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_SourceBase.Data.Entities
{
    [Table("Department")]
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FarmId { get; set; }

        public int FactoryId { get; set; }

        public int? WorkerId { get; set; }

        [Required]
        [StringLength(30)]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public int Sort { get; set; }

        public short Status { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
