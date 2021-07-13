using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeCategoryManagementService.Model
{
    public class CategoryResponse
    {
        public int      category_id                     { get; set; }
        public string   category_code                   { get; set; }
        public string   encrypt_category_id             { get; set; }
        public string   category_name                   { get; set; }
        public string   category_description            { get; set; }
        public int      access_level_id                 { get; set; }
        public int      approval_level_id               { get; set; }
        public int      change_schedule_before          { get; set; }
        public int      change_schedule_after           { get; set; }
        public int      change_log_before               { get; set; }
        public int      change_log_after                { get; set; }
        public int      official_business_before        { get; set; }
        public int      official_business_after         { get; set; }
        public int      overtime_before                 { get; set; }
        public int      overtime_after                  { get; set; }
        public int      offset_before                   { get; set; }
        public int      offset_after                    { get; set; }
        public bool     allow_overtime                  { get; set; }
        public int      holiday_based_id                { get; set; }
        public bool     enable_tardiness                { get; set; }
        public bool     fixed_salary                    { get; set; }
        public int      basis_sss_deduction_id          { get; set; }
        public int      basis_philhealth_deduction_id   { get; set; }
        public int      basis_pagibig_deduction_id      { get; set; }
        public int      created_by                      { get; set; }
        public string   date_created                    { get; set; }
        public bool     active                          { get; set; }
        public int      rate_group_id      { get; set; }
        public int      contribution_group_id      { get; set; }
        public string   approval_level                  { get; set; }
        public string   access_level                  { get; set; }
        public string   holiday_based                   { get; set; }
        public string   basis_sss_deduction             { get; set; }
        public string   basis_philhealth_deduction      { get; set; }
        public string   basis_pagibig_deduction         { get; set; }
        public string   rate_group                      { get; set; }
        public string   contribution_group              { get; set; }
        public string   created_by_name                 { get; set; }
        public string   status                          { get; set; }


    }

    public class CategoryRequest
    {
        public string   category_id                  { get; set; }
        public string   category_code                  { get; set; }
        public string   category_name                   { get; set; }
        public string   category_description            { get; set; }
        public int      access_level_id                 { get; set; }
        public int      approval_level_id               { get; set; }
        public int      change_schedule_before          { get; set; }
        public int      change_schedule_after           { get; set; }
        public int      change_log_before               { get; set; }
        public int      change_log_after                { get; set; }
        public int      official_business_before        { get; set; }
        public int      official_business_after         { get; set; }
        public int      overtime_before                 { get; set; }
        public int      overtime_after                  { get; set; }
        public int      offset_before                   { get; set; }
        public int      offset_after                    { get; set; }
        public bool     allow_overtime                  { get; set; }
        public int      holiday_based_id                { get; set; }
        public bool     enable_tardiness                { get; set; }
        public bool     fixed_salary                    { get; set; }
        public int      basis_sss_deduction_id          { get; set; }
        public int      basis_philhealth_deduction_id   { get; set; }
        public int      basis_pagibig_deduction_id      { get; set; }
        public int      rate_group_id      { get; set; }
        public int      contribution_group_id      { get; set; }
        public string   created_by                      { get; set; }
        public string   date_created                    { get; set; }
        public bool     active                          { get; set; }
        public string   series_code                     { get; set; }
              
    }
}
