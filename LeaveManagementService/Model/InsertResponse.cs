using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementService.Model
{
    public class InsertResponse
    {
        public int id { get; set; }
        public bool is_restricted { get; set; }
        public bool late_filing { get; set; }
        public bool is_save { get; set; }
        public string description { get; set; }
        public string error_message { get; set; }
    }


    public class UploadInRequest
    {
        public string created_by { get; set; }
        public string series_code { get; set; }



    }
}
