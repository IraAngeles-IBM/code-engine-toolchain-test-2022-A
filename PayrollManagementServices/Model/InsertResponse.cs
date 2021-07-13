using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollManagementService.Model
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
        public int transaction_id { get; set; }
        public string series_code { get; set; }
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
        public string created_by { get; set; }
    }


    public class UploadInRequest
    {
        public string created_by { get; set; }
        public string series_code { get; set; }



    }
}
