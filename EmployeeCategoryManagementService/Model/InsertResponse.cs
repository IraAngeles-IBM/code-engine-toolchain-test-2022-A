using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeCategoryManagementService.Model
{
    public class InsertResponse
    {
        public int id { get; set; }
        public string description { get; set; }
        public string error_message { get; set; }
    }

}
