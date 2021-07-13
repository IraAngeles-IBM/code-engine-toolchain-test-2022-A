using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollSetupManagementService.Model
{
    public class PagibigRequest
    {
        public int         contribution_group_id    { get; set; }
        public int          share_type_id            { get; set; }
        public decimal     range_from               { get; set; }
        public decimal     range_to                 { get; set; }
        public decimal     employer_share           { get; set; }
        public decimal     employee_share           { get; set; }
        public string      created_by               { get; set; }
        public string      series_code               { get; set; }
    }
    public class PagibigResponse
    {
        public int          contribution_group_id   { get; set; }
        public int          share_type_id            { get; set; }
        public decimal      range_from              { get; set; }
        public decimal      range_to                { get; set; }
        public decimal      employer_share          { get; set; }
        public decimal      employee_share          { get; set; }
        public int          created_by              { get; set; }
        public string       date_created            { get; set; }
    }

    public class philhealthRequest
    {
        public int         contribution_group_id    { get; set; }
        public decimal     premium_rate             { get; set; }
        public decimal     minimum                  { get; set; }
        public decimal     maximum                  { get; set; }
        public string      created_by               { get; set; }
        public string      series_code               { get; set; }
    }
    public class philhealthResponse
    {
        public int         contribution_group_id    { get; set; }
        public decimal     premium_rate             { get; set; }
        public decimal     minimum                  { get; set; }
        public decimal     maximum                  { get; set; }
        public int         created_by               { get; set; }
        public string      date_created             { get; set; }
    }

    

    public class sssRequest
    {
        public int          contribution_group_id    { get; set; }
        public decimal      range_from              { get; set; }
        public decimal      range_to                { get; set; }
        public decimal      salary_base             { get; set; }
        public decimal      base_amount             { get; set; }
        public decimal      employer_share          { get; set; }
        public decimal      employee_share          { get; set; }
        public decimal      employer_mpf            { get; set; }
        public decimal      employee_mpf            { get; set; }
        public decimal      employee_compensation   { get; set; }
        public string       created_by              { get; set; }
        public string      series_code               { get; set; }
    }
    public class sssResponse
    {
        public int          contribution_group_id    { get; set; }
        public decimal      range_from              { get; set; }
        public decimal      range_to                { get; set; }
        public decimal      salary_base             { get; set; }
        public decimal      base_amount             { get; set; }
        public decimal      employer_share          { get; set; }
        public decimal      employee_share          { get; set; }
        public decimal      employer_mpf            { get; set; }
        public decimal      employee_mpf            { get; set; }
        public decimal      employee_compensation   { get; set; }
        public int          created_by               { get; set; }
        public string       date_created             { get; set; }
    }
    public class taxRequest
    {
        public int contribution_group_id { get; set; }
        public decimal range_from { get; set; }
        public decimal range_to { get; set; }
        public decimal salary_base { get; set; }
        public decimal base_amount { get; set; }
        public decimal tax_percentage { get; set; }
        public int payroll_type_id { get; set; }
        public string created_by { get; set; }
        public string series_code { get; set; }
    }
    public class taxResponse
    {
        public int contribution_group_id { get; set; }
        public decimal range_from { get; set; }
        public decimal range_to { get; set; }
        public decimal salary_base { get; set; }
        public decimal base_amount { get; set; }
        public decimal tax_percentage { get; set; }
        public int payroll_type_id { get; set; }
        public int created_by { get; set; }
        public string date_created { get; set; }
    }

    
    public class PayrollContributionResponse
    {
        public int          payroll_contribution_id                      { get; set; }
        public string       encrypted_payroll_contribution_id            { get; set; }
        public int          employee_id             { get; set; }
        public string       display_name            { get; set; }
        public string       employee_code           { get; set; }
        public int          government_type_id      { get; set; }
        public string       adjustment_type         { get; set; }
        public int          timing_id               { get; set; }
        public string       timing                  { get; set; }
        public decimal      amount                  { get; set; }
        public bool         taxable_id              { get; set; }
        public string       taxable                 { get; set; }
        public int          created_by              { get; set; }
        public string       date_created            { get; set; }
        public string       status                  { get; set; }
    }
    
    public class PayrollContributionRequest
    {
        public string       payroll_contribution_id                      { get; set; }
        public int          employee_id             { get; set; }
        public int          government_type_id      { get; set; }
        public int          timing_id               { get; set; }
        public decimal      amount                  { get; set; }
        public bool         taxable                 { get; set; }
        public string       created_by              { get; set; }
        public string       series_code            { get; set; }
        public bool         active                  { get; set; }
    }
}
