using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantDefaultSetupService.Model
{
    public class ApprovalSequenceHeaderRequest
    {
        public int module_id { get; set; }
        public int approval_level_id { get; set; }
        public string created_by { get; set; }
        public string series_code { get; set; }

        public ApprovalSequenceStatusRequest[] StatusRequest { get; set; }
    }
    public class ApprovalSequenceStatusRequest
    {
        public string status { get; set; }
        public int index { get; set; }

        public ApprovalSequenceDetailRequest[] DetailRequest { get; set; }
    }
    public class ApprovalSequenceDetailRequest
    {
        public int approver_id { get; set; }
    }


    public class ApprovalSequenceResponse
    {
        public int module_id { get; set; }
        public int approval_level_id { get; set; }
        public string status { get; set; }
        public int seqn { get; set; }
        public int approver_id { get; set; }
        public int created_by { get; set; }
    }


    public class ApprovalEmailSequenceResponse
    {
        public bool is_email { get; set; }
        public string approver_name { get; set; }
        public string email_address { get; set; }
        public string date_created { get; set; }
        public string module_name { get; set; }
        public bool approved { get; set; }
    }


    public class ApprovalEmailNotificationResponse
    {
        public string approver_name { get; set; }
        public string email_address { get; set; }
        public string date_created { get; set; }
        public string module_name { get; set; }
        public string transaction_code { get; set; }
        public string header { get; set; }
    }
}
