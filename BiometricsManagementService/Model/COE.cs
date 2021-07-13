using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilingManagementService.Model
{
    public class COERequest
    {
            public string coe_id        { get; set; }
            public string coe_code      { get; set; }
            public string reason        { get; set; }
            public int    purpose_id    { get; set; }
            public bool   with_pay        { get; set; }
            public string coe_path      { get; set; }
            public bool   active        { get; set; }
            public string approval_level_id      { get; set; }
            public string created_by    { get; set; }
            public string series_code   { get; set; }
        
    }

    
    public class COEIUResponse
    {
            public int     id                       { get; set; }
            public string  encrypt_id               { get; set; }
            public string  code                     { get; set; }
            public string  description              { get; set; }
            public string  company_name             { get; set; }
            public string  company_logo             { get; set; }
            public string  company_address          { get; set; }
            public string  company_address2         { get; set; }
            public string  company_address3         { get; set; }
            public string  email_address            { get; set; }
            public decimal monthly_rate             { get; set; }
            public decimal annual_compensation      { get; set; }
            public string  salutation               { get; set; }
            public string  first_name               { get; set; }
            public string  middle_name              { get; set; }
            public string  last_name                { get; set; }
            public string  suffix                   { get; set; }
            public string  position                 { get; set; }
            public string  philhealth               { get; set; }
            public bool    is_email                 { get; set; }
            public int     purpose_id               { get; set; }
            public string  purpose                  { get; set; }
            public string signatory_1               { get; set; }
            public string  signatory_1_path                  { get; set; }
            public string signatory_2              { get; set; }
            public string  signatory_2_path                  { get; set; }
            public string signatory_3               { get; set; }
            public string  signatory_3_path                  { get; set; }
        
    }


    
    public class COEResponse
    {
            public int    coe_id                { get; set; }
            public string encrypted_coe_id      { get; set; }
            public string coe_code              { get; set; }
            public string reason                { get; set; }
            public string purpose               { get; set; }
            public int    purpose_id            { get; set; }
            public string coe_path              { get; set; }
            public bool   active                { get; set; }
            public int    created_by              {get;set;}
            public string created_by_name         { get; set; }
            public string date_created            {get;set;}
            public string approved                {get;set;}
            public bool   approved_bit                {get;set;}
            public bool   with_pay                {get;set;}
            public string status                  {get;set;}
            public string  company_name             { get; set; }
            public string  company_logo             { get; set; }
            public string  company_address          { get; set; }
            public string  company_address2         { get; set; }
            public string  company_address3         { get; set; }
            public string  email_address            { get; set; }
            public decimal monthly_rate             { get; set; }
            public decimal annual_compensation      { get; set; }
            public string  salutation               { get; set; }
            public string  first_name               { get; set; }
            public string  middle_name              { get; set; }
            public string  last_name                { get; set; }
            public string  suffix                   { get; set; }
            public string  position                 { get; set; }
            public string  philhealth               { get; set; }
            public string  corporate_philhealth               { get; set; }
            public string signatory_1               { get; set; }
            public string  signatory_1_path                  { get; set; }
            public string  signatory_1_title                  { get; set; }
            public string  signatory_1_file_name                  { get; set; }
            public string signatory_2              { get; set; }
            public string  signatory_2_path                  { get; set; }
            public string  signatory_2_title                  { get; set; }
            public string  signatory_2_file_name                  { get; set; }
            public string signatory_3               { get; set; }
            public string  signatory_3_path                  { get; set; }
            public string  signatory_3_title                  { get; set; }
            public string  signatory_3_file_name                  { get; set; }
            public string  telephone                  { get; set; }
            public string  date_hired                  { get; set; }
        
    }
}
