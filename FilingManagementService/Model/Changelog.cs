using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilingManagementService.Model
{

    public class AttendanceCLRequest
    {
        public string employee_id { get; set; }
        public string date { get; set; }
        public string time_in { get; set; }
        public string time_out { get; set; }
        public string created_by { get; set; }
        public string series_code { get; set; }
    }
    public class ChangelogHeaderRequest
    {
        public string   change_log_id        { get; set; }
        public string   change_log_code      { get; set; }
        public string   reason              { get; set; }
        public string   date_from           { get; set; }
        public string   date_to             { get; set; }

        public string   approval_level_id   { get; set; }
        public bool     active              { get; set; }
        public string   series_code         { get; set; }
        public string   category_id         { get; set; }
        public string   created_by          { get; set; }

        public ChangelogDetailRequest[] Detail { get; set; }
    }

    public class ChangelogDetailRequest
    {
        public string  date     { get; set; }
        public string  time_in  { get; set; }
        public string  time_out { get; set; }
        public string remarks  { get; set; }
    }

    

    public class ChangelogHeaderResponse
    {
        public string   encrypted_change_log_id  {get;set;}
        public int      change_log_id           { get; set; }
        public string   change_log_code         { get; set; }
        public string   reason                  { get; set; }
        public string   date_from               { get; set; }
        public string   date_to                 { get; set; }
        public int      created_by              {get;set;}
        public string   created_by_name         { get; set; }
        public bool     active                  {get;set;}
        public string   date_created            {get;set;}
        public bool     approved                {get;set;}
        public string   status                  {get;set;}
    }


    public class ChangelogDetailResponse
    {
        public int      change_log_id   { get; set; }
        public int   employee_id     { get; set; }
        public string   sked_time_in     { get; set; }
        public string   sked_time_out     { get; set; }
        public string   date            { get; set; }
        public string   time_in         { get; set; }
        public string   time_out        { get; set; }
        public string   remarks         { get; set; }
        public bool     is_update         { get; set; }
    }
}
