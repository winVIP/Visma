using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Visma.Models;

namespace Visma.DataAccess
{
    public class VismaDBContext : DbContext
    {
        public VismaDBContext(DbContextOptions options) : base(options) { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ExceptionLog> ExceptionLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string json = File.ReadAllText(Path.GetFullPath("./MockData/EmployeeMockData.json"));
            EmployeeJson[] employees = JsonConvert.DeserializeObject<EmployeeJson[]>(json);

            foreach(EmployeeJson employee in employees)
            {
                modelBuilder.Entity<Employee>().HasData(
                    new
                    {
                        EmployeeID = employee.EmployeeID,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        BirthDate = employee.BirthDate,
                        EmploymentDate = employee.EmploymentDate,
                        BossID = employee.BossID,
                        HomeAddress = employee.HomeAddress,
                        CurrentSalary = employee.CurrentSalary,
                        Role = employee.Role
                    });
            }
        }
    }
}
