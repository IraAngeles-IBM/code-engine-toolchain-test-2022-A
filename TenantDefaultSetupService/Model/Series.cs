using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantDefaultSetupService.Model
{

    public class SeriesRequest
    {
        public string module_id { get; set; }
        public string series_code { get; set; }
    }
    public class SeriesResponse
    {
        public string series_code { get; set; }
    }

    public class Series
    {
        public int      module_id   { get; set; }
        public string   module_name { get; set; }
        public int      series      { get; set; }
        public int      created_by  { get; set; }
        public string   prefix      { get; set; }
        public int      year        { get; set; }
        public int      length      { get; set; }
        public bool     active      { get; set; }
        public string   series_code { get; set; }
    }

    public class SeriesUp
    {
        public int module_id { get; set; }

        public int series { get; set; }

        public string created_by { get; set; }

        public string prefix { get; set; }

        public int year { get; set; }

        public int length { get; set; }

        public bool active { get; set; }

        public string series_code { get; set; }
    }

    public class SeriesTemp
    {
        public int module_id { get; set; }
        public string series_code { get; set; }
        public string created_by { get; set; }
    }



    public class CompanyBranchOutput
    {
        public string description { get; set; }
        public int id { get; set; }
    }

    public class BranchIURequest
    {


        public string branchID { get; set; }

        public string bankAccount { get; set; }

        public string barangay { get; set; }

        public string branchName { get; set; }

        public string building { get; set; }

        public string municipality { get; set; }

        public string pagibig { get; set; }

        public string philhealth { get; set; }

        public int SelectedBank { get; set; }

        public int SelectedBranchCountry { get; set; }

        public int SelectedCity { get; set; }

        public int SelectedIndustry { get; set; }

        public int SelectedPCity { get; set; }

        public int SelectedPCode { get; set; }

        public int SelectedPRegion { get; set; }

        public int SelectedRdoBranch { get; set; }

        public int SelectedRdoOffice { get; set; }

        public int SelectedRegion { get; set; }

        public string sss { get; set; }

        public string street { get; set; }

        public string tin { get; set; }

        public string unit { get; set; }

        public string zipCode { get; set; }

        public string company_id { get; set; }

        public string guid { get; set; }

        public string CreatedBy { get; set; }

        public bool active { get; set; }

        public string company_series_code { get; set; }
        public string branch_series_code { get; set; }


        public IPIU[] iP_IU { get; set; }
        public ContactIU[] Contact_IU { get; set; }
        public EmailIU[] Email_IU { get; set; }

    }

    public class IPIU
    {
        public string branch_id { get; set; }

        public string description { get; set; }

        public string createdBy { get; set; }
    }

    public class ContactIU
    {
        public string branch_id { get; set; }

        public int id { get; set; }

        public string number { get; set; }

        public string createdBy { get; set; }
    }

    public class EmailIU
    {
        public string branch_id { get; set; }

        public int id { get; set; }

        public string email_address { get; set; }

        public string createdBy { get; set; }
    }
}
