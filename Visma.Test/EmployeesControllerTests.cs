using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Visma.Controllers;
using Visma.DataAccess;
using Visma.Models;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Visma.Test
{
    [Collection("EmployeesControllerTests")]
    public class EmployeesControllerTests
    {
        private static EmployeeJson ToEmployeeJson(Employee employee)
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

        [Fact]
        public void GetEmployee_Returns_NotFound_ByID()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB1")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson returns = controller.GetEmployee(9999).Result.Value;

            Assert.Null(returns);

            context.Dispose();
        }

        [Fact]
        public void GetEmployee_Returns_Employee_ByID()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB2")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson actual = controller.GetEmployee(2).Result.Value;

            Assert.IsType<EmployeeJson>(actual);

            EmployeeJson expected = ToEmployeeJson(context.Employees.Find(2));
            Assert.Equal(expected, actual);

            context.Dispose();
        }

        [Fact]
        public void GetEmployees_Returns_AllEmployees()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB3")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            List<EmployeeJson> actual = controller.GetEmployees().Result.Value as List<EmployeeJson>;

            Assert.Equal(context.Employees.Count(), actual.Count);

            context.Dispose();
        }

        [Fact]
        public void GetEmployees_Returns_Employee_ByName_BirthdateFrom_BirthdateTo()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB4")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            List<EmployeeJson> actual = controller.GetEmployee("Culver Carde", new DateTime(1991, 11, 5), new DateTime(1991, 11, 5)).Result.Value as List<EmployeeJson>;

            Assert.Equal(1, actual.Count);

            EmployeeJson expected = ToEmployeeJson(context.Employees.Find(1));
            Assert.Equal(expected, actual[0]);

            context.Dispose();
        }

        [Fact]
        public void GetEmployeesByBossID_Returns_AllCEOEmployees()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB5")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            List<EmployeeJson> actual = controller.GetEmployeesByBossID(1).Result.Value as List<EmployeeJson>;

            List<EmployeeJson> expected = context.Employees.Where(x => x.Boss.EmployeeID == 1).Select(x => ToEmployeeJson(x)).ToList();
            Assert.Equal(expected.Count, actual.Count);

            context.Dispose();
        }

        [Fact]
        public void GetEmployeeCount_Returns_CEOCount()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB6")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            int actual = controller.GetEmployeeCount("CEO").Result.Value;

            int expected = context.Employees.Where(x => x.Role.Equals("CEO")).Count();
            Assert.Equal(expected, actual);

            context.Dispose();
        }

        [Fact]
        public void GetEmployeeCount_Returns_AllCount()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB7")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            int actual = controller.GetEmployeeCount().Result.Value;

            int expected = context.Employees.Count();
            Assert.Equal(expected, actual);

            context.Dispose();
        }

        [Fact]
        public void GetEmployeeCount_Returns_AverageSalaryHumanResources()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB8")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            decimal actual = controller.GetEmployeeAverageSalary("Human Resources").Result.Value;

            decimal expected = context.Employees.Where(x => x.Role.Equals("Human Resources")).Average(x => x.CurrentSalary);
            Assert.Equal(expected, actual);

            context.Dispose();
        }

        [Fact]
        public void GetEmployeeCount_Returns_AverageSalaryAll()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB9")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            decimal actual = controller.GetEmployeeAverageSalary().Result.Value;

            decimal expected = context.Employees.Average(x => x.CurrentSalary);
            Assert.Equal(expected, actual);

            context.Dispose();
        }

        [Fact]
        public void PostEmployee_BadAge()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB10")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 0,
                FirstName = "Test",
                LastName = "Test1",
                BirthDate = DateTime.Now,
                EmploymentDate = DateTime.Now,
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PostEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult.Result as BadRequestObjectResult;

            Assert.Equal("Employee must be at least 18 years old and not older than 70 years", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PostEmployee_NegativeSalary()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB11")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 0,
                FirstName = "Test",
                LastName = "Test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = DateTime.Now,
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = -2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PostEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult.Result as BadRequestObjectResult;

            Assert.Equal("Salary must be non-negative", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PostEmployee_EmploymentDateFutureDate()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB12")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 0,
                FirstName = "Test",
                LastName = "Test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = DateTime.Now.AddYears(1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PostEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult.Result as BadRequestObjectResult;

            Assert.Equal("EmploymentDate cannot be a future date", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PostEmployee_EmploymentDateTooEarly()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB13")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 0,
                FirstName = "Test",
                LastName = "Test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(1999, 1, 1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PostEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult.Result as BadRequestObjectResult;

            Assert.Equal("EmploymentDate cannot be earlier than 2000-01-01", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PostEmployee_FirstNameTooLong()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB14")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 0,
                FirstName = new string('t', 51),
                LastName = "Test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(2000, 1, 1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PostEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult.Result as BadRequestObjectResult;

            Assert.Equal("FirstName cannot be longer than 50 characters", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PostEmployee_LastNameTooLong()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB15")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 0,
                FirstName = new string('t', 50),
                LastName = new string('t', 51),
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(2000, 1, 1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PostEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult.Result as BadRequestObjectResult;

            Assert.Equal("LastName cannot be longer than 50 characters", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PostEmployee_FirstNameEqualLastName()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB16")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 0,
                FirstName = "test",
                LastName = "test",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(2000, 1, 1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PostEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult.Result as BadRequestObjectResult;

            Assert.Equal("FirstName cannot be the same as LastName", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PostEmployee_Only1CEO()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB17")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 0,
                FirstName = "test",
                LastName = "test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(2000, 1, 1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "CEO"
            };

            var actionResult = controller.PostEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult.Result as BadRequestObjectResult;

            Assert.Equal("There can be only 1 employee with CEO role", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PostEmployee_Success()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB18")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 0,
                FirstName = "test",
                LastName = "test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(2000, 1, 1),
                BossID = 1,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            EmployeeJson actual = controller.PostEmployee(employee).Result.Value;

            employee.EmployeeID = 21;

            Assert.Equal(employee, actual);

            context.Dispose();
        }

        [Fact]
        public void PutEmployee_EmployeeNotFound()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB19")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 21,
                FirstName = "Test",
                LastName = "Test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = DateTime.Now,
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = -2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PutEmployee(employee).Result;

            NotFoundObjectResult actual = actionResult as NotFoundObjectResult;

            Assert.Equal("Employee not found", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PutEmployee_NegativeSalary()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB20")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 1,
                FirstName = "Test",
                LastName = "Test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = DateTime.Now,
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = -2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PutEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult as BadRequestObjectResult;

            Assert.Equal("Salary must be non-negative", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PutEmployee_EmploymentDateFutureDate()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB21")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 1,
                FirstName = "Test",
                LastName = "Test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = DateTime.Now.AddYears(1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PutEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult as BadRequestObjectResult;

            Assert.Equal("EmploymentDate cannot be a future date", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PutEmployee_EmploymentDateTooEarly()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB22")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 1,
                FirstName = "Test",
                LastName = "Test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(1999, 1, 1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PutEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult as BadRequestObjectResult;

            Assert.Equal("EmploymentDate cannot be earlier than 2000-01-01", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PutEmployee_FirstNameTooLong()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB23")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 1,
                FirstName = new string('t', 51),
                LastName = "Test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(2000, 1, 1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PutEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult as BadRequestObjectResult;

            Assert.Equal("FirstName cannot be longer than 50 characters", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PutEmployee_LastNameTooLong()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB24")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 1,
                FirstName = new string('t', 50),
                LastName = new string('t', 51),
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(2000, 1, 1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PutEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult as BadRequestObjectResult;

            Assert.Equal("LastName cannot be longer than 50 characters", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PutEmployee_FirstNameEqualLastName()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB25")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 1,
                FirstName = "test",
                LastName = "test",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(2000, 1, 1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actionResult = controller.PutEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult as BadRequestObjectResult;

            Assert.Equal("FirstName cannot be the same as LastName", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PutEmployee_Only1CEO()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB26")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 2,
                FirstName = "test",
                LastName = "test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(2000, 1, 1),
                BossID = null,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "CEO"
            };

            var actionResult = controller.PutEmployee(employee).Result;

            BadRequestObjectResult actual = actionResult as BadRequestObjectResult;

            Assert.Equal("There can be only 1 employee with CEO role", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PutEmployee_Success()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB27")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            EmployeeJson employee = new EmployeeJson()
            {
                EmployeeID = 2,
                FirstName = "test",
                LastName = "test1",
                BirthDate = DateTime.Now.AddYears(-50),
                EmploymentDate = new DateTime(2000, 1, 1),
                BossID = 3,
                HomeAddress = "Test",
                CurrentSalary = 2000.2m,
                Role = "Test"
            };

            var actual = controller.PutEmployee(employee).Result;

            Assert.IsType<NoContentResult>(actual);

            context.Dispose();
        }

        [Fact]
        public void PatchEmployee_EmployeeNotFound()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB28")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            object actionResult = controller.PatchEmployee(21, 2000).Result;

            Assert.IsType<NotFoundResult>(actionResult);

            context.Dispose();
        }

        [Fact]
        public void PatchEmployee_NegativeSalary()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB29")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            object actionResult = controller.PatchEmployee(1, -2000).Result;

            BadRequestObjectResult actual = actionResult as BadRequestObjectResult;

            Assert.Equal("Salary must be non-negative", actual.Value);

            context.Dispose();
        }

        [Fact]
        public void PatchEmployee_Success()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB30")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            var actual = controller.PatchEmployee(1, 2000).Result;

            Assert.IsType<NoContentResult>(actual);

            context.Dispose();
        }

        [Fact]
        public void DeleteEmployee_EmployeeNotFound()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB31")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            object actionResult = controller.DeleteEmployee(21).Result;

            Assert.IsType<NotFoundResult>(actionResult);

            context.Dispose();
        }

        [Fact]
        public void DeleteEmployee_Success()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB32")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            EmployeesController controller = new EmployeesController(context);

            object actionResult = controller.DeleteEmployee(1).Result;

            Assert.IsType<NoContentResult>(actionResult);

            context.Dispose();
        }
    }
}
