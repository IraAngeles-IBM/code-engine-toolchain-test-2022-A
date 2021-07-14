using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrystalReportManagement.Model;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace CrystalReportManagement.Controllers
{
    public class CrystalReportController : Controller
    {


        public ActionResult ExportPayslip(string series_code, string payroll_header_id, int posted_payslip_id, string created_by)
        {
            var movement_resp = 0;
            List<PayrollGenerationResponse> resp = new List<PayrollGenerationResponse>();

            string responseInString = "";
            try
            {
                //using (var wb = new WebClient())
                //{

                //    //string url = "http://localhost:1019/api/PayrollManagement/posted_payslip_view?series_code=" + series_code + "&payroll_header_id=" + payroll_header_id + "&posted_payslip_id=" + posted_payslip_id + "&created_by=" + created_by;
                //    string url = "http://localhost:52549/api/PayrollManagement/posted_payslip_view?series_code=" + series_code + "&payroll_header_id=" + payroll_header_id + "&posted_payslip_id=" + posted_payslip_id + "&created_by=" + created_by;

                //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                //    request.Method = "GET";
                //    String returnString = String.Empty;
                //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                //    {
                //        Stream dataStream = response.GetResponseStream();
                //        StreamReader reader = new StreamReader(dataStream);
                //        returnString = reader.ReadToEnd();
                //        resp = JsonConvert.DeserializeObject<List<PayrollGenerationResponse>>(returnString);
                //        reader.Close();
                //        dataStream.Close();
                //    }

                //}


                ReportDocument crystalReport = new ReportDocument();
                crystalReport.Load(Server.MapPath("~/Reports/HRIS/Payslip.rpt"));


                //DataSet dsHeader = ws.payslip_mass_view(employee_id, date_from, date_to, warehouse_id, company_id, created_by, payslip_no);
                crystalReport.SetDataSource(resp);
                //CrystalReportViewer1.ReportSource = crystalReport;
                //CrystalReportViewer1.ParameterFieldInfo = paramfs;
                //CrystalReportViewer1.HasToggleGroupTreeButton = false;
                //CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                //CrystalReportViewer1.HasToggleParameterPanelButton = false;
                Session["rpt"] = crystalReport;
                Session["rptHeader"] = "PAYSLIP";


                crystalReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat,  "Payslip");

                
            }
            catch (Exception e)
            {
                var message = "Error: " + e.Message;
            }


            return View();



        }

        // GET: CrystalReport
        public ActionResult Index()
        {
            return View();
        }

        // GET: CrystalReport/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CrystalReport/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CrystalReport/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CrystalReport/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CrystalReport/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CrystalReport/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CrystalReport/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
