using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollSetupManagementService.Model
{

    
    public class PayrollRatesDetail
    { 
        public int      rate_group_id   { get; set; }
        public int      payroll_rate_id { get; set; }
        public decimal  rates          { get; set; }
        public string   created_by      { get; set; }
        public string   series_code { get; set; }
    }

    public class PayrollRatesHeaderView
    {
        public string   rate_code       { get; set; }
        public int      rate_group_id   { get; set; }
        public int      created_by      { get; set; }
        public bool     active          { get; set; }
        public string   date_created    { get; set; }

    }

    public class PayrollRatesDetailView
    {

        public int      rate_group_id   { get; set; }
        public int      payroll_rate_id { get; set; }
        public decimal  rates           { get; set; }
    }
}
