using HolidayManagementService.Helper;
using HolidayManagementService.Model;
using HolidayManagementService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HolidayManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayManagementController : ControllerBase
    {

        private IHolidayManagementServices _HolidayManagementServices;

        private EmailSender email;
        private Default_Url url;

        public HolidayManagementController(IHolidayManagementServices HolidayManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _HolidayManagementServices = HolidayManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }


        [HttpPost("HolidayIU")]
        public int HolidayIU(HolidayHeader model)
        {

            int resp = 0;
            SeriesRequest req = new SeriesRequest();

            SeriesResponse res = new SeriesResponse();
            string responseInString = "";
            try
            {
                if (model.holiday_id == "0")
                {

                    try
                    {

                        using (var wb = new WebClient())
                        {

                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                            req.module_id = "11";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.holiday_code = res.series_code;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }


                }
                resp = _HolidayManagementServices.holiday_in_up(model);

               
            }
            catch (Exception e)
            {

                resp = 0;
                Console.WriteLine("Error: " + e.Message);

            }
            return resp;
        }

        [HttpGet("holiday_view_sel")]
        public List<HolidayHeaderResponse> holiday_view_sel(string series_code, string holiday_id)
        {
            var resp = _HolidayManagementServices.holiday_view_sel(series_code, holiday_id);

            return resp;
        }

        [HttpGet("holiday_branch_view")]
        public List<HolidayBranchView> holiday_branch_view(string series_code, string holiday_id)
        {

            var resp = _HolidayManagementServices.holiday_branch_view(series_code, holiday_id);

            return resp;
        }

        [HttpGet("holiday_detail_view")]
        public List<HolidayDetailView> holiday_detail_view(string series_code, string holiday_id)
        {
            var resp = _HolidayManagementServices.holiday_detail_view(series_code, holiday_id);

            return resp;
        }
    }
}
