using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollSetupManagementService.Model
{
    public class RecurringRequest
    {
      public string     recurring_id                {get; set;}
      public string     recurring_code              {get; set;}
      public string     recurring_name              {get; set;}
      public decimal    amount                      {get; set;}
      public int        recurring_type              {get; set;}
      public decimal    minimum_hour                {get; set;}
      public decimal    maximum_hour                {get; set;}
      public bool       taxable                     {get; set;}
      public int        deduction_type_id           {get; set;}
      public int        government_type_id          {get; set;}
      public string     created_by                  {get; set;}
      public bool       active                      {get; set;}
      public string     series_code                 {get; set;}
        
    }


    public class RecurringResponse
    {
      public string     recurring_id                {get; set;}
      public int        int_recurring_id            {get; set;}
      public string     recurring_code              {get; set;}
      public string     recurring_name              {get; set;}
      public decimal    amount                      {get; set;}
      public int        recurring_type              {get; set;}
      public decimal    minimum_hour                {get; set;}
      public decimal    maximum_hour                {get; set;}
      public bool       taxable                     {get; set;}
      public int        deduction_type_id           {get; set;}
      public int        government_type_id          {get; set;}
      public int        created_by                  {get; set;}
      public bool       active                      {get; set;}
      public string     date_created                {get; set;}
    public string		created_by_name          { get; set; }
    public string		status          { get; set; }
        
    }

    
    public class PayrollRecurringRequest
    {
      public string     payroll_recurring_id    {get; set;}
      public int        employee_id             { get; set; }
      public decimal    amount                  { get; set; }
      public int        timing_id               { get; set; }
      public int        adjustment_type_id      { get; set; }
      public int        adjustment_id           { get; set; }
      public bool       taxable                 { get; set; }
      public decimal    minimum_hour            { get; set; }
      public decimal    maximum_hour            { get; set; }
      public string     created_by              {get; set;}
      public bool       active                  {get; set;}
      public string     series_code             {get; set;}
        
    }

    
    public class PayrollRecurringResponse
    {
      public int        payroll_recurring_id               {get; set;}
      public string     encrypted_payroll_recurring_id    {get; set;}
      public string     display_name            {get; set;}
      public string     employee_code           {get; set;}
      public int        employee_id             { get; set; }
      public decimal    amount                  { get; set; }
      public int        timing_id               { get; set; }
      public string     timing                  { get; set; }
      public int        adjustment_type_id      { get; set; }
      public string     adjustment_type         { get; set; }
      public int        adjustment_id           { get; set; }
      public string     recurring_name                  { get; set; }
      public bool       taxable_id              { get; set; }
      public string     taxable                 { get; set; }
      public decimal    minimum_hour            { get; set; }
      public decimal    maximum_hour            { get; set; }
      public int        created_by              {get; set;}
      public bool       active                  {get; set;}
      public string     date_created             {get; set;}
      public string     status                  {get; set;}
        
    }
    
    public class EmployeeMovementRequest
    {
        public int      employee_id               { get; set; }
        public int      movement_type             { get; set; }
        public int      is_dropdown               { get; set; }
        public int      id                        { get; set; }
        public string   description               { get; set; }
        public string   movement_description      { get; set; }
        public string   created_by                { get; set; }
        public string   series_code               { get; set; }
    }
}
