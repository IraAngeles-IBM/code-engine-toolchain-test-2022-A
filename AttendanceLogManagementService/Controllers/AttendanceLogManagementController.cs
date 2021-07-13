using AttendanceLogManagementService.Helper;
using AttendanceLogManagementService.Model;
using AttendanceLogManagementService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceLogManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceLogManagementController : ControllerBase
    {

        private IAttendanceLogManagementServices _AttendanceLogManagementServices;

        private EmailSender email;
        private Default_Url url;

        public AttendanceLogManagementController(IAttendanceLogManagementServices AttendanceLogManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _AttendanceLogManagementServices = AttendanceLogManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }


        [HttpPost("TokenRequest")]
        public AuthenticateResponse TokenRequest(AuthenticateRequest model)
        {
            AuthenticateResponse resp = _AttendanceLogManagementServices.TokenRequest(model);


            return resp;
        }


        [HttpPost("attendance_log_in_up")]
        public int attendance_log_in_up(AttendanceLogRequest model)
        {

            var resp = _AttendanceLogManagementServices.attendance_log_in_up(model);


            return resp;
        }


        [HttpPost("attendance_log_temp")]
        public int attendance_log_temp(List<AttendanceLog> model, string series_code)
        {
            var resp = _AttendanceLogManagementServices.attendance_log_temp(model, series_code);

            return resp;
        }



    }
}
