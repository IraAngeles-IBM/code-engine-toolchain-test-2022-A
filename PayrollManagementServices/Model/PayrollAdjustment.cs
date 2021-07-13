using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollManagementService.Model
{
    public class PayrollAdjustmentRequest
    {
        public string	payroll_header_id { get; set; }
        public string	payroll_adjustment_id { get; set; }
	    public int		employee_id				{ get; set; }
	    public int		adjustment_type_id		{ get; set; }
	    public string	adjustment_name			{ get; set; }
		public decimal	amount					{ get; set; }
		public bool		taxable					{ get; set; }
		public string	created_by				{ get; set; }
		public bool		active					{ get; set; }
		public string	series_code				{ get; set; }

	}
	
    public class PayrollAdjustmentResponse
    {
        public int		payroll_adjustment_id			{ get; set; }
        public string	encrypted_payroll_adjustment_id	{ get; set; }
	    public int		employee_id						{ get; set; }
        public string	display_name					{ get; set; }
        public string	employee_code					{ get; set; }
	    public int		adjustment_type_id				{ get; set; }
	    public string	adjustment_type					{ get; set; }
	    public string	adjustment_name					{ get; set; }
		public decimal	amount							{ get; set; }
		public bool		taxable_id						{ get; set; }
	    public string	taxable							{ get; set; }
		public int		created_by						{ get; set; }
		public string	date_created					{ get; set; }
		public bool		active							{ get; set; }
		public string	status							{ get; set; }

	}
}
