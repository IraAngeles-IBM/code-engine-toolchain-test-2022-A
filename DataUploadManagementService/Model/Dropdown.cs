using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataUploadManagementService.Model
{
    public class DropdownIURequest
    {

        public string series_code { get; set; }

        public int dropdown_id { get; set; }

        public int dropdown_type_id { get; set; }

        public string dropdown_description { get; set; }

        public string created_by { get; set; }

        public bool active { get; set; }

    }

    public class DropdownIUResponse
    {

        public int dropdown_id { get; set; }

        public int dropdown_type_id { get; set; }

        public string dropdown_description { get; set; }

        public Guid created_by { get; set; }

        public bool active { get; set; }
    }

    public class DropdownRequest
    {

        public string dropdown_type_id { get; set; }

        public string dropdown_type { get; set; }
    }

    public class DropdownResponse
    {
        public int id { get; set; }
        public string string_id { get; set; }

        public string description { get; set; }

        public string type_description { get; set; }

        public int type_id { get; set; }

        public bool active { get; set; }

    }

    public class DropdownTypeResponse
    {
        public string id { get; set; }

        public string description { get; set; }



    }

    public class BranchViewResponse
    {


        public int branch_id { get; set; }
        public string encrypted_branch_id { get; set; }

        public string branch_name { get; set; }
    }

    public class CategoryResponse
    {
        public int category_id { get; set; }
        public string category_code { get; set; }
        public string encrypt_category_id { get; set; }
        public string category_name { get; set; }
        public string category_description { get; set; }
        public int access_level_id { get; set; }
        public int approval_level_id { get; set; }
        public int change_schedule_before { get; set; }
        public int change_schedule_after { get; set; }
        public int change_log_before { get; set; }
        public int change_log_after { get; set; }
        public int official_business_before { get; set; }
        public int official_business_after { get; set; }
        public int overtime_before { get; set; }
        public int overtime_after { get; set; }
        public int offset_before { get; set; }
        public int offset_after { get; set; }
        public bool allow_overtime { get; set; }
        public int holiday_based_id { get; set; }
        public bool enable_tardiness { get; set; }
        public bool fixed_salary { get; set; }
        public int basis_sss_deduction_id { get; set; }
        public int basis_philhealth_deduction_id { get; set; }
        public int basis_pagibig_deduction_id { get; set; }
        public int created_by { get; set; }
        public string date_created { get; set; }
        public bool active { get; set; }

    }

    public class ShiftCodeResponse
    {
        public string shift_id { get; set; }
        public int int_shift_id { get; set; }
        public string shift_code { get; set; }
        public string shift_name { get; set; }
        public int grace_period { get; set; }
        public string description { get; set; }
        public bool is_flexi { get; set; }
        public string time_in { get; set; }
        public string time_out { get; set; }
        public int time_out_days_cover { get; set; }
        public decimal total_working_hours { get; set; }
        public bool is_rd_mon { get; set; }
        public bool is_rd_tue { get; set; }
        public bool is_rd_wed { get; set; }
        public bool is_rd_thu { get; set; }
        public bool is_rd_fri { get; set; }
        public bool is_rd_sat { get; set; }
        public bool is_rd_sun { get; set; }

        public string half_day_in { get; set; }
        public int half_day_in_days_cover { get; set; }
        public string half_day_out { get; set; }
        public int half_day_out_days_cover { get; set; }
        public string night_dif_in { get; set; }
        public int night_dif_in_days_cover { get; set; }
        public string night_dif_out { get; set; }
        public int night_dif_out_days_cover { get; set; }
        public string first_break_in { get; set; }
        public int first_break_in_days_cover { get; set; }
        public string first_break_out { get; set; }
        public int first_break_out_days_cover { get; set; }
        public string second_break_in { get; set; }
        public int second_break_in_days_cover { get; set; }
        public string second_break_out { get; set; }
        public int second_break_out_days_cover { get; set; }
        public string third_break_in { get; set; }
        public int third_break_in_days_cover { get; set; }
        public string third_break_out { get; set; }
        public int third_break_out_days_cover { get; set; }
        public int created_by { get; set; }
        public string date_created { get; set; }
        public bool active { get; set; }
        public string series_code { get; set; }

    }

    
	 public class LeaveTypeResponse
	 {
		
		public int			leave_type_id		{ get; set; }
		public string		encrypted_leave_type_id { get; set; }
		public string		leave_type_code		{ get; set; }
		public string		leave_name			{ get; set; }
		public string		description			{ get; set; }
		public int			gender_to_use		{ get; set; }
		public bool			required_attachment	{ get; set; }
		public int			filed_by			{ get; set; }
		public int			leave_start			{ get; set; }
		public decimal		total_leaves		{ get; set; }
		public int			leave_accrued		{ get; set; }
		public decimal		accrued_credits		{ get; set; }
		public decimal		leave_per_month		{ get; set; }
		public decimal		convertible_to_cash	{ get; set; }
		public decimal		taxable_credits		{ get; set; }
		public decimal		non_taxable_credits	{ get; set; }
		public int			priority_to_convert	{ get; set; }
		public int			leave_before		{ get; set; }
		public int			leave_after			{ get; set; }
		public int			created_by			{ get; set; }
		public string		date_created		{ get; set; }
		public bool			active				{ get; set; }
		public string		series_code			{ get; set; }

	 }
}
