using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilingManagementService.Model
{
    public class InsertResponse
    {
        public int id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string error_message { get; set; }
    }


    public class ApprovalResponse
    {
        public int module_id { get; set; }
        public int transaction_id { get; set; }
        public string string_transaction_id { get; set; }
        public bool approved { get; set; }
    }

    public class TransactionStatusRequest
    {
        public int module_id { get; set; }
        public string transaction_id { get; set; }
        public int int_transaction_id { get; set; }
        public string series_code { get; set; }
        public string created_by { get; set; }
    }


    public class ApprovalEmailResponse
    {

        public bool   is_email      { get; set; }
        public string approver_name { get; set; }
        public string email_address { get; set; }
        public string date_created  { get; set; }
        public string module_name   { get; set; }
        public string code          { get; set; }
        public string code_date     { get; set; }
        public bool   approved      { get; set; }
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

    public class ApprovalSequenceResponse
    {
        public int      module_id           { get; set; }
        public int      approval_level_id   { get; set; }
        public string   status              { get; set; }
        public int      seqn                { get; set; }
        public int      approver_id         { get; set; }
        public int      created_by          { get; set; }
    }

    public class ApprovalSequenceRequest
    {
        public int      module_id       { get; set; }
        public int      transaction_id { get; set; }
        public string   status          { get; set; }
        public int      seqn            { get; set; }
        public int      approver_id     { get; set; }
        public string   approval_level_id     { get; set; }
        public string   series_code     { get; set; }
        public string   created_by     { get; set; }
        public string   transaction_code          { get; set; }
    }


    public class UploadInRequest
    {
        public string created_by { get; set; }
        public string series_code { get; set; }



    }


    public class CancelTransactionRequest
    {
        public int      module_id       { get; set; }
        public string   transaction_id  { get; set; }
        public string   remarks         { get; set; }
        public string   created_by      { get; set; }
        public string   series_code     { get; set; }



    }

    public class uploadResponse
    {

        public string transaction_id { get; set; }
        public int created_by { get; set; }
    }
}
