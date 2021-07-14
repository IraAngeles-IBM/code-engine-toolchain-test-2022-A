using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataUploadManagementService.Model
{

    public class DropdownUpload
    {
        public int dropdown_type_id { get; set; }
        public string dropdown_description { get; set; }
        public int created_by { get; set; }
    }


    public class EmployeeUpload
    {

        public string   employee_code           { get; set; }
        public string   user_name               { get; set; }
        public string   user_hash               { get; set; }
        public int      salutation_id           { get; set; }
        public string   display_name            { get; set; }
        public string   first_name              { get; set; }
        public string   middle_name             { get; set; }
        public string   last_name               { get; set; }
        public int      suffix_id               { get; set; }
        public string   nick_name               { get; set; }
        public int      gender_id               { get; set; }
        public int      nationality_id          { get; set; }
        public string   birthday                { get; set; }
        public string   birth_place             { get; set; }
        public int      civil_status_id         { get; set; }
        public string   height                  { get; set; }
        public string   weight                  { get; set; }
        public int      blood_type_id           { get; set; }
        public int      religion_id             { get; set; }
        public string   mobile                  { get; set; }
        public string   phone                   { get; set; }
        public string   office                  { get; set; }
        public string   email_address           { get; set; }
        public string   personal_email_address  { get; set; }
        public string   alternate_number        { get; set; }

        
        
        public string   pre_unit_floor      { get; set; }
        public string   pre_building        { get; set; }
        public string   pre_street          { get; set; }
        public string   pre_barangay        { get; set; }
        public int      pre_province     { get; set; }
        public int      pre_city         { get; set; }
        public int      pre_region       { get; set; }
        public int      pre_country      { get; set; }
        public string   pre_zip_code         { get; set; }
        
        public string   per_unit_floor      { get; set; }
        public string   per_building        { get; set; }
        public string   per_street          { get; set; }
        public string   per_barangay        { get; set; }
        public int      per_province     { get; set; }
        public int      per_city         { get; set; }
        public int      per_region       { get; set; }
        public int      per_country      { get; set; }
        public string   per_zip_code         { get; set; }
        public int      created_by              { get; set; }


    }
    public class EmployeeUploadResponse
    {
        public int id { get; set; }
        public EmployeeUploadresult[] detail { get; set; }

    }
    public class EmployeeUploadresult
    {
        public string   employee_code           { get; set; }
        public int      salutation_id           { get; set; }
        public string   salutation              { get; set; }
        public string   display_name            { get; set; }
        public string   first_name              { get; set; }
        public string   middle_name             { get; set; }
        public string   last_name               { get; set; }
        public int      suffix_id               { get; set; }
        public string   suffix                  { get; set; }
        public string   nick_name               { get; set; }
        public int      gender_id               { get; set; }
        public string   gender                  { get; set; }
        public int      nationality_id          { get; set; }
        public string   nationality             { get; set; }
        public string   birthday                { get; set; }
        public string   birth_place             { get; set; }
        public int      civil_status_id         { get; set; }
        public string   civil_status            { get; set; }
        public string   height                  { get; set; }
        public string   weight                  { get; set; }
        public int      blood_type_id           { get; set; }
        public string   blood_type              { get; set; }
        public int      religion_id             { get; set; }
        public string   religion                { get; set; }
        public string   mobile                  { get; set; }
        public string   phone                   { get; set; }
        public string   office                  { get; set; }
        public string   email_address           { get; set; }
        public string   personal_email_address  { get; set; }
        public string   alternate_number        { get; set; }

        
        
        public string   pre_unit_floor          { get; set; }
        public string   pre_building            { get; set; }
        public string   pre_street              { get; set; }
        public string   pre_barangay            { get; set; }
        public string   pre_province            { get; set; }
        public string   pre_city                { get; set; }
        public string   pre_region              { get; set; }
        public string   pre_country             { get; set; }
        public string   pre_zip_code            { get; set; }
        
        public string   per_unit_floor          { get; set; }
        public string   per_building            { get; set; }
        public string   per_street              { get; set; }
        public string   per_barangay            { get; set; }
        public string   per_province            { get; set; }
        public string   per_city                { get; set; }
        public string   per_region              { get; set; }
        public string   per_country             { get; set; }
        public string   per_zip_code            { get; set; }
        public bool     with_error              { get; set; }
    }

    public class EmployeeInformationUpload
    {

        public string   employee_code       { get; set; }
        public string   bio_id              { get; set; }
        public int      branch_id           { get; set; }
        public int      employee_status_id  { get; set; }
        public int      occupation_id       { get; set; }
        public string   supervisor_code     { get; set; }
        public int      department_id       { get; set; }
        public string   date_hired          { get; set; }
        public string   date_regularized    { get; set; }
        public int      cost_center_id      { get; set; }
        public int      category_id         { get; set; }
        public int      division_id         { get; set; }
        public int      payroll_type_id     { get; set; }
        public decimal  monthly_rate        { get; set; }
        public decimal  semi_monthly_rate   { get; set; }
        public decimal  factor_rate         { get; set; }
        public decimal  daily_rate          { get; set; }
        public decimal  hourly_rate         { get; set; }
        public int      bank_id             { get; set; }
        public string   bank_account        { get; set; }
        public int      confidentiality_id  { get; set; }
        public string   sss                 { get; set; }
        public string   pagibig             { get; set; }
        public string   philhealth          { get; set; }
        public string   tin                 { get; set; }
        public int      created_by          { get; set; }

    }

    public class DataUploadHeaderResponse
    {
        public int seqn { get; set; }

        public string columns { get; set; }
        public string colname { get; set; }
        public bool is_view { get; set; }

    }

    public class ApprovalRequest
    {
        public int module_id { get; set; }
        public string transaction_id { get; set; }
        public int action { get; set; }

        public string remarks { get; set; }

        public string approved_by { get; set; }
        public string series_code { get; set; }
    }

    public class ChangelogUpload
    {

        public string employee_code { get; set; }
        public string reason        { get; set; }
        public string date          { get; set; }
        public string time_in       { get; set; }
        public string time_out      { get; set; }
        public int    created_by    { get; set; }

    }

    public class ChangeScheduleUpload
    {


        public string employee_code { get; set; }
        public int    shift_id      { get; set; }
        public string reason        { get; set; }
        public string date_from     { get; set; }
        public string date_to       { get; set; }
        public int    created_by    { get; set; }
    }

    
    public class LeaveUpload
    {


        public string employee_code { get; set; }
        public int    leave_type_id { get; set; }
        public string date_from     { get; set; }
        public string date_to       { get; set; }
        public bool   is_paid       { get; set; }
        public bool   is_half_day   { get; set; }
        public string description   { get; set; }
        public int    created_by    { get; set; }
    }
    
    
    public class LeaveBalanceUpload
    {


        public string employee_code { get; set; }
        public string year          { get; set; }
        public int    leave_type_id { get; set; }
        public int    balance_type  { get; set; }
        public decimal amount     { get; set; }
        public int    created_by    { get; set; }
    }
    
    public class OBUpload
    {


        public string employee_code     { get; set; }
        public string date_from         { get; set; }
        public string date_to           { get; set; }
        public string company_to_visit  { get; set; }
        public string location          { get; set; }
        public string description       { get; set; }
        public int    created_by        { get; set; }
    }
    
    public class OvertimeUpload
    {


        public string employee_code     { get; set; }
        public int    overtime_type_id  { get; set; }
        public string date_from         { get; set; }
        public string date_to           { get; set; }
        public bool   with_break        { get; set; }
        public string break_in          { get; set; }
        public string break_out         { get; set; }
        public string description       { get; set; }
        public int    created_by        { get; set; }
    }

    
    public class OffsetUpload
    {


        public string  employee_code     { get; set; }
        public string  date              { get; set; }
        public decimal offset_hour       { get; set; }
        public string  reason            { get; set; }
        public int     created_by        { get; set; }
    }
    
    
    public class TimekeepingUpload
    {


        public int     payroll_header_id { get; set; }
        public string  employee_code                                          { get; set; }
        public string  date_from                                               { get; set; }
        public string  date_to                                         {get; set; }
        public decimal overtime_hour                                           { get; set; }
        public decimal offset_hour                                             { get; set; }
        public decimal vl_hour                                      { get; set; }
        public decimal sl_hour                                      { get; set; }
        public decimal otherl_hour                                      { get; set; }
        public decimal lwop_hour                                      { get; set; }
        public decimal is_absent                                      { get; set; }
        public decimal is_present                                      { get; set; }
        public decimal late                                      { get; set; }
        public decimal undertime                                      { get; set; }
        public decimal reg                                      { get; set; }
        public decimal regnd                                      { get; set; }
        public decimal ot                                      { get; set; }
        public decimal ot_e8                                      { get; set; }
        public decimal otnd                                      { get; set; }
        public decimal otnd_e8                                      { get; set; }
        public decimal otrd                                      { get; set; }
        public decimal otrd_e8                                      { get; set; }
        public decimal otrdnd                                      { get; set; }
        public decimal otrdnd_e8                                      { get; set; }
        public decimal lh                                      { get; set; }
        public decimal lhot                                      { get; set; }
        public decimal lhot_e8                                      { get; set; }
        public decimal lhotnd                                      { get; set; }
        public decimal lhotnd_e8                                      { get; set; }
        public decimal lhrd                                      { get; set; }
        public decimal lhrdot                                      { get; set; }
        public decimal lhrdot_e8                                      { get; set; }
        public decimal lhrdotnd                                      { get; set; }
        public decimal lhrdotnd_e8                                      { get; set; }
        public decimal sh                                      { get; set; }
        public decimal shot                                      { get; set; }
        public decimal shot_e8                                      { get; set; }
        public decimal shotnd                                      { get; set; }
        public decimal shotnd_e8                                      { get; set; }
        public decimal shrd                                      { get; set; }
        public decimal shrdot                                      { get; set; }
        public decimal shrdot_e8                                      { get; set; }
        public decimal shrdotnd                                      { get; set; }
        public decimal shrdotnd_e8                                      { get; set; }
        public decimal dh                                      { get; set; }
        public decimal dhot                                      { get; set; }
        public decimal dhot_e8                                      { get; set; }
        public decimal dhotnd                                      { get; set; }
        public decimal dhotnd_e8                                      { get; set; }
        public decimal dhrd                                      { get; set; }
        public decimal dhrdot                                      { get; set; }
        public decimal dhrdot_e8                                      { get; set; }
        public decimal dhrdotnd                                      { get; set; }
        public decimal dhrdotnd_e8                                      { get; set; }
        public int     created_by                                        { get; set; }
        public string  date_created                                     { get; set; }
    }

    
    public class LoanUpload
    {


        public string  employee_code     { get; set; }
        public int     loan_type_id      { get; set; }
        public string  loan_name         { get; set; }
        public decimal total_amount      { get; set; }
        public string  loan_date         { get; set; }
        public string  loan_start        { get; set; }
        public int     terms             { get; set; }
        public int     loan_timing_id    { get; set; }
        public int     created_by        { get; set; }
    }

    
    public class ADUpload
    {


        public string  employee_code        { get; set; }
        public int     adjustment_type_id   { get; set; }
        public int     adjustment_id        { get; set; }
        public int     timing_id            { get; set; }
        public decimal amount               { get; set; }
        public bool    taxable              { get; set; }
        public decimal minumum_hour           { get; set; }
        public decimal maximum_hour           { get; set; }
        public int     created_by           { get; set; }
    }
}
