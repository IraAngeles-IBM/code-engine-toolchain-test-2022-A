using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Net;
using EmployeeCategoryManagementService.Services;
using EmployeeCategoryManagementService.Helper;
using EmployeeCategoryManagementService.Model;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeCategoryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeCategoryManagementController : ControllerBase
    {
        private IEmployeeCategoryManagementServices _EmployeeCategoryManagementServices;

        private EmailSender email;
        private Default_Url url;

        public EmployeeCategoryManagementController(IEmployeeCategoryManagementServices EmployeeCategoryManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _EmployeeCategoryManagementServices = EmployeeCategoryManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }


        [HttpPost("employee_category_in_up")]
        public int employee_category_in_up(CategoryRequest model)
        {
            var resp = 0;

            SeriesRequest req = new SeriesRequest();
            SeriesResponse res = new SeriesResponse();
            string responseInString = "";
            try
            {
                if (model.category_id == "0")
                {


                    using (var wb = new WebClient())
                    {

                        string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                        //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                        req.module_id = "18";
                        req.series_code = model.series_code;

                        wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                        string Stringdata = JsonConvert.SerializeObject(req);
                        responseInString = wb.UploadString(url, Stringdata);
                        //string HtmlResult = wb.UploadValues(url, data);

                        //var response = wb.UploadValues(url, "POST", data);
                        //responseInString = Encoding.UTF8.GetString(response);

                    }
                    res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                    model.category_code = res.series_code;
                }
                resp = _EmployeeCategoryManagementServices.employee_category_in_up(model);
            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp = 0;

            }


            return resp;



        }


        [HttpGet("employee_category_view")]
        public List<CategoryResponse> employee_category_view(string series_code, string category_id, string created_by)
        {
            var resp = _EmployeeCategoryManagementServices.employee_category_view(series_code, category_id,created_by);

            return resp;

        }


        [HttpGet("employee_category_view_sel")]
        public List<CategoryResponse> employee_category_view_sel(string series_code, string category_id, string created_by)
        {
            var resp = _EmployeeCategoryManagementServices.employee_category_view_sel(series_code, category_id, created_by);

            return resp;

        }


        [HttpGet("employee_category_restriction")]
        public InsertResponse employee_category_restriction(string series_code, string transaction_id, string module_id, string category_id, string date_from, string date_to, string created_by)
        {
            var resp = _EmployeeCategoryManagementServices.employee_category_restriction(series_code,transaction_id, module_id,category_id, date_from, date_to, created_by);

            return resp;

        }


    }
}
