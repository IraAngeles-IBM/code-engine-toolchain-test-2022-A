using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilingManagementService.Model
{ 
    public class OffsetHeaderRequest
    {
        public string   offset_id                {get;set;}
        public string   offset_code              {get;set;}
        public string   reason                  {get;set;}
        public decimal  offset_hour             {get;set;}
        public string   date                    {get;set;}
        public string   approval_level_id       {get;set;}
        public bool     active                  {get;set;}
        public string   series_code             {get;set;}
        public string   category_id             {get;set;}
        public string   created_by              {get;set;}
        public bool     late_filing              { get; set; }

        public OffsetDetailRequest[] OffsetDetail       { get; set; }
    }

    public class OffsetDetailRequest
    {
        public int      overtime_id { get; set; }
        public decimal  offset_hour { get; set; }
    }


    public class OffsetHeaderResponse
    {
        public int      offset_id               {get;set;}
        public string   encrypted_offset_id     {get;set;}
        public string   offset_code             {get;set;}
        public string   reason                  {get;set;}
        public decimal  offset_hour             {get;set;}
        public string   date                    {get;set;}
        public int      created_by              {get;set;}
        public string   created_by_name         { get; set; }
        public bool     active                  {get;set;}
        public string   date_created            {get;set;}
        public bool     approved                 {get;set;}
        public string   status                  {get;set;}
    }


    public class OffsetDetailResponse
    {
        public int      offset_id       { get; set; }
        public int      overtime_id     { get; set; }
        public string   overtime_code   { get; set; }
        public string   date_from       { get; set; }
        public string   date_to         { get; set; }
        public decimal  offset_hour     { get; set; }
        public decimal  overtime_hour   { get; set; }
        public decimal  balance_hour     { get; set; }
        public decimal  offset_used     { get; set; }
        public bool     is_overtime_used     { get; set; }
    }
}
