using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilingManagementService.Model
{
    public class DashboardResponse
    {
        public string title { get; set; }
        public string backgroundColor { get; set; }
        public string id { get; set; }
        public int module_id { get; set; }
        public string status { get; set; }
    }
}
