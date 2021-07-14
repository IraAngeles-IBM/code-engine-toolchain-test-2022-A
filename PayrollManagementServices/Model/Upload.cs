using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollManagementService.Model
{
    
    public class TimekeepingUpload
    {


        public int     payroll_header_id { get; set; }
        public string  employee_code                                          { get; set; }
        public string  date_from                                               { get; set; }
        public string  date_to                                         {get; set; }
        public decimal overtime_hour                                           { get; set; }
        public decimal offset_hour                                             { get; set; }
        public decimal vl_hour                                      { get; set; }
        public decimal sl_hour                                      { get; set; }
        public decimal otherl_hour                                      { get; set; }
        public decimal lwop_hour                                      { get; set; }
        public decimal is_absent                                      { get; set; }
        public decimal is_present                                      { get; set; }
        public decimal late                                      { get; set; }
        public decimal undertime                                      { get; set; }
        public decimal reg                                      { get; set; }
        public decimal regnd                                      { get; set; }
        public decimal ot                                      { get; set; }
        public decimal ot_e8                                      { get; set; }
        public decimal otnd                                      { get; set; }
        public decimal otnd_e8                                      { get; set; }
        public decimal otrd                                      { get; set; }
        public decimal otrd_e8                                      { get; set; }
        public decimal otrdnd                                      { get; set; }
        public decimal otrdnd_e8                                      { get; set; }
        public decimal lh                                      { get; set; }
        public decimal lhot                                      { get; set; }
        public decimal lhot_e8                                      { get; set; }
        public decimal lhotnd                                      { get; set; }
        public decimal lhotnd_e8                                      { get; set; }
        public decimal lhrd                                      { get; set; }
        public decimal lhrdot                                      { get; set; }
        public decimal lhrdot_e8                                      { get; set; }
        public decimal lhrdotnd                                      { get; set; }
        public decimal lhrdotnd_e8                                      { get; set; }
        public decimal sh                                      { get; set; }
        public decimal shot                                      { get; set; }
        public decimal shot_e8                                      { get; set; }
        public decimal shotnd                                      { get; set; }
        public decimal shotnd_e8                                      { get; set; }
        public decimal shrd                                      { get; set; }
        public decimal shrdot                                      { get; set; }
        public decimal shrdot_e8                                      { get; set; }
        public decimal shrdotnd                                      { get; set; }
        public decimal shrdotnd_e8                                      { get; set; }
        public decimal dh                                      { get; set; }
        public decimal dhot                                      { get; set; }
        public decimal dhot_e8                                      { get; set; }
        public decimal dhotnd                                      { get; set; }
        public decimal dhotnd_e8                                      { get; set; }
        public decimal dhrd                                      { get; set; }
        public decimal dhrdot                                      { get; set; }
        public decimal dhrdot_e8                                      { get; set; }
        public decimal dhrdotnd                                      { get; set; }
        public decimal dhrdotnd_e8                                      { get; set; }
        public int     created_by                                        { get; set; }
        public string  date_created                                     { get; set; }
    }

    public class uploadTKRequest
    {
        public string series_code              { get; set; }
        public string created_by               { get; set; }
        public string payroll_header_id        { get; set; }
        public string date_from                { get; set; }
        public string date_to                  { get; set; }
        public int    category_id                 { get; set; }
        public int    branch_id                   { get; set; }
        public int    confidential_id             { get; set; }
        public bool    include_tax                { get; set; }
        public bool    include_sss                { get; set; }
        public bool    include_pagibig            { get; set; }
        public bool    include_philhealth         { get; set; }
    }
}
