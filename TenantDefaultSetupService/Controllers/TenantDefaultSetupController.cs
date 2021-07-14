
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TenantDefaultSetupService.Helper;
using TenantDefaultSetupService.Model;
using TenantDefaultSetupService.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TenantDefaultSetupService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantDefaultSetupController : Controller
    {

        private ITenantDefaultSetupService _TenantDefaultSetupService;

        private EmailSender email;
        private Default_Url url;

        public TenantDefaultSetupController(ITenantDefaultSetupService TenantDefaultSetup, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _TenantDefaultSetupService = TenantDefaultSetup;

            email = appSettings.Value;
            url = settings.Value;
        }



        // POST api/<TenantDefaultSetupController>
        [HttpPost("BranchIU")]
        public CompanyBranchOutput BranchIU(BranchIURequest[] model)
        {

            CompanyBranchOutput resp = new CompanyBranchOutput();

            int count = model.Count();
            try
            {

                var resp_Series = _TenantDefaultSetupService.series_multiple_in("6", model[0].company_series_code,count);

                for (int x = 0; x< count;x++)
                {

                    model[x].branch_series_code = resp_Series[x].series_code;
                }

                string responseInString = "";

                using (var wb = new WebClient())
                {
                    string url = "http://localhost:1002/api/BranchManagement/MultipleBranchIU";
                    //string url = "http://localhost:10022/api/BranchManagement/MultipleBranchIU";



                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(model);
                    responseInString = wb.UploadString(url, Stringdata);
                    //string HtmlResult = wb.UploadValues(url, data);

                    //var response = wb.UploadValues(url, "POST", data);
                    //responseInString = Encoding.UTF8.GetString(response);

                }
                resp = JsonConvert.DeserializeObject<CompanyBranchOutput>(responseInString);
            }
            catch (Exception e)
            {

                resp.description = "Error on Tenant Default Setup: " + e.Message;
                resp.id = 0;
                Console.WriteLine("Error on Tenant Default Setup:" + e.Message);

                var resp_Series = _TenantDefaultSetupService.series_multiple_del("6", model[0].company_series_code, count);

            }

            return resp;
        }

        // POST api/<TenantDefaultSetupController>
        [HttpPost("series_in")]
        public SeriesResponse series_in(SeriesRequest model)
        {
            SeriesResponse resp = new SeriesResponse();
            try
            {
                var series_code = Crypto.url_decrypt(model.series_code);

                 resp = _TenantDefaultSetupService.series_in(model.module_id, series_code);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on Tenant Default Setup:" + e.Message);
            }

            return resp;
        }

        [HttpPost("series_up")]
        public int series_up(SeriesUp model)
        {
            //SeriesResponse resp = new SeriesResponse();
            int resp = 0;
            try
            {
                var series_code = Crypto.url_decrypt(model.series_code);

                resp = _TenantDefaultSetupService.series_up(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on Tenant Default Setup:" + e.Message);
            }

            return resp;
        }


        [HttpPost("series_temp_in")]
        public int series_temp_in(SeriesTemp model)
        {
            int resp = 0;
            try
            {

                resp = _TenantDefaultSetupService.series_temp_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on Tenant Default Setup:" + e.Message);
            }

            return resp;
        }


        [HttpGet("series_view")]
        public List<Series> series_view(string series_code, string created_by)
        {
            var result = _TenantDefaultSetupService.series_view(series_code, created_by);
            return result;
        }



        [HttpGet("series_view_next")]
        public List<Series> series_view_next(string series_code, string created_by, int module_id)
        {
            var result = _TenantDefaultSetupService.series_view_next(series_code, created_by, module_id);
            return result;
        }

        [HttpPost("DropdownIU")]
        public DropdownIUResponse DropdownIU(DropdownIURequest model)
        {

            var result = _TenantDefaultSetupService.DropdownIU(model);
            return result;
        }


        [HttpGet("dropdown_view")]
        public List<DropdownResponse> dropdown_view(string dropdowntype_id,string series_code)
        {
            var result = _TenantDefaultSetupService.dropdown_view(dropdowntype_id,series_code);
            return result;
        }


        [HttpGet("dropdown_view_all")]
        public List<DropdownResponse> dropdown_view_all(string dropdowntype_id, string series_code)
        {
            var result = _TenantDefaultSetupService.dropdown_view_all(dropdowntype_id, series_code); 
            return result;
        }



        [HttpGet("approval_sequence_view")]
        public List<ApprovalSequenceResponse> approval_sequence_view(string series_code,int module_id, int approval_level_id)
        {
            var result = _TenantDefaultSetupService.approval_sequence_view(series_code, module_id,approval_level_id);
            return result;
        }


        [HttpGet("transaction_approval_sequence_view")]
        public List<ApprovalSequenceResponse> transaction_approval_sequence_view(string series_code, int module_id,string employee_id, string approval_level_id)
        {
            var result = _TenantDefaultSetupService.transaction_approval_sequence_view(series_code, module_id,employee_id, approval_level_id);
            return result;
        }


        [HttpGet("approval_sequence_email_view")]
        public int approval_sequence_email_view(string series_code, int module_id, string employee_id, string approval_level_id, string transaction_code)
        {
            int resp = 0;
            string html = "";
            try
            {

                var result = _TenantDefaultSetupService.approval_sequence_email_view(series_code, module_id, employee_id, approval_level_id);

                foreach (var item in result)
                {
                    if (item.is_email == true)
                    {


                        var sendGridClient = new SendGridClient(email.ApiKey);

                        ApprovalEmailNotificationResponse notif_param = new ApprovalEmailNotificationResponse();

                        var sendGridMessage = new SendGridMessage();
                        if (item.approved ==false)
                        {

                            notif_param.approver_name = item.approver_name;
                            notif_param.module_name = item.module_name;
                            notif_param.transaction_code = transaction_code;
                            notif_param.date_created = item.date_created;
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
                            notif_param.transaction_code = transaction_code;
                            notif_param.date_created = item.date_created;
                            notif_param.email_address = item.email_address;
                            notif_param.header = item.module_name + " has been Approved!";


                            sendGridMessage.SetFrom(email.email_username, "Aanya HR Notification");
                            sendGridMessage.AddTo(item.email_address, item.approver_name);
                            sendGridMessage.SetTemplateId("d-baf0576148cf4005adc067d4e19319c3");
                            sendGridMessage.SetTemplateData(notif_param);

                        }


                        var response = sendGridClient.SendEmailAsync(sendGridMessage).Result;



                        //var client = new SendGridClient(email.ApiKey);
                        //var from = new EmailAddress(email.email_username, "Aanya HR Notification");
                        //var subject = item.module_name + " Filing";
                        //var to = new EmailAddress(item.email_address, "Approver");
                        //var plainTextContent = "Hi " + item.approver_name + ",";
                        //plainTextContent += System.Environment.NewLine;
                        //plainTextContent += System.Environment.NewLine;
                        //plainTextContent += "You have pending approval for " + item.module_name;
                        //var htmlContent = "";
                        //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, html);
                        //var response = client.SendEmailAsync(msg);

                    }
                }      
                
                resp = 1;
            }catch(Exception e)
            {
                resp = 0;
            }
            return resp;
        }


        private class HelloEmail
        {
            public string Name { get; set; }
        }

        [HttpGet("approval_sequence_module_view")]
        public List<ApprovalSequenceHeaderRequest> approval_sequence_module_view(string series_code, string modules, int approval_level_id)
        {
            var result = _TenantDefaultSetupService.approval_sequence_module_view(series_code, modules, approval_level_id);
            return result;
        }

        [HttpGet("approval_sequence_status_view")]
        public List<ApprovalSequenceResponse> approval_sequence_status_view(string series_code, int module_id, int access_level_id)
        {
            var result = _TenantDefaultSetupService.approval_sequence_status_view(series_code, module_id, access_level_id);
            return result;
        }


        [HttpPost("approval_sequence_in")]
        public int approval_sequence_in(ApprovalSequenceHeaderRequest[] model)
        {
            var result = _TenantDefaultSetupService.approval_sequence_in(model);
            return result;
        }


        [HttpGet("transaction_approval_header")]
        public List<ApprovalHeaderResponse> transaction_approval_header(string series_code, int module_id, string created_by)
        {

            var resp = _TenantDefaultSetupService.transaction_approval_header(series_code, module_id, created_by);

            return resp;
        }


        [HttpGet("transaction_approval_view")]
        public JsonResult transaction_approval_view(string series_code, int module_id, string series, string date_from, string date_to, string created_by)
        {

            var resp = _TenantDefaultSetupService.transaction_approval_view(series_code, module_id,  series, date_from, date_to, created_by);
            var result = JsonConvert.SerializeObject(resp);
            JsonResult json = Json(result);
            return json;
        }

        // POST api/<TenantDefaultSetupController>
        [HttpPost("dropdown_in")]
        public int dropdown_in(DropdownInRequest model)
        {
            int resp = 0;
            try
            {

                resp = _TenantDefaultSetupService.dropdown_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on Tenant Default Setup:" + e.Message);
            }

            return resp;
        }

    }
}
