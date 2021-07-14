using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BranchManagementService.Model
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

    public class BranchIURequest
    {


        public string branchID { get; set; }

        public string bankAccount { get; set; }

        public string barangay { get; set; }

        public string branchName { get; set; }

        public string building { get; set; }

        public int province { get; set; }

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

    public class BranchViewResponse
    {


        public int branch_id { get; set; }
        public string encrypted_branch_id { get; set; }
        public string branch_name { get; set; }
    }

        public class BranchResponse
    {


        public string branch_id { get; set; }
        public string encrypted_branch_id { get; set; }

        public string branch_code { get; set; }

        public string bankAccount { get; set; }

        public string barangay { get; set; }

        public string branchName { get; set; }

        public string building { get; set; }

        public int province { get; set; }

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

        public string instance_name { get; set; }
        public string username { get; set; }
        public string password { get; set; }


        public IPResponse[] IP_IU { get; set; }
        public ContactResponse[] Contact_IU { get; set; }
        public EmailResponse[] Email_IU { get; set; }

    }

    public class IPResponse
    {
        public string branch_id { get; set; }

        public string description { get; set; }

        public string createdBy { get; set; }
    }


    public class ContactResponse
    {
        public string branch_id { get; set; }

        public string id { get; set; }

        public string description { get; set; }

        public string number { get; set; }

        public string createdBy { get; set; }
    }


    public class EmailResponse
    {
        public string branch_id { get; set; }

        public int id { get; set; }

        public string description { get; set; }

        public string email_address { get; set; }

        public string createdBy { get; set; }
    }

    public class CompanyBranchOutput
    {
        public string description { get; set; }
        public int id { get; set; }
    }

}
