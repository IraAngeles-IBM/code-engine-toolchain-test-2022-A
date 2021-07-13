using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ScheduleManagementService.Helper;
using ScheduleManagementService.Model;
using ScheduleManagementService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ScheduleManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleManagementController : Controller
    {

        private IScheduleManagementServices _ScheduleManagementServices;

        private EmailSender email;
        private Default_Url url;

        public ScheduleManagementController(IScheduleManagementServices ScheduleManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _ScheduleManagementServices = ScheduleManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }

        [HttpPost("shift_code_in_up")]
        public int shift_code_in_up(ShiftCodeRequest model)
        {
            var resp = 0;

            SeriesRequest req = new SeriesRequest();
            SeriesResponse res = new SeriesResponse();
            string responseInString = "";
            try
            {
                if (model.shift_id == "0")
                {
                    using (var wb = new WebClient())
                    {

                        //string url = "https://tenantdefaultsetupservices.azurewebsites.net/api/TenantDefaultSetup/series_in";

                        string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";

                        //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

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

                    model.shift_code = res.series_code;
                }
                resp= _ScheduleManagementServices.shift_code_in_up(model);
            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                //resp = 0;
                //resp = message;

            }


            return resp;
        


        }


        [HttpGet("shift_code_view_sel")]
        public List<ShiftCodeResponse> shift_code_view_sel(string series_code, string shift_id,string created_by)
        {

            var resp = _ScheduleManagementServices.shift_code_view_sel(series_code, shift_id,created_by);

            return resp;
        }

        [HttpGet("shift_code_view")]
        public List<ShiftCodeResponse> shift_code_view(string series_code, string shift_id, string created_by)
        {

            var resp = _ScheduleManagementServices.shift_code_view(series_code, shift_id, created_by);

            return resp;
        }



        [HttpPost("shift_code_per_day_in")]
        public int shift_code_per_day_in(ShiftCodePerDayHeaderlRequest model)
        {
            var resp = 0;

            SeriesRequest req = new SeriesRequest();
            SeriesResponse res = new SeriesResponse();
            string responseInString = "";
            try
            {
                if (model.shift_per_day_id == "0")
                {
                    using (var wb = new WebClient())
                    {

                        string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";

                        //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                        req.module_id = "51";
                        req.series_code = model.series_code;

                        wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                        string Stringdata = JsonConvert.SerializeObject(req);
                        responseInString = wb.UploadString(url, Stringdata);
                        //string HtmlResult = wb.UploadValues(url, data);

                        //var response = wb.UploadValues(url, "POST", data);
                        //responseInString = Encoding.UTF8.GetString(response);

                    }
                    res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                    model.shift_per_day_code = res.series_code;
                }
                resp = _ScheduleManagementServices.shift_code_per_day_in(model);
            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                //resp = 0;
                //resp = message;

            }


            return resp;



        }

        [HttpGet("shift_code_per_day_view_sel")]
        public List<ShiftPerDayHeaderResponse> shift_code_per_day_view_sel(string series_code, string shift_per_day_id, string created_by)
        {

            var resp = _ScheduleManagementServices.shift_code_per_day_view_sel( series_code,  shift_per_day_id,  created_by);

            return resp;
        }


        [HttpGet("shift_code_per_day_detail_view_sel")]
        public List<ShiftPerDayDetailResponse> shift_code_per_day_detail_view_sel(string series_code, string shift_per_day_id, string created_by)
        {

            var resp = _ScheduleManagementServices.shift_code_per_day_detail_view_sel(series_code, shift_per_day_id, created_by);

            return resp;
        }

    }
}
