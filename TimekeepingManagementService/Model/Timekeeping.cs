using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimekeepingManagementService.Model
{
    public class TimekeepingHeaderRequest
    {
        public string   timekeeping_header_id   { get; set; }
        public string   timekeeping_header_code { get; set; }
        public string   date_from               { get; set; }
        public string   date_to                 { get; set; }
        public int      payroll_type_id         { get; set; }
        public int      cutoff_id               { get; set; } 
	    public int      month_id                { get; set; }
        public int      category_id             { get; set; } 
	    public int      branch_id               { get; set; } 
        public int      confidentiality_id      { get; set; } 

        public bool     active                  { get; set; }
        public string   approval_level_id       { get; set; }
        public string   created_by              { get; set; }
        public string   series_code             { get; set; }


    }

    
    public class TimekeepingHeaderResponse
    {
        public int      timekeeping_header_id           { get; set; }
        public string   encrypt_timekeeping_header_id   { get; set; }
        public string   timekeeping_header_code { get; set; }
        public string   display_name			{ get; set; }
        public string   date_from               { get; set; }
        public string   date_to                 { get; set; }
        public int      payroll_type_id         { get; set; }
        public string   payroll_type            { get; set; }
        public int      cutoff_id               { get; set; } 
        public string   cutoff					{ get; set; }
	    public int      month_id                { get; set; }
        public string   month					{ get; set; }
        public int      category_id             { get; set; } 
        public string   category				{ get; set; }
	    public int      branch_id               { get; set; } 
        public string   branch					{ get; set; }
        public int      confidentiality_id      { get; set; } 
        public string   confidentiality			{ get; set; }
        public int      tk_count				{ get; set; } 
        public int      created_by                   {get;set;}
        public string   created_by_name			{ get; set; }
        public bool     active                       {get;set;}
        public string   date_created                 {get;set;}
        public bool     approved                       {get;set;}
        public string   status                       {get;set;}


    }

    public class TimekeepingGenerationResponse
	{
        public int		timekeeping_header_id		{ get; set; }
		public int		timekeeping_id				{ get; set; }
		public int		employee_id				    { get; set; }
		public string	employee_code			    { get; set; }
		public string	display_name				{ get; set; }
		public string   date					   { get; set; }
		public string   rest_day					{ get; set; }
		public string   actual_time_in			   { get; set; }
		public string   actual_time_out			   { get; set; }
		public string   time_in					   { get; set; }
		public string   time_out				   { get; set; }
		public string   official_business_in	   { get; set; }
		public string   official_business_out	   { get; set; }
		public decimal	overtime_hour			   { get; set; }
		public decimal	offset_hour				   { get; set; }
		public decimal	vl_hour					   { get; set; }
		public decimal	sl_hour					   { get; set; }
		public decimal	otherl_hour				   { get; set; }
		public decimal	lwop_hour				   { get; set; }
		public bool		is_rest_day				   { get; set; }
		public int		holiday_type			   { get; set; }
		public int		holiday_count			   { get; set; }
		public string   schedule_time_in		   { get; set; }
		public string   schedule_time_out		   { get; set; }
		public decimal	schedule_hour			   { get; set; }
		public decimal	schedule_break_hour		   { get; set; }
		public bool		is_absent				   { get; set; }
		public decimal	working_hour			   { get; set; }
		public decimal	break_hour				   { get; set; }
		public decimal	late					   { get; set; }
		public decimal	undertime				   { get; set; }
		public decimal	first_break_late		   { get; set; }
		public decimal	first_break_undertime	   { get; set; }
		public decimal	second_break_late		   { get; set; }
		public decimal	second_break_undertime	   { get; set; }
		public decimal	third_break_late		   { get; set; }
		public decimal	third_break_undertime	   { get; set; }
		public decimal	total_break_late		   { get; set; }
		public decimal	total_break_undertime	   { get; set; }
		public string   remarks					   { get; set; }
		public string   first_break_in			   { get; set; }
		public string   first_break_out			   { get; set; }
		public string   second_break_in			   { get; set; }
		public string   second_break_out		   { get; set; }
		public string   third_break_in			   { get; set; }
		public string   third_break_out			   { get; set; }
		public decimal	reg						   { get; set; }
		public decimal	regnd					   { get; set; }
		public decimal	ot						   { get; set; }
		public decimal	ot_e8					   { get; set; }
		public decimal	otnd					   { get; set; }
		public decimal	otnd_e8					   { get; set; }
		public decimal	otrd					   { get; set; }
		public decimal	otrd_e8					   { get; set; }
		public decimal	otrdnd					   { get; set; }
		public decimal	otrdnd_e8				   { get; set; }
		public decimal	lh						   { get; set; }
		public decimal	lhot					   { get; set; }
		public decimal	lhot_e8					   { get; set; }
		public decimal	lhotnd					   { get; set; }
		public decimal	lhotnd_e8				   { get; set; }
		public decimal	lhrd					   { get; set; }
		public decimal	lhrdot					   { get; set; }
		public decimal	lhrdot_e8				   { get; set; }
		public decimal	lhrdotnd				   { get; set; }
		public decimal	lhrdotnd_e8				   { get; set; }
		public decimal	sh						   { get; set; }
		public decimal	shot					   { get; set; }
		public decimal	shot_e8					   { get; set; }
		public decimal	shotnd					   { get; set; }
		public decimal	shotnd_e8				   { get; set; }
		public decimal	shrd					   { get; set; }
		public decimal	shrdot					   { get; set; }
		public decimal	shrdot_e8				   { get; set; }
		public decimal	shrdotnd				   { get; set; }
		public decimal	shrdotnd_e8				   { get; set; }
		public decimal	dh						   { get; set; }
		public decimal	dhot					   { get; set; }
		public decimal	dhot_e8					   { get; set; }
		public decimal	dhotnd					   { get; set; }
		public decimal	dhotnd_e8				   { get; set; }
		public decimal	dhrd					   { get; set; }
		public decimal	dhrdot					   { get; set; }
		public decimal	dhrdot_e8				   { get; set; }
		public decimal	dhrdotnd				   { get; set; }
		public decimal	dhrdotnd_e8				   { get; set; }
	}



	public class TimekeepingRequest
	{
		public string timekeeping_header_id { get; set; }
		public string timekeeping_header_code { get; set; }
		public bool active { get; set; }
		public string created_by { get; set; }
		public string series_code { get; set; }


	}

	public class TimekeepingResponse
	{
		public int    timekeeping_id { get; set; }
		public string encrypt_timekeeping_id { get; set; }
		public string timekeeping_code { get; set; }
		public string display_name { get; set; }
		public string date_from { get; set; }
		public string date_to { get; set; }
		public int created_by { get; set; }
		public bool active { get; set; }
		public string date_created { get; set; }


	}

}
