using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantMasterSetupService.Model
{
    public class PayrollRates
    {
        public int payroll_rates_id { get; set; }
        public string description { get; set; }
        public decimal rates { get; set; }
    }
}
