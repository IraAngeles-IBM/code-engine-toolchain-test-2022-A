using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionManagementService.Model
{
    public class MenuViewResponse
    {
        public string order_by { get; set; }

        public int module_id { get; set; }

        public int parent_module_id { get; set; }

        public string module_type { get; set; }
        public string module_name { get; set; }
        public string classes { get; set; }
        public string link { get; set; }
        public bool has_approval { get; set; }
        public int count { get; set; }
    }

    public class ModuleRequest
    {
        public int module_id { get; set; }
        public int access_level_id { get; set; }
        public string created_by { get; set; }


        public string series_code { get; set; }
    }
    public class ReportAccessRequest
    {
        public int report_id { get; set; }
        public int access_level_id { get; set; }
        public string created_by { get; set; }


        public string series_code { get; set; }
    }


    public class ConfidentialityAccessRequest
    {
        public int confidentiality_id { get; set; }
        public int access_level_id { get; set; }
        public string created_by { get; set; }


        public string series_code { get; set; }
    }


    public class DataUploadAccessRequest
    {
        public int data_upload_id { get; set; }
        public int access_level_id { get; set; }
        public string created_by { get; set; }


        public string series_code { get; set; }
    }


    public class ModuleResponse
    {
        public int module_id { get; set; }
        public string module_name { get; set; }
        public string access_level_id { get; set; }
        public int int_access_level_id { get; set; }
        public string created_by { get; set; }
        public bool active { get; set; }


    }


    public class ApprovalAccessResponse
    {
        public int module_id { get; set; }
        public string module_name { get; set; }
        public int microservice { get; set; }
        public int count { get; set; }


    }




    public class ReportAccessResponse
    {
        public int report_id { get; set; }
        public string report_name { get; set; }
        public string access_level_id { get; set; }
        public int int_access_level_id { get; set; }
        public int report_type_id { get; set; }
        public string report_type { get; set; }
        public string link { get; set; }
        public string created_by { get; set; }
        public bool active { get; set; }


    }


    public class ConfidentialityAccessResponse
    {
        public int confidentiality_id { get; set; }
        public string confidentiality_name { get; set; }
        public string access_level_id { get; set; }
        public int int_access_level_id { get; set; }
        public string created_by { get; set; }
        public bool active { get; set; }


    }


    public class DataUploadAccessResponse
    {
        public int data_upload_id { get; set; }
        public string data_upload_name { get; set; }
        public string access_level_id { get; set; }
        public int int_access_level_id { get; set; }
        public string export { get; set; }
        public string upload { get; set; }
        public string save { get; set; }
        public string service { get; set; }
        public string created_by { get; set; }
        public bool active { get; set; }


    }
}
