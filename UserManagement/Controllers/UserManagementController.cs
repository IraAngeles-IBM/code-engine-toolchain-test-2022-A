using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using UserManagementService.Model;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Services;
using UserManagement.Helper;
using Microsoft.Extensions.Options;
using UserManagement.Model;
using System.Net;
using Newtonsoft.Json;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private IUserManagementService _UserManagementService;

        private EmailSender email;
        private Default_Url url;

        public UserManagementController(IUserManagementService UserManagement, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _UserManagementService = UserManagement;

            email = appSettings.Value;
            url = settings.Value;
        }


        // GET api/<UserManagementControllercs>/5
        [HttpPost("AuthenticateLogin")]
        public AuthenticateResponse AuthenticateLogin(AuthenticateRequest model)
        {
            AuthenticateResponse resp = _UserManagementService.AuthenticateLogin(model);
            return resp;
        }


        [HttpGet("employee_active_view")]
        public List<EmployeeResponse> employee_active_view(string series_code, int employee_id)
        {
            var resp = _UserManagementService.employee_active_view(series_code, employee_id);

                return resp;

        }

        [HttpGet("employee_supervisor_view")]
        public List<EmployeeResponse> employee_supervisor_view(string series_code, int employee_id, bool is_supervisor, string created_by)
        {
            var resp = _UserManagementService.employee_supervisor_view(series_code, employee_id,is_supervisor,created_by);

            return resp;

        }
        [HttpGet("employee_view_sel")]
        public List<EmployeeResponse> employee_view_sel(string series_code, string employee_id,string created_by)
        {
            var resp = _UserManagementService.employee_view_sel(series_code, employee_id, created_by);

            return resp;

        }

        [HttpGet("employee_fetch")]
        public List<EmployeeResponse> employee_fetch(string series_code, string employee_id, string created_by,int row, int index)
        {
            var resp = _UserManagementService.employee_fetch(series_code, employee_id, created_by,row,index);

            return resp;

        }


        [HttpPost("employee_in_up")]
        public EmployeeResponse employee_in_up(EmployeeRequest model)
        {
            var movement_resp = 0;
            EmployeeResponse resp = new EmployeeResponse();
            List<EmployeeMovementRequest> emp_resp = new List<EmployeeMovementRequest>();

            SeriesRequest req = new SeriesRequest();
            SeriesResponse res = new SeriesResponse();

            string responseInString = "";
            try
            {
                if(model.encrypt_employee_id =="0")
                { 
                    using (var wb = new WebClient())
                    {

                       string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                        //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";

                        req.module_id = "17";
                        req.series_code = model.series_code;

                        wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                        string Stringdata = JsonConvert.SerializeObject(req);
                        responseInString = wb.UploadString(url, Stringdata);
                        //string HtmlResult = wb.UploadValues(url, data);

                        //var response = wb.UploadValues(url, "POST", data);
                        //responseInString = Encoding.UTF8.GetString(response);

                    }
                    res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                    model.employee_code = res.series_code;
                
                }

                emp_resp = _UserManagementService.employee_in_up(model);

                resp.employee_id = 1;
                if (emp_resp.Count != 0)
                {
                   

                    var movementrequest = emp_resp.ToArray();

                    movement_resp = employee_movement_employee_in(movementrequest);

                    resp.employee_id = movement_resp;

                }


                resp.employee_code = model.employee_code;
            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp.employee_id = 0;
            }


            return resp;



        }


        [HttpPost("employee_profile_up")]
        public int employee_profile_up(UserCredentialRequest model)
        {
            int resp = 0;

            SeriesRequest req = new SeriesRequest();
            SeriesResponse res = new SeriesResponse();
            List<EmployeeMovementRequest> emp_movement = new List<EmployeeMovementRequest>();
            try
            {

                emp_movement = _UserManagementService.employee_profile_up(model);
                resp = 1;


                if (emp_movement.Count != 0)
                {

                    resp = employee_movement_in(emp_movement.ToArray());
                }


            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp = 0;

            }



            return resp;



        }


        [HttpGet("employee_profile_view")]
        public List<EmployeeResponse> employee_profile_view(string series_code, string employee_id)
        {
            var resp = _UserManagementService.employee_profile_view( series_code,  employee_id);

            return resp;

        }

        [HttpPost("employee_movement_employee_in")]
        public int employee_movement_employee_in(EmployeeMovementRequest[] model)
        {
            var resp = 0;
            List<EmployeeMovementRequest> movement_resp = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> movement_resp1 = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> movement_resp2 = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> movement_resp3 = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> movement_resp4 = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> movement_resp5 = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> movement_resp6 = new List<EmployeeMovementRequest>();
            List<BranchViewResponse> branch_resp = new List<BranchViewResponse>();
            List<CategoryResponse> category_resp = new List<CategoryResponse>();
            List<EmployeeResponse> active_emp_resp = new List<EmployeeResponse>();
            List<DropdownResponse> dropdown_resp = new List<DropdownResponse>();
            List<DropdownResponse> dropdownfix_resp = new List<DropdownResponse>();

            //string responseInString = "";
            try
            {

                var distinct_empresp = model.GroupBy(x => x.is_dropdown).Select(group => group.First());

                foreach (var item in distinct_empresp)
                {
                    if (item.is_dropdown == 1)
                    {
                        using (var wb = new WebClient())
                        {

                            string url = "http://localhost:1006/api/TenantDefaultSetup/dropdown_view?dropdowntype_id=0&series_code=" + item.series_code;
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/dropdown_view?dropdowntype_id=0&series_code=" + item.series_code;

                            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                            request.Method = "GET";
                            String returnString = String.Empty;
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                Stream dataStream = response.GetResponseStream();
                                StreamReader reader = new StreamReader(dataStream);
                                returnString = reader.ReadToEnd();
                                dropdown_resp = JsonConvert.DeserializeObject<List<DropdownResponse>>(returnString);
                                reader.Close();
                                dataStream.Close();
                            }

                        }


                        movement_resp1 = (from x in model
                                         join y in dropdown_resp
                                         on x.id equals y.id
                                         where x.is_dropdown.Equals(1)
                                         select new EmployeeMovementRequest
                                         {
                                             employee_id          = x.employee_id  ,
                                             movement_type        = x.movement_type,
                                             is_dropdown          = x.is_dropdown  ,
                                             id                   = x.id           ,
                                             description          = x.description  ,
                                             movement_description = x.id== 0? "": y.description  ,
                                             created_by           = x.created_by   ,
                                             series_code          = x.series_code  ,

                                         }).ToList();


                    }
                    else if (item.is_dropdown == 2)
                    {
                        using (var wb = new WebClient())
                        {

                           string url = "http://localhost:1007/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=0";
                           //string url = "http://localhost:60645/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=0";

                            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                            request.Method = "GET";
                            String returnString = String.Empty;
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                Stream dataStream = response.GetResponseStream();
                                StreamReader reader = new StreamReader(dataStream);
                                returnString = reader.ReadToEnd();
                                dropdownfix_resp = JsonConvert.DeserializeObject<List<DropdownResponse>>(returnString);
                                reader.Close();
                                dataStream.Close();
                            }

                        }



                        movement_resp2 = (from x in model
                                          join y in dropdownfix_resp
                                          on x.id equals y.id
                                          where x.is_dropdown.Equals(2)
                                          select new EmployeeMovementRequest
                                          {
                                              employee_id = x.employee_id,
                                              movement_type = x.movement_type,
                                              is_dropdown = x.is_dropdown,
                                              id = x.id,
                                              description = x.description,
                                              movement_description = x.id == 0 ? "" : y.description,
                                              created_by = x.created_by,
                                              series_code = x.series_code,

                                          }).ToList();

                    }

                    else if (item.is_dropdown == 3)
                    {
                        using (var wb = new WebClient())
                        {

                           string url = "http://localhost:1002/api/BranchManagement/branch_list?series_code=" + item.series_code + "&created_by=" + item.created_by;
                           //string url = "http://localhost:10022/api/BranchManagement/branch_list?series_code=" + item.series_code + "&created_by=" + item.created_by;

                            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                            request.Method = "GET";
                            String returnString = String.Empty;
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                Stream dataStream = response.GetResponseStream();
                                StreamReader reader = new StreamReader(dataStream);
                                returnString = reader.ReadToEnd();
                                branch_resp = JsonConvert.DeserializeObject<List<BranchViewResponse>>(returnString);
                                reader.Close();
                                dataStream.Close();
                            }

                        }
                        movement_resp3 = (from x in model
                                          join y in branch_resp
                                          on x.id equals y.branch_id
                                          where x.is_dropdown.Equals(3)
                                          select new EmployeeMovementRequest
                                          {
                                              employee_id = x.employee_id,
                                              movement_type = x.movement_type,
                                              is_dropdown = x.is_dropdown,
                                              id = x.id,
                                              description = x.description,
                                              movement_description = x.id == 0 ? "" : y.branch_name,
                                              created_by = x.created_by,
                                              series_code = x.series_code,

                                          }).ToList();

                    }

                    else if (item.is_dropdown == 4)
                    {
                         active_emp_resp = _UserManagementService.employee_active_view(item.series_code, 0);

                        movement_resp4 = (from x in model
                                          join y in active_emp_resp
                                          on x.id equals y.employee_id
                                          where x.is_dropdown.Equals(4)
                                          select new EmployeeMovementRequest
                                          {
                                              employee_id = x.employee_id,
                                              movement_type = x.movement_type,
                                              is_dropdown = x.is_dropdown,
                                              id = x.id,
                                              description = x.description,
                                              movement_description = x.id == 0 ? "" : y.display_name,
                                              created_by = x.created_by,
                                              series_code = x.series_code,

                                          }).ToList();
                    }


                    else if (item.is_dropdown ==5)
                    {
                        using (var wb = new WebClient())
                        {

                            string url = "http://localhost:1013/api/EmployeeCategoryManagement/employee_category_view?series_code=" + item.series_code + "&category_id=0&created_by=" + item.created_by;
                            //string url = "http://localhost:51013/api/EmployeeCategoryManagement/employee_category_view?series_code=" + item.series_code + "&category_id=0&created_by=" + item.created_by;

                            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                            request.Method = "GET";
                            String returnString = String.Empty;
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                Stream dataStream = response.GetResponseStream();
                                StreamReader reader = new StreamReader(dataStream);
                                returnString = reader.ReadToEnd();
                                category_resp = JsonConvert.DeserializeObject<List<CategoryResponse>>(returnString);
                                reader.Close();
                                dataStream.Close();
                            }

                        }

                        movement_resp5 = (from x in model
                                          join y in category_resp
                                          on x.id equals y.category_id
                                          where x.is_dropdown.Equals(5)
                                          select new EmployeeMovementRequest
                                          {
                                              employee_id = x.employee_id,
                                              movement_type = x.movement_type,
                                              is_dropdown = x.is_dropdown,
                                              id = x.id,
                                              description = x.description,
                                              movement_description = x.id == 0 ? "" : y.category_name,
                                              created_by = x.created_by,
                                              series_code = x.series_code,

                                          }).ToList();

                    }
                    else
                    {
                        movement_resp = (from x in model
                                         where x.is_dropdown.Equals(0)
                                         select new EmployeeMovementRequest
                                         {
                                             employee_id = x.employee_id,
                                             movement_type = x.movement_type,
                                             is_dropdown = x.is_dropdown,
                                             id = x.id,
                                             description = x.description,
                                             movement_description = "",
                                             created_by = x.created_by,
                                             series_code = x.series_code,

                                         }).ToList();

                    }


                }

                movement_resp6 = (from x in model
                                  where x.id.Equals(0)
                                  select new EmployeeMovementRequest
                                  {
                                      employee_id = x.employee_id,
                                      movement_type = x.movement_type,
                                      is_dropdown = x.is_dropdown,
                                      id = x.id,
                                      description = x.description,
                                      movement_description = "",
                                      created_by = x.created_by,
                                      series_code = x.series_code,

                                  }).ToList();

                foreach (var item in movement_resp1)
                {
                    movement_resp.Add(item);
                }


                foreach (var item in movement_resp2)
                {
                    movement_resp.Add(item);
                }

                foreach (var item in movement_resp3)
                {
                    movement_resp.Add(item);
                }
                foreach (var item in movement_resp4)
                {
                    movement_resp.Add(item);
                }
                foreach (var item in movement_resp5)
                {
                    movement_resp.Add(item);
                }
                foreach (var item in movement_resp6)
                {
                    movement_resp.Add(item);
                }


                if (movement_resp.Count != 0)
                {

                    resp = _UserManagementService.employee_movement_in(movement_resp.ToArray());
                }
                else
                {
                    resp = 1;
                }



            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp = 0;
            }


            return resp;



        }


        [HttpPost("employee_movement_in")]
        public int employee_movement_in(EmployeeMovementRequest[] model)
        {
             
            var resp = _UserManagementService.employee_movement_in(model);


            return resp;

        }


        [HttpGet("employee_movement_sel")]
        public List<EmployeeMovementResponse> employee_movement_sel(string series_code, string employee_id, string created_by, int movement_type, string date_from, string date_to)
        {
            var resp = _UserManagementService.employee_movement_sel(series_code, employee_id, created_by,movement_type,date_from,date_to);

            return resp;

        }

        [HttpGet("employee_schedule_view")]
        public List<EmployeeScheduleResponse> employee_schedule_view(
            string series_code, string shift_id,  string total_working_hours, string date_from, string date_to,
            string tag_type, string id, string shift_code_type, string created_by)
        {
            List<EmployeeScheduleResponse> resp = new List<EmployeeScheduleResponse>();
            List<ShiftCodeResponse> req = new List<ShiftCodeResponse>();
            try
            {
                //string url = "http://localhost:1011/api/ScheduleManagement/shift_code_view?series_code=" + series_code + "&created_by=" + created_by + "&shift_id=" + shift_id;
                ////string url = "http://localhost:53465/api/ScheduleManagement/shift_code_view?series_code=" + series_code + "&created_by=" + created_by + "&shift_id=" +shift_id;

                //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                //request.Method = "GET";
                //String returnString = String.Empty;
                //using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                //{
                //    Stream dataStream = response.GetResponseStream();
                //    StreamReader reader = new StreamReader(dataStream);
                //    returnString = reader.ReadToEnd();
                //    req = JsonConvert.DeserializeObject<List<ShiftCodeResponse>>(returnString);
                //    reader.Close();
                //    dataStream.Close();
                //}



                resp = _UserManagementService.employee_schedule_view(series_code, shift_id, total_working_hours, date_from, date_to,
             tag_type, id,  shift_code_type, created_by);
            }catch(Exception e)
            {
                var message = e.Message;
            }

            return resp;
        }


        [HttpGet("employee_leave_view")]
        public List<EmployeeLeaveResponse> employee_leave_view(
            string series_code, string leave_type_id, string leave_type_code, string leave_name, string gender_to_use, string total_leaves, int tag_type, string id,
             string created_by)
        {

            var resp = _UserManagementService.employee_leave_view(
             series_code, leave_type_id, leave_type_code, leave_name, gender_to_use, total_leaves, tag_type, id,
              created_by);

            return resp;
        }


        [HttpPost("employee_in")]
        public int employee_in(EmployeeInRequest model)
        {
            int resp = 0;
            try
            {

                resp = _UserManagementService.employee_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);
            }

            return resp;
        }


        [HttpPost("employee_information_temp_in")]
        public int employee_information_temp_in(EmployeeInRequest model)
        {
            int resp = 0;
            try
            {

                resp = _UserManagementService.employee_information_temp_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);
            }

            return resp;
        }


        [HttpGet("employee_recurring_view")]
        public List<EmployeeRecurringResponse> employee_recurring_view(string series_code, int adjustment_type_id, int adjustment_id, int timing_id, decimal amount, int tag_type, string id, string created_by)
        {
            List<EmployeeRecurringResponse> resp = new List<EmployeeRecurringResponse>();
            try
            {

                resp = _UserManagementService.employee_recurring_view(series_code, adjustment_type_id, adjustment_id, timing_id, amount, tag_type, id, created_by);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);
            }

            return resp;
        }


        [HttpGet("employee_contribution_view")]
        public List<PayrollContributionResponse> employee_contribution_view(string series_code, int government_type_id, int timing_id, decimal amount, int tag_type, string id, string created_by)
        {
            List<PayrollContributionResponse> resp = new List<PayrollContributionResponse>();
            try
            {

                resp = _UserManagementService.employee_contribution_view( series_code,  government_type_id,  timing_id,  amount,  tag_type,  id,  created_by);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);
            }

            return resp;
        }

        [HttpGet("employee_adjustment_view")]
        public List<PayrollAdjustmentResponse> employee_adjustment_view(string series_code, int adjustment_type_id, string adjustment_name, decimal amount, bool taxable, int tag_type, string id, string created_by)
        {
            List<PayrollAdjustmentResponse> resp = new List<PayrollAdjustmentResponse>();
            try
            {

                resp = _UserManagementService.employee_adjustment_view( series_code,  adjustment_type_id,  adjustment_name,  amount,  taxable,  tag_type,  id,  created_by);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on User Management Service:" + e.Message);
            }

            return resp;
        }

    }
}
