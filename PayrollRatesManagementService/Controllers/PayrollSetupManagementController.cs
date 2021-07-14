using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PayrollSetupManagementService.Helper;
using PayrollSetupManagementService.Model;
using PayrollSetupManagementService.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PayrollSetupManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollSetupManagementController : ControllerBase
    {

        private IPayrollSetupManagementServices _IPayrollSetupManagementServices;

        private EmailSender email;
        private Default_Url url;

        public PayrollSetupManagementController(IPayrollSetupManagementServices PayrollSetupManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _IPayrollSetupManagementServices = PayrollSetupManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }


        [HttpPost("payroll_rates_in")]
        public int payroll_rates_in(PayrollRatesDetail[] model)
        {
            var resp = _IPayrollSetupManagementServices.payroll_rates_in(model);

            return resp;
        }


        [HttpGet("payroll_rates_detail_view_sel")]
        public List<PayrollRatesDetailView> payroll_rates_detail_view_sel(string series_code, int rate_group_id)
        {
            var resp = _IPayrollSetupManagementServices.payroll_rates_detail_view_sel(series_code, rate_group_id);

            return resp;
        }

        [HttpPost("pagibig_table_in")]
        public int pagibig_table_in(PagibigRequest[] model)
        {
            var resp = _IPayrollSetupManagementServices.pagibig_table_in(model);

            return resp;
        }

        [HttpPost("philhealth_table_in")]
        public int philhealth_table_in(philhealthRequest[] model)
        {
            var resp = _IPayrollSetupManagementServices.philhealth_table_in(model);

            return resp;
        }

        [HttpPost("sss_table_in")]
        public int sss_table_in(sssRequest[] model)
        {
            var resp = _IPayrollSetupManagementServices.sss_table_in(model);

            return resp;
        }

        [HttpPost("tax_table_in")]
        public int tax_table_in(taxRequest[] model)
        {
            var resp = _IPayrollSetupManagementServices.tax_table_in(model);

            return resp;
        }

        [HttpPost("recurring_in_up")]
        public int recurring_in_up(RecurringRequest model)
        {
            var resp = 0;

            SeriesRequest req = new SeriesRequest();
            SeriesResponse res = new SeriesResponse();
            string responseInString = "";
            try
            {

                using (var wb = new WebClient())
                {

                    //string url = "https://tenantdefaultsetupservices.azurewebsites.net/api/TenantDefaultSetup/series_in";

                    string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";


                    //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                    req.module_id = "12";
                    req.series_code = model.series_code;

                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req);
                    responseInString = wb.UploadString(url, Stringdata);
                    //string HtmlResult = wb.UploadValues(url, data);

                    //var response = wb.UploadValues(url, "POST", data);
                    //responseInString = Encoding.UTF8.GetString(response);

                }
                res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                model.recurring_code = res.series_code;
                resp = _IPayrollSetupManagementServices.recurring_in_up(model);
            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp = 0;

            }

            return resp;
        }

        [HttpGet("sss_table_view_sel")]
        public List<sssResponse> sss_table_view_sel(string series_code, int contribution_group_id)
        {
            var resp = _IPayrollSetupManagementServices.sss_table_view_sel(series_code, contribution_group_id);

            return resp;
        }

        [HttpGet("tax_table_view_sel")]
        public List<taxResponse> tax_table_view_sel(string series_code, int contribution_group_id, int payroll_type_id)
        {
            var resp = _IPayrollSetupManagementServices.tax_table_view_sel(series_code, contribution_group_id,payroll_type_id);

            return resp;
        }

        [HttpGet("philhealth_table_view_sel")]
        public List<philhealthResponse> philhealth_table_view_sel(string series_code, int contribution_group_id)
        {
            var resp = _IPayrollSetupManagementServices.philhealth_table_view_sel(series_code, contribution_group_id);

            return resp;
        }

        [HttpGet("pagibig_table_view_sel")]
        public List<PagibigResponse> pagibig_table_view_sel(string series_code, int contribution_group_id)
        {
            var resp = _IPayrollSetupManagementServices.pagibig_table_view_sel(series_code, contribution_group_id);

            return resp;
        }


        [HttpGet("recurring_view_sel")]
        public List<RecurringResponse> recurring_view_sel(string series_code, string recurring_id,string created_by)
        {
            var resp = _IPayrollSetupManagementServices.recurring_view_sel(series_code, recurring_id, created_by);

            return resp;
        }



        [HttpGet("recurring_view")]
        public List<RecurringResponse> recurring_view(string series_code, string recurring_type, string created_by)
        {
            var resp = _IPayrollSetupManagementServices.recurring_view(series_code, recurring_type, created_by);

            return resp;
        }

        [HttpPost("payroll_recurring_in")]
        public int payroll_recurring_in(PayrollRecurringRequest[] model)
        {
            var response = 0;
            var movement_resp = 0;

            List<EmployeeMovementRequest> emp_resp = new List<EmployeeMovementRequest>();

            string responseInString = "";
            try
            {


                emp_resp = _IPayrollSetupManagementServices.payroll_recurring_in(model);

                if (emp_resp.Count != 0)
                {

                    List<EmployeeMovementRequest> req = new List<EmployeeMovementRequest>();
                   


                        using (var wb = new WebClient())
                        {

                        string url = "http://localhost:1008/api/UserManagement/employee_movement_in";
                        //string url = "http://localhost:10001/api/UserManagement/employee_movement_in";



                        wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(emp_resp);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                    response = JsonConvert.DeserializeObject<int>(responseInString);
                    }

              

            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                response = 0;
            }


            return response;



        }




        [HttpGet("payroll_recurring_view")]
        public List<PayrollRecurringResponse> payroll_recurring_view(string series_code, string employee_id, string created_by)
        {
            var resp = _IPayrollSetupManagementServices.payroll_recurring_view(series_code, employee_id, created_by);

            return resp;
        }

        #region "Loan"

        [HttpPost("loan_in_up")]
        public int loan_in_up(LoanHeaderRequest model)
        {
            int resp = 0;

            

                SeriesRequest req = new SeriesRequest();
                SeriesResponse res = new SeriesResponse();
             List<EmployeeMovementRequest> emp_movement = new List<EmployeeMovementRequest>();
                string responseInString = "";
                try
                {
                    if (model.loan_id == "0")
                    {


                        using (var wb = new WebClient())
                        {


                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "29";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.loan_code = res.series_code;


                    }




                emp_movement = _IPayrollSetupManagementServices.loan_in_up(model);
                resp = 1;

                responseInString = "";

                using (var wb = new WebClient())
                {

                    string url = "http://localhost:1008/api/UserManagement/employee_movement_in";
                    //string url = "http://localhost:10001/api/UserManagement/employee_movement_in";



                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(emp_movement);
                    responseInString = wb.UploadString(url, Stringdata);
                    //string HtmlResult = wb.UploadValues(url, data);

                    //var response = wb.UploadValues(url, "POST", data);
                    //responseInString = Encoding.UTF8.GetString(response);

                }
                resp = JsonConvert.DeserializeObject<int>(responseInString);


            }
                catch (Exception e)
                {
                    var message = "Error: " + e.Message;
                    resp = 0;

                }



            return resp;



        }



        [HttpGet("loan_view_sel")]
        public List<LoanResponse> loan_view_sel(string series_code, string loan_id, string created_by)
        {
            var resp = _IPayrollSetupManagementServices.loan_view_sel( series_code,  loan_id,  created_by);

            return resp;
        }



        [HttpGet("loan_load")]
        public List<LoanLoadResponse> loan_load(string series_code, decimal total_amount, string loan_start, int terms, int timing_id, string created_by)
        {
            var resp = _IPayrollSetupManagementServices.loan_load( series_code,  total_amount,  loan_start,  terms,  timing_id,  created_by);

            return resp;
        }



        [HttpGet("loan_detail_view")]
        public List<LoanDetailResponse> loan_detail_view(string series_code, string loan_id, string created_by)
        {
            var resp = _IPayrollSetupManagementServices.loan_detail_view(series_code, loan_id, created_by);

            return resp;
        }




        #endregion

        #region "Upload Saving"

        [HttpPost("loan_in")]
        public int loan_in(UploadInRequest model)
        {
            int resp = 0;
            try
            {

                SeriesTemp req = new SeriesTemp();
                string responseInString = "";
                using (var wb = new WebClient())
                {

                    string url = "http://localhost:1006/api/TenantDefaultSetup/series_temp_in";
                    //string url = "http://localhost:10006/api/TenantDefaultSetup/series_temp_in";

                    req.module_id = 29;
                    req.series_code = model.series_code;
                    req.created_by = model.created_by;

                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req);
                    responseInString = wb.UploadString(url, Stringdata);

                }
                var res = JsonConvert.DeserializeObject<int>(responseInString);


                resp = _IPayrollSetupManagementServices.loan_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on Payroll Setup Management Service:" + e.Message);
            }

            return resp;
        }


        [HttpPost("payroll_recurring_upload_in")]
        public int payroll_recurring_upload_in(UploadInRequest model)
        {
            int resp = 0;
            try
            {

                //SeriesTemp req = new SeriesTemp();
                //string responseInString = "";
                //using (var wb = new WebClient())
                //{

                //    string url = "http://localhost:1006/api/TenantDefaultSetup/series_temp_in";
                //    //string url = "http://localhost:10006/api/TenantDefaultSetup/series_temp_in";

                //    req.module_id = 27;
                //    req.series_code = model.series_code;
                //    req.created_by = model.created_by;

                //    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                //    string Stringdata = JsonConvert.SerializeObject(req);
                //    responseInString = wb.UploadString(url, Stringdata);

                //}
                //var res = JsonConvert.DeserializeObject<int>(responseInString);


                resp = _IPayrollSetupManagementServices.payroll_recurring_upload_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on Payroll Setup Management Service:" + e.Message);
            }

            return resp;
        }
        #endregion

        #region "Payroll Contribution"


        [HttpPost("payroll_contribution_in")]
        public int  payroll_contribution_in(PayrollContributionRequest[] model)
        {
            int resp = 0;

            SeriesRequest req = new SeriesRequest();
            SeriesResponse res = new SeriesResponse();
            List<EmployeeMovementRequest> emp_movement = new List<EmployeeMovementRequest>();
            string responseInString = "";
            try
            {



                emp_movement = _IPayrollSetupManagementServices.payroll_contribution_in(model);
                resp = 1;

                responseInString = "";

                using (var wb = new WebClient())
                {

                    //string url = "http://localhost:1008/api/UserManagement/employee_movement_in";
                    string url = "http://localhost:10001/api/UserManagement/employee_movement_in";



                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(emp_movement);
                    responseInString = wb.UploadString(url, Stringdata);
                    //string HtmlResult = wb.UploadValues(url, data);

                    //var response = wb.UploadValues(url, "POST", data);
                    //responseInString = Encoding.UTF8.GetString(response);

                }
                resp = JsonConvert.DeserializeObject<int>(responseInString);


            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp = 0;

            }



            return resp;



        }

        [HttpGet("payroll_contribution_view")]
        public List<PayrollContributionResponse> payroll_contribution_view(string series_code, string employee_id, string created_by)
        {
            var resp = _IPayrollSetupManagementServices.payroll_contribution_view( series_code,  employee_id,  created_by);

            return resp;
        }


        #endregion


        //#region "Payroll"


        //[HttpPost("payroll_header_in")]
        //public int payroll_header_in(PayrollHeaderRequest model)
        //{
          
        //        ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

        //        SeriesRequest req = new SeriesRequest();
        //        SeriesResponse res = new SeriesResponse();

        //        string responseInString = "";
        //        try
        //        {


        //                using (var wb = new WebClient())
        //                {


        //                    string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
        //                    //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

        //                    req.module_id = "34";
        //                    req.series_code = model.series_code;

        //                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
        //                    string Stringdata = JsonConvert.SerializeObject(req);
        //                    responseInString = wb.UploadString(url, Stringdata);
        //                    //string HtmlResult = wb.UploadValues(url, data);

        //                    //var response = wb.UploadValues(url, "POST", data);
        //                    //responseInString = Encoding.UTF8.GetString(response);

        //                }
        //                res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

        //                model.payroll_header_code = res.series_code;






        //            var resp = _IPayrollSetupManagementServices.payroll_header_in(model);

        //            if (resp.id != 0)
        //            {

        //                approval_req.module_id = 34;
        //                approval_req.transaction_id = resp.id;
        //                approval_req.series_code = model.series_code;
        //                approval_req.approval_level_id = model.approval_level_id;
        //                var approval = approval_process_in(approval_req);

        //            }



        //        }
        //        catch (Exception e)
        //        {
        //            var message = "Error: " + e.Message;
        //            resp.id = 0;
        //            resp.error_message = "Error: " + e.Message; ;

        //        }



        //    return resp;



        //}

        //[HttpGet("payroll_contribution_view")]
        //public List<PayrollContributionResponse> payroll_contribution_view(string series_code, string employee_id, string created_by)
        //{
        //    var resp = _IPayrollSetupManagementServices.payroll_contribution_view(series_code, employee_id, created_by);

        //    return resp;
        //}


        //#endregion

    }
}
