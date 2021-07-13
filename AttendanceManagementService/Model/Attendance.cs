using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceManagementService.Model
{
    public class AttendanceLogRequest
    {
       public string bio_id          {get;set;}
       public string date_time       {get;set;}
       public int    in_out          {get;set;}
       public string terminal_id     {get;set;}
       public int    created_by      {get;set;}
       public string date_created    {get;set;}
       public string date_time_new   {get;set;}
       public int    in_out_new      {get;set;}
       public string created_by_new  {get;set; }
       public string series_code    { get; set; }

    }


    public class AttendanceLogApprovalRequest
    {
        public string transaction_id { get; set; }
        public string created_by { get; set; }
        public string series_code { get; set; }

    }

    public class LogRequest
    {
        public string created_by { get; set; }

        public string series_code { get; set; }
    }

    public class AttendanceLog
    {
        public string bio_id        { get; set; }
        public string date_time     { get; set; }
        public int    in_out        { get; set; }
        public string terminal_id   { get; set; }
        public int    created_by    { get; set; }
        public string date_created { get; set; }
    }

    public class AttendanceCLResponse
    {
        public int    employee_id   { get; set; }
        public string date          { get; set; }
        public string time_in       { get; set; }
        public string time_out      { get; set; }
        public string sked_time_in  { get; set; }
        public string sked_time_out { get; set; }
        public string remarks       { get; set; }
        public bool   is_update       { get; set; }
    }


    public class AttendanceCLRequest
    {
        public string employee_id { get; set; }
        public string date { get; set; }
        public string time_in { get; set; }
        public string time_out { get; set; }
        public string created_by { get; set; }
        public string series_code { get; set; }
    }


    public class AttendanceRequest
    {
        public int in_out { get; set; }
        public string created_by { get; set; }
        public string series_code { get; set; }
    }



    public class EmployeeAttendanceResponse
    {
        public int    employee_id { get; set; }
        public string encrypt_employee_id { get; set; }
        public string employee_code { get; set; }
        public string display_name  { get; set; }
        public string date          { get; set; }
        public string time_in       { get; set; }
        public string time_out      { get; set; }
        public string sked_in  { get; set; }
        public string sked_out { get; set; }
        public string remarks       { get; set; }
        public bool missing_logs  { get; set; }
        public bool is_add { get; set; }
    }


    public class EmployeeAttendanceRequest
    {
        public string employee_id   { get; set; }
        public string date          { get; set; }
        public string time_in       { get; set; }
        public string time_out      { get; set; }
        public string sked_time_in  { get; set; }
        public string sked_time_out { get; set; }
        public string series_code   { get; set; }
        public string created_by    { get; set; }
    }
}
