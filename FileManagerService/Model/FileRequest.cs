using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerService.Model
{
    public class FileRequest
    {
        public int module_id { get;set; }
        public int transaction_id { get; set; }
        public string file_path { get; set; }

        public string file_name { get; set; }
        public string file_type{ get; set; }
        public string created_by { get; set; }

        public string series_code { get; set; }

    }


    public class FileDelRequest
    {
        public int module_id { get; set; }
        public int transaction_id { get; set; }
        public string series_code { get; set; }

    }

    public class FileResponse
    {
        public int      module_id       { get; set; }
        public int      transaction_id  { get; set; }
        public string   file_path       { get; set; }
        public string   file_name       { get; set; }
        public string   file_class      { get; set; }
        public int      created_by      { get; set; }
        public string   date_created    { get; set; }
        public string file_type { get; set; }


    }
}
