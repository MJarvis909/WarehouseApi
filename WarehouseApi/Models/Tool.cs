using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseApi.Controllers
{
    public class Tool
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string Producer { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public int? Size { get; set; }

    }
}
