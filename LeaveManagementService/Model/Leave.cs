using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementService.Model
{
    public class LeaveTypeRequest
	{
    public string		leave_type_id		{ get; set; }
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
	public string		created_by			{ get; set; }
	public bool			active				{ get; set; }
	public string		series_code			{ get; set; }

	}

	 public class LeaveTypeResponse
	 {
		
		public int			leave_type_id		{ get; set; }
		public string		encrypted_leave_type_id { get; set; }
		public string		leave_type_code		{ get; set; }
		public string		leave_name			{ get; set; }
		public string		display_name			{ get; set; }
		public string		filed_by_name			{ get; set; }
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

	public class LeaveEntitlementRequest
    {
		public string leave_type_id { get; set; }
		public int employee_id { get; set; }
		public string created_by { get; set; }
		public string series_code { get; set; }
	}

	
    public class LeaveRequest
	{
		public string		leave_type_id		{ get; set; }
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
		public string		created_by			{ get; set; }
		public bool			active				{ get; set; }
		public string		series_code			{ get; set; }

	}

	public class EmployeeLeaveResponse
    {
		public int		employee_id				   {get;set;}
		public int		leave_type_id			   {get;set;}
		public string	leave_type_code			   {get;set;}
		public string	leave_name				   {get;set;}
		public string	description				   {get;set;}
		public int		gender_to_use			   {get;set;}
		public bool		required_attachment		   {get;set;}
		public int		filed_by				   {get;set;}
		public string	leave_start				   {get;set;}
		public decimal	total_leaves			   {get;set;}
		public decimal  leave_accrued			   {get;set;}
		public string	leave_accrued_description  {get;set;}
		public decimal  accrued_credits			   {get;set;}
		public decimal  leave_total				   {get;set;}
		public decimal  leave_used				   {get;set;}
		public decimal  leave_balance			   {get;set;}
		public decimal  leave_per_month			   {get;set;}
		public decimal  convertible_to_cash		   {get;set;}
		public decimal  taxable_credits			   {get;set;}
		public decimal  non_taxable_credits		   {get;set;}
		public int		priority_to_convert		   {get;set;}
		public int		created_by				   {get;set;}
		public string	date_created			   {get;set;}
		public bool		active					   {get;set;}
	}

}
