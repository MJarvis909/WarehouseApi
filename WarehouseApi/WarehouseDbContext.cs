using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace WarehouseApi
{
    public class WarehouseDbContext
    {
        private readonly string _connectionString;
        public WarehouseDbContext()
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 5432,
                Username = "postgres",
                Password = "1052",
                Database = "warehouse_db"
            };

            _connectionString = connectionStringBuilder.ConnectionString;
        }
        public IDbConnection Connection => new NpgsqlConnection(_connectionString);
    }
}
