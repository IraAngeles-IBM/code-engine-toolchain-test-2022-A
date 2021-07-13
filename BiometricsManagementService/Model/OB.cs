using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilingManagementService.Model
{
    public class OBRequest
    {
        public string   official_business_id    {get;set;}
        public string   official_business_code  {get;set;}
        public string   date_from               {get;set;}
        public string   date_to                 {get;set;}
        public string   company_to_visit        {get;set;}
        public string   location                {get;set;}
        public string   description             {get;set;}
        public string   attachment              {get;set;}
        public string   created_by              {get;set;}
        public string   approval_level_id       {get;set;}
        public bool     active                  {get;set;}
        public string   series_code             {get;set;}
        public string   category_id             {get;set;}
    }
    public class OBResponse
    {
        public int      official_business_id         {get;set; }
        public string   encrypt_official_business_id { get; set; }
        public string   official_business_code       {get;set;}
        public string   date_from                    {get;set;}
        public string   date_to                      {get;set;}
        public string   company_to_visit             {get;set;}
        public string   location                     {get;set;}
        public string   description                  {get;set;}
        public int      created_by                   {get;set;}
        public string   created_by_name         { get; set; }
        public bool     active                       {get;set;}
        public string   date_created                 {get;set;}
        public bool     approved                       {get;set;}
        public string     status                       {get;set;}
    }
}
