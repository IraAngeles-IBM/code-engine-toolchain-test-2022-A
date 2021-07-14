using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml;
using PayrollManagementService.Helper;
using PayrollManagementService.Model;
using PayrollManagementService.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PayrollManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollManagementController : ControllerBase
    {
        private IPayrollManagementServices _IPayrollManagementServices;

        private EmailSender email;
        private Default_Url url;

        public PayrollManagementController(IPayrollManagementServices PayrollManagementServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _IPayrollManagementServices = PayrollManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }


        #region "Approval and Status"
        [HttpPost("approval_process_in")]
        public ApprovalResponse approval_process_in(ApprovalSequenceRequest model)
        {

            List<ApprovalSequenceResponse> ap_resp = new List<ApprovalSequenceResponse>();
            ApprovalResponse resp = new ApprovalResponse();
            TransactionStatusRequest status_req = new TransactionStatusRequest();
            List<ApprovalSequenceRequest> req = new List<ApprovalSequenceRequest>();
            string url2 = "http://localhost:1006/api/TenantDefaultSetup/transaction_approval_sequence_view?module_id=" + model.module_id + "&employee_id=" + model.created_by + "&approval_level_id=" + model.approval_level_id + "&series_code=" + model.series_code;
            //string url2 = "http://localhost:10006/api/TenantDefaultSetup/transaction_approval_sequence_view?module_id=" + model.module_id +"&approval_level_id=" + model.approval_level_id + "&series_code=" + model.series_code;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url2);
            request.Method = "GET";
            String returnString = String.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                returnString = reader.ReadToEnd();
                ap_resp = JsonConvert.DeserializeObject<List<ApprovalSequenceResponse>>(returnString);
                reader.Close();
                dataStream.Close();
            }

            foreach (var item in ap_resp)
            {


                model.status = item.status;
                model.seqn = item.seqn;
                model.approver_id = item.approver_id;


            }

            req = (from item in ap_resp
                   select new ApprovalSequenceRequest()
                   {
                       module_id = model.module_id,
                       transaction_id = model.transaction_id,
                       status = item.status,
                       seqn = item.seqn,
                       approver_id = item.approver_id,
                       approval_level_id = model.approval_level_id,
                       series_code = model.series_code,
                       created_by = model.created_by

                   }).ToList();


            resp = _IPayrollManagementServices.approval_process_in(req.ToArray());

            status_req.module_id = model.module_id;
            status_req.transaction_id = model.transaction_id;
            status_req.series_code = model.series_code;

            var status_update = _IPayrollManagementServices.transaction_status_up(status_req);
            resp.approved = status_update.approved;
            return resp;
        }



        [HttpPost("transaction_approval_up")]
        public ApprovalResponse transaction_approval_up(ApprovalRequest model)
        {
            string responseInString = "";
            var resp = _IPayrollManagementServices.transaction_approval_up(model);


            if (resp.approved == true)
            {
                if (resp.module_id == 32)
                {
                    using (var wb = new WebClient())
                    {

                        AttendanceLogApprovalRequest req = new AttendanceLogApprovalRequest();
                        string url = "http://localhost:1014/api/AttendanceManagement/employee_schedule_detail_auto";
                        //string url = "http://localhost:48532/api/AttendanceManagement/employee_schedule_detail_auto";

                        req.created_by = model.approved_by;
                        req.series_code = model.series_code;
                        req.transaction_id = model.transaction_id;

                        wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                        string Stringdata = JsonConvert.SerializeObject(req);
                        responseInString = wb.UploadString(url, Stringdata);


                        //string HtmlResult = wb.UploadValues(url, data);

                        //var response = wb.UploadValues(url, "POST", data);
                        //responseInString = Encoding.UTF8.GetString(response);

                    }
                    var id = JsonConvert.DeserializeObject<int>(responseInString);
                }
                else if (resp.module_id == 33)
                {
                    using (var wb = new WebClient())
                    {

                        AttendanceLogApprovalRequest req = new AttendanceLogApprovalRequest();
                        string url = "http://localhost:1014/api/AttendanceManagement/attendance_log_approval_in";
                        //string url = "http://localhost:48532/api/AttendanceManagement/attendance_log_approval_in";

                        req.created_by = model.approved_by;
                        req.series_code = model.series_code;
                        req.transaction_id = model.transaction_id;

                        wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                        string Stringdata = JsonConvert.SerializeObject(req);
                        responseInString = wb.UploadString(url, Stringdata);


                        //string HtmlResult = wb.UploadValues(url, data);

                        //var response = wb.UploadValues(url, "POST", data);
                        //responseInString = Encoding.UTF8.GetString(response);

                    }
                    var id = JsonConvert.DeserializeObject<int>(responseInString);
                }

            }

            return resp;
        }


        #endregion

        #region "Payroll"
        [HttpPost("payroll_header_in")]
        public PayrollHeaderInReponse payroll_header_in(PayrollHeaderRequest model)
        {
            PayrollHeaderInReponse resp = new PayrollHeaderInReponse();

                ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

                SeriesRequest req = new SeriesRequest();
                SeriesResponse res = new SeriesResponse();

                string responseInString = "";
                try
                {
                  


                        using (var wb = new WebClient())
                        {


                            string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                            //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";


                            req.module_id = "26";
                            req.series_code = model.series_code;

                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                        res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                        model.payroll_header_code = res.series_code;






                     resp = _IPayrollManagementServices.payroll_header_in(model);

                //if (resp != 0)
                //{

                //    approval_req.module_id = 34;
                //    approval_req.transaction_id = resp;
                //    approval_req.series_code = model.series_code;
                //    approval_req.approval_level_id = model.approval_level_id;
                //     approval_req.created_by = model.created_by;
                //    var approval = approval_process_in(approval_req);

                //}



            }
                catch (Exception e)
                {
                    var message = "Error: " + e.Message;

                }



            return resp;



        }


        [HttpGet("payroll_header_view_sel")]
        public List<PayrollHeaderResponse> payroll_header_view_sel(string series_code, string payroll_header_id, string created_by)
        {

            var resp = _IPayrollManagementServices.payroll_header_view_sel( series_code,  payroll_header_id,  created_by);

            return resp;
        }


        #endregion

        #region "Payroll Adjustment"

        [HttpPost("payroll_adjustment_in")]
        public int payroll_adjustment_in(PayrollAdjustmentRequest[] model)
        {
            int resp = 0;
            try
            {
                resp = _IPayrollManagementServices.payroll_adjustment_in(model);

            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp = 0;

            }



            return resp;



        }

        [HttpGet("payroll_adjustment_view")]
        public List<PayrollAdjustmentResponse> payroll_adjustment_view(string series_code, string payroll_header_id, string created_by)
        {

            var resp = _IPayrollManagementServices.payroll_adjustment_view( series_code,  payroll_header_id,  created_by);

            return resp;
        }
        #endregion


        #region "Payroll Generation"


        [HttpGet("payroll_generation_view")]
        public List<PayrollGenerationResponse> payroll_generation_view(string series_code, string payroll_header_id, int category_id, int branch_id, int confidential_id, bool include_tax, bool include_sss, bool include_pagibig, bool include_philhealth, string created_by)
        {

            var resp = _IPayrollManagementServices.payroll_generation_view( series_code,  payroll_header_id,  category_id,  branch_id,  confidential_id,  include_tax,  include_sss,  include_pagibig,  include_philhealth,  created_by);

            return resp;
        }


        [HttpPost("payroll_in")]
        public int payroll_in(PayrollRequest model)
        {
            int resp = 0;

            ApprovalSequenceRequest approval_req = new ApprovalSequenceRequest();

            SeriesRequest req = new SeriesRequest();
            SeriesResponse res = new SeriesResponse();

            string responseInString = "";
            try
            {



                using (var wb = new WebClient())
                {


                    string url = "http://localhost:1006/api/TenantDefaultSetup/series_in";
                    //string url = "http://localhost:10006/api/TenantDefaultSetup/series_in";


                    req.module_id = "28";
                    req.series_code = model.series_code;

                    wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string Stringdata = JsonConvert.SerializeObject(req);
                    responseInString = wb.UploadString(url, Stringdata);
                    //string HtmlResult = wb.UploadValues(url, data);

                    //var response = wb.UploadValues(url, "POST", data);
                    //responseInString = Encoding.UTF8.GetString(response);

                }
                res = JsonConvert.DeserializeObject<SeriesResponse>(responseInString);

                model.payroll_code = res.series_code;






                resp = _IPayrollManagementServices.payroll_in(model);

                if (resp != 0)
                {

                    approval_req.module_id = 28;
                    approval_req.transaction_id = resp;
                    approval_req.series_code = model.series_code;
                    approval_req.approval_level_id = model.approval_level_id;
                    approval_req.created_by = model.created_by;
                    var approval = approval_process_in(approval_req);

                }



            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;

            }



            return resp;



        }


        [HttpGet("payroll_view_sel")]
        public List<PayrollResponse> payroll_view_sel(string series_code, string payroll_header_id, int payroll_id, string created_by)
        {

            var resp = _IPayrollManagementServices.payroll_view_sel( series_code,  payroll_header_id,  payroll_id,  created_by);

            return resp;
        }


        [HttpGet("payroll_view")]
        public List<PayrollResponse> payroll_view(string series_code, string payroll_header_id, int payroll_id, string created_by)
        {

            var resp = _IPayrollManagementServices.payroll_view(series_code, payroll_header_id, payroll_id, created_by);

            return resp;
        }


        [HttpGet("payslip_detail_temp_view")]
        public List<PayslipDetaiResponse> payslip_detail_temp_view(string series_code, string payroll_header_id, int employee_id, string created_by)
        {

            var resp = _IPayrollManagementServices.payslip_detail_temp_view( series_code,  payroll_header_id,  employee_id,  created_by);

            return resp;
        }


        [HttpGet("payslip_detail_view")]
        public List<PayslipDetaiResponse> payslip_detail_view(string series_code, string payroll_header_id, int payroll_id, int payslip_id, int employee_id, string created_by)
        {

            var resp = _IPayrollManagementServices.payslip_detail_view( series_code,  payroll_header_id,  payroll_id,  payslip_id,  employee_id,  created_by);

            return resp;
        }
        #endregion


        #region "Payslip"
        [HttpGet("payslip_view")]
        public List<PayrollGenerationResponse> payslip_view(string series_code, string payroll_header_id, int payroll_id, int payslip_id, string created_by)
        {

            var resp = _IPayrollManagementServices.payslip_view(series_code, payroll_header_id, payroll_id, payslip_id, created_by);

            return resp;
        }
        #endregion



        #region "Posted Payslip"



        [HttpPost("posted_payslip_in")]
        public int posted_payslip_in(PostedPayrollRequest[] model)
        {
            int resp = 0;



            try
            {


                resp = _IPayrollManagementServices.posted_payslip_in(model);




            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
                resp = 0;
            }



            return resp;



        }

        [HttpGet("posted_payslip_view")]
        public List<PayrollGenerationResponse> posted_payslip_view(string series_code, string payroll_header_id, int posted_payslip_id, string created_by)
        {

            var resp = _IPayrollManagementServices.posted_payslip_view( series_code,  payroll_header_id,  posted_payslip_id,  created_by);

            return resp;
        }


        [HttpGet("posted_payslip_employee_view")]
        public List<PayrollGenerationResponse> posted_payslip_employee_view(string series_code, string employee_id, string created_by)
        {

            var resp = _IPayrollManagementServices.posted_payslip_employee_view(series_code, employee_id, created_by);

            return resp;
        }

        [HttpGet("posted_payslip_detail_view")]
        public List<PayslipDetaiResponse> posted_payslip_detail_view(string series_code, string payroll_header_id, int posted_payslip_id, int employee_id, string created_by)
        {

            var resp = _IPayrollManagementServices.posted_payslip_detail_view( series_code,  payroll_header_id,  posted_payslip_id,  employee_id,  created_by);

            return resp;
        }



        [HttpGet("posted_payslip_detail_employee_view")]
        public List<PayslipDetaiResponse> posted_payslip_detail_employee_view(string series_code, string payroll_header_id, int posted_payslip_id, int employee_id, string created_by)
        {

            var resp = _IPayrollManagementServices.posted_payslip_detail_employee_view(series_code, payroll_header_id, posted_payslip_id, employee_id, created_by);

            return resp;
        }

        #endregion

        #region"Timekeeping"
        [HttpGet("timekeeping_final_temp_view_sel")]
        public List<TimekeepingGenerationResponse> timekeeping_final_temp_view_sel(string series_code, string payroll_header_id, int employee_id, string created_by)
         {

            var resp = _IPayrollManagementServices.timekeeping_final_temp_view_sel(series_code, payroll_header_id,  employee_id, created_by);

            return resp;
        }

        [HttpGet("timekeeping_final_view_sel")]
        public List<TimekeepingGenerationResponse> timekeeping_final_view_sel(string series_code, string payroll_header_id, int payroll_id, int payslip_id, int employee_id, int timekeeping_header_id, string created_by)
        {

            var resp = _IPayrollManagementServices.timekeeping_final_view_sel( series_code,  payroll_header_id,  payroll_id,  payslip_id,  employee_id,  timekeeping_header_id,  created_by);

            return resp;
        }


        [HttpGet("posted_timekeeping_final_view_sel")]
        public List<TimekeepingGenerationResponse> posted_timekeeping_final_view_sel(string series_code, string payroll_header_id, int poseted_payslip_id, int employee_id, int timekeeping_header_id, string created_by)
        {
             
            var resp = _IPayrollManagementServices.posted_timekeeping_final_view_sel(series_code, payroll_header_id, poseted_payslip_id, employee_id, timekeeping_header_id, created_by);

            return resp;
        }




        [HttpPost("timekeeping_upload_in")]
        public int timekeeping_upload_in(UploadInRequest model)
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


                resp = _IPayrollManagementServices.timekeeping_upload_in(model);


            }
            catch (Exception e)
            {

                Console.WriteLine("Error on Payroll Management Service:" + e.Message);
            }

            return resp;
        }
        #endregion

        #region"Uploader"


        [HttpPost("timekeeping_upload")]
        public int timekeeping_upload(IFormFile formfile, string series_code, string created_by, string payroll_header_id, string date_from, string date_to)
        {
            int resp = 0;
            payroll_header_id = Crypto.url_decrypt(payroll_header_id);
            List<TimekeepingUpload> list = new List<TimekeepingUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _IPayrollManagementServices.timekeeping_upload_del(series_code, payroll_header_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {
                                DateTime datetime = DateTime.Now;


                                list.Add(new TimekeepingUpload()
                                {
                                    payroll_header_id = Convert.ToInt32(payroll_header_id),
                                    employee_code = (worksheet.Cells[row, 1].Value.ToString()),
                                    date_from = date_from,
                                    date_to = date_to,
                                    overtime_hour = Convert.ToDecimal(worksheet.Cells[row, 2].Value.ToString()),
                                    offset_hour = Convert.ToDecimal(worksheet.Cells[row, 3].Value.ToString()),
                                    vl_hour = Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString()),
                                    sl_hour = Convert.ToDecimal(worksheet.Cells[row, 5].Value.ToString()),
                                    otherl_hour = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()),
                                    lwop_hour = Convert.ToDecimal(worksheet.Cells[row, 7].Value.ToString()),
                                    is_absent = Convert.ToDecimal(worksheet.Cells[row, 8].Value.ToString()),
                                    is_present = Convert.ToDecimal(worksheet.Cells[row, 9].Value.ToString()),
                                    late = Convert.ToDecimal(worksheet.Cells[row, 10].Value.ToString()),
                                    undertime = Convert.ToDecimal(worksheet.Cells[row, 11].Value.ToString()),
                                    reg = Convert.ToDecimal(worksheet.Cells[row, 12].Value.ToString()),
                                    regnd = Convert.ToDecimal(worksheet.Cells[row, 13].Value.ToString()),
                                    ot = Convert.ToDecimal(worksheet.Cells[row, 14].Value.ToString()),
                                    ot_e8 = Convert.ToDecimal(worksheet.Cells[row, 15].Value.ToString()),
                                    otnd = Convert.ToDecimal(worksheet.Cells[row, 16].Value.ToString()),
                                    otnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 17].Value.ToString()),
                                    otrd = Convert.ToDecimal(worksheet.Cells[row, 18].Value.ToString()),
                                    otrd_e8 = Convert.ToDecimal(worksheet.Cells[row, 19].Value.ToString()),
                                    otrdnd = Convert.ToDecimal(worksheet.Cells[row, 20].Value.ToString()),
                                    otrdnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 21].Value.ToString()),
                                    lh = Convert.ToDecimal(worksheet.Cells[row, 22].Value.ToString()),
                                    lhot = Convert.ToDecimal(worksheet.Cells[row, 23].Value.ToString()),
                                    lhot_e8 = Convert.ToDecimal(worksheet.Cells[row, 24].Value.ToString()),
                                    lhotnd = Convert.ToDecimal(worksheet.Cells[row, 25].Value.ToString()),
                                    lhotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 26].Value.ToString()),
                                    lhrd = Convert.ToDecimal(worksheet.Cells[row, 27].Value.ToString()),
                                    lhrdot = Convert.ToDecimal(worksheet.Cells[row, 28].Value.ToString()),
                                    lhrdot_e8 = Convert.ToDecimal(worksheet.Cells[row, 29].Value.ToString()),
                                    lhrdotnd = Convert.ToDecimal(worksheet.Cells[row, 30].Value.ToString()),
                                    lhrdotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 31].Value.ToString()),
                                    sh = Convert.ToDecimal(worksheet.Cells[row, 32].Value.ToString()),
                                    shot = Convert.ToDecimal(worksheet.Cells[row, 33].Value.ToString()),
                                    shot_e8 = Convert.ToDecimal(worksheet.Cells[row, 34].Value.ToString()),
                                    shotnd = Convert.ToDecimal(worksheet.Cells[row, 35].Value.ToString()),
                                    shotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 36].Value.ToString()),
                                    shrd = Convert.ToDecimal(worksheet.Cells[row, 37].Value.ToString()),
                                    shrdot = Convert.ToDecimal(worksheet.Cells[row, 38].Value.ToString()),
                                    shrdot_e8 = Convert.ToDecimal(worksheet.Cells[row, 39].Value.ToString()),
                                    shrdotnd = Convert.ToDecimal(worksheet.Cells[row, 40].Value.ToString()),
                                    shrdotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 41].Value.ToString()),
                                    dh = Convert.ToDecimal(worksheet.Cells[row, 42].Value.ToString()),
                                    dhot = Convert.ToDecimal(worksheet.Cells[row, 43].Value.ToString()),
                                    dhot_e8 = Convert.ToDecimal(worksheet.Cells[row, 44].Value.ToString()),
                                    dhotnd = Convert.ToDecimal(worksheet.Cells[row, 45].Value.ToString()),
                                    dhotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 46].Value.ToString()),
                                    dhrd = Convert.ToDecimal(worksheet.Cells[row, 47].Value.ToString()),
                                    dhrdot = Convert.ToDecimal(worksheet.Cells[row, 48].Value.ToString()),
                                    dhrdot_e8 = Convert.ToDecimal(worksheet.Cells[row, 49].Value.ToString()),
                                    dhrdotnd = Convert.ToDecimal(worksheet.Cells[row, 50].Value.ToString()),
                                    dhrdotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 51].Value.ToString()),
                                    created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),
                                    date_created = datetime.ToString(),




                                }) ;
                            }

                        }

                        resp = _IPayrollManagementServices.timekeeping_upload(list, series_code, created_by);

                        //var response =  _IPayrollManagementServices.payroll_generation_view(series_code, payroll_header_id, category_id, branch_id, confidential_id, include_tax, include_sss, include_pagibig, include_philhealth, created_by);
                    }
                }
                catch (Exception e)
                {
                    var a = e.Message;
                    resp = 0;
                }
            }

            return resp;
        }


        #endregion

        #region "Reports"

        [HttpGet("report_1601c")]
        public List<rpt1601cResponse> report_1601c(string series_code, string employee_id, string date_from, string date_to)
        {

            var resp = _IPayrollManagementServices.report_1601c(series_code, employee_id, date_from, date_to);

            return resp;
        }

        #endregion
    }
}
