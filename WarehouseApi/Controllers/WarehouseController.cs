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
        //-----------------GET-------------------
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
                                                            tool.id AS Id,
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
                                                            tool.id AS Id,
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
                                                            tool.id AS Id,
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
                                                            tool.id AS Id,
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

            var result = connection.Query<BorrowedTools>(@"SELECT 
                                                    borrowed.id AS ToolId,
                                                    borrowed.employee_id AS EmployeeId,
                                                    borrowed.borrowed_at AS BorrowedAt,
                                                    borrowed.reason AS Reason,
                                                    borrowed.quantity AS Quantity,
                                                    borrowed.is_active AS IsActive
                                                    FROM tools.borrowed
                                                    WHERE borrowed.is_active = @borrowed", new { borrowed });
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
        //----------------------------POST--------------------
        [HttpPost("tool")]
        public IActionResult CreateTool([FromBody]Tool tool)
        {
            var dbContext = new WarehouseDbContext();

            var typeId = dbContext.Connection.QueryFirst<int>(@"
                SELECT id FROM tools.tool_type WHERE LOWER(type) LIKE LOWER(@type)",
                new { tool.Type });

            var producerId = dbContext.Connection.QueryFirst<int>(@"
                SELECT id FROM tools.producers WHERE LOWER(name) LIKE LOWER(@producer)", 
                new { tool.Producer});

            dbContext.Connection.Execute(@"INSERT INTO tools.tool(name, type_id, price, producer_id, maintenance_date, size)
                                        VALUES(@Name,@TypeId, @Price, @ProducerId, @MaintenanceDate, @Size)",
            new
            {
                tool.Name,
                tool.Price,
                tool.MaintenanceDate,
                tool.Size,
                typeId,
                producerId
            });
            return Created("", "tool created successfully");
        }
        [HttpPost("producer")]
        public IActionResult CreateProducer([FromBody]Producer producer)
        {
            var dbContext = new WarehouseDbContext();

            dbContext.Connection.Execute(@"INSERT INTO tools.producer(name)
                                        VALUES(@Name)",
            new
            {
                producer.Name
            });
            return Created("", "producer created successfully");
        }
        [HttpPost("employee")]
        public IActionResult CreateEmployee([FromBody]Employee employee)
        {
            var dbContext = new WarehouseDbContext();

            dbContext.Connection.Execute(@"INSERT INTO tools.employee(first_name, last_name, salary, employed_at)
                                        VALUES(@FirstName, @LastName, @Salary, @EmployedAt)",
            new
            {
                employee.FirstName,
                employee.LastName,
                employee.Salary,
                employee.EmployedAt
            });
            return Created("", "employee created successfully");
        }
        [HttpPost("borrowTool/employeeId/{EmployeeId}/toolId/{ToolId}/quantity/{quantity}/reason/{reason}")]
        public IActionResult BorrowTool([FromBody]BorrowedTools borrowedTools)
        {
            var dbContext = new WarehouseDbContext();

            var employeeId = dbContext.Connection.QueryFirst<int>(@"SELECT Id FROM tools.employee
                WHERE employee.id = @EmployeeId", new { borrowedTools.EmployeeId });

            var toolId = dbContext.Connection.QueryFirst<int>(@"SELECT id FROM tools.tool
                WHERE tool.id = @ToolId", new { borrowedTools.ToolId });

            var borrowedAt = DateTime.Now;
            bool isActive = true;

            if (employeeId <= 0)
            {
                return BadRequest("Employee was not found");
            }
            else if (toolId <= 0)
            {
                return BadRequest("Tool was not found");
            }
            else if (string.IsNullOrEmpty(borrowedTools.Reason))
            {
                return BadRequest("Field reason cannot be empty");
            }
            else
            {
                dbContext.Connection.Execute(@"INSERT INTO tools.borrowed(employee_id, borrowed_at, reason, tool_id, is_active, quantity)
                    VALUES(@employeeId, @borrowedAt, @reason, @toolId, @isActive, @quantity)",
                    new { 
                        borrowedTools.EmployeeId,
                        borrowedAt,
                        borrowedTools.Reason,
                        borrowedTools.ToolId,
                        isActive,
                        borrowedTools.Quantity
                    });
            }
            return Created("", "Tool borrowed successfully");
        }
        //-----------PUT-------------------
        [HttpPut("producer")]
        public IActionResult UpdateProducer([FromBody]Producer producer)
        {
            var dbContext = new WarehouseDbContext();

            dbContext.Connection.Execute(@"UPDATE tools.producers SET name = @name WHERE id = @id", 
            new { producer.Name, producer.Id });
            return Ok("Producer succesfully updated");
        }
        [HttpPut("tool")]
        public IActionResult UpdateTool([FromBody]Tool tool)
        {
            var dbContext = new WarehouseDbContext();

            var TypeId = dbContext.Connection.QueryFirst<int>(@"
                SELECT id FROM tools.type WHERE LOWER(type) LIKE LOWER(@Type)", 
                new { tool.Type });

            var ProducerId = dbContext.Connection.QueryFirst<int>(@"
                SELECT id FROM tools.producer WHERE LOWER(name) LIKE LOWER(@Name)",
                new { tool.Producer });

            dbContext.Connection.Execute(@"UPDATE tools.tool SET 
                                        name = @Name, 
                                        type_id = @TypeId,
                                        price = @Price,
                                        producer_id = @ProducerId,
                                        maintenance_date = @MaintenanceDate,
                                        size = @Size
                                        WHERE id = @Id",
            new
            {
                tool.Name,
                TypeId,
                tool.Price,
                ProducerId,
                tool.MaintenanceDate,
                tool.Size,
                tool.Id
            });
            return Ok("Tool successfully updated");
        }
        [HttpPut("employee")]
        public IActionResult UpdateEmployee([FromBody]Employee employee)
        {
            var dbContext = new WarehouseDbContext();

            dbContext.Connection.Execute(@"UPDATE tools.employee SET
                                        first_name = @FirstName,
                                        last_name = @LastName,
                                        salary = @Salary,
                                        employed_at = @EmployedAt
                                        WHERE id = @Id",
            new
            { 
                employee.FirstName,
                employee.LastName,
                employee.Salary,
                employee.EmployedAt,
                employee.Id
            });
            return Ok("Employee successfully updated");
        }
        //---------------DELETE-------------
        [HttpDelete("employee/firstName/{firstName}/lastName/{lastName}")]
        public IActionResult DeleteEmployee(string firstName, string lastName)
        {
            var dbContext = new WarehouseDbContext();
            dbContext.Connection.Execute(@"DELETE FROM tools.employees
                                           WHERE LOWER(first_name) LIKE LOWER(@firstName)
                                           AND LOWER(last_name) LIKE LOWER(@lastName)",
            new { firstName, lastName });
            return Ok("Employee deleted successfully");
        }
        [HttpDelete("tool/name/{name}")]
        public IActionResult DeleteTool(string name)
        {
            var dbContext = new WarehouseDbContext();
            dbContext.Connection.Execute(@"DELETE FROM tools.tool 
                                           WHERE LOWER(name) LIKE LOWER(@name)",
            new { name });
            return Ok("Tool deleted successfully");
        }
        [HttpDelete("producer/name/{name}")]
        public IActionResult DeleteProducer(string name)
        {
            var dbContext = new WarehouseDbContext();
            dbContext.Connection.Execute(@"DELETE FROM tools.producer
                                           WHERE LOWER(name) LIKE LOWER(@name)",
            new { name });
            return Ok("Tool deleted successfully");
        }
    }
}
