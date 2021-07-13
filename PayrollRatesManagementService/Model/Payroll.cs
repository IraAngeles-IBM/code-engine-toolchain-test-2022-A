using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollSetupManagementService.Model
{
    public class PayrollHeaderRequest
    {
        public string payroll_header_code       { get; set; }
        public int    timekeeping_header_id     { get; set; }
        public string created_by                { get; set; }
        public bool   active                    { get; set; }
        public string series_code               { get; set; }
    }

    
    public class PayrollHeaderResponse
    {
        public int    payroll_header_id                   { get; set; }
        public string encrypted_payroll_header_id         { get; set; }
        public string payroll_header_code                 { get; set; }
        public int    payroll_type_id                     { get; set; }
        public string payroll_type                        { get; set; }
        public int    cutoff_id                           { get; set; }
        public string cutoff                              { get; set; }
        public int    month_id                            { get; set; }
        public string month                               { get; set; }
        public string date_from                           { get; set; }
        public string date_to                             { get; set; }
        public string pay_date                            { get; set; }
        public int    category_id                         { get; set; }
        public string category_name                       { get; set; }
        public int    branch_id                           { get; set; }
        public string branch                              { get; set; }
        public int    confidentiality_id                  { get; set; }
        public string confidentiality                     { get; set; }
        public int    timekeeping_header_id               { get; set; }
        public int    created_by                          { get; set; }
        public string display_name                        { get; set; }
        public bool   active                              { get; set; }
        public string date_created                        { get; set; }
        public bool   approved                            { get; set; }
        public string status                              { get; set; }
        public int    tk_count                            { get; set; }
    }
}
