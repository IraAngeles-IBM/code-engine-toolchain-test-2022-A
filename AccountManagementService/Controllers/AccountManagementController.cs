using AccountManagementService.Helper;
using AccountManagementService.Model;
using AccountManagementService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AccountManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountManagementController : ControllerBase
    {

        private IAccountManagementService _AccountManagementService;

        private EmailSender email;
        private Default_Url url;

        public AccountManagementController(IAccountManagementService AccountManagement, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _AccountManagementService = AccountManagement;

            email = appSettings.Value;
            url = settings.Value;
        }


        // GET api/<AccountManagementController>/5
        [HttpPost("Registration")]
        public RegistrationResponse Registration(RegistrationRequest model)
        {
            RegistrationResponse resp = new RegistrationResponse();
             resp = _AccountManagementService.Registration(model);

            if (resp.email_address == null)
            {
                return resp;
            }
            else
            {
                //var apiKey = Environment.GetEnvironmentVariable("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");
                //var client = new SendGridClient("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");
                //var from = new EmailAddress("test@example.com", "Example User");
                //var subject = "Sending with SendGrid is Fun";
                //var to = new EmailAddress("test@example.com", "Example User");
                //var plainTextContent = "and easy to do anywhere, even with C#";
                //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
                //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                //var response =  client.SendEmailAsync(msg);


                Execute().Wait();

                //System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                //mail.To.Add(resp.email_address);
                //mail.From = new MailAddress(email.email_username, email.email_name, System.Text.Encoding.UTF8);
                //mail.Subject = "This mail is send from asp.net application";
                //mail.SubjectEncoding = System.Text.Encoding.UTF8;
                //mail.Body = "<a href='" + url.name + "/login/" + resp.id + "' > button </a>";
                //mail.BodyEncoding = System.Text.Encoding.UTF8;
                //mail.IsBodyHtml = true;
                //mail.Priority = MailPriority.High;
                //SmtpClient client = new SmtpClient();
                //client.Credentials = new System.Net.NetworkCredential(email.email_username, email.email_password);
                //client.Port = email.port;
                //client.Host = email.host;

                //client.EnableSsl = true;
                //try
                //{
                //    client.Send(mail);
                //    //Page.RegisterStartupScript("UserMsg", "<script>alert('Successfully Send...');if(alert){ window.location='SendMail.aspx';}</script>");
                //}
                //catch (Exception ex)
                //{
                //    Exception ex2 = ex;
                //    string errorMessage = string.Empty;
                //    while (ex2 != null)
                //    {
                //        errorMessage += ex2.ToString();
                //        ex2 = ex2.InnerException;
                //    }
                //    //Page.RegisterStartupScript("UserMsg", "<script>alert('Sending Failed...');if(alert){ window.location='SendMail.aspx';}</script>");
                //}

            }

            return resp;
        }



        [HttpPost("Verification")]
        public VerificationResponse Verification(VerificationRequest model)
        {
            var response = _AccountManagementService.Verification(model);
            if (response.id == null)
            {
                response.id = "0";
            }
            return response;
        }


        static async Task Execute()
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient("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");
            var from = new EmailAddress("notification@aanyahr.com", "Example User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("joira@illimitado.com", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        [HttpPost("AuthenticateLogin")]
        public AuthenticateResponse AuthenticateLogin(AuthenticateRequest model)
        {
            AuthenticateResponse resp = _AccountManagementService.AuthenticateLogin(model);
            UserAuthenticateRequest ureq = new UserAuthenticateRequest();
            try
            {

                string responseInString = "";
                if (resp.series != "")
                {
                    using (var wb = new WebClient())
                    {
                        string url = "http://localhost:1008/api/UserManagement/AuthenticateLogin";
                        //string url = "http://localhost:10001/api/UserManagement/AuthenticateLogin";

                        ureq.username = model.username;
                        ureq.password = model.password;
                        ureq.company_code = model.company_code;
                        ureq.series_code = resp.series;

                        wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                        string Stringdata = JsonConvert.SerializeObject(ureq);
                        responseInString = wb.UploadString(url, Stringdata);
                        //string HtmlResult = wb.UploadValues(url, data);

                        //var response = wb.UploadValues(url, "POST", data);
                        //responseInString = Encoding.UTF8.GetString(response);


                    }
                    resp = JsonConvert.DeserializeObject<AuthenticateResponse>(responseInString);
                }
            }
            catch (Exception e)
            {
                resp.routing = "Error: " + e.Message;
            }

            return resp;



        }


        [HttpPost("CompanyBranchIU")]
        public CompanyBranchOutput CompanyBranchIU(CompanyBranchIU model)
        {

            //BranchResponse resbranch = new BranchResponse();
            CompanyBranchOutput res = new CompanyBranchOutput();


            var resp = _AccountManagementService.CompanyIU(model.company_IU);

            string responseInString = "";
            if (resp.companyID == null)
            {

                res.description = "Company data creation have a problem!";
                res.id = 0;
            }
            else
            {
                try
                {



                    using (var wb = new WebClient())
                    {

                        string url = "http://localhost:1006/api/TenantDefaultSetup/BranchIU";

                        //string url = "http://localhost:10006/api/TenantDefaultSetup/BranchIU";
                        model.Branch_IU[0].company_series_code = resp.company_series_code;

                        model.Branch_IU[0].company_id = resp.companyID;

                        wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                        string Stringdata = JsonConvert.SerializeObject(model.Branch_IU);
                        responseInString = wb.UploadString(url, Stringdata);
                        //string HtmlResult = wb.UploadValues(url, data);

                        //var response = wb.UploadValues(url, "POST", data);
                        //responseInString = Encoding.UTF8.GetString(response);

                    }
                    res = JsonConvert.DeserializeObject<CompanyBranchOutput>(responseInString);
                }
                catch (Exception e)
                {
                    var in_active = _AccountManagementService.company_in_active(resp.companyID); 

                }
            }



            return res;
        }


        [HttpPost("CompanyIU")]
        public CompanyResponse CompanyIU(CompanyIURequest model)
        {


            var result = _AccountManagementService.CompanyIU(model);


            if (result.companyID == null)
            {
                result.companyID = "0";
            }
            return result;
        }

        [HttpGet("company_view_sel")]
        public List<CompanyResponse> company_view_sel(string company_id, string created_by)
        {
            //dropdowntype_id = dropdowntype_id == "null" ? "0" : dropdowntype_id;
            //dropdown_type = dropdown_type == "null" ? "" : dropdown_type;

            var result = _AccountManagementService.company_view_sel(company_id, created_by);
            return result;
        }


    }
}
