using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Visma.DataAccess;
using Visma.Models;

namespace Visma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly VismaDBContext _context;

        public EmployeesController(VismaDBContext context)
        {
            _context = context;
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeJson>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return ToEmployeeJson(employee);
        }

        // GET: api/Employees
        [HttpGet("name/{name?}/birthdateFrom/{birthdateFrom?}/birthdateTo/{birthdateTo?}")]
        public async Task<ActionResult<IEnumerable<EmployeeJson>>> GetEmployee(string name = "", DateTime? birthdateFrom = null, DateTime? birthdateTo = null)
        {
            name = name == null ? name = "" : name.Trim();
            List<Employee> employees = await _context.Employees.Where(
                x =>
                (
                    name.Trim().Equals(string.Empty)
                    || ((x.FirstName + " " + x.LastName).Contains(name)
                    || (x.LastName + " " + x.FirstName).Contains(name))
                )
                &&
                (
                    birthdateFrom == null
                    || x.BirthDate >= birthdateFrom
                )
                &&
                (
                    birthdateTo == null
                    || x.BirthDate <= birthdateTo
                )).ToListAsync();

            return employees.Select(x => ToEmployeeJson(x)).ToList();
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeJson>>> GetEmployees()
        {
            List<Employee> employees = await _context.Employees.ToListAsync();
            return employees.Select(x => ToEmployeeJson(x)).ToList();
        }

        // GET: api/Employees
        [HttpGet("bossID/{bossID}")]
        public async Task<ActionResult<IEnumerable<EmployeeJson>>> GetEmployeesByBossID(int bossID)
        {
            List<Employee> employees = await _context.Employees.Where(
                x => x.Boss.EmployeeID == bossID).ToListAsync();
            return employees.Select(x => ToEmployeeJson(x)).ToList();
        }

        // GET: api/Employees
        [HttpGet("count/{role}")]
        public async Task<ActionResult<int>> GetEmployeeCount(string role = null)
        {
            if(role == null)
            {
                return await _context.Employees.CountAsync();
            }
            else
            {
                return await _context.Employees.Where(x => x.Role.Equals(role)).CountAsync();
            }
        }

        // GET: api/Employees
        [HttpGet("averageSalary/{role}")]
        public async Task<ActionResult<decimal>> GetEmployeeAverageSalary(string role = null)
        {
            if (role == null)
            {
                return await _context.Employees.AverageAsync(x => x.CurrentSalary);
            }
            else
            {
                return await _context.Employees.Where(x => x.Role.Equals(role)).AverageAsync(x => x.CurrentSalary);
            }
        }

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeJson>> PostEmployee(EmployeeJson employee)
        {
            int age = employee.GetAge();
            if (age > 70 || age < 18)
            {
                return BadRequest("Employee must be at least 18 years old and not older than 70 years");
            }
            if (employee.CurrentSalary < 0)
            {
                return BadRequest("Salary must be non-negative");
            }
            if (employee.EmploymentDate > DateTime.Now)
            {
                return BadRequest("EmploymentDate cannot be a future date");
            }
            if (employee.EmploymentDate < new DateTime(2000, 1, 1))
            {
                return BadRequest("EmploymentDate cannot be earlier than 2000-01-01");
            }
            if(employee.FirstName.Length > 50)
            {
                return BadRequest("FirstName cannot be longer than 50 characters");
            }
            if (employee.LastName.Length > 50)
            {
                return BadRequest("LastName cannot be longer than 50 characters");
            }
            if (employee.FirstName.Equals(employee.LastName))
            {
                return BadRequest("FirstName cannot be the same as LastName");
            }
            if (employee.Role.Equals("CEO") && _context.Employees.Where(x => x.Role.Equals("CEO")).Count() > 0)
            {
                return BadRequest("There can be only 1 employee with CEO role");
            }
            if (employee.BossID == null && !employee.Role.Equals("CEO"))
            {
                return BadRequest("Only CEO role has no boss");
            }
            if (employee.BossID != null && !EmployeeExists(employee.BossID.GetValueOrDefault()))
            {
                return NotFound("Boss not found");
            }

            Employee newEmployee = new Employee()
            {
                EmployeeID = 0,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                BirthDate = employee.BirthDate,
                EmploymentDate = employee.EmploymentDate,
                Boss = employee.BossID == null ? null : _context.Employees.Where(x => x.EmployeeID == employee.BossID).First(),
                HomeAddress = employee.HomeAddress,
                CurrentSalary = employee.CurrentSalary,
                Role = employee.Role
            };

            _context.Employees.Add(newEmployee);

            await _context.SaveChangesAsync();

            int id = _context.Employees.Max(x => x.EmployeeID);

            return ToEmployeeJson(await _context.Employees.FindAsync(id));
        }

        // PUT: api/Employees/5
        [HttpPut]
        public async Task<IActionResult> PutEmployee(EmployeeJson employee)
        {
            if (!EmployeeExists(employee.EmployeeID))
            {
                return NotFound("Employee not found");
            }
            int age = employee.GetAge();
            if (age > 70 || age < 18)
            {
                return BadRequest("Employee must be at least 18 years old and not older than 70 years");
            }
            if (employee.CurrentSalary < 0)
            {
                return BadRequest("Salary must be non-negative");
            }
            if (employee.EmploymentDate > DateTime.Now)
            {
                return BadRequest("EmploymentDate cannot be a future date");
            }
            if (employee.EmploymentDate < new DateTime(2000, 1, 1))
            {
                return BadRequest("EmploymentDate cannot be earlier than 2000-01-01");
            }
            if (employee.FirstName.Length > 50)
            {
                return BadRequest("FirstName cannot be longer than 50 characters");
            }
            if (employee.LastName.Length > 50)
            {
                return BadRequest("LastName cannot be longer than 50 characters");
            }
            if (employee.FirstName.Equals(employee.LastName))
            {
                return BadRequest("FirstName cannot be the same as LastName");
            }
            if (employee.Role.Equals("CEO") && _context.Employees.Where(x => x.Role.Equals("CEO") && x.EmployeeID != employee.EmployeeID).Count() > 0)
            {
                return BadRequest("There can be only 1 employee with CEO role");
            }
            if (employee.BossID == null && !employee.Role.Equals("CEO"))
            {
                return BadRequest("Only CEO role has no boss");
            }
            if (employee.BossID != null && !EmployeeExists(employee.BossID.GetValueOrDefault()))
            {
                return NotFound("Boss not found");
            }

            Employee newEmployee = new Employee()
            {
                EmployeeID = employee.EmployeeID,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                BirthDate = employee.BirthDate,
                EmploymentDate = employee.EmploymentDate,
                Boss = employee.BossID == null ? null : _context.Employees.Where(x => x.EmployeeID == employee.BossID).First(),
                HomeAddress = employee.HomeAddress,
                CurrentSalary = employee.CurrentSalary,
                Role = employee.Role
            };

            _context.Employees.Attach(newEmployee);

            _context.Entry(newEmployee).Property(x => x.FirstName).IsModified = true;
            _context.Entry(newEmployee).Property(x => x.LastName).IsModified = true;
            _context.Entry(newEmployee).Property(x => x.BirthDate).IsModified = true;
            _context.Entry(newEmployee).Property(x => x.EmploymentDate).IsModified = true;
            _context.Entry(newEmployee).Reference(x => x.Boss).IsModified = true;
            _context.Entry(newEmployee).Property(x => x.HomeAddress).IsModified = true;
            _context.Entry(newEmployee).Property(x => x.CurrentSalary).IsModified = true;
            _context.Entry(newEmployee).Property(x => x.Role).IsModified = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Employees/5
        [HttpPatch("{id}/{salary}")]
        public async Task<IActionResult> PatchEmployee(int id, decimal salary)
        {
            if (!EmployeeExists(id))
            {
                return NotFound();
            }
            if (salary < 0)
            {
                return BadRequest("Salary must be non-negative");
            }

            Employee employee = new Employee() { EmployeeID = id, CurrentSalary = salary };

            _context.Employees.Attach(employee);

            _context.Entry(employee).Property(x => x.CurrentSalary).IsModified = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeID == id);
        }

        private EmployeeJson ToEmployeeJson(Employee employee)
        {
            return new EmployeeJson()
            {
                EmployeeID = employee.EmployeeID,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                BirthDate = employee.BirthDate,
                EmploymentDate = employee.EmploymentDate,
                BossID = employee.Boss == null ? null : employee.Boss.EmployeeID,
                HomeAddress = employee.HomeAddress,
                CurrentSalary = employee.CurrentSalary,
                Role = employee.Role
            };
        }
    }
}
