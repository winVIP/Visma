using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Visma.Models
{
    public class EmployeeJson
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime EmploymentDate { get; set; }
        public int? BossID { get; set; }
        public string HomeAddress { get; set; }
        public decimal CurrentSalary { get; set; }
        public string Role { get; set; }
        public int GetAge()
        {
            DateTime now = DateTime.Now;
            int age = now.Year - this.BirthDate.Year;
            // In case of a leap year
            if (this.BirthDate > now.AddYears(-age)) age--;
            return age;
        }

        public override bool Equals(object e)
        {
            if (!(e is EmployeeJson)) return false;

            EmployeeJson employee = e as EmployeeJson;
            if(employee.EmployeeID != this.EmployeeID) return false;
            if(employee.FirstName != this.FirstName) return false;
            if(employee.LastName != this.LastName) return false;
            if(employee.BirthDate != this.BirthDate) return false;
            if(employee.EmploymentDate != this.EmploymentDate) return false;
            if(employee.BossID != this.BossID) return false;
            if(employee.HomeAddress != this.HomeAddress) return false;
            if(employee.CurrentSalary != this.CurrentSalary) return false;
            if(employee.Role != this.Role) return false;
            return true;
        }
    }
}
