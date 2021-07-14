using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TimekeepingManagementService.Helper;
using TimekeepingManagementService.Model;
using TimekeepingManagementService.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TimekeepingManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimekeepingManagementController : ControllerBase
    {

        private ITimekeepingManagementServices _TimekeepingManagementServices;

        private EmailSender email;
        private Default_Url url;

        public TimekeepingManagementController(ITimekeepingManagementServices TimekeepingManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _TimekeepingManagementServices = TimekeepingManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }


        [HttpPost("approval_process_in")]
        public ApprovalResponse approval_process_in(ApprovalSequenceRequest model)
        {

            List<ApprovalSequenceResponse> ap_resp = new List<ApprovalSequenceResponse>();
            ApprovalResponse resp = new ApprovalResponse();
            TransactionStatusRequest status_req = new TransactionStatusRequest();
            List<ApprovalSequenceRequest> req = new List<ApprovalSequenceRequest>();
            string url2 = "http://localhost:1006/api/TenantDefaultSetup/transaction_approval_sequence_view?module_id=" + model.module_id +"&employee_id=" + model.created_by + "&approval_level_id=" + model.approval_level_id + "&series_code=" + model.series_code;
            //string url2 = "http://localhost:10006/api/TenantDefaultSetup/transaction_approval_sequence_view?module_id=" + model.module_id +"&approval_level_id=" + model.approval_level_id + "&series_code=" + model.series_code;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url2);
            request.Method = "GET";
            String returnString = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                returnString = reader.ReadToEnd();
                ap_resp = JsonConvert.DeserializeObject<List<ApprovalSequenceResponse>>(returnString);
                reader.Close();
                dataStream.Close();
            }

            foreach (var item in ap_resp)
            {


                model.status = item.status;
                model.seqn = item.seqn;
                model.approver_id = item.approver_id;


            }

            req = (from item in ap_resp
                   select new ApprovalSequenceRequest()
                   {
                       module_id = model.module_id,
                       transaction_id = model.transaction_id,
                       status = item.status,
                       seqn = item.seqn,
                       approver_id = item.approver_id,
                       approval_level_id = model.approval_level_id,
                       series_code = model.series_code,
                       created_by = model.created_by

                   }).ToList();


            resp = _TimekeepingManagementServices.approval_process_in(req.ToArray());

            status_req.module_id = model.module_id;
            status_req.transaction_id = model.transaction_id;
            status_req.series_code = model.series_code;

            var status_update = _TimekeepingManagementServices.transaction_status_up(status_req);
            resp.approved = status_update.approved;
            return resp;
        }

        #region "Timekeeping"
        [HttpPost("timekeeping_header_in_up")]
        public InsertResponse timekeeping_header_in_up(TimekeepingHeaderRequest model)
        {
            var resp = new InsertResponse();


                ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

                SeriesRequest req = new SeriesRequest();
                SeriesResponse res = new SeriesResponse();

                string responseInString = "";
                try
                {
                    if (model.timekeeping_header_id == "0")
                    {


                        using (var wb = new WebClient())
                        {


                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "46";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.timekeeping_header_code = res.series_code;


                    }




                    resp = _TimekeepingManagementServices.timekeeping_header_in_up(model);

                    if (resp.id != 0)
                    {

                        approval_req.module_id = 46;
                        approval_req.transaction_id = resp.id;
                        approval_req.series_code = model.series_code;
                        approval_req.approval_level_id = model.approval_level_id;
                        approval_req.created_by = model.created_by;
                        var approval = approval_process_in(approval_req);



                    }



                }
                catch (Exception e)
                {
                    var message = "Error: " + e.Message;
                    resp.id = 0;
                    resp.error_message = "Error: " + e.Message; ;

                }


            return resp;



        }

        [HttpGet("timekeeping_header_view_sel")]
        public List<TimekeepingHeaderResponse> timekeeping_header_view_sel(string series_code, string timekeeping_header_id, string created_by)
        {

            var resp = _TimekeepingManagementServices.timekeeping_header_view_sel(series_code, timekeeping_header_id, created_by);

            return resp;
        }


        [HttpGet("timekeeping_header_view")]
        public List<TimekeepingHeaderResponse> timekeeping_header_view(string series_code, string timekeeping_header_id, string created_by)
        {

            var resp = _TimekeepingManagementServices.timekeeping_header_view(series_code, timekeeping_header_id, created_by);

            return resp;
        }


        [HttpGet("timekeeping_generation_view")]
        public List<TimekeepingGenerationResponse> timekeeping_generation_view(string series_code, string timekeeping_header_id, string created_by)
        {

            var resp = _TimekeepingManagementServices.timekeeping_generation_view(series_code, timekeeping_header_id, created_by);

            return resp;
        }



        [HttpGet("timekeeping_generation_employee")]
        public List<TimekeepingGenerationResponse> timekeeping_generation_employee(string series_code, string employee_id, string date_from, string date_to, string created_by)
        {

            var resp = _TimekeepingManagementServices.timekeeping_generation_employee( series_code,  employee_id,  date_from,  date_to,  created_by);

            return resp;
        }


        [HttpGet("payroll_cutoff_view")]
        public List<PayrollCutoffResponse> payroll_cutoff_view(string series_code,int cutoff_id,int month_id, string created_by)
        {

            var resp = _TimekeepingManagementServices.payroll_cutoff_view(series_code, cutoff_id,month_id, created_by);

            return resp;
        }



        [HttpPost("timekeeping_in_up")]
        public InsertResponse timekeeping_in_up(TimekeepingRequest model)
        {
            var resp = new InsertResponse();


            try
            {


                resp = _TimekeepingManagementServices.timekeeping_in_up(model);

            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp.id = 0;
                resp.error_message = "Error: " + e.Message; ;

            }


            return resp;



        }


        [HttpGet("timekeeping_view_sel")]
        public List<TimekeepingResponse> timekeeping_view_sel(string series_code, string timekeeping_header_id, string timekeeping_id, string created_by)
        {

            var resp = _TimekeepingManagementServices.timekeeping_view_sel(series_code, timekeeping_header_id,timekeeping_id, created_by);

            return resp;
        }



        [HttpGet("timekeeping_detail_view_sel")]
        public List<TimekeepingGenerationResponse> timekeeping_detail_view_sel(string series_code, string timekeeping_header_id, string timekeeping_id, string created_by)
        {

            var resp = _TimekeepingManagementServices.timekeeping_detail_view_sel(series_code, timekeeping_header_id, timekeeping_id, created_by);

            return resp;
        }

        [HttpGet("timekeeping_final_view_sel")]
        public List<TimekeepingGenerationResponse> timekeeping_final_view_sel(string series_code, string timekeeping_header_id, string employee_id, string date_from, string date_to, string created_by)
        {

            var resp = _TimekeepingManagementServices.timekeeping_final_view_sel(series_code, timekeeping_header_id, employee_id, date_from, date_to, created_by);

            return resp;
        }



        #endregion

        #region PayrollCutoff

        [HttpPost("payroll_cutoff_up")]
        public InsertResponse payroll_cutoff_up(PayrollCutoffRequest[] model)
        {
            var resp = new InsertResponse();


            try
            {

                resp = _TimekeepingManagementServices.payroll_cutoff_up(model);

              


            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp.id = 0;
                resp.error_message = "Error: " + e.Message; ;

            }


            return resp;



        }


        [HttpGet("payroll_cutoff_sel")]
        public List<PayrollCutoffSelResponse> payroll_cutoff_sel(string series_code, int payroll_cutoff_id, string created_by)
        {
            var resp = new List<PayrollCutoffSelResponse>();

            resp = _TimekeepingManagementServices.payroll_cutoff_sel( series_code,  payroll_cutoff_id,  created_by);


            return resp;



        }

        #endregion
    }
}
