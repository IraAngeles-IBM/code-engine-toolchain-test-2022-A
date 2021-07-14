using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayManagementService.Model
{
    public class HolidayHeader
    {
        public string holiday_id { get; set; }
        public string holiday_code { get; set; }
        public string holiday_header_name { get; set; }
        public string holiday_description { get; set; }
        public string created_by { get; set; }
        public string series_code { get; set; }
        public bool active { get; set; }


        public HolidayBranch[] Branch { get; set; }
        public HolidayDetail[] Detail { get; set; }
    }

    public class HolidayDetail
    {
        public int holiday_type_id { get; set; }
        public string holiday_name { get; set; }
        public string holiday_date { get; set; }
    }

    public class HolidayBranch
    {
        public int branch_id { get; set; }
    }

    public class HolidayBranchView    
    {
        public int branch_id { get; set; }
        public int holiday_id { get; set; }
    }

    public class HolidayDetailView
    {
        public int holiday_id { get; set; }
        public string holiday_name { get; set; }
        public string description { get; set; }
        public int holiday_type_id { get; set; }
        public string holiday_date { get; set; }
        public int created_by { get; set; }
        public string date_created { get; set; }
    }


    public class HolidayHeaderResponse
    {
        public int      holiday_id          { get; set; }
        public string   holiday_id_encrypted { get; set; }
        public string   holiday_code        { get; set; }
        public string   holiday_header_name { get; set; }
        public string   holiday_description { get; set; }
        public int      created_by          { get; set; }
        public string   created_by_name          { get; set; }
        public string		status          { get; set; }
        public string   date_created        { get; set; }
        public bool     active              { get; set; }
    }


    public class HolidayDetailResponse
    {
        public int holiday_type_id { get; set; }
        public string holiday_name { get; set; }
        public string holiday_date { get; set; }
    }

}
