using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ReportManagementService.Helper;
using ReportManagementService.Model;
using ReportManagementService.Service;
using Syncfusion.HtmlConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReportManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportManagementController : Controller
    {
        private IConverter _converter;


        private IReportManagementServices _ReportManagementServices;
        private readonly IWebHostEnvironment _environment;

        private EmailSender email;
        private Default_Url url;

        public ReportManagementController(IReportManagementServices ReportManagementServices, IWebHostEnvironment IWebHostEnvironment, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _environment = IWebHostEnvironment;
            _ReportManagementServices = ReportManagementServices;

            email = appSettings.Value;
            url = settings.Value;
        }


        [HttpGet("payslip")]
        public IActionResult payslip(string series_code, string payroll_header_id, int posted_payslip_id, string created_by)
        {
            HtmlToPdfConverter conv = new HtmlToPdfConverter();
            WebKitConverterSettings settings = new WebKitConverterSettings();
            settings.WebKitPath = Path.Combine(_environment.ContentRootPath, "report");

            conv.ConverterSettings = settings;
            //var globalSettings = new GlobalSettings
            //{
            //    ColorMode = ColorMode.Color,
            //    Orientation = Orientation.Portrait,
            //    PaperSize = PaperKind.A4,
            //    Margins = new MarginSettings { Top = 10 },
                
            //    DocumentTitle = "PDF Report",
            //    Out = @"D:\PDFCreator\Payslip.pdf"
            //};

            //var sb = new StringBuilder();
            //sb.Append(@"
            //              <div class='col-md-12'>
            //                <div class='row'>
            //                  <div class='col-md-8 align-self-center'>
            //                    <h6 class=''>{{payslipHeader.company_name}}</h6>
            //                    <!-- <p class='text-muted'>Lot 10 Block 2 First Philippine Industrial Park II SEZ
            //                      Brgy. San Rafael, Sto. Tomas, Batangas 4234
            //                      </p> -->
            //                  </div>
            //                  <div class='col-md-4'>
            //                    <img style='width:150px;height:100px;' src='{{url}}'>
            //                  </div>
            //                </div>
            //              </div>");

            //var objectSettings = new ObjectSettings
            //{
            //    PagesCount = true,
            //    HtmlContent =  sb.ToString(),
                        
            //    //WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            //    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
            //    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            //};
            //var pdf = new HtmlToPdfDocument()
            //{
            //    GlobalSettings = globalSettings,
            //    Objects = { objectSettings }
            //};
            //_converter.Convert(pdf);

            return Ok();
        }





        [HttpGet("report_header")]
        public List<DataUploadHeaderResponse> report_header(string series_code, int dropdown_id, string created_by)
        {

            var resp = _ReportManagementServices.report_header(series_code, dropdown_id, created_by);

            return resp;
        }


        [HttpGet("report_view")]
        public JsonResult report_view(string series_code, string date_from, string date_to, int employee_id, int dropdown_id, string created_by)
        {

            var resp = _ReportManagementServices.report_view( series_code,  date_from,  date_to,  employee_id,  dropdown_id,  created_by);
            var result = JsonConvert.SerializeObject(resp);
            JsonResult json = Json(result);
            return json;
        }



    }
}
