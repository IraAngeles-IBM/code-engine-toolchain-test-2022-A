using LogServices.Helper;
using LogServices.Model;
using LogServices.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LogServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogServiceController : ControllerBase
    {
        private ILogService _LogService;

        private EmailSender email;
        private Default_Url url;

        public LogServiceController(ILogService LogService, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _LogService = LogService;

            email = appSettings.Value;
            url = settings.Value;
        }

        [HttpPost("error_log_in")]
        public string error_log_in(ErrorLogsRequest model)
        {
            var resp = _LogService.error_log_in(model);
            return resp;
        }


        [HttpGet("system_notification_view")]
        public List<NotificationResponse> system_notification_view(string series_code, string date_from, string date_to, int module_id, string created_by)
        {

            var resp = _LogService.system_notification_view(series_code, date_from, date_to, module_id, created_by);

            return resp;
        }

        [HttpGet("system_notification_fetch_view")]
        public List<NotificationResponse> system_notification_fetch_view(string series_code, int row, int index, string created_by)
        {

            var resp = _LogService.system_notification_fetch_view(series_code, row, index, created_by);

            return resp;
        }

        [HttpGet("system_log_view")]
        public List<LogResponse> system_log_view(string series_code, string date_from, string date_to, int module_id, int transaction_type_id, string created_by)
        {

            var resp = _LogService.system_log_view(series_code, date_from, date_to, module_id,transaction_type_id, created_by);

            return resp;
        }

        [HttpGet("system_fetch_view")]
        public List<LogResponse> system_fetch_view(string series_code, int row, int index, string created_by)
        {

            var resp = _LogService.system_fetch_view(series_code, row, index, created_by);

            return resp;
        }


    }
}
