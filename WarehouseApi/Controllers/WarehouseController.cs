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

        [HttpGet("tool")]
        public IActionResult GetTools()
        {
            var connection = new WarehouseDbContext().Connection;

            var result = connection.Query<Tool>(@"SELECT id AS Id,
                                                  name AS Name,
                                                  type_id AS TypeId,
                                                  price AS Price,
                                                  producer_id AS ProducerId,
                                                  maintenance_date AS MaintenanceDate,
                                                  size AS Size
                                                  FROM tools.tool");

            return Ok(result);
        }


        [HttpGet("tools")]
        public IActionResult GetTools(string name, string type, string producer)
        {
            var dbContext = new WarehouseDbContext();
            IEnumerable<Tool> result = default;

            if (!string.IsNullOrWhiteSpace(name))
            {
                result = dbContext.Connection.Query<Tool>(@"SELECT
                                                            tool.name AS Name,
                                                            tool_type.type AS Type,
                                                            tool.price AS Price,
                                                            producers.name AS Producer,
                                                            tool.maintenance_date AS MaintenanceDate,
                                                            tool.size AS Size
                                                            FROM tools.tool
                                                            JOIN tools.producers ON tool.producer_id = producers.id
                                                            JOIN tools.tool_type ON tool.type_id = tool_type.id
                                                            WHERE  LOWER(tool.name) LIKE LOWER(@name)", new { name });

            }
            else if (!string.IsNullOrWhiteSpace(type))
            {
                result = dbContext.Connection.Query<Tool>(@"SELECT 
                                                            tool.name AS Name,
                                                            tool_type.type AS Type,
                                                            tool.price AS Price,
                                                            producers.name AS Producer,
                                                            tool.maintenance_date AS MaintenanceDate,
                                                            tool.size AS Size
                                                            FROM tools.tool
                                                            JOIN tools.producers ON tool.producer_id = producers.id
                                                            JOIN tools.tool_type ON tool.type_id = tool_type.id
                                                            WHERE  LOWER(tool_type.type) LIKE LOWER(@type)", new { type });
            }
            else if (!string.IsNullOrWhiteSpace(producer))
            {
                result = dbContext.Connection.Query<Tool>(@"SELECT 
                                                            tool.name AS Name,
                                                            tool_type.type AS Type,
                                                            tool.price AS Price,
                                                            producers.name AS Producer,
                                                            tool.maintenance_date AS MaintenanceDate,
                                                            tool.size AS Size
                                                            FROM tools.tool
                                                            JOIN tools.producers ON tool.producer_id = producers.id
                                                            JOIN tools.tool_type ON tool.type_id = tool_type.id
                                                            WHERE  LOWER(producers.name) LIKE LOWER(@producer)", new { producer });
            }
            else
            {
                result = dbContext.Connection.Query<Tool>(@"SELECT 
                                                            tool.name AS Name,
                                                            tool_type.type AS Type,
                                                            tool.price AS Price,
                                                            producers.name AS Producer,
                                                            tool.maintenance_date AS MaintenanceDate,
                                                            tool.size AS Size
                                                            FROM tools.tool
                                                            JOIN tools.producers ON tool.producer_id = producers.id
                                                            JOIN tools.tool_type ON tool.type_id = tool_type.id");
            }


            return Ok(result);
        }
        [HttpGet("tool/byproducer")]
        public IActionResult GetToolsByProducer(int producer)
        {
            var connection = new WarehouseDbContext().Connection;
            var result = connection.Query<Tool>(@"SELECT id AS Id,
                                                  name AS Name,
                                                  type_id AS TypeId,
                                                  price AS Price,
                                                  producer_id AS ProducerId,
                                                  maintenance_date AS MaintenanceDate,
                                                  size AS Size
                                                  FROM tools.tool
                                                  WHERE  producer_id = @producer", new { producer });

            return Ok(result);
        }

        [HttpGet("tool/borrowed/{borrowed}")]

        public IActionResult GetTools(bool borrowed)
        {
            var connection = new WarehouseDbContext().Connection;

            var result = connection.Query<Borrowed>(@"SELECT tool.id AS Id,
                                                    tool.name AS Name,
                                                    tool_type.type AS Type,
                                                    price AS Price,
                                                    producers.name AS Producer,
                                                    maintenance_date AS MaintenanceDate,
                                                    size AS Size
                                                    FROM tools.tool
                                                    JOIN tools.tool_type ON tool.type_id = tool_type.id
                                                    JOIN tools.producers ON tool.producer_id = producers.id
                                                    JOIN tools.borrowed ON tool.id = borrowed.tool_id
                                                    WHERE is_active = @borrowed", new { borrowed });

            return Ok(result);
        }

        [HttpGet("producer")]
        public IActionResult GetProducers()
        {
            var connection = new WarehouseDbContext().Connection;

            var result = connection.Query<Producer>(@"SELECT id AS Id,
                                                    name AS Name
                                                    FROM tools.producers");
            return Ok(result);

        }

    }
}
