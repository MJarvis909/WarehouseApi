using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace WarehouseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarehouseController : ControllerBase
    {
        [HttpGet("employees")]

        public IActionResult GetEmployees()
        {
            var connection = new WarehouseDbContext().Connection;

            var result = connection.Query<Employee>(@"SELECT
                                                    id AS Id,
                                                    first_name AS FirstName,
                                                    last_name AS LastName,
                                                    salary AS Salary,
                                                    employed_at AS EmployedAt
                                                    FROM tools.employees;");
            return Ok(result);
        }

        [HttpGet("employees/{id}")]

        public IActionResult GetEmployees(int id)
        {
            var connection = new WarehouseDbContext().Connection;

            var result = connection.Query<Employee>(@"SELECT
                                                    id AS Id,
                                                    first_name AS FirstName,
                                                    last_name AS LastName,
                                                    salary AS Salary,
                                                    employed_at AS EmployedAt
                                                    FROM tools.employees
                                                    WHERE Id = @id", new {id});
            return Ok(result);
        }

        [HttpGet("employees/firstName/{firstName}/lastName/{lastName}")]

        public IActionResult GetEmployees(string firstName, string lastName)
        {
            var connection = new WarehouseDbContext().Connection;

            var result = connection.Query<string>(@"SELECT name
                                                    FROM tools.tool
                                                    JOIN tools.borrowed ON tool.id = borrowed.tool_id
                                                    JOIN tools.employees ON borrowed.employee_id = employees.id
                                                    WHERE LOWER(employees.first_name) = LOWER(@firstName)
                                                    AND LOWER(employees.last_name) = LOWER(@lastName)", new {firstName, lastName});
            return Ok(result);
        }
    }
}
