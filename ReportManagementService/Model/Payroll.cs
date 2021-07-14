using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollManagementService.Model
{
    public class PayrollHeaderRequest
    {
        public string payroll_header_code       { get; set; }
        public int    timekeeping_header_id     { get; set; }
        public string created_by                { get; set; }
        public string approval_level_id       { get; set; }
        public bool   active                    { get; set; }
        public string series_code               { get; set; }
    }

	public class PayrollHeaderInReponse
	{
		public int	  payroll_header_id { get; set; }
		public string encrypted_payroll_header_id { get; set; }
	}

    
    public class PayrollHeaderResponse
    {
        public int    payroll_header_id                   { get; set; }
        public string encrypted_payroll_header_id         { get; set; }
        public string payroll_header_code                 { get; set; }
        public string timekeeping_header_code                 { get; set; }
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

     public class PayrollGenerationResponse
    {
		
		public int		payroll_id						{ get; set; }
		public int		payslip_id						{ get; set; }
		public int		posted_payslip_id				{ get; set; }
		public string	payroll_code					{ get; set; }
		public string	posted_payslip_code				{ get; set; }
		public int		timekeeping_header_id			{ get; set; }
		public string	employee_code					{ get; set; }
		public string	last_name						{ get; set; }
		public string	first_name						{ get; set; }
		public string	file_status						{ get; set; }
		public string	tax_status						{ get; set; }
		public decimal	daily_rate						{ get; set; }
		public decimal	semi_monthly_rate				{ get; set; }
		public decimal  monthly_rate					{ get; set; }
		public decimal	hourly_rate						{ get; set; }
		public decimal	late						{ get; set; }
		public decimal	undertime						{ get; set; }
		public decimal	absent						{ get; set; }
		public decimal	sss						{ get; set; }
		public decimal	pagibig						{ get; set; }
		public decimal	philhealth						{ get; set; }
		public int		payroll_header_id				{ get; set; }
		public string	pay_date						{ get; set; }
		public int		employee_id						{ get; set; }
		public decimal	basic_salary					{ get; set; }
		public decimal	misc_amount						{ get; set; }
		public decimal	leave_amount					{ get; set; }
		public decimal	overtime						{ get; set; }
		public decimal	overtime_holiday				{ get; set; }
		public decimal	other_tax_income				{ get; set; }
		public decimal	adjustments						{ get; set; }
		public decimal	gross_income					{ get; set; }
		public decimal	witholding_tax					{ get; set; }
		public decimal	net_salary_after_tax			{ get; set; }
		public decimal	employee_sss					{ get; set; }
		public decimal	employee_mcr					{ get; set; }
		public decimal	employee_pagibig				{ get; set; }
		public decimal	other_ntax_income				{ get; set; }
		public decimal	loan_payments					{ get; set; }
		public decimal	deductions						{ get; set; }
		public decimal	net_salary						{ get; set; }
		public decimal	employer_sss					{ get; set; }
		public decimal	employer_mcr					{ get; set; }
		public decimal	employer_ec						{ get; set; }
		public decimal	employer_pagibig				{ get; set; }
		public decimal	payroll_cost					{ get; set; }
		public decimal	ytd_gross						{ get; set; }
		public decimal	ytd_witholding					{ get; set; }
		public decimal	ytd_sss							{ get; set; }
		public decimal	ytd_mcr							{ get; set; }
		public decimal	ytd_pagibig						{ get; set; }
		public decimal	ytd_13ntax						{ get; set; }
		public decimal	ytd_13tax						{ get; set; }
		public string	payment_type					{ get; set; }
		public string	bank_account					{ get; set; }
		public string	bank_name						{ get; set; }
		public string	comment_field					{ get; set; }
		public string	error_field						{ get; set; }
		public string	date_employed					{ get; set; }
		public string	date_terminated					{ get; set; }
		public string	cost_center						{ get; set; }
		public string	Currency						{ get; set; }
		public decimal	exchange_rate					{ get; set; }
		public string	payment_freq					{ get; set; }
		public decimal	mtd_gross						{ get; set; }
		public decimal	mtd_basic						{ get; set; }
		public decimal	mtd_sss_employee				{ get; set; }
		public decimal	mtd_mcr_employee				{ get; set; }
		public decimal	mtd_pagibig_employee			{ get; set; }
		public decimal	mtd_sss_employer				{ get; set; }
		public decimal	mtd_mcr_employer				{ get; set; }
		public decimal	mtd_ec_employer					{ get; set; }
		public decimal	mtd_pagibig_employer			{ get; set; }
		public decimal	mtd_wh_tax						{ get; set; }
		public decimal	monthly_basic					{ get; set; }
		public decimal	monthly_allow					{ get; set; }
		public decimal	mtd_ntax						{ get; set; }
		public string	date_from						{ get; set; }
		public string	date_to							{ get; set; }
		public decimal	total_addition					{ get; set; }
		public decimal	total_deduction					{ get; set; }
		public string	company_name							{ get; set; }
		public string	company_logo							{ get; set; }
		public string	pay_period							{ get; set; }
		public string	cutoff_date							{ get; set; }
		public string	department							{ get; set; }
		public string	position							{ get; set; }
		public string	payroll_type							{ get; set; }
		public string	salary_rate							{ get; set; }
		public bool		is_posted							{ get; set; }
		public bool		is_checked							{ get; set; }
		public decimal	regnd_ms					{ get; set; }

		public string	display_name							{ get; set; }
		public int		created_by						{ get; set; }
		public string	date_created					{ get; set; }
		public bool		active							{ get; set; }

    }

	public class PayrollRequest
    {
		public string	payroll_code			{ get; set; }
		public string	payroll_header_id		{ get; set; }
		public int		category_id				{ get; set; }
		public int		branch_id				{ get; set; }
		public int		confidential_id			{ get; set; }
		public bool		include_tax				{ get; set; }
		public bool		include_sss				{ get; set; }
		public bool		include_pagibig			{ get; set; }
		public bool		include_philhealth		{ get; set; }
        public string	created_by              { get; set; }
        public string	approval_level_id       { get; set; }
        public bool		active                  { get; set; }
        public string	series_code             { get; set; }
	}

	
	public class PayrollResponse
    {
		
		public int		payroll_id				{ get; set; }
		public string	encrypted_payroll_id	{ get; set; }
		public string	payroll_code			{ get; set; }
		public string	date_from				{ get; set; }
		public string	date_to					{ get; set; }
		public int		category_id				{ get; set; }
		public string	category_name			{ get; set; }
		public int		branch_id				{ get; set; }
		public string	branch					{ get; set; }
		public int		confidentiality_id		{ get; set; }
		public string	confidentiality			{ get; set; }
        public int		created_by              { get; set; }
        public string	display_name			{ get; set; }
        public bool		active                  { get; set; }
        public string	date_created			{ get; set; }
        public bool		approved				{ get; set; }
        public string	status					{ get; set; }
	}

	
    public class PayslipDetaiResponse
    {
        public int		payroll_header_id   { get; set; }
        public int		posted_payslip_id   { get; set; }
        public int		payroll_id   { get; set; }
        public int		payslip_id   { get; set; }
        public int		employee_id         { get; set; }
        public int		detail_group_id     { get; set; }
        public int		detail_type_id      { get; set; }
        public int		detail_id           { get; set; }
        public string	detail              { get; set; }
        public decimal	total               { get; set; }
        public decimal	amount              { get; set; }
		public bool		taxable_id			{ get; set; }
        public string	taxable             { get; set; }
        public int		created_by          { get; set; }
        public string	date_created        { get; set; }
	}

	public class PostedPayrollRequest
    {
		public string	payroll_header_id		{ get; set; }
		public int		payroll_id				{ get; set; }
		public int		payslip_id				{ get; set; }
		public int		employee_id				{ get; set; }
        public string	created_by				{ get; set; }
        public string	series_code             { get; set; }
	}
}
