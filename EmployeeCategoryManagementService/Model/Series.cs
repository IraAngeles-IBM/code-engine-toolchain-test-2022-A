using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeCategoryManagementService.Model
{

    public class SeriesRequest
    {
        public string module_id { get; set; }
        public string series_code { get; set; }
    }

    public class SeriesResponse
    {
        public string series_code { get; set; }
    }

}
