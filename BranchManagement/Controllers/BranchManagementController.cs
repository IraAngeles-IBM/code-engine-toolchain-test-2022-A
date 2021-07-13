using BranchManagementService.Helper;
using BranchManagementService.Model;
using BranchManagementService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BranchManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchManagementController : ControllerBase
    {

        private IBranchManagementServices _BranchManagementServices;

        private EmailSender email;
        private Default_Url url;

        public BranchManagementController(IBranchManagementServices BranchManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _BranchManagementServices = BranchManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }

        [HttpPost("MultipleBranchIU")]
        public CompanyBranchOutput MultipleBranchIU(BranchIURequest[] model)
        {

            CompanyBranchOutput resp = new CompanyBranchOutput();
            try
            {

           

            var branch_result = _BranchManagementServices.MultipleBranchIU(model);

            if (branch_result[0].branch_id == null)
            {

                resp.description = " Branch data creation have a problem!";
                resp.id = 0;
            }
            else
            {

                resp.description = "Saving Successful! ";
                resp.id = 1;
            }
            }
            catch (Exception e)
            {

                resp.description = "Error: " + e.Message;
                resp.id = 0;
                Console.WriteLine("Error: " + e.Message);

            }
            return resp;
        }


        [HttpPost("BranchIU")]
        public CompanyBranchOutput BranchIU(BranchIURequest model)
        {

            CompanyBranchOutput resp = new CompanyBranchOutput();
            SeriesRequest req= new SeriesRequest();

            SeriesResponse res = new SeriesResponse();
            string responseInString = "";
            try
            {
                if (model.branchID == "0")
                {

                    try
                    {

                        using (var wb = new WebClient())
                        {

                            //string url = "https://tenantdefaultsetupservices.azurewebsites.net/api/TenantDefaultSetup/series_in";
                            
                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "6";
                            req.series_code = model.company_series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.branch_series_code = res.series_code;
                    }
                    catch (Exception e)
                    {


                    }


                }
                var branch_result = _BranchManagementServices.BranchIU(model);

                if (branch_result.branch_id == null)
                {

                    resp.description = " Branch data creation have a problem!";
                    resp.id = 0;
                }
                else
                {

                    resp.description = "Saving Successful! ";
                    resp.id = 1;
                }
            }
            catch (Exception e)
            {

                resp.description = "Error: " + e.Message;
                resp.id = 0;
                Console.WriteLine("Error: " + e.Message);

            }
            return resp;
        }


        [HttpGet("branch_view")]
        public List<BranchResponse> branch_view(string company_series_code, string company_id, string branch_id, string created_by)
        {
            List<BranchResponse> result = new List<BranchResponse>();
            try
            {

                 result = _BranchManagementServices.branch_view(company_series_code, company_id, branch_id, created_by);
            }catch(Exception e)
            {
                result[0].branchName = e.Message;
            }
            return result;
        }



        [HttpGet("branch_list")]
        public List<BranchViewResponse> branch_list(string series_code,  string created_by)
        {
            List<BranchViewResponse> result = new List<BranchViewResponse>();
            try
            {

                result = _BranchManagementServices.branch_list(series_code, created_by);
            }
            catch (Exception e)
            {
                result[0].branch_name = e.Message;
            }
            return result;
        }



        [HttpGet("branch_ip_view")]
        public List<IPResponse> branch_ip_view(string company_series_code, string company_id, string branch_id, string created_by)
        {

            var result = _BranchManagementServices.branch_ip_view(company_series_code, company_id, branch_id, created_by);
            return result;
        }

        [HttpGet("branch_contact_view")]
        public List<ContactResponse> branch_contact_view(string company_series_code, string company_id, string branch_id, string created_by)
        {

            var result = _BranchManagementServices.branch_contact_view(company_series_code, company_id, branch_id, created_by);
            return result;
        }

        [HttpGet("branch_email_view")]
        public List<EmailResponse> branch_email_view(string company_series_code, string company_id, string branch_id, string created_by)
        {

            var result = _BranchManagementServices.branch_email_view(company_series_code, company_id, branch_id, created_by);
            return result;
        }



    }
}
