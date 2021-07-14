using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilingManagementService.Model
{
    public class OTRequest
    {
        public string   overtime_id             {get;set;}
        public string   overtime_code           {get;set;}
        public int      overtime_type_id        {get;set;}
        public string   date_from               {get;set;}
        public string   date_to                 {get;set;}
        public bool     with_break              {get;set;}
        public string   break_in                {get;set;}
        public string   break_out               {get;set;}
        public string   description             {get;set;}
        public string   approval_level_id       {get;set;}
        public bool     active                  {get;set;}
        public string   series_code             {get;set;}
        public string   category_id             {get;set;}
        public string   created_by              {get;set;}
    }
    public class OTResponse
    {
        public int   overtime_id             {get;set;}
        public string   encrypted_overtime_id   { get; set; }
        public string   overtime_code           {get;set;}
        public string   overtime_type           {get;set;}
        public int      overtime_type_id        {get;set;}
        public string   date_from               {get;set;}
        public string   date_to                 {get;set;}
        public string   render_date_from        {get;set;}
        public string   render_date_to          {get;set;}
        public bool     with_break              {get;set;}
        public string   break_in                {get;set;}
        public string   break_out               {get;set;}
        public string   description             {get;set;}
        public int      created_by              {get;set;}
        public string   created_by_name         { get; set; }
        public bool     active                  {get;set;}
        public string   date_created            {get;set;}
        public bool     approved                 {get;set;}
        public string   status                  {get;set;}
    }
    
    public class OTRenderResponse
    {
        public int         overtime_id      {get;set;}
        public int         employee_id      {get;set;}
        public string      employee_code    {get;set;}
        public string      display_name     {get;set;}
        public string      overtime_code    {get;set;}
        public string      schedule_in      {get;set;}
        public string      schedule_out     {get;set;}
        public string      time_in          {get;set;}
        public string      time_out         {get;set;}
        public string      file_ot_in       {get;set;}
        public string      file_ot_out      {get;set;}
        public string      final_in         {get;set;}
        public string      final_out        {get;set;}
        public bool        is_edit          {get;set;}

    }


    public class OTRenderRequest
    {
        public int overtime_id { get; set; }
        public string final_in { get; set; }
        public string final_out { get; set; }
        public string series_code { get; set; }
        public string created_by { get; set; }

    }
}
