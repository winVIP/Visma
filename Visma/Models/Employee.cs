using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Visma.Models
{
    public class Employee
    {
        [Key]
        [Required]
        public int EmployeeID { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public DateTime EmploymentDate { get; set; }

        [ForeignKey("BossID")]
        public virtual Employee Boss { get; set; }
        //public int BossID { get; set; }
        [Required]
        [MaxLength(200)]
        public string HomeAddress { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentSalary { get; set; }
        [Required]
        [MaxLength(100)]
        public string Role { get; set; }
    }
}
