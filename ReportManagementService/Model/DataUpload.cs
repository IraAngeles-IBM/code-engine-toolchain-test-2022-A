using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportManagementService.Model
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
        public int    laeve_type_id { get; set; }
        public string date_from     { get; set; }
        public string date_to       { get; set; }
        public bool   is_paid       { get; set; }
        public bool   is_half_day   { get; set; }
        public string description   { get; set; }
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

    
    public class OffseteUpload
    {


        public string  employee_code     { get; set; }
        public string  date              { get; set; }
        public decimal offset_hour       { get; set; }
        public string  reason            { get; set; }
        public int     created_by        { get; set; }
    }
}
