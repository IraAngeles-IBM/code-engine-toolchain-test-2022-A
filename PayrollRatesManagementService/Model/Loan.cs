using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollSetupManagementService.Model
{
    public class LoanHeaderRequest
    {
        public string   loan_id             { get; set; }
        public string   loan_code             { get; set; }
        public int      loan_type_id        { get; set; }
        public string   loan_name           { get; set; }
        public int      employee_id         { get; set; }
        public decimal  total_amount         { get; set; }
        public string   loan_date           { get; set; }
        public string   loan_start          { get; set; }
        public int      terms               { get; set; }
        public int      loan_timing_id      { get; set; }
        public string   created_by          { get; set; }
        public bool     active              { get; set; }
        public string   series_code          { get; set; }

        public LoanDetailRequest[] Detail   { get; set; }
    }
    
    public class LoanDetailRequest
    {
        public int      loan_detail_id          { get; set; }
        public string   date                    { get; set; }
        public decimal  amount                  { get; set; }
        public bool     active                  { get; set; }
    }

    
    public class LoanLoadResponse
    {
        public string   date                    { get; set; }
        public decimal  amount                  { get; set; }
        public decimal  paid                    { get; set; }
        public decimal  balance                 { get; set; }
        public string   payslip                 { get; set; }
        public bool     active                  { get; set; }
    }
    
    public class LoanDetailResponse
    {
        public int      loan_detail_id          { get; set; }
        public int      loan_id                 { get; set; }
        public string   date                    { get; set; }
        public decimal  amount                  { get; set; }
        public decimal  paid                    { get; set; }
        public decimal  balance                 { get; set; }
        public string   payslip                 { get; set; }
        public int      created_by              { get; set; }
        public string   date_created            { get; set; }
        public bool     active                  { get; set; }
    }

    public class LoanResponse
    {
        public int      loan_id             { get; set; }
        public string   encrypted_loan_id   { get; set; }
        public string   loan_code        { get; set; }
        public int      loan_type_id        { get; set; }
        public string   loan_type           { get; set; }
        public string   loan_name           { get; set; }
        public int      employee_id         { get; set; }
        public string   employee_code        { get; set; }
        public string   display_name        { get; set; }
        public decimal  total_amount         { get; set; }
        public string   loan_date           { get; set; }
        public string   loan_start          { get; set; }
        public int      terms               { get; set; }
        public int      loan_timing_id      { get; set; }
        public string   loan_timing         { get; set; }
        public int      created_by          { get; set; }
        public string   created_by_name        { get; set; }
        public string   date_created        { get; set; }
        public bool     active              { get; set; }
        public string   status        { get; set; }
    }
    public class LoanIUResponse
    {
        public int loan_id { get; set; }
    }
}
