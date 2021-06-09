using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseApi
{
    public class Tool
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public decimal Price { get; set; }
        public int ProducerId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public int Size { get; set; }

    }
}
