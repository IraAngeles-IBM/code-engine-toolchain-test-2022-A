using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilingManagementService.Model
{
  
    public class LeaveRequest
    {
        public string   leave_id                {get;set;}
        public string   leave_code              {get;set;}
        public int      leave_type_id           {get;set;}
        public string   date_from               {get;set;}
        public string   date_to                 {get;set;}
        public bool     is_half_day             {get;set;}
        public bool     is_paid                 {get;set;}
        public string   description             {get;set;}
        public string   employee_id              {get;set;}
        public bool     with_attachment         {get;set;}
        public string   approval_level_id       {get;set;}
        public bool     active                  {get;set;}
        public string   series_code             {get;set;}
        public string   category_id             {get;set;}
        public string   created_by              {get;set;}
    }
    public class LeaveResponse
    {
        public int      leave_id                {get;set;}
        public string   encrypted_leave_id      { get; set; }
        public string   leave_name              {get;set;}
        public string   leave_code              {get;set;}
        public int      leave_type_id           {get;set;}
        public string   date_from               {get;set;}
        public string   date_to                 {get;set;}
        public bool     is_half_day             {get;set;}
        public bool     is_paid                 {get;set;}
        public decimal   leave_balance             {get;set;}
        public string   description             {get;set;}
        public int      created_by              {get;set;}
        public string   encrypted_created_by         { get; set; }
        public string   created_by_name         { get; set; }
        public bool     active                  {get;set;}
        public string   date_created            {get;set;}
        public bool     approved                 {get;set;}
        public string   status                  {get;set;}
    }

}
