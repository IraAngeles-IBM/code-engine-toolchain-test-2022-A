using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimekeepingManagementService.Model
{
	public class PayrollCutoffResponse
	{
		public int	  payroll_cutoff_id			{get;set; }
		public string encypted_payroll_cutoff_id  {get;set; }
		public string payroll_setup		 { get; set; }
		public int	  payroll_setup_id	 { get; set; }
		public string cutoff { get; set; }
		public int	  cutoff_id			 {get;set; }
		public string date_start		 {get;set; }
		public string date_end			 {get;set; }
		public string date_from			 {get;set; }
		public string date_to			 {get;set; }
		public bool	   active			 {get;set; }
		public int	  created_by		 {get;set; }
		public string date_created		 {get;set; }
	}
	
	public class PayrollCutoffSelResponse
	{
		public int	  payroll_setup_id			{get;set; }
		public string encypted_payroll_setup_id { get;set; }
		public string payroll_setup				 { get; set; }
		public string cutoff					{ get; set; }
		public int	  cutoff_id					 {get;set; }
		public int	  payroll_cutoff_id					 {get;set; }
		public string date_start				 {get;set; }
		public string date_end					 {get;set; }
		public string pay_day					 {get;set; }
		public bool	   active					 {get;set; }
		public int	  created_by				 {get;set; }
		public string date_created				 {get;set; }
		public int    ds_month					{ get; set; }
		public int    de_month					{ get; set; }
		public int    pd_month					{ get; set; }
		public bool   lock_id					{ get; set; }
		public string locked						{ get; set; }
		public string approval_lock { get; set; }
		public bool approval_lock_id { get; set; }
	}

	public class PayrollCutoffRequest
	{
		public string payroll_cutoff_id { get; set; }
		public int date_start { get; set; }
		public int date_end { get; set; }
		public int pay_day { get; set; }
		public int ds_month { get; set; }
		public int de_month { get; set; }
		public int pd_month { get; set; }
		public bool lock_id { get; set; }
		public bool approval_lock { get; set; }
		public string created_by { get; set; }
		public string series_code { get; set; }
	}
}
