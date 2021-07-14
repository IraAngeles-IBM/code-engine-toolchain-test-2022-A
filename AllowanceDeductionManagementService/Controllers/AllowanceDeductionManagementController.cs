using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AllowanceDeductionManagementService.Controllers
{
    public class AllowanceDeductionManagementController : Controller
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

                using (var wb = new WebClient())
                {

                    //string url = "https://tenantdefaultsetupservices.azurewebsites.net/api/TenantDefaultSetup/series_in";

                    string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";

                    req.module_id = "10";
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
                resp = _LeaveManagementServices.leave_type_in_up(model);
            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp = 0;

            }


            return resp;
        


        }


        [HttpPost("leave_type_view_sel")]
        public List<LeaveTypeResponse> leave_type_view_sel(string series_code, int leave_type_id)
        {

            var resp = _LeaveManagementServices.leave_type_view_sel(series_code,leave_type_id);

            return resp;
        }

    }
}
