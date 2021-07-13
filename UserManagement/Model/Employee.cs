using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Model
{

    public class EmployeeInRequest
    {
        public string created_by { get; set; }
        public string series_code { get; set; }



    }

    public class EmployeeActiveResponse
    {

        public int employee_id { get; set; }
        public string display_name { get; set; }
    }
    public class EmployeeResponse
    {

        public int       employee_id { get; set; }
        public string     encrypt_employee_id { get; set; }
        public string display_name { get; set; }
        public string employee_code { get; set; }
        public string user_name { get; set; }
        public string user_hash { get; set; }
        public string decrypted_user_hash { get; set; }
        public int salutation_id { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public int suffix_id { get; set; }
        public string nick_name { get; set; }
        public int gender_id { get; set; }
        public int nationality_id { get; set; }
        public string birthday { get; set; }
        public string birth_place { get; set; }
        public int civil_status_id { get; set; }
        public string height { get; set; }
        public string weight { get; set; }
        public int blood_type_id { get; set; }
        public int religion_id { get; set; }
        public string mobile { get; set; }
        public string phone { get; set; }
        public string office { get; set; }
        public string email_address { get; set; }
        public string personal_email_address { get; set; }
        public string alternate_number { get; set; }
        public string present_address { get; set; }
        public string permanent_address { get; set; }
        public string image_path { get; set; }
        public string old_image_path { get; set; }
        public int created_by { get; set; }
        public string created_by_name { get; set; }
        public bool active { get; set; }
        public string date_created { get; set; }
        public string pagibig { get; set; }
        public string sss { get; set; }
        public string philhealth { get; set; }
        public string tin { get; set; }

        public string bio_id { get; set; }
        public int branch_id { get; set; }
        public int employee_status_id { get; set; }
        public int occupation_id { get; set; }
        public int supervisor_id { get; set; }
        public int department_id { get; set; }
        public string date_hired { get; set; }
        public string date_regularized { get; set; }
        public int cost_center_id { get; set; }
        public int category_id { get; set; }
        public int division_id { get; set; }
        public int payroll_type_id { get; set; }
        public decimal monthly_rate { get; set; }
        public decimal semi_monthly_rate { get; set; }
        public decimal factor_rate { get; set; }
        public decimal daily_rate { get; set; }
        public decimal hourly_rate { get; set; }
        public int bank_id { get; set; }
        public string bank_account { get; set; }
        public int confidentiality_id { get; set; }

        
        public string salutation        { get; set; }
        public string suffix            { get; set; }
        public string gender            { get; set; }
        public string nationality       { get; set; }
        public string civil_status      { get; set; }
        public string blood_type        { get; set; }
        public string religion          { get; set; }
        public string branch            { get; set; }
        public string employee_status   { get; set; }
        public string occupation        { get; set; }
        public string supervisor        { get; set; }
        public string department        { get; set; }
        public string cost_center       { get; set; }
        public string category          { get; set; }
        public string division          { get; set; }
        public string payroll_type      { get; set; }
        public string bank              { get; set; }
        public string confidentiality   { get; set; }

        
        public string   pre_unit_floor      { get; set; }
        public string   pre_building        { get; set; }
        public string   pre_street          { get; set; }
        public string   pre_barangay        { get; set; }
        public int      pre_province_id     { get; set; }
        public string   pre_province        { get; set; }
        public int      pre_city_id         { get; set; }
        public string   pre_city            { get; set; }
        public int      pre_region_id       { get; set; }
        public string   pre_region          { get; set; }
        public int      pre_country_id      { get; set; }
        public string   pre_country         { get; set; }
        public string   pre_zipcode         { get; set; }
        
        public string   per_unit_floor      { get; set; }
        public string   per_building        { get; set; }
        public string   per_street          { get; set; }
        public string   per_barangay        { get; set; }
        public int      per_province_id     { get; set; }
        public string   per_province        { get; set; }
        public int      per_city_id         { get; set; }
        public string   per_city            { get; set; }
        public int      per_region_id       { get; set; }
        public string   per_region          { get; set; }
        public int      per_country_id      { get; set; }
        public string   per_country         { get; set; }
        public string   per_zipcode         { get; set; }

    }

    public class EmployeeProfileResponse
    {

        
        public string display_name { get; set; }
        public string employee_code { get; set; }
        public string user_name { get; set; }
        public string user_hash { get; set; }
        public string decrypted_user_hash { get; set; }
        public string salutation { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string suffix { get; set; }
        public string nick_name { get; set; }
        public string gender { get; set; }
        public string nationality { get; set; }
        public string birthday { get; set; }
        public string birth_place { get; set; }
        public string civil_status { get; set; }
        public string height { get; set; }
        public string weight { get; set; }
        public string blood_type { get; set; }
        public string religion { get; set; }
        public string mobile { get; set; }
        public string phone { get; set; }
        public string office { get; set; }
        public string email_address { get; set; }
        public string personal_email_address { get; set; }
        public string alternate_number { get; set; }
        public string present_address { get; set; }
        public string permanent_address { get; set; }
        public string image_path { get; set; }
        public string old_image_path { get; set; }
        public string created_by { get; set; }
        public bool active { get; set; }
        public string date_created { get; set; }
        public string pagibig { get; set; }
        public string sss { get; set; }
        public string philhealth { get; set; }
        public string tin { get; set; }

        public string bio_id { get; set; }
        public string branch { get; set; }
        public string employee_status { get; set; }
        public string occupation { get; set; }
        public string supervisor { get; set; }
        public string department { get; set; }
        public string date_hired { get; set; }
        public string date_regularized { get; set; }
        public string cost_center { get; set; }
        public string category { get; set; }
        public string division { get; set; }
        public string payroll_type { get; set; }
        public decimal monthly_rate { get; set; }
        public decimal semi_monthly_rate { get; set; }
        public decimal factor_rate { get; set; }
        public decimal daily_rate { get; set; }
        public decimal hourly_rate { get; set; }
        public string bank { get; set; }
        public string bank_account { get; set; }
        public string confidentiality { get; set; }

    }
    public class EmployeeRequest
    {

        public int employee_id { get; set; }
        public string encrypt_employee_id { get; set; }
        public string display_name { get; set; }
        public string employee_code { get; set; }
        public string user_name { get; set; }
        public string user_hash { get; set; }
        public int salutation_id { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public int suffix_id { get; set; }
        public string nick_name { get; set; }
        public int gender_id { get; set; }
        public int nationality_id { get; set; }
        public string birthday { get; set; }
        public string birth_place { get; set; }
        public int civil_status_id { get; set; }
        public decimal height { get; set; }
        public decimal weight { get; set; }
        public int blood_type_id { get; set; }
        public int religion_id { get; set; }
        public string mobile { get; set; }
        public string phone { get; set; }
        public string office { get; set; }
        public string email_address { get; set; }
        public string personal_email_address { get; set; }
        public string alternate_number { get; set; }
        public string present_address { get; set; }
        public string permanent_address { get; set; }
        public string image_path { get; set; }
        public string old_image_path { get; set; }
        public string created_by { get; set; }
        public bool active { get; set; }
        public string series_code { get; set; }

        
        public string   pre_unit_floor      { get; set; }
        public string   pre_building        { get; set; }
        public string   pre_street          { get; set; }
        public string   pre_barangay        { get; set; }
        public int      pre_province_id     { get; set; }
        public int      pre_city_id         { get; set; }
        public int      pre_region_id       { get; set; }
        public int      pre_country_id      { get; set; }
        public string   pre_zipcode         { get; set; }
        
        public string   per_unit_floor      { get; set; }
        public string   per_building        { get; set; }
        public string   per_street          { get; set; }
        public string   per_barangay        { get; set; }
        public int      per_province_id     { get; set; }
        public int      per_city_id         { get; set; }
        public int      per_region_id       { get; set; }
        public int      per_country_id      { get; set; }
        public string   per_zipcode         { get; set; }


        public EmployeeInformationRequest EIRequest { get; set; }

    }
    public class EmployeeInformationRequest
    {

        public string bio_id { get; set; }
        public int branch_id { get; set; }
        public int employee_status_id { get; set; }
        public int occupation_id { get; set; }
        public int supervisor_id { get; set; }
        public int department_id { get; set; }
        public string date_hired { get; set; }
        public string date_regularized { get; set; }
        public int cost_center_id { get; set; }
        public int category_id { get; set; }
        public int division_id { get; set; }
        public int payroll_type_id { get; set; }
        public decimal monthly_rate { get; set; }
        public decimal semi_monthly_rate { get; set; }
        public decimal factor_rate { get; set; }
        public decimal daily_rate { get; set; }
        public decimal hourly_rate { get; set; }
        public int bank_id { get; set; }
        public string bank_account { get; set; }
        public int confidentiality_id { get; set; }
        public string sss { get; set; }
        public string pagibig { get; set; }
        public string philhealth { get; set; }
        public string tin { get; set; }

    }


    public class EmployeeInformationResponse
    {

        public string bio_id { get; set; }
        public int branch_id { get; set; }
        public int employee_status_id { get; set; }
        public int occupation_id { get; set; }
        public int supervisor_id { get; set; }
        public int department_id { get; set; }
        public string date_hired { get; set; }
        public string date_regularized { get; set; }
        public int cost_center_id { get; set; }
        public int category_id { get; set; }
        public int division_id { get; set; }
        public int payroll_type_id { get; set; }
        public decimal monthly_rate { get; set; }
        public decimal semi_monthly_rate { get; set; }
        public decimal factor_rate { get; set; }
        public decimal daily_rate { get; set; }
        public decimal hourly_rate { get; set; }
        public int bank_id { get; set; }
        public string bank_account { get; set; }
        public int confidentiality_id { get; set; }

    }

    public class EmployeeMovementRequest
    {
        public int      employee_id               { get; set; }
        public int      movement_type             { get; set; }
        public int      is_dropdown               { get; set; }
        public int      id                        { get; set; }
        public string   description               { get; set; }
        public string   movement_description      { get; set; }
        public string   created_by                { get; set; }
        public string   series_code               { get; set; }
    }

     public class EmployeeMovementResponse
    {
        public string   employee_code             { get; set; }
        public string   display_name              { get; set; }
        public int      movement_type             { get; set; }
        public string   movement_type_description              { get; set; }
        public string   description               { get; set; }
        public string   created_by                { get; set; }
        public string   date_created              { get; set; }
    }

    public class EmployeeScheduleResponse
    {
       public int       shift_id                      {get;set;}
       public string    encrypt_shift_id              {get;set;}
       public int       employee_id                   {get;set;}
       public string    employee_code                 {get;set;}
       public string    display_name                  {get;set;}
       public string    date_from                     {get;set;}
       public string    date_to                       {get;set;}
       public string    time_in                       {get;set;}
       public string    time_out                      {get;set;}
       public string   total_working_hours           {get;set; }
        public decimal total_working_hours_decimal { get; set; }

        public int    created_by                       {get;set;}

	public int			int_shift_id				{ get; set; }
	public string		shift_code					{ get; set; }
	public string		shift_name					{ get; set; }
	public int			grace_period				{ get; set; }
	public string		description					{ get; set; }
	public bool			is_flexi					{ get; set; }
	public int			shift_code_type				{ get; set; }
	public int			time_out_days_cover			{ get; set; }
	public bool			is_rd_mon					{ get; set; }
	public bool			is_rd_tue					{ get; set; }
	public bool			is_rd_wed					{ get; set; }
	public bool			is_rd_thu					{ get; set; }
	public bool			is_rd_fri					{ get; set; }
	public bool			is_rd_sat					{ get; set; }
	public bool			is_rd_sun					{ get; set; }	
	public string		half_day_in					{ get; set; }
	public int			half_day_in_days_cover		{ get; set; }
	public string		half_day_out				{ get; set; }
	public int			half_day_out_days_cover		{ get; set; }
	public string		night_dif_in				{ get; set; }
	public int			night_dif_in_days_cover		{ get; set; }
	public string		night_dif_out				{ get; set; }
	public int			night_dif_out_days_cover	{ get; set; }
	public string		first_break_in				{ get; set; }
	public int			first_break_in_days_cover	{ get; set; }
	public string		first_break_out				{ get; set; }
	public int			first_break_out_days_cover	{ get; set; }
	public string		second_break_in				{ get; set; }
	public int			second_break_in_days_cover	{ get; set; }
	public string		second_break_out			{ get; set; }
	public int			second_break_out_days_cover { get; set; }
	public string		third_break_in				{ get; set; }
	public int			third_break_in_days_cover	{ get; set; }
	public string		third_break_out				{ get; set; }
	public int			third_break_out_days_cover	{ get; set; }
    public string		created_by_name             { get; set; }
    public string		status                      { get; set; }
	public string		date_created				{ get; set; }
	public bool			active						{ get; set; }
	public string		series_code					{ get; set; }

    }

    
    public class EmployeeLeaveResponse
    {
       public int       leave_type_id                   {get;set;}
       public string    encrypt_leave_type_id           {get;set;}
       public int       employee_id                     {get;set;}
       public string    employee_code                   {get;set;}
       public string    display_name                    {get;set;}
       public string    leave_type_code                 {get;set;}
       public string    leave_name                      {get;set;}
       public string    total_leaves                    {get;set;}
       public int       created_by                       {get;set;}
    }

    
	 public class ShiftCodeResponse
	{
    public string		shift_id					{ get; set; }
	public int			int_shift_id				{ get; set; }
	public string		shift_code					{ get; set; }
	public string		shift_name					{ get; set; }
	public int			grace_period				{ get; set; }
	public string		description					{ get; set; }
	public bool			is_flexi					{ get; set; }
	public string		time_in						{ get; set; }
	public string		time_out					{ get; set; }
	public int			time_out_days_cover			{ get; set; }
	public decimal		total_working_hours			{ get; set; }
	public string		half_day_in					{ get; set; }
	public int			half_day_in_days_cover		{ get; set; }
	public string		half_day_out				{ get; set; }
	public int			half_day_out_days_cover		{ get; set; }
	public string		night_dif_in				{ get; set; }
	public int			night_dif_in_days_cover		{ get; set; }
	public string		night_dif_out				{ get; set; }
	public int			night_dif_out_days_cover	{ get; set; }
	public string		first_break_in				{ get; set; }
	public int			first_break_in_days_cover	{ get; set; }
	public string		first_break_out				{ get; set; }
	public int			first_break_out_days_cover	{ get; set; }
	public string		second_break_in				{ get; set; }
	public int			second_break_in_days_cover	{ get; set; }
	public string		second_break_out			{ get; set; }
	public int			second_break_out_days_cover { get; set; }
	public string		third_break_in				{ get; set; }
	public int			third_break_in_days_cover	{ get; set; }
	public string		third_break_out				{ get; set; }
	public int			third_break_out_days_cover	{ get; set; }
	public int			created_by					{ get; set; }
	public string		date_created				{ get; set; }
	public bool			active						{ get; set; }
	public string		series_code					{ get; set; }

	}

    public class EmployeeRecurringResponse
    {
        public int      employee_id             { get; set; }
        public string   employee_code           { get; set; }
        public string   display_name            { get; set; }
        public decimal  amount                  { get; set; }
        public int      timing_id               { get; set; }
        public int      adjustment_type_id      { get; set; }
        public int      adjustment_id           { get; set; }
        public string   timing                  { get; set; }
        public bool      taxable_id              { get; set; }
        public string   taxable                 { get; set; }
        public decimal  minimum_hour            { get; set; }
        public decimal  maximum_hour            { get; set; }


    }

    public class UserCredentialRequest
    {
        public string employee_id     { get; set; }
        public string user_name       { get; set; }
        public string user_hash       { get; set; }
        public string series_code       { get; set; }
        public string created_by       { get; set; }
    }
}