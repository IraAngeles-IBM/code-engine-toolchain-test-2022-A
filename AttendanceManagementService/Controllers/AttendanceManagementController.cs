using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AttendanceManagementService.Helper;
using AttendanceManagementService.Model;
using AttendanceManagementService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AttendanceManagementService.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceManagementController : Controller
    {

        private IAttendanceManagementServices _AttendanceManagementServices;

        private EmailSender email;
        private Default_Url url;

        public AttendanceManagementController(IAttendanceManagementServices AttendanceManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _AttendanceManagementServices = AttendanceManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }

        [HttpPost("employee_schedule_detail_in")]
        public int employee_schedule_detail_in(EmployeeScheduleDetailRequest[] model)
        {
            var resp = 0;

            List<EmployeeMovementRequest> req = new List<EmployeeMovementRequest>();
            try
            {
               
                req = _AttendanceManagementServices.employee_schedule_detail_in(model);
                string responseInString = "";

                using (var wb = new WebClient())
                {

                    string url = "http://localhost:1008/api/UserManagement/employee_movement_in";
                    //string url = "http://localhost:63620/api/UserManagement/employee_movement_in";



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
                resp = 0;

            }


            return resp;
        


        }



        [HttpGet("employee_schedule_detail_view_sel")]
        public List<EmployeeScheduleDetailResponse> employee_schedule_detail_view_sel(string series_code, string shift_id, string created_by)
        {

            var resp = _AttendanceManagementServices.employee_schedule_detail_view_sel(series_code, shift_id, created_by);

            return resp;
        }


        [HttpGet("employee_schedule_view_sel")]
        public List<EmployeeScheduleResponse> employee_schedule_view_sel(string series_code, string shift_id, string employee_id, string date_from, string date_to, string created_by)
        {

            var resp = _AttendanceManagementServices.employee_schedule_view_sel( series_code,  shift_id,  employee_id,  date_from,  date_to,  created_by);

            return resp;
        }


        [HttpPost("attendance_log_temp_in")]
        public int attendance_log_temp_in(LogRequest model)
        {
            var resp = _AttendanceManagementServices.attendance_log_temp_in(model);

            return resp;
        }


        [HttpPost("attendance_log_in_up")]
        public int attendance_log_in_up(AttendanceLogRequest model)
        {
            var resp = _AttendanceManagementServices.attendance_log_in_up(model);

            return resp;
        }


        [HttpPost("attendance_log_deleted_in")]
        public int attendance_log_deleted_in(AttendanceLogRequest model)
        {
            var resp = _AttendanceManagementServices.attendance_log_deleted_in(model);

            return resp;
        }



        [HttpGet("attendance_log_sel")]
        public List<AttendanceLog> attendance_log_sel(string series_code, string date_from, string date_to, string bio_id, string created_by)
        {

            var resp = _AttendanceManagementServices.attendance_log_sel(series_code, date_from,date_to,bio_id, created_by);

            return resp;
        }


        [HttpGet("employee_attendance_cl_view")]
        public List<AttendanceCLResponse> employee_attendance_cl_view(string series_code, string date_from, string date_to, string employee_id, string created_by)
        {

            var resp = _AttendanceManagementServices.employee_attendance_cl_view(series_code, date_from, date_to, employee_id, created_by);

            return resp;
        }



        [HttpGet("employee_attendance_dashboard_view")]
        public List<AttendanceDashboardResponse> employee_attendance_dashboard_view(string series_code, string date_from, string date_to, string employee_id, string created_by)
        {

            var resp = _AttendanceManagementServices.employee_attendance_dashboard_view(series_code, date_from, date_to, employee_id, created_by);

            return resp;
        }



        [HttpPost("attendance_log_cl_in")]
        public int attendance_log_cl_in(AttendanceCLRequest model)
        {
            var resp = _AttendanceManagementServices.attendance_log_cl_in(model);

            return resp;
        }



        [HttpPost("employee_schedule_detail_auto")]
        public int employee_schedule_detail_auto(AttendanceLogApprovalRequest model)
        {
            var resp = _AttendanceManagementServices.employee_schedule_detail_auto(model);

            return resp;
        }


        [HttpPost("attendance_log_approval_in")]
        public int attendance_log_approval_in(AttendanceLogApprovalRequest model)
        {
            var resp = _AttendanceManagementServices.attendance_log_approval_in(model);

            return resp;
        }


        [HttpPost("attendance_log_in")]
        public int attendance_log_in(AttendanceRequest model)
        {
            var resp = _AttendanceManagementServices.attendance_log_in(model);

            return resp;
        }


        [HttpGet("employee_attendance_view")]
        public List<EmployeeAttendanceResponse> employee_attendance_view(string series_code, string date_from, string date_to, string employee_id, bool missing_logs_only, bool is_supervisor, string created_by)
        {

            var resp = _AttendanceManagementServices.employee_attendance_view( series_code,  date_from,  date_to,  employee_id,  missing_logs_only,  is_supervisor, created_by);

            return resp;
        }


        [HttpPost("employee_attendance_up")]
        public int employee_attendance_up(EmployeeAttendanceRequest model)
        {
            var resp = _AttendanceManagementServices.employee_attendance_up(model);

            return resp;
        }

    }
}
