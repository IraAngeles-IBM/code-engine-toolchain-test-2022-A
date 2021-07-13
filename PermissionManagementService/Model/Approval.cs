using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionManagementService.Model
{
    public class ApprovalHeaderResponse
    {
        public int seqn { get; set; }

        public string  columns { get; set; }

    }
    public class ApprovalRequest
    {
        public int module_id { get; set; }
        public string transaction_id { get; set; }
        public int action { get; set; }

        public string remarks { get; set; }

        public string approved_by { get; set; }
        public string series_code { get; set; }
    }
}
