using DataUploadManagementService.Helper;
using DataUploadManagementService.Model;
using DataUploadManagementService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using DocumentFormat.OpenXml.Spreadsheet;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataUploadManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataUploadManagementController : Controller
    {

        private IDataUploadManagementServices _DataUploadManagementServices;
        private readonly IWebHostEnvironment _environment;

        private EmailSender email;
        private Default_Url url;

        public DataUploadManagementController(IDataUploadManagementServices DataUploadManagementServices, IWebHostEnvironment IWebHostEnvironment, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _environment = IWebHostEnvironment;
            _DataUploadManagementServices = DataUploadManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }

        [HttpPost("UploadImage")]
        public ActionResult UploadImage(IFormFile formfile, string series_code, string created_by,string code, int module_id)
        {
            int ret = 0;
            var resp = "";
            var file = formfile;

            series_code = series_code == "code" ? "code" : Crypto.url_decrypt(series_code);

            if (file.Length > 0)
            {

                foreach (var f in Request.Form.Files)
                {

                    var folder = series_code + "\\" + Crypto.url_decrypt(created_by) + "\\"+ module_id + "\\" + code; ;

                    //if (code != null)
                    //{
                    //    folder = folder + "\\" + code;
                    //}

                    var fileName = f.FileName;
                    var directory = Path.Combine(_environment.WebRootPath, folder);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    var fullPath = Path.Combine(_environment.WebRootPath, folder) + $@"\{fileName}";

                    using (FileStream fs = System.IO.File.Create(fullPath))
                    {
                        f.CopyTo(fs);
                        fs.Flush();
                    }
                    resp = folder + $@"\{fileName}";
                    ret = 1;
                }

            }

            //return resp;

            return Ok(new { response = ret });
        }



        [HttpGet("data_upload_header")]
        public List<DataUploadHeaderResponse> data_upload_header(string series_code, int dropdown_id, string created_by)
        {

            var resp = _DataUploadManagementServices.data_upload_header( series_code, dropdown_id,  created_by);

            return resp;
        }


        [HttpGet("data_upload_view")]
        public JsonResult data_upload_view(string series_code, int dropdown_id, string created_by)
        {

            var resp = _DataUploadManagementServices.data_upload_view( series_code,  dropdown_id,  created_by);
            var result = JsonConvert.SerializeObject(resp);
            JsonResult json = Json(result);
            return json;
        }



        #region "Exports"

        [HttpGet("ExportAttendanceLog")]
        public ActionResult ExportAttendanceLog()
        {
            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("AttendanceExport");



            workSheet.Cells[1, 1].Value = "Biometric ID";
            workSheet.Cells[1, 2].Value = "Date & Time";
            workSheet.Cells[1, 3].Value = "In or Out";
            workSheet.Cells[1, 4].Value = "Terminal ID";
            workSheet.Cells[1, 8].Value = "Legend";



            workSheet.Cells[2, 1].Value = "0001";
            workSheet.Cells[2, 2].Value = "01/01/2021 8:00 AM";
            workSheet.Cells[2, 3].Value = "0";
            workSheet.Cells[2, 4].Value = "1";



            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 8].Style.Font.Bold = true;



            workSheet.Cells[2, 9].Value = "ID";
            workSheet.Cells[2, 10].Value = "Date and Time";
            workSheet.Cells[2, 11].Value = "In or Out";
            workSheet.Cells[2, 12].Value = "Terminal ID";
            workSheet.Cells[2, 9].Style.Font.Bold = true;
            workSheet.Cells[2, 10].Style.Font.Bold = true;
            workSheet.Cells[2, 11].Style.Font.Bold = true;
            workSheet.Cells[2, 12].Style.Font.Bold = true;



            workSheet.Cells[3, 9].Value = "001";
            workSheet.Cells[3, 10].Value = "01/01/2021 8:00 AM";
            workSheet.Cells[3, 11].Value = "0 = In, 1 = Out";
            workSheet.Cells[3, 12].Value = "1";


            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();




            var fileName = "Attendance Log.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;
            //var memoryStream = new MemoryStream();
            //package.SaveAs(memoryStream);



            //memoryStream.Position = 0;
            ////return File(memoryStream, contentType, fileName);


            using (FileStream fs = System.IO.File.Create(fullPath))
            {
                //f.CopyTo(fs);
                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportDropdown")]
        public ActionResult ExportDropdown()
        {
            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Dropdown Type";
            workSheet.Cells[1, 2].Value = "Description";
            workSheet.Cells[1, 4].Value = "Legend";



            workSheet.Cells[2, 1].Value = "1";
            workSheet.Cells[2, 2].Value = "Dropdown Sample";



            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;

            List<DropdownTypeResponse> dropdownfix_resp = new List<DropdownTypeResponse>();

           
            using (var wb = new WebClient())
                {

                string url = "http://localhost:1007/api/TenantMasterSetup/dropdown_type_view_fix?active=1";
                //string url = "http://localhost:60645/api/TenantMasterSetup/dropdown_type_view_fix?active=1";

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Method = "GET";
                    String returnString = String.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        returnString = reader.ReadToEnd();
                        dropdownfix_resp = JsonConvert.DeserializeObject<List<DropdownTypeResponse>>(returnString);
                        reader.Close();
                        dataStream.Close();
                    }

                }

            workSheet.Cells[2, 4].Value = "ID";
            workSheet.Cells[2, 5].Value = "Dropdown";
            workSheet.Cells[2, 4].Style.Font.Bold = true;
            workSheet.Cells[2, 5].Style.Font.Bold = true;

            for (var x = 0; x< dropdownfix_resp.Count; x++)
            {
                var y =  x+3;
                workSheet.Cells[y,4].Value = dropdownfix_resp[x].id;
                workSheet.Cells[y, 5].Value = dropdownfix_resp[x].description;
            }


            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();




            var fileName = "Dropdown Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;
            //var memoryStream = new MemoryStream();
            //package.SaveAs(memoryStream);



            //memoryStream.Position = 0;
            ////return File(memoryStream, contentType, fileName);


            using (FileStream fs = System.IO.File.Create(fullPath))
            {
                //f.CopyTo(fs);
                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportEmployee")]
        public ActionResult ExportEmployee(string series_code)
        {
            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "Salutation ID";
            workSheet.Cells[1, 3].Value = "Display Name";
            workSheet.Cells[1, 4].Value = "First Name";
            workSheet.Cells[1, 5].Value = "Middle Name";
            workSheet.Cells[1, 6].Value = "Last Name";
            workSheet.Cells[1, 7].Value = "Suffix ID";
            workSheet.Cells[1, 8].Value = "Nick Name";
            workSheet.Cells[1, 9].Value = "Gender ID";
            workSheet.Cells[1, 10].Value = "Nationality ID";
            workSheet.Cells[1, 11].Value = "Birthday";
            workSheet.Cells[1, 12].Value = "Birth Place";
            workSheet.Cells[1, 13].Value = "Civil Status ID";
            workSheet.Cells[1, 14].Value = "Height";
            workSheet.Cells[1, 15].Value = "Weight";
            workSheet.Cells[1, 16].Value = "Blood Type ID";
            workSheet.Cells[1, 17].Value = "Religion ID";
            workSheet.Cells[1, 18].Value = "Mobile Number";
            workSheet.Cells[1, 19].Value = "Phone Number";
            workSheet.Cells[1, 20].Value = "Office Number";
            workSheet.Cells[1, 21].Value = "Email Address";
            workSheet.Cells[1, 22].Value = "Personal Email Address";
            workSheet.Cells[1, 23].Value = "Alternate Number";

            workSheet.Cells[1, 24].Value = "Present Unit Floor";
            workSheet.Cells[1, 25].Value = "Present Building";
            workSheet.Cells[1, 26].Value = "Present Street";
            workSheet.Cells[1, 27].Value = "Present Barangay";
            workSheet.Cells[1, 28].Value = "Present Province";
            workSheet.Cells[1, 29].Value = "Present City";
            workSheet.Cells[1, 30].Value = "Present Region";
            workSheet.Cells[1, 31].Value = "Present Country";
            workSheet.Cells[1, 32].Value = "Present Zip Code";

            workSheet.Cells[1, 33].Value = "Permanent Unit Floor";
            workSheet.Cells[1, 34].Value = "Permanent Building";
            workSheet.Cells[1, 35].Value = "Permanent Street";
            workSheet.Cells[1, 36].Value = "Permanent Barangay";
            workSheet.Cells[1, 37].Value = "Permanent Province";
            workSheet.Cells[1, 38].Value = "Permanent City";
            workSheet.Cells[1, 39].Value = "Permanent Region";
            workSheet.Cells[1, 40].Value = "Permanent Country";
            workSheet.Cells[1, 41].Value = "Permanent Zip Code";


            workSheet.Cells[1, 45].Value = "Legend";

            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 6].Style.Font.Bold = true;
            workSheet.Cells[1, 7].Style.Font.Bold = true;
            workSheet.Cells[1, 8].Style.Font.Bold = true;
            workSheet.Cells[1, 9].Style.Font.Bold = true;
            workSheet.Cells[1, 10].Style.Font.Bold = true;
            workSheet.Cells[1, 11].Style.Font.Bold = true;
            workSheet.Cells[1, 12].Style.Font.Bold = true;
            workSheet.Cells[1, 13].Style.Font.Bold = true;
            workSheet.Cells[1, 14].Style.Font.Bold = true;
            workSheet.Cells[1, 15].Style.Font.Bold = true;
            workSheet.Cells[1, 16].Style.Font.Bold = true;
            workSheet.Cells[1, 17].Style.Font.Bold = true;
            workSheet.Cells[1, 18].Style.Font.Bold = true;
            workSheet.Cells[1, 19].Style.Font.Bold = true;
            workSheet.Cells[1, 20].Style.Font.Bold = true;
            workSheet.Cells[1, 21].Style.Font.Bold = true;
            workSheet.Cells[1, 22].Style.Font.Bold = true;
            workSheet.Cells[1, 23].Style.Font.Bold = true;

            workSheet.Cells[1, 24].Style.Font.Bold = true;
            workSheet.Cells[1, 25].Style.Font.Bold = true;
            workSheet.Cells[1, 26].Style.Font.Bold = true;
            workSheet.Cells[1, 27].Style.Font.Bold = true;
            workSheet.Cells[1, 28].Style.Font.Bold = true;
            workSheet.Cells[1, 29].Style.Font.Bold = true;
            workSheet.Cells[1, 30].Style.Font.Bold = true;
            workSheet.Cells[1, 31].Style.Font.Bold = true;
            workSheet.Cells[1, 32].Style.Font.Bold = true;
            workSheet.Cells[1, 33].Style.Font.Bold = true;
            workSheet.Cells[1, 34].Style.Font.Bold = true;
            workSheet.Cells[1, 35].Style.Font.Bold = true;
            workSheet.Cells[1, 36].Style.Font.Bold = true;
            workSheet.Cells[1, 37].Style.Font.Bold = true;
            workSheet.Cells[1, 38].Style.Font.Bold = true;
            workSheet.Cells[1, 39].Style.Font.Bold = true;
            workSheet.Cells[1, 40].Style.Font.Bold = true;
            workSheet.Cells[1, 41].Style.Font.Bold = true;



            workSheet.Cells[1, 45].Style.Font.Bold = true;



            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "0";
            workSheet.Cells[2, 3].Value = "Juan P. Dela Cruz";
            workSheet.Cells[2, 4].Value = "Juan";
            workSheet.Cells[2, 5].Value = "Pablo";
            workSheet.Cells[2, 6].Value = "Dela Cruz";
            workSheet.Cells[2, 7].Value = "0";
            workSheet.Cells[2, 8].Value = "Nick";
            workSheet.Cells[2, 9].Value = "0";
            workSheet.Cells[2, 10].Value = "0";
            workSheet.Cells[2, 11].Value = "01/01/1994";
            workSheet.Cells[2, 12].Value = "NCR";
            workSheet.Cells[2, 13].Value = "0";
            workSheet.Cells[2, 14].Value = "5Feet";
            workSheet.Cells[2, 15].Value = "55kg";
            workSheet.Cells[2, 16].Value = "0";
            workSheet.Cells[2, 17].Value = "0";
            workSheet.Cells[2, 18].Value = "Mobile Number";
            workSheet.Cells[2, 19].Value = "Phone Number";
            workSheet.Cells[2, 20].Value = "Office Number";
            workSheet.Cells[2, 21].Value = "juandelacruz@company.com";
            workSheet.Cells[2, 22].Value = "juandelacruz@gmail.com";
            workSheet.Cells[2, 23].Value = "Alternate Number";
            workSheet.Cells[2, 24].Value = "Sampaloc Manila";
            workSheet.Cells[2, 25].Value = "Taytay, Rizal";


            workSheet.Cells[2, 24].Value = "Unit A";
            workSheet.Cells[2, 25].Value = "Building 1";
            workSheet.Cells[2, 26].Value = "Pogi Street";
            workSheet.Cells[2, 27].Value = "Barangay Dimakita";
            workSheet.Cells[2, 28].Value = "0";
            workSheet.Cells[2, 29].Value = "0";
            workSheet.Cells[2, 30].Value = "0";
            workSheet.Cells[2, 31].Value = "0";
            workSheet.Cells[2, 32].Value = "1001";
                            
            workSheet.Cells[2, 33].Value =  "Unit A";
            workSheet.Cells[2, 34].Value =  "Building 1";
            workSheet.Cells[2, 35].Value =  "Pogi Street";
            workSheet.Cells[2, 36].Value =  "Barangay Dimakita";
            workSheet.Cells[2, 37].Value =  "0";
            workSheet.Cells[2, 38].Value =  "0";
            workSheet.Cells[2, 39].Value =  "0";
            workSheet.Cells[2, 40].Value =  "0";
            workSheet.Cells[2, 41].Value =  "1001";


            List<DropdownResponse> dropdownfix_resp = new List<DropdownResponse>();
            List<DropdownResponse> dropdown_resp = new List<DropdownResponse>();
            List<DropdownResponse> res1 = new List<DropdownResponse>();
            List<DropdownResponse> res2 = new List<DropdownResponse>();
            List<DropdownResponse> res3 = new List<DropdownResponse>();
            List<DropdownResponse> res4 = new List<DropdownResponse>();
            List<DropdownResponse> res5 = new List<DropdownResponse>();
            List<DropdownResponse> res6 = new List<DropdownResponse>();
            List<DropdownResponse> res7 = new List<DropdownResponse>();
            List<DropdownResponse> res8 = new List<DropdownResponse>();
            List<DropdownResponse> res9 = new List<DropdownResponse>();
            List<DropdownResponse> res10 = new List<DropdownResponse>();
            List<DropdownResponse> res11 = new List<DropdownResponse>();


            using (var wb = new WebClient())
            {

                string url = "http://localhost:1007/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=3,9,10,32,61,";
                //string url = "http://localhost:60645/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=3,9,10,32,61,";

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

            using (var wb = new WebClient())
            {

                string url = "http://localhost:1006/api/TenantDefaultSetup/dropdown_view?dropdowntype_id=29,30,31,33,34,35,&series_code=" + series_code;
                //string url = "http://localhost:10006/api/TenantDefaultSetup/dropdown_view?dropdowntype_id=29,30,31,33,34,35,&series_code=" + series_code;

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

            res1 = (from x in dropdown_resp
                    where x.type_id.Equals(29)
                              select new DropdownResponse
                              {
                                  id = x.id,
                                  description = x.description,

                              }).ToList();

            res2 = (from x in dropdown_resp
                    where x.type_id.Equals(30)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();


            res3 = (from x in dropdown_resp
                    where x.type_id.Equals(31)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();


            res4 = (from x in dropdownfix_resp
                    where x.type_id.Equals(32)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();



            res5 = (from x in dropdown_resp
                    where x.type_id.Equals(33)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();



            res6 = (from x in dropdown_resp
                    where x.type_id.Equals(34)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();



            res7 = (from x in dropdown_resp
                    where x.type_id.Equals(35)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();


            res8 = (from x in dropdownfix_resp
                    where x.type_id.Equals(3)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();



            res9 = (from x in dropdownfix_resp
                    where x.type_id.Equals(9)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();

            res10 = (from x in dropdownfix_resp
                    where x.type_id.Equals(10)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();


            res11 = (from x in dropdownfix_resp
                     where x.type_id.Equals(61)
                     select new DropdownResponse
                     {
                         id = x.id,
                         description = x.description,

                     }).ToList();


            var y = 0;
            if(res1.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "Salutation ID";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res1.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res1[x].id;
                    workSheet.Cells[y, 46].Value = res1[x].description;
                }
            }

            if (res2.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "Suffix ID";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res2.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res2[x].id;
                    workSheet.Cells[y, 46].Value = res2[x].description;
                }
            }

            if (res3.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "Gender ID";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res3.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res3[x].id;
                    workSheet.Cells[y, 46].Value = res3[x].description;
                }
            }

          

            if (res5.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "Civil Status ID";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res5.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res5[x].id;
                    workSheet.Cells[y, 46].Value = res5[x].description;
                }
            }

            if (res6.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "Blood Type ID";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res6.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res6[x].id;
                    workSheet.Cells[y, 46].Value = res6[x].description;
                }
            }

            if (res7.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "Religion";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res7.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res7[x].id;
                    workSheet.Cells[y, 46].Value = res7[x].description;
                }
            }


            if (res4.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "Nationality ID";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res4.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res4[x].id;
                    workSheet.Cells[y, 46].Value = res4[x].description;
                }
            }



            if (res8.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "Country ID";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res8.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res8[x].id;
                    workSheet.Cells[y, 46].Value = res8[x].description;
                }
            }


            if (res9.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "City ID";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res9.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res9[x].id;
                    workSheet.Cells[y, 46].Value = res9[x].description;
                }
            }


            if (res10.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "Region ID";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res10.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res10[x].id;
                    workSheet.Cells[y, 46].Value = res10[x].description;
                }
            }


            if (res11.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 45].Value = "Province ID";
                workSheet.Cells[y, 46].Value = "Description";
                workSheet.Cells[y, 45].Style.Font.Bold = true;
                workSheet.Cells[y, 46].Style.Font.Bold = true;

                for (var x = 0; x < res11.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 45].Value = res11[x].id;
                    workSheet.Cells[y, 46].Value = res11[x].description;
                }
            }



            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();




            var fileName = "Employee Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;
            //var memoryStream = new MemoryStream();
            //package.SaveAs(memoryStream);



            //memoryStream.Position = 0;
            ////return File(memoryStream, contentType, fileName);


            using (FileStream fs = System.IO.File.Create(fullPath))
            {
                //f.CopyTo(fs);
                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportEmployeeInformation")]
        public ActionResult ExportEmployeeInformation(string series_code,string created_by)
        {

            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "Bio ID";
            workSheet.Cells[1, 3].Value = "Branch ID";
            workSheet.Cells[1, 4].Value = "Employee Status ID";
            workSheet.Cells[1, 5].Value = "Occupation ID";
            workSheet.Cells[1, 6].Value = "Supervisor Code";
            workSheet.Cells[1, 7].Value = "Department ID";
            workSheet.Cells[1, 8].Value = "Date Hired";
            workSheet.Cells[1, 9].Value = "Date Regularized";
            workSheet.Cells[1, 10].Value = "Cost Center ID";
            workSheet.Cells[1, 11].Value = "Category ID";
            workSheet.Cells[1, 12].Value = "Division ID";
            workSheet.Cells[1, 13].Value = "Payroll Type ID";
            workSheet.Cells[1, 14].Value = "Monthly Rate";
            workSheet.Cells[1, 15].Value = "Semi Monthly Rate";
            workSheet.Cells[1, 16].Value = "factor Rate";
            workSheet.Cells[1, 17].Value = "Daily Rate";
            workSheet.Cells[1, 18].Value = "Hourly Rate";
            workSheet.Cells[1, 19].Value = "Bank ID";
            workSheet.Cells[1, 20].Value = "Bank Account";
            workSheet.Cells[1, 21].Value = "Confidentiality ID";
            workSheet.Cells[1, 22].Value = "SSS #";
            workSheet.Cells[1, 23].Value = "Pagibig #";
            workSheet.Cells[1, 24].Value = "Philhealth #";
            workSheet.Cells[1, 25].Value = "TIN";


            workSheet.Cells[1, 28].Value = "Legend";

            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 6].Style.Font.Bold = true;
            workSheet.Cells[1, 7].Style.Font.Bold = true;
            workSheet.Cells[1, 8].Style.Font.Bold = true;
            workSheet.Cells[1, 9].Style.Font.Bold = true;
            workSheet.Cells[1, 10].Style.Font.Bold = true;
            workSheet.Cells[1, 11].Style.Font.Bold = true;
            workSheet.Cells[1, 12].Style.Font.Bold = true;
            workSheet.Cells[1, 13].Style.Font.Bold = true;
            workSheet.Cells[1, 14].Style.Font.Bold = true;
            workSheet.Cells[1, 15].Style.Font.Bold = true;
            workSheet.Cells[1, 16].Style.Font.Bold = true;
            workSheet.Cells[1, 17].Style.Font.Bold = true;
            workSheet.Cells[1, 18].Style.Font.Bold = true;
            workSheet.Cells[1, 19].Style.Font.Bold = true;
            workSheet.Cells[1, 20].Style.Font.Bold = true;
            workSheet.Cells[1, 21].Style.Font.Bold = true;
            workSheet.Cells[1, 22].Style.Font.Bold = true;
            workSheet.Cells[1, 23].Style.Font.Bold = true;
            workSheet.Cells[1, 24].Style.Font.Bold = true;
            workSheet.Cells[1, 25].Style.Font.Bold = true;
            workSheet.Cells[1, 28].Style.Font.Bold = true;


            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "BIO0001";
            workSheet.Cells[2, 3].Value = "0";
            workSheet.Cells[2, 4].Value = "0";
            workSheet.Cells[2, 5].Value = "0";
            workSheet.Cells[2, 6].Value = "EMP0015";
            workSheet.Cells[2, 7].Value = "0";
            workSheet.Cells[2, 8].Value = "01/01/2021";
            workSheet.Cells[2, 9].Value = "01/01/2021";
            workSheet.Cells[2, 10].Value = "0";
            workSheet.Cells[2, 11].Value = "0";
            workSheet.Cells[2, 12].Value = "0";
            workSheet.Cells[2, 13].Value = "0";
            workSheet.Cells[2, 14].Value = "15000";
            workSheet.Cells[2, 15].Value = "7500";
            workSheet.Cells[2, 16].Value = "315";
            workSheet.Cells[2, 17].Value = "500";
            workSheet.Cells[2, 18].Value = "50";
            workSheet.Cells[2, 19].Value = "0";
            workSheet.Cells[2, 20].Value = "01515121315";
            workSheet.Cells[2, 21].Value = "0";
            workSheet.Cells[2, 22].Value = "01515121315";
            workSheet.Cells[2, 23].Value = "01515121315";
            workSheet.Cells[2, 24].Value = "01515121315";
            workSheet.Cells[2, 25].Value = "01515121315";



            List<DropdownResponse> dropdownfix_resp = new List<DropdownResponse>();
            List<DropdownResponse> dropdown_resp = new List<DropdownResponse>();
            List<CategoryResponse> category_resp = new List<CategoryResponse>();
            List<BranchViewResponse> branch_resp = new List<BranchViewResponse>();
            List<DropdownResponse> res1 = new List<DropdownResponse>();
            List<DropdownResponse> res2 = new List<DropdownResponse>();
            List<DropdownResponse> res3 = new List<DropdownResponse>();
            List<DropdownResponse> res4 = new List<DropdownResponse>();
            List<DropdownResponse> res5 = new List<DropdownResponse>();
            List<DropdownResponse> res6 = new List<DropdownResponse>();
            List<DropdownResponse> res7 = new List<DropdownResponse>();
            List<DropdownResponse> res8 = new List<DropdownResponse>();


            using (var wb = new WebClient())
            {

                string url = "http://localhost:1007/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=36,41,42,";
                //string url = "http://localhost:60645/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=36,41,42,";

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

            using (var wb = new WebClient())
            {

                string url = "http://localhost:1006/api/TenantDefaultSetup/dropdown_view?dropdowntype_id=37,38,39,40,2,&series_code=" + series_code;
                //string url = "http://localhost:10006/api/TenantDefaultSetup/dropdown_view?dropdowntype_id=37,38,39,40,2,&series_code=" + series_code;

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



            using (var wb = new WebClient())
            {

                string url = "http://localhost:1013/api/EmployeeCategoryManagement/employee_category_view?series_code=" + series_code + "&category_id=0&created_by=" + created_by;
                //string url = "http://localhost:50013/api/EmployeeCategoryManagement/employee_category_view?series_code=" + series_code + "&category_id=0&created_by=" + created_by;

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



                using (var wb = new WebClient())
                {

                    string url = "http://localhost:1002/api/BranchManagement/branch_list?series_code=" + series_code + "&created_by=" + created_by;
                    //string url = "http://localhost:10022/api/BranchManagement/branch_list?series_code=" + series_code + "&created_by=" + created_by;

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

                res1 = (from x in dropdownfix_resp
                        where x.type_id.Equals(36)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();

            res2 = (from x in dropdownfix_resp
                    where x.type_id.Equals(41)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();


            res3 = (from x in dropdownfix_resp
                    where x.type_id.Equals(42)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();


            res4 = (from x in dropdown_resp
                    where x.type_id.Equals(37)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();



            res5 = (from x in dropdown_resp
                    where x.type_id.Equals(38)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();



            res6 = (from x in dropdown_resp
                    where x.type_id.Equals(39)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();



            res7 = (from x in dropdown_resp
                    where x.type_id.Equals(40)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();


            res8 = (from x in dropdown_resp
                    where x.type_id.Equals(2)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();



            var y = 0;

            y = y + 2;
            workSheet.Cells[y, 28].Value = "Branch ID";
            workSheet.Cells[y, 29].Value = "Description";
            workSheet.Cells[y, 28].Style.Font.Bold = true;
            workSheet.Cells[y, 29].Style.Font.Bold = true;
            if (branch_resp.Count != 0)
            {

                for (var x = 0; x < branch_resp.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 28].Value = branch_resp[x].branch_id;
                    workSheet.Cells[y, 29].Value = branch_resp[x].branch_name;
                }
            }

            y = y + 2;
            workSheet.Cells[y, 28].Value = "Category ID";
            workSheet.Cells[y, 29].Value = "Description";
            workSheet.Cells[y, 28].Style.Font.Bold = true;
            workSheet.Cells[y, 29].Style.Font.Bold = true;
            if (category_resp.Count != 0)
            {

                for (var x = 0; x < category_resp.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 28].Value = category_resp[x].category_id;
                    workSheet.Cells[y, 29].Value = category_resp[x].category_description;
                }
            }

            y = y + 2;
            workSheet.Cells[y, 28].Value = "Employee Status ID";
            workSheet.Cells[y, 29].Value = "Description";
            workSheet.Cells[y, 28].Style.Font.Bold = true;
            workSheet.Cells[y, 29].Style.Font.Bold = true;
            if (res1.Count != 0)
            {

                for (var x = 0; x < res1.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 28].Value = res1[x].id;
                    workSheet.Cells[y, 29].Value = res1[x].description;
                }
            }

            y = y + 2;
            workSheet.Cells[y, 28].Value = "Payroll Type ID";
            workSheet.Cells[y, 29].Value = "Description";
            workSheet.Cells[y, 28].Style.Font.Bold = true;
            workSheet.Cells[y, 29].Style.Font.Bold = true;

            if (res2.Count != 0)
            {

                for (var x = 0; x < res2.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 28].Value = res2[x].id;
                    workSheet.Cells[y, 29].Value = res2[x].description;
                }
            }

            y = y + 2;
            workSheet.Cells[y, 28].Value = "Confidentiality ID";
            workSheet.Cells[y, 29].Value = "Description";
            workSheet.Cells[y, 28].Style.Font.Bold = true;
            workSheet.Cells[y, 29].Style.Font.Bold = true;

            if (res3.Count != 0)
            {
                for (var x = 0; x < res3.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 28].Value = res3[x].id;
                    workSheet.Cells[y, 29].Value = res3[x].description;
                }
            }


            y = y + 2;
            workSheet.Cells[y, 28].Value = "Occupation ID";
            workSheet.Cells[y, 29].Value = "Description";
            workSheet.Cells[y, 28].Style.Font.Bold = true;
            workSheet.Cells[y, 29].Style.Font.Bold = true;

            if (res4.Count != 0)
            {
                for (var x = 0; x < res4.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 28].Value = res4[x].id;
                    workSheet.Cells[y, 29].Value = res4[x].description;
                }
            }


            y = y + 2;
            workSheet.Cells[y, 28].Value = "Department ID";
            workSheet.Cells[y, 29].Value = "Description";
            workSheet.Cells[y, 28].Style.Font.Bold = true;
            workSheet.Cells[y, 29].Style.Font.Bold = true;

            if (res5.Count != 0)
            {
                for (var x = 0; x < res5.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 28].Value = res5[x].id;
                    workSheet.Cells[y, 29].Value = res5[x].description;
                }
            }

            y = y + 2;
            workSheet.Cells[y, 28].Value = "Cost Center ID";
            workSheet.Cells[y, 29].Value = "Description";
            workSheet.Cells[y, 28].Style.Font.Bold = true;
            workSheet.Cells[y, 29].Style.Font.Bold = true;

            if (res6.Count != 0)
            {
                for (var x = 0; x < res6.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 28].Value = res6[x].id;
                    workSheet.Cells[y, 29].Value = res6[x].description;
                }
            }

            y = y + 2;
            workSheet.Cells[y, 28].Value = "Division ID";
            workSheet.Cells[y, 29].Value = "Description";
            workSheet.Cells[y, 28].Style.Font.Bold = true;
            workSheet.Cells[y, 29].Style.Font.Bold = true;

            if (res7.Count != 0)
            {
                for (var x = 0; x < res7.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 28].Value = res7[x].id;
                    workSheet.Cells[y, 29].Value = res7[x].description;
                }
            }

            y = y + 2;
            workSheet.Cells[y, 28].Value = "Bank ID";
            workSheet.Cells[y, 29].Value = "Description";
            workSheet.Cells[y, 28].Style.Font.Bold = true;
            workSheet.Cells[y, 29].Style.Font.Bold = true;

            if (res8.Count != 0)
            {
                for (var x = 0; x < res7.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 28].Value = res8[x].id;
                    workSheet.Cells[y, 29].Value = res8[x].description;
                }
            }





            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();




            var fileName = "Employee Information Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;
            //var memoryStream = new MemoryStream();
            //package.SaveAs(memoryStream);



            //memoryStream.Position = 0;
            ////return File(memoryStream, contentType, fileName);


            using (FileStream fs = System.IO.File.Create(fullPath))
            {
                //f.CopyTo(fs);
                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportChangelog")]
        public ActionResult ExportChangelog(string series_code, string created_by)
        {

            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "Reason";
            workSheet.Cells[1, 3].Value = "Date";
            workSheet.Cells[1, 4].Value = "Time In";
            workSheet.Cells[1, 5].Value = "Time Out";


            workSheet.Cells[1, 8].Value = "Legend";

            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 8].Style.Font.Bold = true;


            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "No logs";
            workSheet.Cells[2, 3].Value = "01/01/2021";
            workSheet.Cells[2, 4].Value = "01/01/2021 8:00 AM";
            workSheet.Cells[2, 5].Value = "01/01/2021 8:00 PM";




            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();




            var fileName = "Change log Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;
   

            using (FileStream fs = System.IO.File.Create(fullPath))
            {

                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportChangeSchedule")]
        public ActionResult ExportChangeSchedule(string series_code, string created_by)
        {

            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "Shift ID";
            workSheet.Cells[1, 3].Value = "Reason";
            workSheet.Cells[1, 4].Value = "Date From";
            workSheet.Cells[1, 5].Value = "Date To";


            workSheet.Cells[1, 8].Value = "Legend";

            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 8].Style.Font.Bold = true;


            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "0";
            workSheet.Cells[2, 3].Value = "Change Schedule";
            workSheet.Cells[2, 4].Value = "01/01/2021";
            workSheet.Cells[2, 5].Value = "01/01/2021";

            List<ShiftCodeResponse> shift = new List<ShiftCodeResponse>();


            using (var wb = new WebClient())
            {

                string url = "http://localhost:1011/api/ScheduleManagement/shift_code_view?series_code=" + series_code +"&shift_id=0&created_by=" + created_by;
                //string url = "http://localhost:44364/api/ScheduleManagement/shift_code_view?series_code=" + series_code +"&shift_id=0&created_by=" + created_by;

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                String returnString = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    returnString = reader.ReadToEnd();
                    shift = JsonConvert.DeserializeObject<List<ShiftCodeResponse>>(returnString);
                    reader.Close();
                    dataStream.Close();
                }

            }
            var y = 2;
            if (shift.Count != 0)
            {
                for (var x = 0; x < shift.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 8].Value = shift[x].int_shift_id;
                    workSheet.Cells[y, 9].Value = shift[x].shift_name;
                }
            }



            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();




            var fileName = "Change Schedule Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;


            using (FileStream fs = System.IO.File.Create(fullPath))
            {

                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }

        [HttpGet("ExportLeave")]
        public ActionResult ExportLeave(string series_code, string created_by)
        {

            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "Leave Type ID";
            workSheet.Cells[1, 3].Value = "Date From";
            workSheet.Cells[1, 4].Value = "Date To";
            workSheet.Cells[1, 5].Value = "Paid?";
            workSheet.Cells[1, 6].Value = "Half Day?";
            workSheet.Cells[1, 7].Value = "Reason";


            workSheet.Cells[1, 10].Value = "Legend";

            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 6].Style.Font.Bold = true;
            workSheet.Cells[1, 7].Style.Font.Bold = true;

            workSheet.Cells[1, 10].Style.Font.Bold = true;


            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "0";
            workSheet.Cells[2, 3].Value = "01/01/2021";
            workSheet.Cells[2, 4].Value = "01/01/2021";
            workSheet.Cells[2, 5].Value = "YES";
            workSheet.Cells[2, 6].Value = "NO";
            workSheet.Cells[2, 7].Value = "Out of Town";

            List<LeaveTypeResponse> leave = new List<LeaveTypeResponse>();


            using (var wb = new WebClient())
            {

                string url = "http://localhost:1012/api/LeaveManagement/leave_type_view?series_code=" + series_code + "&leave_type_id=0&created_by=" + created_by;
                //string url = "http://localhost:63893/api/LeaveManagement/leave_type_view?series_code=" + series_code + "&leave_type_id=0&created_by=" + created_by;

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                String returnString = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    returnString = reader.ReadToEnd();
                    leave = JsonConvert.DeserializeObject<List<LeaveTypeResponse>>(returnString);
                    reader.Close();
                    dataStream.Close();
                }

            }
            var y = 2;
            if (leave.Count != 0)
            {
                for (var x = 0; x < leave.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 10].Value = leave[x].leave_type_id;
                    workSheet.Cells[y, 11].Value = leave[x].leave_name;
                }
            }



            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();




            var fileName = "Leave Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;


            using (FileStream fs = System.IO.File.Create(fullPath))
            {

                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportOB")]
        public ActionResult ExportOB(string series_code, string created_by)
        {

            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            
            
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "Date From";
            workSheet.Cells[1, 3].Value = "Date To";
            workSheet.Cells[1, 4].Value = "Company To Visit";
            workSheet.Cells[1, 5].Value = "Location";
            workSheet.Cells[1, 6].Value = "Description";



            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 6].Style.Font.Bold = true;

            workSheet.Cells[1, 8].Style.Font.Bold = true;


            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "01/01/2021 8:00 AM";
            workSheet.Cells[2, 3].Value = "01/01/2021 8:00 PM";
            workSheet.Cells[2, 4].Value = "Illimitado Inc.";
            workSheet.Cells[2, 5].Value = "Makati";
            workSheet.Cells[2, 6].Value = "OB Filing";


            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();




            var fileName = "Official Business Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;


            using (FileStream fs = System.IO.File.Create(fullPath))
            {

                package.SaveAs(fs);
                fs.Flush();
            }
           

            return Ok(new { response = filepath });
        }


        [HttpGet("ExportOvertime")]
        public ActionResult ExportOvertime(string series_code, string created_by)
        {

            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "Overtime Type ID";
            workSheet.Cells[1, 3].Value = "Date From";
            workSheet.Cells[1, 4].Value = "Date To";
            workSheet.Cells[1, 5].Value = "With Break";
            workSheet.Cells[1, 6].Value = "Break In";
            workSheet.Cells[1, 7].Value = "Break Out";
            workSheet.Cells[1, 8].Value = "Description";


            workSheet.Cells[1, 10].Value = "Legend";

            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 6].Style.Font.Bold = true;
            workSheet.Cells[1, 7].Style.Font.Bold = true;
            workSheet.Cells[1, 8].Style.Font.Bold = true;

            workSheet.Cells[1, 10].Style.Font.Bold = true;


            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "0";
            workSheet.Cells[2, 3].Value = "01/01/2021 8:00 AM";
            workSheet.Cells[2, 4].Value = "01/01/2021 8:00 PM";
            workSheet.Cells[2, 5].Value = "YES";
            workSheet.Cells[2, 6].Value = "01/01/2021 10:00 AM";
            workSheet.Cells[2, 7].Value = "01/01/2021 10:15 AM";
            workSheet.Cells[2, 8].Value = "Overtime Filing";


            List<DropdownResponse> dropdownfix_resp = new List<DropdownResponse>();





            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();




            var fileName = "Overtime Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;
                

            using (FileStream fs = System.IO.File.Create(fullPath))
            {

                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportOffset")]
        public ActionResult ExportOffset(string series_code, string created_by)
        {

            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "Date";
            workSheet.Cells[1, 3].Value = "Offset Minute";
            workSheet.Cells[1, 4].Value = "Description";



            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;



            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "01/01/2021";
            workSheet.Cells[2, 3].Value = "1";
            workSheet.Cells[2, 4].Value = "Offset Filing";




            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();




            var fileName = "Offset Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;


            using (FileStream fs = System.IO.File.Create(fullPath))
            {

                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportLeaveBalance")]
        public ActionResult ExportLeaveBalance(string series_code, string created_by)
        {

            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "year";
            workSheet.Cells[1, 3].Value = "Leave Type";
            workSheet.Cells[1, 4].Value = "Balance Type";
            workSheet.Cells[1, 5].Value = "Value";

            workSheet.Cells[1, 8].Value = "Legend";


            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 8].Style.Font.Bold = true;



            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "2021";
            workSheet.Cells[2, 3].Value = "1";
            workSheet.Cells[2, 4].Value = "1";
            workSheet.Cells[2, 5].Value = "1";






            List<LeaveTypeResponse> leave = new List<LeaveTypeResponse>();


            using (var wb = new WebClient())
            {

                string url = "http://localhost:1012/api/LeaveManagement/leave_type_view?series_code=" + series_code + "&leave_type_id=0&created_by=" + created_by;
                //string url = "http://localhost:63893/api/LeaveManagement/leave_type_view?series_code=" + series_code + "&leave_type_id=0&created_by=" + created_by;

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                String returnString = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    returnString = reader.ReadToEnd();
                    leave = JsonConvert.DeserializeObject<List<LeaveTypeResponse>>(returnString);
                    reader.Close();
                    dataStream.Close();
                }

            }
            var y = 2;
            if (leave.Count != 0)
            {
                workSheet.Cells[y, 9].Value = "Leave Type ID";
                workSheet.Cells[y, 10].Value = "Description";
                workSheet.Cells[y, 9].Style.Font.Bold = true;
                workSheet.Cells[y, 10].Style.Font.Bold = true;

                for (var x = 0; x < leave.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 9].Value = leave[x].leave_type_id;
                    workSheet.Cells[y, 10].Value = leave[x].leave_name;
                }
            }


            List<DropdownResponse> dropdownfix_resp = new List<DropdownResponse>();


            using (var wb = new WebClient())
            {

                string url = "http://localhost:1007/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=64,";
                //string url = "http://localhost:60645/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=64,";

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

            if (dropdownfix_resp.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 9].Value = "Balance Type ID";
                workSheet.Cells[y, 10].Value = "Description";
                workSheet.Cells[y, 9].Style.Font.Bold = true;
                workSheet.Cells[y, 10].Style.Font.Bold = true;

                for (var x = 0; x < dropdownfix_resp.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 9].Value = dropdownfix_resp[x].id;
                    workSheet.Cells[y, 10].Value = dropdownfix_resp[x].description;
                }
            }


            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();






            var fileName = "Leave Balance Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;


            using (FileStream fs = System.IO.File.Create(fullPath))
            {

                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportTimekeeping")]
        public ActionResult ExportTimekeeping(string series_code, string created_by)
        {

            int company_id = Convert.ToInt32(Crypto.password_decrypt(Request.Cookies["CompanyId"]));
            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value  = "Employee Code";
            workSheet.Cells[1, 2].Value  = "Total Overtime Hour";
            workSheet.Cells[1, 3].Value  = "Offset Hour";
            workSheet.Cells[1, 4].Value  = "VL Hour";
            workSheet.Cells[1, 5].Value  = "SL Hour";
            workSheet.Cells[1, 6].Value  = "Other Leave Hour";
            workSheet.Cells[1, 7].Value  = "LWOP Hour";
            workSheet.Cells[1, 8].Value  = "Absent";
            workSheet.Cells[1, 9].Value  = "Present";
            workSheet.Cells[1, 10].Value = "Late";
            workSheet.Cells[1, 11].Value = "Undertime";
            workSheet.Cells[1, 12].Value = "Regular Hour";
            workSheet.Cells[1, 13].Value = "Regular ND Hour";
            workSheet.Cells[1, 14].Value = "Overtime";
            workSheet.Cells[1, 15].Value = "OT Excess of 8Hrs";
            workSheet.Cells[1, 16].Value = "OT ND Hour";
            workSheet.Cells[1, 17].Value = "OT ND Excess of 8Hrs";
            workSheet.Cells[1, 18].Value = "OT RD";
            workSheet.Cells[1, 19].Value = "OT RD Excess of 8Hrs";
            workSheet.Cells[1, 20].Value = "OT RD ND";
            workSheet.Cells[1, 21].Value = "OT RD ND Excess of 8Hrs";
            workSheet.Cells[1, 22].Value = "Legal Holiday";
            workSheet.Cells[1, 23].Value = "Legal Holiday OT";
            workSheet.Cells[1, 24].Value = "Legal Holiday OT Excess of 8Hrs";
            workSheet.Cells[1, 25].Value = "Legal Holiday OT ND Hour";
            workSheet.Cells[1, 26].Value = "Legal Holiday OT ND Excess of 8Hrs";
            workSheet.Cells[1, 27].Value = "Legal Holiday RD";
            workSheet.Cells[1, 28].Value = "Legal Holiday OT RD";
            workSheet.Cells[1, 29].Value = "Legal Holiday OT RD Excess of 8Hrs";
            workSheet.Cells[1, 30].Value = "Legal Holiday OT RD ND";
            workSheet.Cells[1, 31].Value = "Legal Holiday OT RD ND Excess of 8Hrs";
            workSheet.Cells[1, 32].Value = "Special Holiday";
            workSheet.Cells[1, 33].Value = "Special Holiday OT";
            workSheet.Cells[1, 34].Value = "Special Holiday OT Excess of 8Hrs";
            workSheet.Cells[1, 35].Value = "Special Holiday OT ND Hour";
            workSheet.Cells[1, 36].Value = "Special Holiday OT ND Excess of 8Hrs";
            workSheet.Cells[1, 37].Value = "Special Holiday RD";
            workSheet.Cells[1, 38].Value = "Special Holiday OT RD";
            workSheet.Cells[1, 39].Value = "Special Holiday OT RD Excess of 8Hrs";
            workSheet.Cells[1, 40].Value = "Special Holiday OT RD ND";
            workSheet.Cells[1, 41].Value = "Special Holiday OT RD ND Excess of 8Hrs";
            workSheet.Cells[1, 42].Value = "Double Holiday";
            workSheet.Cells[1, 43].Value = "Double Holiday OT";
            workSheet.Cells[1, 44].Value = "Double Holiday OT Excess of 8Hrs";
            workSheet.Cells[1, 45].Value = "Double Holiday OT ND Hour";
            workSheet.Cells[1, 46].Value = "Double Holiday OT ND Excess of 8Hrs";
            workSheet.Cells[1, 47].Value = "Double Holiday RD";
            workSheet.Cells[1, 48].Value = "Double Holiday OT RD";
            workSheet.Cells[1, 49].Value = "Double Holiday OT RD Excess of 8Hrs";
            workSheet.Cells[1, 50].Value = "Double Holiday OT RD ND";
            workSheet.Cells[1, 51].Value = "Double Holiday OT RD ND Excess of 8Hrs";

            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 6].Style.Font.Bold = true;
            workSheet.Cells[1, 7].Style.Font.Bold = true;
            workSheet.Cells[1, 8].Style.Font.Bold = true;
            workSheet.Cells[1, 9].Style.Font.Bold = true;
            workSheet.Cells[1, 10].Style.Font.Bold = true;
            workSheet.Cells[1, 11].Style.Font.Bold = true;
            workSheet.Cells[1, 12].Style.Font.Bold = true;
            workSheet.Cells[1, 13].Style.Font.Bold = true;
            workSheet.Cells[1, 14].Style.Font.Bold = true;
            workSheet.Cells[1, 15].Style.Font.Bold = true;
            workSheet.Cells[1, 16].Style.Font.Bold = true;
            workSheet.Cells[1, 17].Style.Font.Bold = true;
            workSheet.Cells[1, 18].Style.Font.Bold = true;
            workSheet.Cells[1, 19].Style.Font.Bold = true;
            workSheet.Cells[1, 20].Style.Font.Bold = true;
            workSheet.Cells[1, 21].Style.Font.Bold = true;
            workSheet.Cells[1, 22].Style.Font.Bold = true;
            workSheet.Cells[1, 23].Style.Font.Bold = true;
            workSheet.Cells[1, 24].Style.Font.Bold = true;
            workSheet.Cells[1, 25].Style.Font.Bold = true;
            workSheet.Cells[1, 26].Style.Font.Bold = true;
            workSheet.Cells[1, 27].Style.Font.Bold = true;
            workSheet.Cells[1, 28].Style.Font.Bold = true;
            workSheet.Cells[1, 29].Style.Font.Bold = true;
            workSheet.Cells[1, 30].Style.Font.Bold = true;
            workSheet.Cells[1, 31].Style.Font.Bold = true;
            workSheet.Cells[1, 32].Style.Font.Bold = true;
            workSheet.Cells[1, 33].Style.Font.Bold = true;
            workSheet.Cells[1, 34].Style.Font.Bold = true;
            workSheet.Cells[1, 35].Style.Font.Bold = true;
            workSheet.Cells[1, 36].Style.Font.Bold = true;
            workSheet.Cells[1, 37].Style.Font.Bold = true;
            workSheet.Cells[1, 38].Style.Font.Bold = true;
            workSheet.Cells[1, 39].Style.Font.Bold = true;
            workSheet.Cells[1, 40].Style.Font.Bold = true;
            workSheet.Cells[1, 41].Style.Font.Bold = true;
            workSheet.Cells[1, 42].Style.Font.Bold = true;
            workSheet.Cells[1, 43].Style.Font.Bold = true;
            workSheet.Cells[1, 44].Style.Font.Bold = true;
            workSheet.Cells[1, 45].Style.Font.Bold = true;
            workSheet.Cells[1, 46].Style.Font.Bold = true;
            workSheet.Cells[1, 47].Style.Font.Bold = true;
            workSheet.Cells[1, 48].Style.Font.Bold = true;
            workSheet.Cells[1, 49].Style.Font.Bold = true;
            workSheet.Cells[1, 50].Style.Font.Bold = true;
            workSheet.Cells[1, 51].Style.Font.Bold = true;




            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "1";
            workSheet.Cells[2, 3].Value = "1";
            workSheet.Cells[2, 4].Value = "1";
            workSheet.Cells[2, 5].Value = "1";
            workSheet.Cells[2, 6].Value = "1";
            workSheet.Cells[2, 7].Value = "1";
            workSheet.Cells[2, 8].Value = "1";
            workSheet.Cells[2, 9].Value = "1";
            workSheet.Cells[2, 10].Value = "1";
            workSheet.Cells[2, 11].Value = "1";
            workSheet.Cells[2, 12].Value = "1";
            workSheet.Cells[2, 13].Value = "1";
            workSheet.Cells[2, 14].Value = "1";
            workSheet.Cells[2, 15].Value = "1";
            workSheet.Cells[2, 16].Value = "1";
            workSheet.Cells[2, 17].Value = "1";
            workSheet.Cells[2, 18].Value = "1";
            workSheet.Cells[2, 19].Value = "1";
            workSheet.Cells[2, 20].Value = "1";
            workSheet.Cells[2, 21].Value = "1";
            workSheet.Cells[2, 22].Value = "1";
            workSheet.Cells[2, 23].Value = "1";
            workSheet.Cells[2, 24].Value = "1";
            workSheet.Cells[2, 25].Value = "1";
            workSheet.Cells[2, 26].Value = "1";
            workSheet.Cells[2, 27].Value = "1";
            workSheet.Cells[2, 28].Value = "1";
            workSheet.Cells[2, 29].Value = "1";
            workSheet.Cells[2, 30].Value = "1";
            workSheet.Cells[2, 31].Value = "1";
            workSheet.Cells[2, 32].Value = "1";
            workSheet.Cells[2, 33].Value = "1";
            workSheet.Cells[2, 34].Value = "1";
            workSheet.Cells[2, 35].Value = "1";
            workSheet.Cells[2, 36].Value = "1";
            workSheet.Cells[2, 37].Value = "1";
            workSheet.Cells[2, 38].Value = "1";
            workSheet.Cells[2, 39].Value = "1";
            workSheet.Cells[2, 40].Value = "1";
            workSheet.Cells[2, 41].Value = "1";
            workSheet.Cells[2, 42].Value = "1";
            workSheet.Cells[2, 43].Value = "1";
            workSheet.Cells[2, 44].Value = "1";
            workSheet.Cells[2, 45].Value = "1";
            workSheet.Cells[2, 46].Value = "1";
            workSheet.Cells[2, 47].Value = "1";
            workSheet.Cells[2, 48].Value = "1";
            workSheet.Cells[2, 49].Value = "1";
            workSheet.Cells[2, 50].Value = "1";
            workSheet.Cells[2, 51].Value = "1";



            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();



            var fileName = "Timekeeping Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;


            using (FileStream fs = System.IO.File.Create(fullPath))
            {

                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportLoan")]
        public ActionResult ExportLoan(string series_code, string created_by)
        {

            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "Loan Type";
            workSheet.Cells[1, 3].Value = "Loan Name";
            workSheet.Cells[1, 4].Value = "Total Amount";
            workSheet.Cells[1, 5].Value = "Loan Date";
            workSheet.Cells[1, 6].Value = "Loan Start";
            workSheet.Cells[1, 7].Value = "Terms";
            workSheet.Cells[1, 8].Value = "Loan Timing";

            workSheet.Cells[1, 10].Value = "Legend";


            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 6].Style.Font.Bold = true;
            workSheet.Cells[1, 7].Style.Font.Bold = true;
            workSheet.Cells[1, 8].Style.Font.Bold = true;

            workSheet.Cells[1, 10].Style.Font.Bold = true;



            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "1";
            workSheet.Cells[2, 3].Value = "Loan Deduction";
            workSheet.Cells[2, 4].Value = "1000";
            workSheet.Cells[2, 5].Value = "01-01-21";
            workSheet.Cells[2, 6].Value = "01-01-21";
            workSheet.Cells[2, 7].Value = "10";
            workSheet.Cells[2, 8].Value = "1";



            var y = 2;
          
            List<DropdownResponse> dropdownfix_resp = new List<DropdownResponse>();
            List<DropdownResponse> loan_type = new List<DropdownResponse>();
            List<DropdownResponse> loan_timing = new List<DropdownResponse>();


            using (var wb = new WebClient())
            {

                string url = "http://localhost:1007/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=58,59,";
                //string url = "http://localhost:60645/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=58,59,";

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
            loan_type = (from x in dropdownfix_resp
                           where x.type_id.Equals(58)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();

            loan_timing = (from x in dropdownfix_resp
                         where x.type_id.Equals(59)
                    select new DropdownResponse
                    {
                        id = x.id,
                        description = x.description,

                    }).ToList();

            if (loan_type.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 10].Value = "Loan Type ID";
                workSheet.Cells[y, 11].Value = "Description";
                workSheet.Cells[y, 10].Style.Font.Bold = true;
                workSheet.Cells[y, 11].Style.Font.Bold = true;

                for (var x = 0; x < loan_type.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 10].Value = loan_type[x].id;
                    workSheet.Cells[y, 11].Value = loan_type[x].description;
                }
            }


            if (loan_timing.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 10].Value = "Loan Timing ID";
                workSheet.Cells[y, 11].Value = "Description";
                workSheet.Cells[y, 10].Style.Font.Bold = true;
                workSheet.Cells[y, 11].Style.Font.Bold = true;

                for (var x = 0; x < loan_timing.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 10].Value = loan_timing[x].id;
                    workSheet.Cells[y, 11].Value = loan_timing[x].description;
                }
            }


            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();






            var fileName = "Loan Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;


            using (FileStream fs = System.IO.File.Create(fullPath))
            {

                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        [HttpGet("ExportAD")]
        public ActionResult ExportAD(string series_code, string created_by)
        {

            ExcelPackage package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add("Export");



            workSheet.Cells[1, 1].Value = "Employee Code";
            workSheet.Cells[1, 2].Value = "Adjustment Type";
            workSheet.Cells[1, 3].Value = "Adjustment";
            workSheet.Cells[1, 4].Value = "Timing";
            workSheet.Cells[1, 5].Value = "Amount";
            workSheet.Cells[1, 6].Value = "Taxable";
            workSheet.Cells[1, 7].Value = "Minimum Hr";
            workSheet.Cells[1, 8].Value = "Maximum Hr";

            workSheet.Cells[1, 10].Value = "Legend";


            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 2].Style.Font.Bold = true;
            workSheet.Cells[1, 3].Style.Font.Bold = true;
            workSheet.Cells[1, 4].Style.Font.Bold = true;
            workSheet.Cells[1, 5].Style.Font.Bold = true;
            workSheet.Cells[1, 6].Style.Font.Bold = true;
            workSheet.Cells[1, 7].Style.Font.Bold = true;
            workSheet.Cells[1, 8].Style.Font.Bold = true;

            workSheet.Cells[1, 10].Style.Font.Bold = true;



            workSheet.Cells[2, 1].Value = "EMP0001";
            workSheet.Cells[2, 2].Value = "1";
            workSheet.Cells[2, 3].Value = "1";
            workSheet.Cells[2, 4].Value = "1";
            workSheet.Cells[2, 5].Value = "1000";
            workSheet.Cells[2, 6].Value = "Yes";
            workSheet.Cells[2, 7].Value = "0";
            workSheet.Cells[2, 8].Value = "0";



            var y = 2;

            List<DropdownResponse> dropdownfix_resp = new List<DropdownResponse>();
            List<DropdownResponse> addition_timing = new List<DropdownResponse>();
            List<DropdownResponse> deduction_timing = new List<DropdownResponse>();
            List<DropdownResponse> adjustment_type = new List<DropdownResponse>();
            List<RecurringResponse> rec = new List<RecurringResponse>();

            using (var wb = new WebClient())
            {

                string url = "http://localhost:1007/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=26,56,57,";
                //string url = "http://localhost:60645/api/TenantMasterSetup/dropdown_fix_view?dropdowntype_id=26,56,57,";

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



            using (var wb = new WebClient())
            {

                string url = "http://localhost:1010/api/PayrollSetupManagement/recurring_view_sel?series_code=" + series_code + "&recurring_id=0&created_by=" + created_by;
                //string url = "http://localhost:61355/api/PayrollSetupManagement/recurring_view_sel?series_code=" + series_code + "&recurring_id=0&created_by=" + created_by;

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                String returnString = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    returnString = reader.ReadToEnd();
                    rec = JsonConvert.DeserializeObject<List<RecurringResponse>>(returnString);
                    reader.Close();
                    dataStream.Close();
                }

            }


            addition_timing = (from x in dropdownfix_resp
                           where x.type_id.Equals(56)
                           select new DropdownResponse
                           {
                               id = x.id,
                               description = x.description,

                           }).ToList();

            deduction_timing = (from x in dropdownfix_resp
                               where x.type_id.Equals(57)
                               select new DropdownResponse
                               {
                                   id = x.id,
                                   description = x.description,

                               }).ToList();


            adjustment_type = (from x in dropdownfix_resp
                                where x.type_id.Equals(26)
                                select new DropdownResponse
                                {
                                    id = x.id,
                                    description = x.description,

                                }).ToList();


            if (adjustment_type.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 10].Value = "Adjustment Type ID";
                workSheet.Cells[y, 11].Value = "Description";
                workSheet.Cells[y, 10].Style.Font.Bold = true;
                workSheet.Cells[y, 11].Style.Font.Bold = true;

                for (var x = 0; x < adjustment_type.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 10].Value = adjustment_type[x].id;
                    workSheet.Cells[y, 11].Value = adjustment_type[x].description;
                }
            }

            if (addition_timing.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 10].Value = "Timing ID (Addition)";
                workSheet.Cells[y, 11].Value = "Description";
                workSheet.Cells[y, 10].Style.Font.Bold = true;
                workSheet.Cells[y, 11].Style.Font.Bold = true;

                for (var x = 0; x < addition_timing.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 10].Value = addition_timing[x].id;
                    workSheet.Cells[y, 11].Value = addition_timing[x].description;
                }
            }

            if (deduction_timing.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 10].Value = "Timing ID (Deduction)";
                workSheet.Cells[y, 11].Value = "Description";
                workSheet.Cells[y, 10].Style.Font.Bold = true;
                workSheet.Cells[y, 11].Style.Font.Bold = true;

                for (var x = 0; x < deduction_timing.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 10].Value = deduction_timing[x].id;
                    workSheet.Cells[y, 11].Value = deduction_timing[x].description;
                }
            }

            if (rec.Count != 0)
            {
                y = y + 2;
                workSheet.Cells[y, 10].Value = "Adjustment ID";
                workSheet.Cells[y, 11].Value = "Description";
                workSheet.Cells[y, 10].Style.Font.Bold = true;
                workSheet.Cells[y, 11].Style.Font.Bold = true;




                for (var x = 0; x < rec.Count; x++)
                {
                    y = y + 1;
                    workSheet.Cells[y, 10].Value = rec[x].int_recurring_id;
                    workSheet.Cells[y, 11].Value = rec[x].recurring_name;
                }
            }


            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();






            var fileName = "One Time Adjustment Template.xlsx";
            var customDir = "UploadTemplate\\";
            var directory = Path.Combine(_environment.WebRootPath, customDir);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

            var filepath = customDir + fileName;


            using (FileStream fs = System.IO.File.Create(fullPath))
            {

                package.SaveAs(fs);
                fs.Flush();
            }


            return Ok(new { response = filepath });
        }


        #endregion

        #region "Uploading"

        [HttpPost("attendance_log_temp")]
        public List<AttendanceLog> attendance_log_temp(IFormFile formfile, string series_code, string created_by)
        {
            List<AttendanceLog> resp = new List<AttendanceLog>();

            List<AttendanceLog> list = new List<AttendanceLog>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null && worksheet.Cells[row, 3].Value != null
                                && worksheet.Cells[row, 4].Value != null)
                            {
                                var date_time = "";
                                try
                                {
                                    double d = double.Parse(worksheet.Cells[row, 2].Value.ToString());
                                    date_time = Convert.ToString(DateTime.FromOADate(d));
                                }
                                catch (Exception e)
                                {
                                    date_time = e.Message;
                                }


                                list.Add(new AttendanceLog()
                                {
                                    bio_id = worksheet.Cells[row, 1].Value.ToString(),
                                    date_time = date_time,
                                    in_out = Convert.ToInt32(worksheet.Cells[row, 3].Value.ToString()),
                                    terminal_id = worksheet.Cells[row, 4].Value.ToString(),
                                    created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),

                                });
                            }

                        }

                        resp = _DataUploadManagementServices.attendance_log_temp(list, series_code);


                    }
                }
                catch (Exception e)
                {
                    var a = e.Message;
                }
            }

            return resp;
        }



        [HttpPost("dropdown_upload")]
        public int dropdown_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;

            List<DropdownUpload> list = new List<DropdownUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null )
                            {
                              


                                list.Add(new DropdownUpload()
                                {

                                    dropdown_type_id = Convert.ToInt32(worksheet.Cells[row, 1].Value.ToString()),
                                    dropdown_description = worksheet.Cells[row, 2].Value.ToString(),
                                    created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),



                                });
                            }

                        }

                        resp = _DataUploadManagementServices.dropdown_upload(list, series_code,dropdown_id,created_by);


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


        [HttpPost("employee_upload")]
        public JsonResult employee_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            EmployeeUploadResponse resp = new EmployeeUploadResponse();

            List<EmployeeUploadresult> upload_result = new List<EmployeeUploadresult>();
            resp.id = 0;

            resp.detail = upload_result.ToArray();

            List<EmployeeUpload> list = new List<EmployeeUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                var userhash                = "";
                var employee_code           = "";
                var user_name               = "";
                var user_hash               = "";
                int salutation_id           = 0;
                var display_name            = "";
                var first_name              = "";
                var middle_name             = "";
                var last_name               = "";
                int suffix_id               = 0;
                var nick_name               = "";
                int gender_id               = 0;
                int nationality_id          = 0;
                var bday                    = "";
                var birth_place             = "";
                int civil_status_id         = 0;
                var height                  = "";
                var weight                  = "";
                int blood_type_id           = 0;
                int religion_id             = 0;
                var mobile                  = "";
                var phone                   = "";
                var office                  = "";
                var email_address           = "";
                var personal_email_address  = "";
                var alternate_number        = "";
                var pre_unit_floor          = "";
                var pre_building            = "";
                var pre_street              = "";
                var pre_barangay            = "";
                int pre_province            = 0;
                int pre_city                = 0;
                int pre_region              = 0;
                int pre_country             = 0;
                var pre_zip_code            = "";
                var per_unit_floor          = "";
                var per_building            = "";
                var per_street              = "";
                var per_barangay            = "";
                int per_province            = 0;
                int per_city                = 0;
                int per_region              = 0;
                int per_country             = 0;
                var per_zip_code            = "";

                var salutation                  = "";
                var suffix                      = "";
                var gender                      = "";
                var nationality                 = "";
                var civil_status                = "";
                var blood_type                  = "";
                var religion                    = "";
                bool  with_error                 = false;

                int row_count = 0;
                string column_name = "";
                try
                {  


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            row_count = row;
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {
                                try
                                {
                                    DateTime birthday = Convert.ToDateTime((worksheet.Cells[row, 11].Value.ToString()));
                                    userhash = birthday.ToString("MMddyy");
                                    

                                }
                                catch(Exception e)
                                {
                                    bday = e.Message;
                                }



                                try
                                {
                                    employee_code = (worksheet.Cells[row, 1].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 1].Value.ToString());
                                    user_name = (worksheet.Cells[row, 1].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 1].Value.ToString());


                                }
                                catch (Exception e)
                                {

                                }

                                try
                                {
                                    userhash = (worksheet.Cells[row, 6].Value.ToString()) + "@" + userhash;
                                    userhash = Crypto.password_encrypt(userhash);
                                    bday = (worksheet.Cells[row, 11].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 11].Value.ToString());
                                    userhash = Crypto.password_encrypt(userhash);
                                    bday = (worksheet.Cells[row, 11].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 11].Value.ToString());

                                    user_hash = userhash;
                                }
                                catch (Exception e)
                                {

                                }
                                
                                try
                                {
                                    salutation_id = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString() == null ? 0 : (worksheet.Cells[row, 2].Value.ToString()));

                                }
                                catch (Exception e)
                                {
                                    salutation = e.Message;

                                }

                                try
                                {
                                    display_name = (worksheet.Cells[row, 3].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 3].Value.ToString());

                                }
                                catch (Exception e)
                                {
                                    display_name = e.Message;

                                }


                                try
                                {
                                    display_name = (worksheet.Cells[row, 3].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 3].Value.ToString());

                                }
                                catch (Exception e)
                                {
                                    display_name = e.Message;

                                }

                                first_name              = (worksheet.Cells[row, 4].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 4].Value.ToString());
                                middle_name             = (worksheet.Cells[row, 5].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 5].Value.ToString());
                                last_name               = (worksheet.Cells[row, 6].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 6].Value.ToString());
                                suffix_id               = Convert.ToInt32(worksheet.Cells[row, 7].Value.ToString() == null ? 0 : (worksheet.Cells[row, 7].Value.ToString()));
                                nick_name               = (worksheet.Cells[row, 8].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 8].Value.ToString());
                                gender_id               = Convert.ToInt32(worksheet.Cells[row, 9].Value.ToString() == null ? 0 : (worksheet.Cells[row, 9].Value.ToString()));
                                nationality_id          = Convert.ToInt32(worksheet.Cells[row, 10].Value.ToString() == null ? 0 : (worksheet.Cells[row, 10].Value.ToString()));
                                birth_place             = (worksheet.Cells[row, 12].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 12].Value.ToString());
                                civil_status_id         = Convert.ToInt32(worksheet.Cells[row, 13].Value.ToString() == null ? 0 : (worksheet.Cells[row, 13].Value.ToString()));
                                height                  = (worksheet.Cells[row, 14].Value.ToString() == null ? "-" : (worksheet.Cells[row, 14].Value.ToString()));
                                weight                  = (worksheet.Cells[row, 15].Value.ToString() == null ? "-" : (worksheet.Cells[row, 15].Value.ToString()));
                                blood_type_id           = Convert.ToInt32(worksheet.Cells[row, 16].Value.ToString() == null ? 0 : (worksheet.Cells[row, 16].Value.ToString()));
                                religion_id             = Convert.ToInt32(worksheet.Cells[row, 17].Value.ToString() == null ? 0 : (worksheet.Cells[row, 17].Value.ToString()));
                                mobile                  = (worksheet.Cells[row, 18].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 18].Value.ToString());
                                phone                   = (worksheet.Cells[row, 19].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 19].Value.ToString());
                                office                  = (worksheet.Cells[row, 20].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 20].Value.ToString());
                                email_address           = (worksheet.Cells[row, 21].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 21].Value.ToString());
                                personal_email_address  = (worksheet.Cells[row, 22].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 22].Value.ToString());
                                alternate_number        = (worksheet.Cells[row, 23].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 23].Value.ToString());
                                pre_building            = (worksheet.Cells[row, 25].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 24].Value.ToString());
                                pre_street              = (worksheet.Cells[row, 26].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 25].Value.ToString());
                                pre_unit_floor          = (worksheet.Cells[row, 24].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 26].Value.ToString());
                                pre_barangay            = (worksheet.Cells[row, 27].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 27].Value.ToString());
                                pre_province            = Convert.ToInt32(worksheet.Cells[row, 28].Value.ToString() == null ? 0 : (worksheet.Cells[row, 28].Value.ToString()));
                                pre_city                = Convert.ToInt32(worksheet.Cells[row, 29].Value.ToString() == null ? 0 : (worksheet.Cells[row, 29].Value.ToString()));
                                pre_region              = Convert.ToInt32(worksheet.Cells[row, 30].Value.ToString() == null ? 0 : (worksheet.Cells[row, 30].Value.ToString()));
                                pre_country             = Convert.ToInt32(worksheet.Cells[row, 31].Value.ToString() == null ? 0 : (worksheet.Cells[row, 31].Value.ToString()));
                                pre_zip_code            = (worksheet.Cells[row, 32].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 32].Value.ToString());
                                per_unit_floor          = (worksheet.Cells[row, 33].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 33].Value.ToString());
                                per_building            = (worksheet.Cells[row, 34].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 34].Value.ToString());
                                per_street              = (worksheet.Cells[row, 35].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 35].Value.ToString());
                                per_barangay            = (worksheet.Cells[row, 36].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 36].Value.ToString());
                                per_province            = Convert.ToInt32(worksheet.Cells[row, 37].Value.ToString() == null ? 0 : (worksheet.Cells[row, 37].Value.ToString()));
                                per_city                = Convert.ToInt32(worksheet.Cells[row, 38].Value.ToString() == null ? 0 : (worksheet.Cells[row, 38].Value.ToString()));
                                per_region              = Convert.ToInt32(worksheet.Cells[row, 39].Value.ToString() == null ? 0 : (worksheet.Cells[row, 39].Value.ToString()));
                                per_country             = Convert.ToInt32(worksheet.Cells[row, 40].Value.ToString() == null ? 0 : (worksheet.Cells[row, 40].Value.ToString()));
                                per_zip_code            = (worksheet.Cells[row, 41].Value.ToString()) == null ? "-" : (worksheet.Cells[row, 41].Value.ToString());
                                
                                
                                
                                list.Add(new EmployeeUpload()
                                {
                                    
                                    employee_code = (worksheet.Cells[row, 1].Value.ToString()),
                                    user_name = (worksheet.Cells[row, 1].Value.ToString()),
                                    user_hash = userhash,
                                    salutation_id = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString()),
                                    display_name = (worksheet.Cells[row, 3].Value.ToString()),
                                    first_name = (worksheet.Cells[row, 4].Value.ToString()),
                                    middle_name = (worksheet.Cells[row, 5].Value.ToString()),
                                    last_name = (worksheet.Cells[row, 6].Value.ToString()),
                                    suffix_id = Convert.ToInt32(worksheet.Cells[row, 7].Value.ToString()),
                                    nick_name = (worksheet.Cells[row, 8].Value.ToString()),
                                    gender_id = Convert.ToInt32(worksheet.Cells[row, 9].Value.ToString()),
                                    nationality_id = Convert.ToInt32(worksheet.Cells[row, 10].Value.ToString()),
                                    birthday = (worksheet.Cells[row, 11].Value.ToString()),
                                    birth_place = (worksheet.Cells[row, 12].Value.ToString()),
                                    civil_status_id = Convert.ToInt32(worksheet.Cells[row, 13].Value.ToString()),
                                    height = (worksheet.Cells[row, 14].Value.ToString()),
                                    weight = (worksheet.Cells[row, 15].Value.ToString()),
                                    blood_type_id = Convert.ToInt32(worksheet.Cells[row, 16].Value.ToString()),
                                    religion_id = Convert.ToInt32(worksheet.Cells[row, 17].Value.ToString()),
                                    mobile = (worksheet.Cells[row, 18].Value.ToString()),
                                    phone = (worksheet.Cells[row, 19].Value.ToString()),
                                    office = (worksheet.Cells[row, 20].Value.ToString()),
                                    email_address = (worksheet.Cells[row, 21].Value.ToString()),
                                    personal_email_address = (worksheet.Cells[row, 22].Value.ToString()),
                                    alternate_number = (worksheet.Cells[row, 23].Value.ToString()),
                                    pre_unit_floor  = (worksheet.Cells[row, 24].Value.ToString()),
                                    pre_building    = (worksheet.Cells[row, 25].Value.ToString()),
                                    pre_street      = (worksheet.Cells[row, 26].Value.ToString()),
                                    pre_barangay    = (worksheet.Cells[row, 27].Value.ToString()),
                                    pre_province    = Convert.ToInt32(worksheet.Cells[row, 28].Value.ToString()),
                                    pre_city        = Convert.ToInt32(worksheet.Cells[row, 29].Value.ToString()),
                                    pre_region      = Convert.ToInt32(worksheet.Cells[row, 30].Value.ToString()),
                                    pre_country     = Convert.ToInt32(worksheet.Cells[row, 31].Value.ToString()),
                                    pre_zip_code    = (worksheet.Cells[row, 32].Value.ToString()),

                                    per_unit_floor  = (worksheet.Cells[row, 33].Value.ToString()),
                                    per_building    = (worksheet.Cells[row, 34].Value.ToString()),
                                    per_street      = (worksheet.Cells[row, 35].Value.ToString()),
                                    per_barangay    = (worksheet.Cells[row, 36].Value.ToString()),
                                    per_province    = Convert.ToInt32(worksheet.Cells[row, 37].Value.ToString()),
                                    per_city        = Convert.ToInt32(worksheet.Cells[row, 38].Value.ToString()),
                                    per_region      = Convert.ToInt32(worksheet.Cells[row, 39].Value.ToString()),
                                    per_country     = Convert.ToInt32(worksheet.Cells[row, 40].Value.ToString()),
                                    per_zip_code    = (worksheet.Cells[row, 41].Value.ToString()),
                                    //present_address = (worksheet.Cells[row, 24].Value.ToString()),
                                    //permanent_address = (worksheet.Cells[row, 25].Value.ToString()),
                                    created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),



                                });
                            }

                        }

                        int response = _DataUploadManagementServices.employee_upload(list, series_code, dropdown_id, created_by);


                    }
                }
                catch (Exception e)
                {
                    var a =  e.Message;
                    resp.id = 0;
                    
                }
            }

            var result = JsonConvert.SerializeObject(resp);
            JsonResult json = Json(result);

            return json;
        }



        [HttpPost("employee_information_upload")]
        public int employee_information_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;

            List<EmployeeInformationUpload> list = new List<EmployeeInformationUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {



                                list.Add(new EmployeeInformationUpload()
                                {
                                    employee_code       = (worksheet.Cells[row, 1].Value.ToString()),
                                    bio_id              = (worksheet.Cells[row, 2].Value.ToString()),
                                    branch_id           = Convert.ToInt32(worksheet.Cells[row, 3].Value.ToString()),
                                    employee_status_id  = Convert.ToInt32(worksheet.Cells[row, 4].Value.ToString()),
                                    occupation_id       = Convert.ToInt32(worksheet.Cells[row, 5].Value.ToString()),
                                    supervisor_code     = (worksheet.Cells[row, 6].Value.ToString()),
                                    department_id       = Convert.ToInt32(worksheet.Cells[row, 7].Value.ToString()),
                                    date_hired          = (worksheet.Cells[row, 8].Value.ToString()),
                                    date_regularized    = (worksheet.Cells[row, 9].Value.ToString()),
                                    cost_center_id      = Convert.ToInt32(worksheet.Cells[row, 10].Value.ToString()),
                                    category_id         = Convert.ToInt32(worksheet.Cells[row, 11].Value.ToString()),
                                    division_id         = Convert.ToInt32(worksheet.Cells[row, 12].Value.ToString()),
                                    payroll_type_id     = Convert.ToInt32(worksheet.Cells[row, 13].Value.ToString()),
                                    monthly_rate        = Convert.ToDecimal(worksheet.Cells[row, 14].Value.ToString()),
                                    semi_monthly_rate   = Convert.ToDecimal(worksheet.Cells[row, 15].Value.ToString()),
                                    factor_rate         = Convert.ToDecimal(worksheet.Cells[row, 16].Value.ToString()),
                                    daily_rate          = Convert.ToDecimal(worksheet.Cells[row, 17].Value.ToString()),
                                    hourly_rate         = Convert.ToDecimal(worksheet.Cells[row, 18].Value.ToString()),
                                    bank_id             = Convert.ToInt32(worksheet.Cells[row, 19].Value.ToString()),
                                    bank_account        = (worksheet.Cells[row, 20].Value.ToString()),
                                    confidentiality_id  = Convert.ToInt32(worksheet.Cells[row, 21].Value.ToString()),
                                    sss                 = (worksheet.Cells[row, 22].Value.ToString()),
                                    pagibig             = (worksheet.Cells[row, 23].Value.ToString()),
                                    philhealth          = (worksheet.Cells[row, 24].Value.ToString()),
                                    tin                 = (worksheet.Cells[row, 25].Value.ToString()),
                                    created_by          = Convert.ToInt32(Crypto.url_decrypt(created_by)),
                                  



                                });
                            }

                        }

                        resp = _DataUploadManagementServices.employee_information_upload(list, series_code, dropdown_id, created_by);


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


        [HttpPost("changelog_upload")]
        public int changelog_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;

            List<ChangelogUpload> list = new List<ChangelogUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {



                                list.Add(new ChangelogUpload()
                                {
                                    employee_code   = (worksheet.Cells[row, 1].Value.ToString()),
                                    reason          = (worksheet.Cells[row, 2].Value.ToString()),
                                    date            = (worksheet.Cells[row, 3].Value.ToString()),
                                    time_in         = (worksheet.Cells[row, 4].Value.ToString()),
                                    time_out        = (worksheet.Cells[row, 5].Value.ToString()),
                                    created_by      = Convert.ToInt32(Crypto.url_decrypt(created_by)),




                                });
                            }

                        }

                        resp = _DataUploadManagementServices.changelog_upload(list, series_code, dropdown_id, created_by);


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


        [HttpPost("changeschedule_upload")]
        public int changeschedule_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;

            List<ChangeScheduleUpload> list = new List<ChangeScheduleUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {



                                list.Add(new ChangeScheduleUpload()
                                {
                                    employee_code = (worksheet.Cells[row, 1].Value.ToString()),
                                    shift_id      = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString()),
                                    reason        = (worksheet.Cells[row, 3].Value.ToString()),
                                    date_from     = (worksheet.Cells[row, 4].Value.ToString()),
                                    date_to       = (worksheet.Cells[row, 5].Value.ToString()),
                                    created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),




                                });
                            }

                        }

                        resp = _DataUploadManagementServices.changeschedule_upload(list, series_code, dropdown_id, created_by);


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


        [HttpPost("leave_upload")]
        public int leave_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;

            List<LeaveUpload> list = new List<LeaveUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {
                                bool is_paid = true;
                                bool is_half_day = false;
                                try
                                {
                                    if (worksheet.Cells[row, 5].Value.ToString().ToUpper() == "YES")
                                    {
                                        is_paid = true;
                                    } else if (worksheet.Cells[row, 5].Value.ToString().ToUpper() == "NO")
                                    {

                                        is_paid = false;
                                    }


                                    if (worksheet.Cells[row, 6].Value.ToString().ToUpper() == "YES")
                                    {
                                        is_half_day = true;
                                    }
                                    else if (worksheet.Cells[row, 6].Value.ToString().ToUpper() == "NO")
                                    {

                                        is_half_day = false;
                                    }
                                }
                                catch(Exception e)
                                {

                                }



                                list.Add(new LeaveUpload()
                                {
                                    employee_code = (worksheet.Cells[row, 1].Value.ToString()),
                                    leave_type_id = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString()),
                                    date_from     = (worksheet.Cells[row, 3].Value.ToString()),
                                    date_to       = (worksheet.Cells[row, 4].Value.ToString()),
                                    is_paid       = is_paid,
                                    is_half_day   = is_half_day,
                                    description   = (worksheet.Cells[row, 7].Value.ToString()),

                                    created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),




                                });
                            }

                        }

                        resp = _DataUploadManagementServices.leave_upload(list, series_code, dropdown_id, created_by);


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


        [HttpPost("leave_balance_upload")]
        public int leave_balance_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;

            List<LeaveBalanceUpload> list = new List<LeaveBalanceUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {

                                list.Add(new LeaveBalanceUpload()
                                {
                                    employee_code = (worksheet.Cells[row, 1].Value.ToString()),
                                    year = (worksheet.Cells[row, 2].Value.ToString()),
                                    leave_type_id = Convert.ToInt32(worksheet.Cells[row, 3].Value.ToString()),
                                    balance_type = Convert.ToInt32(worksheet.Cells[row, 4].Value.ToString()),
                                    amount = Convert.ToDecimal(worksheet.Cells[row, 5].Value.ToString()),
                                    created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),




                                });
                            }

                        }

                        resp = _DataUploadManagementServices.leave_balance_upload(list, series_code, dropdown_id, created_by);


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

        [HttpPost("ob_upload")]
        public int ob_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;

            List<OBUpload> list = new List<OBUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {



                                list.Add(new OBUpload()
                                {
                                    employee_code       = (worksheet.Cells[row, 1].Value.ToString()),
                                    date_from           = (worksheet.Cells[row, 2].Value.ToString()),
                                    date_to             = (worksheet.Cells[row, 3].Value.ToString()),
                                    company_to_visit    = (worksheet.Cells[row, 4].Value.ToString()),
                                    location            = (worksheet.Cells[row, 5].Value.ToString()),
                                    description         = (worksheet.Cells[row, 6].Value.ToString()),
                                    created_by          = Convert.ToInt32(Crypto.url_decrypt(created_by)),




                                });
                            }

                        }

                        resp = _DataUploadManagementServices.ob_upload(list, series_code, dropdown_id, created_by);


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




        [HttpPost("overtime_upload")]
        public int overtime_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;

            List<OvertimeUpload> list = new List<OvertimeUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {
                                bool with_break = true;
                                try
                                {
                                    if (worksheet.Cells[row, 5].Value.ToString().ToUpper() == "YES")
                                    {
                                        with_break = true;
                                    }
                                    else if (worksheet.Cells[row, 5].Value.ToString().ToUpper() == "NO")
                                    {

                                        with_break = false;
                                    }


                                }
                                catch (Exception e)
                                {

                                }


                                list.Add(new OvertimeUpload()
                                {
                                    employee_code        = (worksheet.Cells[row, 1].Value.ToString()),
                                    overtime_type_id     = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString()),
                                    date_from            = (worksheet.Cells[row, 3].Value.ToString()),
                                    date_to              = (worksheet.Cells[row, 4].Value.ToString()),
                                    with_break           = with_break,
                                    break_in             = (worksheet.Cells[row, 6].Value.ToString()),
                                    break_out            = (worksheet.Cells[row, 7].Value.ToString()),
                                    description          = (worksheet.Cells[row, 8].Value.ToString()),
                                    created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),




                                });
                            }

                        }

                        resp = _DataUploadManagementServices.overtime_upload(list, series_code, dropdown_id, created_by);


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



        [HttpPost("offset_upload")]
        public int offset_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;

            List<OffsetUpload> list = new List<OffsetUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {



                                list.Add(new OffsetUpload()
                                {
                                    employee_code    = (worksheet.Cells[row, 1].Value.ToString()),
                                    date             = (worksheet.Cells[row, 2].Value.ToString()),
                                    offset_hour      = Convert.ToDecimal(worksheet.Cells[row, 3].Value.ToString()),
                                    reason           = (worksheet.Cells[row, 4].Value.ToString()),
                                    
                                    created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),




                                });
                            }

                        }

                        resp = _DataUploadManagementServices.offset_upload(list, series_code, dropdown_id, created_by);


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



        [HttpPost("loan_upload")]
        public int loan_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            List<LoanUpload> list = new List<LoanUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {



                                list.Add(new LoanUpload()
                                {

                                    employee_code   = (worksheet.Cells[row, 1].Value.ToString()),
                                    loan_type_id    = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString()),
                                    loan_name       = (worksheet.Cells[row, 3].Value.ToString()),
                                    total_amount    = Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString()),
                                    loan_date       = (worksheet.Cells[row, 5].Value.ToString()),
                                    loan_start      = (worksheet.Cells[row, 6].Value.ToString()),
                                    terms           = Convert.ToInt32(worksheet.Cells[row, 7].Value.ToString()),
                                    loan_timing_id  = Convert.ToInt32(worksheet.Cells[row, 8].Value.ToString()),

                                    created_by          = Convert.ToInt32(Crypto.url_decrypt(created_by)),




                                });
                            }

                        }

                        resp = _DataUploadManagementServices.loan_upload(list, series_code, dropdown_id, created_by);


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


        [HttpPost("ad_upload")]
        public int ad_upload(IFormFile formfile, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            List<ADUpload> list = new List<ADUpload>();
            var file = formfile;
            using (var stream = new MemoryStream())
            {
                formfile.CopyToAsync(stream);
                try
                {


                    var del_resp = _DataUploadManagementServices.data_upload_del(series_code, dropdown_id, created_by);


                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                            {
                                bool tax = false;
                                try
                                {
                                    if (worksheet.Cells[row, 6].Value.ToString().ToUpper() == "YES")
                                    {
                                        tax = true;
                                    }
                                    else if (worksheet.Cells[row, 6].Value.ToString().ToUpper() == "NO")
                                    {

                                        tax = false;
                                    }
                                }catch(Exception e)
                                {

                                }

                                list.Add(new ADUpload()
                                {
                                    employee_code       = (worksheet.Cells[row, 1].Value.ToString()),
                                    adjustment_type_id  = Convert.ToInt32(worksheet.Cells[row, 2].Value.ToString()),
                                    adjustment_id       = Convert.ToInt32(worksheet.Cells[row, 3].Value.ToString()),
                                    timing_id           = Convert.ToInt32(worksheet.Cells[row, 4].Value.ToString()),
                                    amount              = Convert.ToDecimal(worksheet.Cells[row, 5].Value.ToString()),
                                    taxable             = tax,
                                    minumum_hour        = Convert.ToInt32(worksheet.Cells[row, 7].Value.ToString()),
                                    maximum_hour        = Convert.ToInt32(worksheet.Cells[row, 8].Value.ToString()),

                                    created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),




                                });;
                            }

                        }

                        resp = _DataUploadManagementServices.ad_upload(list, series_code, dropdown_id, created_by);


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


        //#region"Uploader"


        //[HttpPost("timekeeping_upload")]
        //public int timekeeping_upload(IFormFile formfile, string series_code, string created_by, string payroll_header_id, string date_from, string date_to)
        //{
        //    int resp = 0;
        //    payroll_header_id = Crypto.url_decrypt(payroll_header_id);
        //    List<TimekeepingUpload> list = new List<TimekeepingUpload>();
        //    var file = formfile;
        //    using (var stream = new MemoryStream())
        //    {
        //        formfile.CopyToAsync(stream);
        //        try
        //        {


        //            var del_resp = _DataUploadManagementServices.timekeeping_upload_del(series_code, payroll_header_id, created_by);


        //            using (var package = new ExcelPackage(stream))
        //            {
        //                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //                var rowCount = worksheet.Dimension.Rows;

        //                for (int row = 2; row <= rowCount; row++)
        //                {
        //                    if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
        //                    {
        //                        DateTime datetime = DateTime.Now;


        //                        list.Add(new TimekeepingUpload()
        //                        {
        //                            payroll_header_id = Convert.ToInt32(payroll_header_id),
        //                            employee_code = (worksheet.Cells[row, 1].Value.ToString()),
        //                            date_from = date_from,
        //                            date_to = date_to,
        //                            overtime_hour = Convert.ToDecimal(worksheet.Cells[row, 2].Value.ToString()),
        //                            offset_hour = Convert.ToDecimal(worksheet.Cells[row, 3].Value.ToString()),
        //                            vl_hour = Convert.ToDecimal(worksheet.Cells[row, 4].Value.ToString()),
        //                            sl_hour = Convert.ToDecimal(worksheet.Cells[row, 5].Value.ToString()),
        //                            otherl_hour = Convert.ToDecimal(worksheet.Cells[row, 6].Value.ToString()),
        //                            lwop_hour = Convert.ToDecimal(worksheet.Cells[row, 7].Value.ToString()),
        //                            is_absent = Convert.ToDecimal(worksheet.Cells[row, 8].Value.ToString()),
        //                            is_present = Convert.ToDecimal(worksheet.Cells[row, 9].Value.ToString()),
        //                            late = Convert.ToDecimal(worksheet.Cells[row, 10].Value.ToString()),
        //                            undertime = Convert.ToDecimal(worksheet.Cells[row, 11].Value.ToString()),
        //                            reg = Convert.ToDecimal(worksheet.Cells[row, 12].Value.ToString()),
        //                            regnd = Convert.ToDecimal(worksheet.Cells[row, 13].Value.ToString()),
        //                            ot = Convert.ToDecimal(worksheet.Cells[row, 14].Value.ToString()),
        //                            ot_e8 = Convert.ToDecimal(worksheet.Cells[row, 15].Value.ToString()),
        //                            otnd = Convert.ToDecimal(worksheet.Cells[row, 16].Value.ToString()),
        //                            otnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 17].Value.ToString()),
        //                            otrd = Convert.ToDecimal(worksheet.Cells[row, 18].Value.ToString()),
        //                            otrd_e8 = Convert.ToDecimal(worksheet.Cells[row, 19].Value.ToString()),
        //                            otrdnd = Convert.ToDecimal(worksheet.Cells[row, 20].Value.ToString()),
        //                            otrdnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 21].Value.ToString()),
        //                            lh = Convert.ToDecimal(worksheet.Cells[row, 22].Value.ToString()),
        //                            lhot = Convert.ToDecimal(worksheet.Cells[row, 23].Value.ToString()),
        //                            lhot_e8 = Convert.ToDecimal(worksheet.Cells[row, 24].Value.ToString()),
        //                            lhotnd = Convert.ToDecimal(worksheet.Cells[row, 25].Value.ToString()),
        //                            lhotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 26].Value.ToString()),
        //                            lhrd = Convert.ToDecimal(worksheet.Cells[row, 27].Value.ToString()),
        //                            lhrdot = Convert.ToDecimal(worksheet.Cells[row, 28].Value.ToString()),
        //                            lhrdot_e8 = Convert.ToDecimal(worksheet.Cells[row, 29].Value.ToString()),
        //                            lhrdotnd = Convert.ToDecimal(worksheet.Cells[row, 30].Value.ToString()),
        //                            lhrdotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 31].Value.ToString()),
        //                            sh = Convert.ToDecimal(worksheet.Cells[row, 32].Value.ToString()),
        //                            shot = Convert.ToDecimal(worksheet.Cells[row, 33].Value.ToString()),
        //                            shot_e8 = Convert.ToDecimal(worksheet.Cells[row, 34].Value.ToString()),
        //                            shotnd = Convert.ToDecimal(worksheet.Cells[row, 35].Value.ToString()),
        //                            shotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 36].Value.ToString()),
        //                            shrd = Convert.ToDecimal(worksheet.Cells[row, 37].Value.ToString()),
        //                            shrdot = Convert.ToDecimal(worksheet.Cells[row, 38].Value.ToString()),
        //                            shrdot_e8 = Convert.ToDecimal(worksheet.Cells[row, 39].Value.ToString()),
        //                            shrdotnd = Convert.ToDecimal(worksheet.Cells[row, 40].Value.ToString()),
        //                            shrdotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 41].Value.ToString()),
        //                            dh = Convert.ToDecimal(worksheet.Cells[row, 42].Value.ToString()),
        //                            dhot = Convert.ToDecimal(worksheet.Cells[row, 43].Value.ToString()),
        //                            dhot_e8 = Convert.ToDecimal(worksheet.Cells[row, 44].Value.ToString()),
        //                            dhotnd = Convert.ToDecimal(worksheet.Cells[row, 45].Value.ToString()),
        //                            dhotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 46].Value.ToString()),
        //                            dhrd = Convert.ToDecimal(worksheet.Cells[row, 47].Value.ToString()),
        //                            dhrdot = Convert.ToDecimal(worksheet.Cells[row, 48].Value.ToString()),
        //                            dhrdot_e8 = Convert.ToDecimal(worksheet.Cells[row, 49].Value.ToString()),
        //                            dhrdotnd = Convert.ToDecimal(worksheet.Cells[row, 50].Value.ToString()),
        //                            dhrdotnd_e8 = Convert.ToDecimal(worksheet.Cells[row, 51].Value.ToString()),
        //                            created_by = Convert.ToInt32(Crypto.url_decrypt(created_by)),
        //                            date_created = datetime.ToString(),




        //                        });
        //                    }

        //                }

        //                resp = _DataUploadManagementServices.timekeeping_upload(list, series_code, created_by);

        //                //var response =  _IPayrollManagementServices.payroll_generation_view(series_code, payroll_header_id, category_id, branch_id, confidential_id, include_tax, include_sss, include_pagibig, include_philhealth, created_by);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            var a = e.Message;
        //            resp = 0;
        //        }
        //    }

        //    return resp;
        //}


        //#endregion
        //[HttpGet("ExportAttendanceLog")]
        //public ActionResult UploadFile(string series_code)
        //{
        //    series_code = Crypto.password_decrypt(series_code);
        //    ExcelPackage package = new ExcelPackage();

        //    var fileName = "Attendance Log.xlsx";
        //    var customDir = "UploadTemplate\\";
        //    var directory = Path.Combine(_environment.WebRootPath, customDir);

        //    if (!Directory.Exists(directory))
        //        Directory.CreateDirectory(directory);

        //    var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

        //    var filepath = customDir + fileName;
        //    //var memoryStream = new MemoryStream();
        //    //package.SaveAs(memoryStream);



        //    //memoryStream.Position = 0;
        //    ////return File(memoryStream, contentType, fileName);


        //    using (FileStream fs = System.IO.File.Create(fullPath))
        //    {
        //        //f.CopyTo(fs);
        //        package.SaveAs(fs);
        //        fs.Flush();
        //    }


        //    return Ok(new { response = filepath });
        //}

    }
}
