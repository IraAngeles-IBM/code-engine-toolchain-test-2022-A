using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using LeaveManagementService.Helper;
using LeaveManagementService.Model;
using LeaveManagementService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LeaveManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveManagementController : Controller
    {

        private ILeaveManagementServices _LeaveManagementServices;

        private EmailSender email;
        private Default_Url url;

        public LeaveManagementController(ILeaveManagementServices LeaveManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _LeaveManagementServices = LeaveManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }

        [HttpPost("leave_type_in_up")]
        public int leave_type_in_up(LeaveTypeRequest model)
        {
            var resp = 0;

            SeriesRequest req = new SeriesRequest();
            SeriesResponse res = new SeriesResponse();
            string responseInString = "";
            try
            {
                if (model.leave_type_id == "0")
                {


                    using (var wb = new WebClient())
                    {

                        string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";

                        req.module_id = "43";
                        req.series_code = model.series_code;

                        wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                        string Stringdata = JsonConvert.SerializeObject(req);
                        responseInString = wb.UploadString(url, Stringdata);
                        //string HtmlResult = wb.UploadValues(url, data);

                        //var response = wb.UploadValues(url, "POST", data);
                        //responseInString = Encoding.UTF8.GetString(response);

                    }
                    res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                    model.leave_type_code = res.series_code;
                }
                resp = _LeaveManagementServices.leave_type_in_up(model);
            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp = 0;

            }


            return resp;
        


        }


        [HttpGet("leave_type_view_sel")]
        public List<LeaveTypeResponse> leave_type_view_sel(string series_code, string leave_type_id, string created_by)
        {

            var resp = _LeaveManagementServices.leave_type_view_sel(series_code, leave_type_id, created_by);

            return resp;
        }


        [HttpGet("leave_type_view")]
        public List<LeaveTypeResponse> leave_type_view(string series_code, string leave_type_id, string created_by)
        {

            var resp = _LeaveManagementServices.leave_type_view(series_code, leave_type_id, created_by);

            return resp;
        }


        [HttpPost("leave_entitlement_in")]
        public int leave_entitlement_in(LeaveEntitlementRequest[] model)
        {
            var resp = 0;
            List<EmployeeMovementRequest> req = new List<EmployeeMovementRequest>();
            try
            {

                 req = _LeaveManagementServices.leave_entitlement_in(model);

                string responseInString = "";

                using (var wb = new WebClient())
                {

                    string url = "http://localhost:1008/api/UserManagement/employee_movement_in";
                    //string url = "http://localhost:10006/api/UserManagement/employee_movement_in";



                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req);
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

            }


            return resp;



        }


        [HttpPost("leave_balance_in")]
        public int leave_balance_in(UploadInRequest model)
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

                //    req.module_id = 34;
                //    req.series_code = model.series_code;
                //    req.created_by = model.created_by;

                //    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                //    string Stringdata = JsonConvert.SerializeObject(req);
                //    responseInString = wb.UploadString(url, Stringdata);

                //}
                //var res = JsonConvert.DeserializeObject<int>(responseInString);


                resp = _LeaveManagementServices.leave_balance_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on Leave Management Service:" + e.Message);
            }

            return resp;
        }



        [HttpGet("leave_employee_view")]
        public List<EmployeeLeaveResponse> leave_employee_view(string series_code, string leave_type_id, string employee_id, string created_by)
        {

            var resp = _LeaveManagementServices.leave_employee_view(series_code, leave_type_id,employee_id, created_by);

            return resp;
        }


        [HttpGet("leave_restriction")]
        public InsertResponse leave_restriction(string series_code, string leave_id, string employee_id, int leave_type_id, bool with_pay, bool is_half_day, bool with_attachment, string date_from, string date_to, string created_by)
        {
            var resp = _LeaveManagementServices.leave_restriction(series_code, leave_id, employee_id, leave_type_id, with_pay, is_half_day, with_attachment, date_from, date_to, created_by);

            return resp;

        }

    }
}
