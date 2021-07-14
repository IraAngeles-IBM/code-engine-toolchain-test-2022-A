using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementService.Model
{
    public class EmployeeMovementRequest
    {
        public int      employee_id               { get; set; }
        public int      movement_type             { get; set; }
        public int      is_dropdown               { get; set; }
        public int      id                        { get; set; }
        public string   description               { get; set; }
        public string   movement_description      { get; set; }
        public string   created_by                { get; set; }
        public string   series_code               { get; set; }
    }
}
