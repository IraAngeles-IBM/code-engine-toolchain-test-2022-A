using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogServices.Model
{
    public class ErrorLogsRequest
    {
        public string module_id { get; set; }
        public string transaction_id { get; set; }
        public string error_log { get; set; }
        public string created_by { get; set; }
    }


    public class SystemLogsRequest
    {
        public string module_id { get; set; }
        public string transaction_id { get; set; }
        public string transaction_type_id { get; set; }
        public string description { get; set; }
        public string created_by { get; set; }
    }



    public class NotificationResponse
    {
        public int     notification_id          { get; set; }
        public string  encrypt_notification_id  { get; set; }
		public int     module_id                { get; set; }
		public int     transaction_id           { get; set; }
		public string  transaction_type         { get; set; }
		public string  description              { get; set; }
		public bool    is_viewed                { get; set; }
		public int     created_by               { get; set; }
		public string  date_created             { get; set; }
		public string  display_name             { get; set; }
		public string  image_path               { get; set; }
        public string  lapse                    { get; set; }
        public string  icon                     { get; set; }
    }

    

    public class LogResponse
    {

        public int     log_id               { get; set; }
        public string  encrypt_log_id       { get; set; }
		public int     module_id            { get; set; }
		public int     transaction_id       { get; set; }
		public int     transaction_type_id  { get; set; }
		public string transaction_type  { get; set; }
		public string  description          { get; set; }
		public bool    is_viewed            { get; set; }
		public int     created_by           { get; set; }
		public string  date_created         { get; set; }
		public string  display_name         { get; set; }
		public string  image_path           { get; set; }
        public string  lapse                { get; set; }
        public string  icon                 { get; set; }
    }



}
