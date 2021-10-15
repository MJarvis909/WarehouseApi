using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarehouseApi.Controllers
{
    public class BorrowedTools
    {
        public int ToolId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public string Reason { get; set; }
        public int Quantity { get; set; }
        public bool Active { get; set; }

    }
}
