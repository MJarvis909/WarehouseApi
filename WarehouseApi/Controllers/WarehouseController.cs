﻿using Microsoft.AspNetCore.Mvc;
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
                                                    employee.id AS Id,
                                                    employee.first_name AS FirstName,
                                                    employee.last_name AS LastName,
                                                    employee.salary AS Salary,
                                                    employee.employed_at AS EmployedAt
                                                    FROM tools.employee;");
            return Ok(result);
        }

        [HttpGet("employees/{id}")]

        public IActionResult GetEmployees(int id)
        {
            var connection = new WarehouseDbContext().Connection;

            var result = connection.Query<Employee>(@"SELECT
                                                    employee.first_name AS FirstName,
                                                    employee.last_name AS LastName,
                                                    employee.salary AS Salary,
                                                    employee.employed_at AS EmployedAt
                                                    FROM tools.employee
                                                    WHERE Id = @id", new {id});
            return Ok(result);
        }

        [HttpGet("employees/firstName/{firstName}/lastName/{lastName}")]

        public IActionResult GetEmployees(string firstName, string lastName)
        {
            var connection = new WarehouseDbContext().Connection;

            var result = connection.Query<string>(@"SELECT tool.name
                                                    FROM tools.tool
                                                    JOIN tools.borrowed ON tool.id = borrowed.tool_id
                                                    JOIN tools.employee ON borrowed.employee_id = employee.id
                                                    WHERE LOWER(employee.first_name) = LOWER(@firstName)
                                                    AND LOWER(employee.last_name) = LOWER(@lastName)", new {firstName, lastName});
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
                                                            producer.name AS Producer,
                                                            tool.maintenance_date AS MaintenanceDate,
                                                            tool.size AS Size
                                                            FROM tools.tool
                                                            JOIN tools.producer ON tool.producer_id = producer.id
                                                            JOIN tools.tool_type ON tool.type_id = tool_type.id
                                                            WHERE  LOWER(tool.name) LIKE LOWER(@name)", new { name });

            }
            else if (!string.IsNullOrWhiteSpace(type))
            {
                result = dbContext.Connection.Query<Tool>(@"SELECT 
                                                            tool.name AS Name,
                                                            tool_type.type AS Type,
                                                            tool.price AS Price,
                                                            producer.name AS Producer,
                                                            tool.maintenance_date AS MaintenanceDate,
                                                            tool.size AS Size
                                                            FROM tools.tool
                                                            JOIN tools.producer ON tool.producer_id = producer.id
                                                            JOIN tools.tool_type ON tool.type_id = tool_type.id
                                                            WHERE  LOWER(tool_type.type) LIKE LOWER(@type)", new { type });
            }
            else if (!string.IsNullOrWhiteSpace(producer))
            {
                result = dbContext.Connection.Query<Tool>(@"SELECT 
                                                            tool.name AS Name,
                                                            tool_type.type AS Type,
                                                            tool.price AS Price,
                                                            producer.name AS Producer,
                                                            tool.maintenance_date AS MaintenanceDate,
                                                            tool.size AS Size
                                                            FROM tools.tool
                                                            JOIN tools.producer ON tool.producer_id = producer.id
                                                            JOIN tools.tool_type ON tool.type_id = tool_type.id
                                                            WHERE  LOWER(producer.name) LIKE LOWER(@producer)", new { producer });
            }
            else
            {
                result = dbContext.Connection.Query<Tool>(@"SELECT 
                                                            tool.name AS Name,
                                                            tool_type.type AS Type,
                                                            tool.price AS Price,
                                                            producer.name AS Producer,
                                                            tool.maintenance_date AS MaintenanceDate,
                                                            tool.size AS Size
                                                            FROM tools.tool
                                                            JOIN tools.producer ON tool.producer_id = producer.id
                                                            JOIN tools.tool_type ON tool.type_id = tool_type.id");
            }


            return Ok(result);
        }
        [HttpGet("tools/byproducer")]
        public IActionResult GetToolsByProducer(string producer)
        {
            var connection = new WarehouseDbContext().Connection;
            var result = connection.Query<Tool>(@"SELECT 
                                                  tool.name AS Name,
                                                  tool_type.type AS Type,
                                                  price AS Price,
                                                  producer.name AS Producer,
                                                  tool.maintenance_date AS MaintenanceDate,
                                                  tool.size AS Size
                                                  FROM tools.tool
                                                  JOIN tools.tool_type ON tool.type_id = tool_type.id
                                                  JOIN tools.producer ON tool.producer_id = producer.id
                                                  WHERE  LOWER(producer.name) LIKE LOWER(@producer)", new { producer });

            return Ok(result);
        }

        [HttpGet("tools/borrowed/{borrowed}")]

        public IActionResult GetTools(bool borrowed)
        {
            var connection = new WarehouseDbContext().Connection;

            var result = connection.Query<Borrowed>(@"SELECT 
                                                    tool.name AS Name,
                                                    tool_type.type AS Type,
                                                    tool.price AS Price,
                                                    producer.name AS Producer,
                                                    tool.maintenance_date AS MaintenanceDate,
                                                    tool.size AS Size
                                                    FROM tools.tool
                                                    JOIN tools.tool_type ON tool.type_id = tool_type.id
                                                    JOIN tools.producer ON tool.producer_id = producer.id
                                                    JOIN tools.borrowed ON tool.id = borrowed.tool_id
                                                    WHERE is_active = @borrowed", new { borrowed });

            return Ok(result);
        }

        [HttpGet("producers")]
        public IActionResult GetProducers(string type)
        {
            var dbContext = new WarehouseDbContext();
            IEnumerable<Producer> result = default;

            if (!string.IsNullOrWhiteSpace(type))
            {
                result = dbContext.Connection.Query<Producer>(@"SELECT 
                                                                DISTINCT producer.name AS Name,
                                                                tool_type.type AS Type
                                                                FROM tools.producer
                                                                JOIN tools.tool ON tool.producer_id = producer.id 
                                                                JOIN tools.tool_type ON tool.type_id = tool_type.id
                                                                WHERE LOWER(tool_type.type) LIKE LOWER(@type)", new { type });
            }
            else
            {
                result = dbContext.Connection.Query<Producer>(@"SELECT 
                                                                producer.name AS Name
                                                                FROM tools.producer");
            }
            
            return Ok(result);

        }

    }
}
