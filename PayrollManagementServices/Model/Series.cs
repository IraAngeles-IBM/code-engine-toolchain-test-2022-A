using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollManagementService.Model
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

    public class SeriesTemp
    {
        public int module_id { get; set; }
        public string series_code { get; set; }
        public string created_by { get; set; }
    }

}
