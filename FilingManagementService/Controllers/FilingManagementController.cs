using FilingManagementService.Helper;
using FilingManagementService.Model;
using FilingManagementService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FilingManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilingManagementController : ControllerBase
    {

        private IFilingManagementServices _FilingManagementServices;

        private EmailSender email;
        private Default_Url url;

        public FilingManagementController(IFilingManagementServices FilingManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _FilingManagementServices = FilingManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }


        #region "Approval and Status"
        [HttpPost("approval_process_in")]
        public ApprovalResponse  approval_process_in(ApprovalSequenceRequest model)
        {

            List<ApprovalSequenceResponse> ap_resp = new List<ApprovalSequenceResponse>();
            ApprovalResponse resp = new ApprovalResponse();
            TransactionStatusRequest status_req = new TransactionStatusRequest();
            List<ApprovalSequenceRequest> req = new List<ApprovalSequenceRequest>();
            string url2 = "http://localhost:1006/api/TenantDefaultSetup/transaction_approval_sequence_view?module_id=" + model.module_id + "&employee_id=" + model.created_by  + "&approval_level_id=" + model.approval_level_id + "&series_code=" + model.series_code;
            //string url2 = "http://localhost:10006/api/TenantDefaultSetup/transaction_approval_sequence_view?module_id=" + model.module_id + "&employee_id=" + model.created_by  + "&approval_level_id=" + model.approval_level_id + "&series_code=" + model.series_code;

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

            foreach(var item in ap_resp)
            {


            model.status = item.status;
            model.seqn = item.seqn;
            model.approver_id = item.approver_id;


            }

            req = (from  item in ap_resp
                   select new ApprovalSequenceRequest()
                   {
                       module_id = model.module_id,
                       transaction_id = model.transaction_id,
                       status = item.status,
                       seqn = item.seqn,
                       approver_id = item.approver_id,
                       approval_level_id =   model.approval_level_id,
                       series_code = model.series_code,
                       created_by = model.created_by

                   }).ToList();


            resp = _FilingManagementServices.approval_process_in(req.ToArray());

            status_req.module_id = model.module_id;
            status_req.int_transaction_id = model.transaction_id;
            status_req.series_code = model.series_code;
            status_req.created_by = model.created_by;

            var status_update = _FilingManagementServices.transaction_status_up(status_req);
            resp.approved = status_update.approved;


            string url = "http://localhost:1006/api/TenantDefaultSetup/approval_sequence_email_view?module_id=" + model.module_id + "&employee_id=" + model.created_by + "&approval_level_id=" + model.approval_level_id + "&series_code=" + model.series_code + "&transaction_code=" + model.transaction_code;
            //string url = "http://localhost:10006/api/TenantDefaultSetup/approval_sequence_email_view?module_id=" + model.module_id + "&employee_id=" + model.created_by  + "&approval_level_id=" + model.approval_level_id + "&series_code=" + model.series_code + "&transaction_code=" + model.transaction_code;

            int ret = 0;
            request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            returnString = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                returnString = reader.ReadToEnd();
                ret = JsonConvert.DeserializeObject<int>(returnString);
                reader.Close();
                dataStream.Close();
            }



            return resp;
        }



        [HttpPost("transaction_approval_up")]
        public ApprovalResponse transaction_approval_up(ApprovalRequest model)
        {
            
            string responseInString = "";
            TransactionStatusRequest approval_status_req = new TransactionStatusRequest();
            var resp = _FilingManagementServices.transaction_approval_up(model);

            approval_status_req.created_by = model.approved_by;
            approval_status_req.transaction_id = (model.transaction_id);
            approval_status_req.created_by = model.approved_by;
            approval_status_req.series_code = model.series_code;
            approval_status_req.module_id = model.module_id;

            var status_update = _FilingManagementServices.transaction_status_approver_up(approval_status_req);
            var email_resp = transaction_approval_email(model.series_code, model.module_id, model.transaction_id, model.approved_by);

            if (resp.approved == true)
            {
                
                if(resp.module_id == 32)
                {
                    using (var wb = new WebClient())
                    {

                        AttendanceLogApprovalRequest req = new AttendanceLogApprovalRequest();
                        string url = "http://localhost:1014/api/AttendanceManagement/employee_schedule_detail_auto";
                        //string url = "http://localhost:48532/api/AttendanceManagement/employee_schedule_detail_auto";

                        req.created_by = model.approved_by;
                        req.series_code = model.series_code;
                        req.transaction_id = model.transaction_id;

                        wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                        string Stringdata = JsonConvert.SerializeObject(req);
                        responseInString = wb.UploadString(url, Stringdata);


                        //string HtmlResult = wb.UploadValues(url, data);

                        //var response = wb.UploadValues(url, "POST", data);
                        //responseInString = Encoding.UTF8.GetString(response);

                    }
                    var id = JsonConvert.DeserializeObject<int>(responseInString);
                }
                else if (resp.module_id == 33)
                {
                    using (var wb = new WebClient())
                    {

                        AttendanceLogApprovalRequest req = new AttendanceLogApprovalRequest();
                       string url = "http://localhost:1014/api/AttendanceManagement/attendance_log_approval_in";
                       //string url = "http://localhost:48532/api/AttendanceManagement/attendance_log_approval_in";

                        req.created_by = model.approved_by;
                        req.series_code = model.series_code;
                        req.transaction_id = model.transaction_id;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                        

                        //string HtmlResult = wb.UploadValues(url, data);

                        //var response = wb.UploadValues(url, "POST", data);
                        //responseInString = Encoding.UTF8.GetString(response);

                    }
                    var id = JsonConvert.DeserializeObject<int>(responseInString);
                }

            }

            return resp;
        }


        [HttpGet("approval_sequence_email_view")]
        public int  transaction_approval_email(string series_code, int module_id, string transaction_id, string created_by)
        {
            int resp = 0;
            try
            {

                var result = _FilingManagementServices.transaction_approval_email( series_code,  module_id,  transaction_id,  created_by);

                foreach (var item in result)
                {
                    if (item.is_email == true)
                    {


                        var sendGridClient = new SendGridClient(email.ApiKey);

                        ApprovalEmailNotificationResponse notif_param = new ApprovalEmailNotificationResponse();

                        var sendGridMessage = new SendGridMessage();
                        if (item.approved == false)
                        {

                            notif_param.approver_name = item.approver_name;
                            notif_param.module_name = item.module_name;
                            notif_param.transaction_code = item.code;
                            notif_param.date_created = item.code_date;
                            notif_param.email_address = item.email_address;
                            notif_param.header = "Pending for Approval!";

                            sendGridMessage.SetFrom(email.email_username, "Aanya HR Notification");
                            sendGridMessage.AddTo(item.email_address, item.approver_name);
                            sendGridMessage.SetTemplateId("d-a2ec8d1052654fc7b87541013bcfe4cb");
                            sendGridMessage.SetTemplateData(notif_param);
                        }
                        else
                        {

                            notif_param.approver_name = item.approver_name;
                            notif_param.module_name = item.module_name;
                            notif_param.transaction_code = item.code;
                            notif_param.date_created = item.code_date;
                            notif_param.email_address = item.email_address;
                            notif_param.header = item.module_name + " has been Approved!";


                            sendGridMessage.SetFrom(email.email_username, "Aanya HR Notification");
                            sendGridMessage.AddTo(item.email_address, item.approver_name);
                            sendGridMessage.SetTemplateId("d-baf0576148cf4005adc067d4e19319c3");
                            sendGridMessage.SetTemplateData(notif_param);

                        }


                        var response = sendGridClient.SendEmailAsync(sendGridMessage).Result;

                    }
                }

                resp = 1;
            }
            catch (Exception e)
            {
                resp = 0;
            }
            return resp;
        }



        #endregion

        #region "Official Business"
        [HttpPost("official_business_in_up")]
        public InsertResponse official_business_in_up(OBRequest model)
        {
            var resp = new InsertResponse();

            string url2 = "http://localhost:1013/api/EmployeeCategoryManagement/employee_category_restriction?series_code=" + model.series_code + "&transaction_id=" + model.official_business_id  + "&module_id=35&date_from=" + model.date_from + "&date_to=" + model.date_to + "&category_id=" + model.category_id + "&created_by=" + model.created_by;
            //string url2 = "http://localhost:51113/api/EmployeeCategoryManagement/employee_category_restriction?series_code=" + model.series_code + "&transaction_id=" + model.official_business_id  + "&module_id=35&date_from=" + model.date_from + "&date_to=" + model.date_to + "&category_id=" + model.category_id + "&created_by=" + model.created_by;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url2);
            request.Method = "GET";
            String returnString = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                returnString = reader.ReadToEnd();
                resp = JsonConvert.DeserializeObject<InsertResponse>(returnString);
                reader.Close();
                dataStream.Close();
            }

            if(resp.id == 0)
            {
                ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

                SeriesRequest req = new SeriesRequest();
                SeriesResponse res = new SeriesResponse();

                string responseInString = "";
                try
                {
                    if (model.official_business_id == "0")
                    {


                        using (var wb = new WebClient())
                        {


                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "35";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.official_business_code = res.series_code;


                    }




                    resp = _FilingManagementServices.official_business_in_up(model);

                    if (resp.id != 0)
                    {

                        approval_req.module_id = 35;
                        approval_req.transaction_id = resp.id;
                        approval_req.series_code = model.series_code;
                        approval_req.approval_level_id = model.approval_level_id;
                        approval_req.created_by = model.created_by;
                        approval_req.transaction_code = model.official_business_code;
                        var approval = approval_process_in(approval_req);

                    }



                }
                catch (Exception e)
                {
                    var message = "Error: " + e.Message;
                    resp.id = 0;
                    resp.error_message  = "Error: " + e.Message; ;

                }


            }
            else
            {
                resp.id = 0;
            }
            return resp;



        }


        [HttpGet("official_business_view_sel")]
        public List<OBResponse> official_business_view_sel(string series_code, string official_business_id, int file_type, string created_by)
        {

            var resp = _FilingManagementServices.official_business_view_sel(series_code, official_business_id,  file_type, created_by);

            return resp;
        }

        #endregion



        #region "Overtime"
        [HttpPost("overtime_in_up")]
        public InsertResponse overtime_in_up(OTRequest model)
        {
            var resp = new InsertResponse();

            string url2 = "http://localhost:1013/api/EmployeeCategoryManagement/employee_category_restriction?series_code=" + model.series_code + "&transaction_id=" + model.overtime_id + "&module_id=36&date_from=" + model.date_from + "&date_to=" + model.date_to + "&category_id=" + model.category_id + "&created_by=" + model.created_by;
            //string url2 = "http://localhost:51113/api/EmployeeCategoryManagement/employee_category_restriction?series_code=" + model.series_code + "&transaction_id=" + model.overtime_id + "&module_id=36&date_from=" + model.date_from + "&date_to=" + model.date_to + "&category_id=" + model.category_id + "&created_by=" + model.created_by;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url2);
            request.Method = "GET";
            String returnString = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                returnString = reader.ReadToEnd();
                resp = JsonConvert.DeserializeObject<InsertResponse>(returnString);
                reader.Close();
                dataStream.Close();
            }

            if (resp.id == 0)
            {
                ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

                SeriesRequest req = new SeriesRequest();
                SeriesResponse res = new SeriesResponse();

                string responseInString = "";
                try
                {
                    if (model.overtime_id == "0")
                    {


                        using (var wb = new WebClient())
                        {


                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "36";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.overtime_code = res.series_code;


                    }




                    resp = _FilingManagementServices.overtime_in_up(model);

                    if (resp.id != 0)
                    {

                        approval_req.module_id = 36;
                        approval_req.transaction_id = resp.id;
                        approval_req.series_code = model.series_code;
                        approval_req.approval_level_id = model.approval_level_id;
                        approval_req.created_by = model.created_by;
                        approval_req.transaction_code = model.overtime_code;
                        var approval = approval_process_in(approval_req);

                    }



                }
                catch (Exception e)
                {
                    var message = "Error: " + e.Message;
                    resp.id = 0;
                    resp.error_message = "Error: " + e.Message; ;

                }


            }
            else
            {
                resp.id = 0;
            }

            return resp;



        }


        [HttpGet("overtime_view_sel")]
        public List<OTResponse> overtime_view_sel(string series_code, string overtime_id, int file_type, string created_by)
        {

            var resp = _FilingManagementServices.overtime_view_sel(series_code, overtime_id,  file_type, created_by);

            return resp;
        }


        [HttpPost("overtime_render_up")]
        public InsertResponse overtime_render_up(OTRenderRequest[] model)
        {

            var resp = _FilingManagementServices.overtime_render_up(model);

            return resp;
        }



        [HttpGet("overtime_render_view")]
        public List<OTRenderResponse> overtime_render_view(string series_code, string employee_id, string date_from, string date_to, string created_by)
        {

            var resp = _FilingManagementServices.overtime_render_view( series_code,  employee_id,  date_from,  date_to,  created_by);

            return resp;
        }


        #endregion


        #region "Leave"
        [HttpPost("leave_in_up")]
        public InsertResponse leave_in_up(LeaveRequest model)
        {
            var resp = new InsertResponse();

            string url2 = "http://localhost:1012/api/LeaveManagement/leave_restriction?series_code=" + model.series_code + "&leave_id=" + model.leave_id + "&employee_id="+ model.employee_id + "&leave_type_id=" + model.leave_type_id + "&with_pay=" + model.is_paid + "&is_half_day=" + model.is_half_day + "&with_attachment=" + model.with_attachment + "&date_from=" + model.date_from + "&date_to=" + model.date_to + "&created_by=" + model.created_by;
            //string url2 = "http://localhost:63893/api/LeaveManagement/leave_restriction?series_code=" + model.series_code + "&employee_id="+ model.employee_id + "&leave_type_id=" + model.leave_type_id + "&with_pay=" + model.is_paid + "&is_half_day=" + model.is_half_day + "&with_attachment=" + model.with_attachment + "&date_from=" + model.date_from + "&date_to=" + model.date_to + "&created_by=" + model.created_by;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url2);
            request.Method = "GET";
            String returnString = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                returnString = reader.ReadToEnd();
                resp = JsonConvert.DeserializeObject<InsertResponse>(returnString);
                reader.Close();
                dataStream.Close();
            }

            if (resp.id == 0)
            {
                ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

                SeriesRequest req = new SeriesRequest();
                SeriesResponse res = new SeriesResponse();

                string responseInString = "";
                try
                {
                    if (model.leave_id == "0")
                    {


                        using (var wb = new WebClient())
                        {


                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "34";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.leave_code = res.series_code;


                    }




                    resp = _FilingManagementServices.leave_in_up(model);

                    if (resp.id != 0)
                    {

                        approval_req.module_id = 34;
                        approval_req.transaction_id = resp.id;
                        approval_req.series_code = model.series_code;
                        approval_req.approval_level_id = model.approval_level_id;
                        approval_req.created_by = model.created_by;
                        approval_req.transaction_code = model.leave_code;
                        var approval = approval_process_in(approval_req);

                    }



                }
                catch (Exception e)
                {
                    var message = "Error: " + e.Message;
                    resp.id = 0;
                    resp.error_message = "Error: " + e.Message; ;

                }


            }
            else
            {
                resp.id = 0;
            }

            return resp;



        }


        [HttpGet("leave_view_sel")]
        public List<LeaveResponse> leave_view_sel(string series_code, string leave_id, int file_type, string created_by)
        {

            var resp = _FilingManagementServices.leave_view_sel(series_code, leave_id,  file_type, created_by);

            return resp;
        }

        #endregion


        #region "Offset"
        [HttpPost("Offset_in_up")]
        public InsertResponse Offset_in_up(OffsetHeaderRequest model)
        {
            var resp = new InsertResponse();

            string url2 = "http://localhost:1013/api/EmployeeCategoryManagement/employee_category_restriction?series_code=" + model.series_code + "&transaction_id=" + model.offset_id + "&module_id=37&date_from=" + model.date + "&date_to=" + model.date + "&category_id=" + model.category_id + "&created_by=" + model.created_by;
            //string url2 = "http://localhost:51113/api/EmployeeCategoryManagement/employee_category_restriction?series_code=" + model.series_code + "&transaction_id=" + model.offset_id + "&module_id=37&date_from=" + model.date + "&date_to=" + model.date + "&category_id=" + model.category_id + "&created_by=" + model.created_by;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url2);
            request.Method = "GET";
            String returnString = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                returnString = reader.ReadToEnd();
                resp = JsonConvert.DeserializeObject<InsertResponse>(returnString);
                reader.Close();
                dataStream.Close();
            }

            if (resp.id == 0)
            {
                ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

                SeriesRequest req = new SeriesRequest();
                SeriesResponse res = new SeriesResponse();

                string responseInString = "";
                try
                {
                    if (model.offset_id == "0")
                    {


                        using (var wb = new WebClient())
                        {


                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "37";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.offset_code = res.series_code;


                    }




                    resp = _FilingManagementServices.offset_in_up(model);

                    if (resp.id != 0)
                    {

                        approval_req.module_id = 37;
                        approval_req.transaction_id = resp.id;
                        approval_req.series_code = model.series_code;
                        approval_req.approval_level_id = model.approval_level_id;
                        approval_req.created_by = model.created_by;
                        approval_req.transaction_code = model.offset_code;
                        var approval = approval_process_in(approval_req);

                    }



                }
                catch (Exception e)
                {
                    var message = "Error: " + e.Message;
                    resp.id = 0;
                    resp.error_message = "Error: " + e.Message; ;

                }


            }
            else
            {
                resp.id = 0;
            }

            return resp;



        }


        [HttpGet("offset_view_sel")]
        public List<OffsetHeaderResponse> offset_view_sel(string series_code, string offset_id, int file_type, string created_by)
        {

            var resp = _FilingManagementServices.offset_view_sel(series_code, offset_id,  file_type, created_by);

            return resp;
        }



        [HttpGet("offset_detail_view")]
        public List<OffsetDetailResponse> offset_detail_view(string series_code, string offset_id, int file_type, string employee_id, string created_by)
        {

            var resp = _FilingManagementServices.offset_detail_view(series_code, offset_id,  file_type, employee_id, created_by);

            return resp;
        }

        #endregion


        #region "Change Log"
        [HttpPost("change_log_in_up")]
        public InsertResponse change_log_in_up(ChangelogHeaderRequest model)
        {
            var resp = new InsertResponse();
            var time_in = "";
            var time_out = "";
            foreach (var item in model.Detail)
            {
                time_in = item.time_in;
                time_out = item.time_out;

            }
           string url2 = "http://localhost:1013/api/EmployeeCategoryManagement/employee_category_restriction?series_code=" + model.series_code + "&transaction_id=" + model.change_log_id + "&module_id=33&date_from=" + time_in + "&date_to=" + time_out + "&category_id=" + model.category_id + "&created_by=" + model.created_by;
            //string url2 = "http://localhost:51113/api/EmployeeCategoryManagement/employee_category_restriction?series_code=" + model.series_code + "&transaction_id=" + model.change_log_id + "&module_id=33&date_from=" + model.date_from + "&date_to=" + model.date_to + "&category_id=" + model.category_id + "&created_by=" + model.created_by;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url2);
            request.Method = "GET";
            String returnString = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                returnString = reader.ReadToEnd();
                resp = JsonConvert.DeserializeObject<InsertResponse>(returnString);
                reader.Close();
                dataStream.Close();
            }

            if (resp.id == 0)
            {
                ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

                SeriesRequest req = new SeriesRequest();
                SeriesResponse res = new SeriesResponse();

                string responseInString = "";
                try
                {
                    if (model.change_log_id == "0")
                    {


                        using (var wb = new WebClient())
                        {


                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "33";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.change_log_code = res.series_code;


                    }




                    resp = _FilingManagementServices.change_log_in_up(model);

                    if (resp.id != 0)
                    {

                        approval_req.module_id = 33;
                        approval_req.transaction_id = resp.id;
                        approval_req.series_code = model.series_code;
                        approval_req.approval_level_id = model.approval_level_id;
                        approval_req.created_by = model.created_by;
                        approval_req.transaction_code = model.change_log_code;
                        var approval = approval_process_in(approval_req);

                        if (approval.approved == true)
                        {
                            using (var wb = new WebClient())
                            {

                                AttendanceCLRequest aclr = new AttendanceCLRequest();
                                string url = "http://localhost:1014/api/AttendanceManagement/attendance_log_cl_in";
                                //string url = "http://localhost:48532/api/AttendanceManagement/attendance_log_cl_in";

                                aclr.employee_id = model.created_by;
                                aclr.created_by = model.created_by;
                                aclr.series_code = model.series_code;


                                foreach (var item in model.Detail)
                                {
                                    aclr.date = item.date;
                                    aclr.time_in = item.time_in;
                                    aclr.time_out = item.time_out;


                                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                                    string Stringdata = JsonConvert.SerializeObject(aclr);
                                    responseInString = wb.UploadString(url, Stringdata);
                                }

                                //string HtmlResult = wb.UploadValues(url, data);

                                //var response = wb.UploadValues(url, "POST", data);
                                //responseInString = Encoding.UTF8.GetString(response);

                            }
                            var id = JsonConvert.DeserializeObject<int>(responseInString);
                        }
                    }



                }
                catch (Exception e)
                {
                    var message = "Error: " + e.Message;
                    resp.id = 0;
                    resp.error_message = "Error: " + e.Message; ;

                }


            }
            else
            {
                resp.id = 0;
            }

            return resp;



        }


        [HttpGet("change_log_view_sel")]
        public List<ChangelogHeaderResponse> change_log_view_sel(string series_code, string change_log_id, int file_type, string created_by)
        {

            var resp = _FilingManagementServices.change_log_view_sel(series_code, change_log_id,  file_type, created_by);

            return resp;
        }



        [HttpGet("change_log_detail_view_sel")]
        public List<ChangelogDetailResponse> change_log_detail_view_sel(string series_code, string change_log_id, int file_type, string created_by)
        {

            var resp = _FilingManagementServices.change_log_detail_view_sel(series_code, change_log_id,  file_type, created_by);

            return resp;
        }

        #endregion


        #region "Change Schedule"
        [HttpPost("change_schedule_in_up")]
        public InsertResponse change_schedule_in_up(ChangeScheduleRequest model)
        {
            var resp = new InsertResponse();

            string url2 = "http://localhost:1013/api/EmployeeCategoryManagement/employee_category_restriction?series_code=" + model.series_code + "&transaction_id=" + model.change_schedule_id + "&module_id=32&date_from=" + model.date_from + "&date_to=" + model.date_to + "&category_id=" + model.category_id + "&created_by=" + model.created_by;
            //string url2 = "http://localhost:51113/api/EmployeeCategoryManagement/employee_category_restriction?series_code=" + model.series_code + "&transaction_id=" + model.change_schedule_id + "&module_id=32&date_from=" + model.date_from + "&date_to=" + model.date_to + "&category_id=" + model.category_id + "&created_by=" + model.created_by;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url2);
            request.Method = "GET";
            String returnString = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                returnString = reader.ReadToEnd();
                resp = JsonConvert.DeserializeObject<InsertResponse>(returnString);
                reader.Close();
                dataStream.Close();
            }

            if (resp.id == 0)
            {
                ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

                SeriesRequest req = new SeriesRequest();
                SeriesResponse res = new SeriesResponse();

                string responseInString = "";
                try
                {
                    if (model.change_schedule_id == "0")
                    {


                        using (var wb = new WebClient())
                        {


                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "32";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.change_schedule_code = res.series_code;


                    }




                    resp = _FilingManagementServices.change_schedule_in_up(model);

                    if (resp.id != 0)
                    {
                        model.change_schedule_id = Convert.ToString(resp.id);
                        var resp_detail = _FilingManagementServices.change_schedule_detail_in(model);

                        approval_req.module_id = 32;
                        approval_req.transaction_id = resp.id;
                        approval_req.series_code = model.series_code;
                        approval_req.approval_level_id = model.approval_level_id;
                        approval_req.created_by = model.created_by;
                        approval_req.transaction_code = model.change_schedule_code;
                        var approval = approval_process_in(approval_req);

                        if (approval.approved == true)
                        {
                            using (var wb = new WebClient())
                            {


                                string url = "http://localhost:1014/api/AttendanceManagement/employee_schedule_detail_in";
                                //string url = "http://localhost:48532/api/AttendanceManagement/employee_schedule_detail_in";

                                wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                                string Stringdata = JsonConvert.SerializeObject(resp_detail.ToArray());
                                responseInString = wb.UploadString(url, Stringdata);
                                //string HtmlResult = wb.UploadValues(url, data);

                                //var response = wb.UploadValues(url, "POST", data);
                                //responseInString = Encoding.UTF8.GetString(response);

                            }
                            var id = JsonConvert.DeserializeObject<int>(responseInString);
                        }





                    }



                }
                catch (Exception e)
                {
                    var message = "Error: " + e.Message;
                    resp.id = 0;
                    resp.error_message = "Error: " + e.Message; ;

                }


            }
            else
            {
                resp.id = 0;
            }

            return resp;



        }


        [HttpGet("change_schedule_view_sel")]
        public List<ChangeScheduleResponse> change_schedule_view_sel(string series_code, string change_schedule_id, int file_type, string created_by)
        {

            var resp = _FilingManagementServices.change_schedule_view_sel(series_code, change_schedule_id,  file_type, created_by);

            return resp;
        }

        [HttpGet("change_schedule_shift_view")]
        public List<ScheduleShiftResponse> change_schedule_shift_view(string series_code, string employee_id, string shift_id, string date_from, string date_to, string created_by)
        {


            var resp = _FilingManagementServices.change_schedule_shift_view(series_code, employee_id, shift_id, date_from, date_to, created_by);

            return resp;
        }
        
        [HttpGet("change_schedule_detail_view")]
        public List<ScheduleShiftResponse> change_schedule_detail_view(string series_code, string change_schedule_id, int file_type, string created_by)
        {

            var resp = _FilingManagementServices.change_schedule_detail_view(series_code, change_schedule_id, file_type, created_by);

            return resp;
        }


        #endregion


        #region "COE"
        [HttpPost("coe_request_in_up")]
        public COEIUResponse coe_request_in_up(COERequest model)
        {
            var resp = new COEIUResponse();

            var response = new COEIUResponse();

                ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

                SeriesRequest req = new SeriesRequest();
                SeriesResponse res = new SeriesResponse();

                string responseInString = "";
                try
                {
                    if (model.coe_id == "0")
                    {


                        using (var wb = new WebClient())
                        {


                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "52";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.coe_code = res.series_code;


                    }




                    resp = _FilingManagementServices.coe_request_in_up(model);

                    if (resp.id != 0)
                    {

                        approval_req.module_id = 52;
                        approval_req.transaction_id = resp.id;
                        approval_req.series_code = model.series_code;
                        approval_req.approval_level_id = model.approval_level_id;
                        approval_req.created_by = model.created_by;
                        approval_req.transaction_code = model.coe_code;
                        var approval = approval_process_in(approval_req);

                    }



                }
                catch (Exception e)
                {
                    var message = "Error: " + e.Message;
                    resp.id = 0;
                    resp.description = "Error: " + e.Message; ;

                }

            return resp;



        }


        [HttpGet("coe_request_view_sel")]
        public List<COEResponse> coe_request_view_sel(string series_code, string coe_id, int file_type,  string created_by)
        {

            var resp = _FilingManagementServices.coe_request_view_sel(series_code, coe_id, file_type,  created_by);

            return resp;
        }

        #endregion

        #region "Upload Saving"


        [HttpPost("change_log_in")]
        public int change_log_in(UploadInRequest model)
        {
            uploadResponse file_resp = new uploadResponse();


            SeriesTemp req = new SeriesTemp();
            string responseInString = "";
            using (var wb = new WebClient())
            {

                string url = "http://localhost:1006/api/TenantDefaultSetup/series_temp_in";
                //string url = "http://localhost:10006/api/TenantDefaultSetup/series_temp_in";

                req.module_id = 33;
                req.series_code = model.series_code;
                req.created_by = model.created_by;

                wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                string Stringdata = JsonConvert.SerializeObject(req);
                responseInString = wb.UploadString(url, Stringdata);

            }
             var res = JsonConvert.DeserializeObject<int>(responseInString);


            int resp = 0;
            try
            {

                file_resp = _FilingManagementServices.change_log_in(model);
                resp = file_resp.created_by;


                using (var wb = new WebClient())
                {

                    AttendanceLogApprovalRequest req_automation = new AttendanceLogApprovalRequest();

                    req_automation.transaction_id =file_resp.transaction_id;
                    req_automation.series_code = model.series_code;
                    req_automation.created_by = model.created_by;

                    string url = "http://localhost:1014/api/AttendanceManagement/attendance_log_approval_in";
                    //string url = "http://localhost:48532/api/AttendanceManagement/attendance_log_approval_in";


                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req_automation);
                    responseInString = wb.UploadString(url, Stringdata);

                }
                var response = JsonConvert.DeserializeObject<int>(responseInString);
            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);

                resp = 0;
            }

            return resp;
        }


        [HttpPost("change_schedule_in")]
        public int change_schedule_in(UploadInRequest model)
        {

            uploadResponse file_resp = new uploadResponse();
            int resp = 0;
            try
            {
                SeriesTemp req = new SeriesTemp();
                string responseInString = "";
                using (var wb = new WebClient())
                {

                    string url = "http://localhost:1006/api/TenantDefaultSetup/series_temp_in";
                    //string url = "http://localhost:10006/api/TenantDefaultSetup/series_temp_in";

                    req.module_id = 32;
                    req.series_code = model.series_code;
                    req.created_by = model.created_by;

                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req);
                    responseInString = wb.UploadString(url, Stringdata);

                }
                var res = JsonConvert.DeserializeObject<int>(responseInString);



                file_resp = _FilingManagementServices.change_schedule_in(model);
                resp = file_resp.created_by;


                using (var wb = new WebClient())
                {

                    AttendanceLogApprovalRequest req_automation = new AttendanceLogApprovalRequest();

                    string url = "http://localhost:1014/api/AttendanceManagement/employee_schedule_detail_auto";
                    //string url = "http://localhost:48532/api/AttendanceManagement/employee_schedule_detail_auto";
                    
                    req_automation.transaction_id = file_resp.transaction_id;
                    req_automation.series_code = model.series_code;
                    req_automation.created_by = model.created_by;


                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req_automation);
                    responseInString = wb.UploadString(url, Stringdata);


                    //string HtmlResult = wb.UploadValues(url, data);

                    //var response = wb.UploadValues(url, "POST", data);
                    //responseInString = Encoding.UTF8.GetString(response);

                }
                var id = JsonConvert.DeserializeObject<int>(responseInString);

            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);
            }

            return resp;
        }


        [HttpPost("leave_in")]
        public int leave_in(UploadInRequest model)
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

                    req.module_id = 34;
                    req.series_code = model.series_code;
                    req.created_by = model.created_by;

                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req);
                    responseInString = wb.UploadString(url, Stringdata);

                }
                var res = JsonConvert.DeserializeObject<int>(responseInString);


                resp = _FilingManagementServices.leave_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);
            }

            return resp;
        }


        [HttpPost("official_business_in")]
        public int official_business_in(UploadInRequest model)
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

                    req.module_id = 35;
                    req.series_code = model.series_code;
                    req.created_by = model.created_by;

                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req);
                    responseInString = wb.UploadString(url, Stringdata);

                }
                var res = JsonConvert.DeserializeObject<int>(responseInString);


                resp = _FilingManagementServices.official_business_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);
            }

            return resp;
        }


        [HttpPost("offset_in")]
        public int offset_in(UploadInRequest model)
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

                    req.module_id = 37;
                    req.series_code = model.series_code;
                    req.created_by = model.created_by;

                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req);
                    responseInString = wb.UploadString(url, Stringdata);

                }
                var res = JsonConvert.DeserializeObject<int>(responseInString);


                resp = _FilingManagementServices.offset_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);
            }

            return resp;
        }


        [HttpPost("overtime_in")]
        public int overtime_in(UploadInRequest model)
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

                    req.module_id = 36;
                    req.series_code = model.series_code;
                    req.created_by = model.created_by;

                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req);
                    responseInString = wb.UploadString(url, Stringdata);

                }
                var res = JsonConvert.DeserializeObject<int>(responseInString);



                resp = _FilingManagementServices.overtime_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);
            }

            return resp;
        }

        #endregion



        [HttpPost("transaction_cancel_up")]
        public int transaction_cancel_up(CancelTransactionRequest model)
        {

            var resp = _FilingManagementServices.transaction_cancel_up(model);

            return resp;
        }

        [HttpGet("filing_dashboard_view")]
        public List<DashboardResponse> filing_dashboard_view(string series_code, string employee_id, bool approver, int count, string created_by)
        {

            var resp = _FilingManagementServices.filing_dashboard_view(series_code, employee_id, approver, count, created_by);

            return resp;
        }

    }
}
