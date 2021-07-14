using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalReportManagement.Model;
using Newtonsoft.Json;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Security;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CrystalReportManagement
{
    public partial class reportviewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            
            
        }

        private readonly AspNetHostingPermission _environment;

        string ipaddress = ConfigurationManager.ConnectionStrings["ipaddress"].ConnectionString;
        string apiKey = ConfigurationManager.ConnectionStrings["ApiKey"].ConnectionString;
        string email_sender = ConfigurationManager.ConnectionStrings["email_sender"].ConnectionString;

        protected void CrystalReportViewer1_Init(object sender, EventArgs e)
        {
            List<PayrollGenerationResponse> resp = new List<PayrollGenerationResponse>();
            List<PayrollGenerationResponse> header = new List<PayrollGenerationResponse>();
            List<PayslipDetaiResponse> detail_resp = new List<PayslipDetaiResponse>();
            List<PayslipDetaiReport> deductions = new List<PayslipDetaiReport>();
            List<PayslipDetaiReport> loan = new List<PayslipDetaiReport>();
            List<PayslipDetaiReport> overtime = new List<PayslipDetaiReport>();
            List<PayslipDetaiReport> taxable_earning = new List<PayslipDetaiReport>();
            List<PayslipDetaiReport> nt_earning = new List<PayslipDetaiReport>();

            try
            {


                string series_code = Request.QueryString["series_code"].ToString();
                string payroll_header_id = Request.QueryString["payroll_header_id"].ToString();
                string posted_payslip_id = Request.QueryString["posted_payslip_id"].ToString();
                string created_by = Request.QueryString["created_by"].ToString();
                string employee_id = Request.QueryString["employee_id"].ToString();

                employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
                using (var wb = new WebClient())
                {
                    string url = "";

                        //url = "http://localhost:1019/api/PayrollManagement/posted_payslip_view?series_code=" + series_code + "&payroll_header_id=" + payroll_header_id + "&posted_payslip_id=" + posted_payslip_id + "&created_by=" + created_by;
                        url = "http://localhost:52549/api/PayrollManagement/posted_payslip_view?series_code=" + series_code + "&payroll_header_id=" + payroll_header_id + "&posted_payslip_id=" + posted_payslip_id + "&created_by=" + created_by;

                    

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Method = "GET";
                    String returnString = String.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        returnString = reader.ReadToEnd();
                        resp = JsonConvert.DeserializeObject<List<PayrollGenerationResponse>>(returnString);
                        reader.Close();
                        dataStream.Close();
                    }

                }

                using (var wb = new WebClient())
                {

                    //string url = "http://localhost:1019/api/PayrollManagement/posted_payslip_detail_employee_view?series_code=" + series_code + "&payroll_header_id=" + payroll_header_id + "&posted_payslip_id=" + posted_payslip_id + "&created_by=" + created_by + "&employee_id=" + employee_id;
                    string url = "http://localhost:52549/api/PayrollManagement/posted_payslip_detail_employee_view?series_code=" + series_code + "&payroll_header_id=" + payroll_header_id + "&posted_payslip_id=" + posted_payslip_id + "&created_by=" + created_by + "&employee_id=" + employee_id;

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Method = "GET";
                    String returnString = String.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        returnString = reader.ReadToEnd();
                        detail_resp = JsonConvert.DeserializeObject<List<PayslipDetaiResponse>>(returnString);
                        reader.Close();
                        dataStream.Close();
                    }

                }


                ReportDocument crystalReport = new ReportDocument();
                
                crystalReport.Load(Server.MapPath("~/Reports/rptPayslip.rpt"));
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                deductions = (from x in detail_resp
                              where x.detail_group_id.Equals(5)
                              select new PayslipDetaiReport
                              {
                                  //payroll_header_id = x.payroll_header_id,
                                  posted_payslip_id = x.posted_payslip_id,
                                  //payroll_id = x.payroll_id,
                                  //payslip_id = x.payslip_id,
                                  //employee_id = x.employee_id,
                                  detail_group_id = x.detail_group_id,
                                  //detail_type_id = x.detail_type_id,
                                  //detail_id = x.detail_id,
                                  detail = x.detail,
                                  //total = x.total,
                                  amount = x.amount,
                                  //taxable_id = x.taxable_id,
                                  taxable = x.taxable,
                                  //created_by = x.created_by,
                                  //date_created = x.date_created,

                              }).ToList();


                dt = ToDataTable(deductions);
                dt.TableName = "Deduction";
                ds.Tables.Add(dt);


                nt_earning = (from x in detail_resp
                              where x.detail_group_id.Equals(3)
                              select new PayslipDetaiReport
                              {
                                  //payroll_header_id = x.payroll_header_id,
                                  posted_payslip_id = x.posted_payslip_id,
                                  //payroll_id = x.payroll_id,
                                  //payslip_id = x.payslip_id,
                                  //employee_id = x.employee_id,
                                  detail_group_id = x.detail_group_id,
                                  //detail_type_id = x.detail_type_id,
                                  //detail_id = x.detail_id,
                                  detail = x.detail,
                                  //total = x.total,
                                  amount = x.amount,
                                  //taxable_id = x.taxable_id,
                                  taxable = x.taxable,
                                  //created_by = x.created_by,
                                  //date_created = x.date_created,

                              }).ToList();

                dt = ToDataTable(nt_earning);
                dt.TableName = "NTEarnings";
                ds.Tables.Add(dt);


                taxable_earning = (from x in detail_resp
                                   where x.detail_group_id.Equals(2)
                                   select new PayslipDetaiReport
                                   {
                                       //payroll_header_id = x.payroll_header_id,
                                       posted_payslip_id = x.posted_payslip_id,
                                       //payroll_id = x.payroll_id,
                                       //payslip_id = x.payslip_id,
                                       //employee_id = x.employee_id,
                                       detail_group_id = x.detail_group_id,
                                       //detail_type_id = x.detail_type_id,
                                       //detail_id = x.detail_id,
                                       detail = x.detail,
                                       //total = x.total,
                                       amount = x.amount,
                                       //taxable_id = x.taxable_id,
                                       taxable = x.taxable,
                                       //created_by = x.created_by,
                                       //date_created = x.date_created,

                                   }).ToList();

                dt = ToDataTable(taxable_earning);
                dt.TableName = "TEarnings";
                ds.Tables.Add(dt);


                overtime = (from x in detail_resp
                            where x.detail_group_id.Equals(1)
                            select new PayslipDetaiReport
                            {
                                //payroll_header_id = x.payroll_header_id,
                                posted_payslip_id = x.posted_payslip_id,
                                //payroll_id = x.payroll_id,
                                //payslip_id = x.payslip_id,
                                //employee_id = x.employee_id,
                                detail_group_id = x.detail_group_id,
                                //detail_type_id = x.detail_type_id,
                                //detail_id = x.detail_id,
                                detail = x.detail,
                                //total = x.total,
                                amount = x.amount,
                                //taxable_id = x.taxable_id,
                                taxable = x.taxable,
                                //created_by = x.created_by,
                                //date_created = x.date_created,

                            }).ToList();


                dt = ToDataTable(overtime);
                dt.TableName = "Overtime";
                ds.Tables.Add(dt);

                loan = (from x in detail_resp
                        where x.detail_group_id.Equals(4)
                        select new PayslipDetaiReport
                        {
                            //payroll_header_id = x.payroll_header_id,
                            posted_payslip_id = x.posted_payslip_id,
                            //payroll_id = x.payroll_id,
                            //payslip_id = x.payslip_id,
                            //employee_id = x.employee_id,
                            detail_group_id = x.detail_group_id,
                            //detail_type_id = x.detail_type_id,
                            //detail_id = x.detail_id,
                            detail = x.detail,
                            //total = x.total,
                            amount = x.amount,
                            //taxable_id = x.taxable_id,
                            taxable = x.taxable,
                            //created_by = x.created_by,
                            //date_created = x.date_created,

                        }).ToList();

                dt = ToDataTable(loan);
                dt.TableName = "Loan";
                ds.Tables.Add(dt);


                dt = ToDataTable(detail_resp);
                dt.TableName = "PayslipDetail";
                ds.Tables.Add(dt);

                foreach (var item in resp)
                {
                    header = new List<PayrollGenerationResponse>();
                    header.Add(item);
                    if (ds.Tables.Contains("Payslip"))
                    {

                        ds.Tables.Remove("Payslip");
                    }
                    dt = ToDataTable(header);
                    dt.TableName = "Payslip";
                    ds.Tables.Add(dt);


                    ParameterFields paramfs = new ParameterFields();
                    ParameterField paramf = new ParameterField();
                    ParameterDiscreteValue dvalue = new ParameterDiscreteValue();
                    paramf.Name = "ipAddress";
                    dvalue.Value = ipaddress;
                    paramf.CurrentValues.Add(dvalue);
                    paramfs.Add(paramf);


                    crystalReport.SetDataSource(ds);
                    CrystalReportViewer1.ReportSource = crystalReport;
                    crystalReport.SetParameterValue("ipAddress", ipaddress);
                    //var a = CrystalReportViewer1.ParameterFieldInfo;
                    //CrystalReportViewer1.ParameterFieldInfo = paramfs;
                    CrystalReportViewer1.HasToggleGroupTreeButton = false;
                    CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                    CrystalReportViewer1.HasToggleParameterPanelButton = false;
                    Session["rpt"] = crystalReport;
                    Session["rptHeader"] = "PAYSLIP";
                    crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "Payslip");


                    var path = Server.MapPath("~/ExportPayslip/Payslip-" + item.last_name + ".pdf");

                    //var path = "D:\\HR-ILM\\ILMServer\\ILMServer\\CrystalReportManagement\\ExportPayslip\\Payslip" + item.last_name + ".pdf";





                    crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, path);


                    //password protected
                    PdfDocument document = PdfReader.Open(path, "some text");

                    PdfSecuritySettings securitySettings = document.SecuritySettings;


                    //securitySettings.UserPassword = item.tin_no.Replace("-", "");
                    //securitySettings.OwnerPassword = "owner";


                    //securitySettings.PermitAccessibilityExtractContent = false;
                    //securitySettings.PermitAnnotations = false;
                    //securitySettings.PermitAssembleDocument = false;
                    //securitySettings.PermitExtractContent = false;
                    //securitySettings.PermitFormsFill = true;
                    //securitySettings.PermitFullQualityPrint = false;
                    //securitySettings.PermitModifyDocument = false;
                    //securitySettings.PermitPrint = false;

                    document.Save(path);

                    Process.Start(path);

                    if (item.is_email)
                    {

                        var sendGridClient = new SendGridClient(apiKey);

                        EmailRequest email = new EmailRequest();

                        var sendGridMessage = new SendGridMessage();

                        email.approver_name = item.display_name;
                        email.module_name = "Payslip";
                        email.transaction_code = item.payroll_code;
                        email.date_created = item.pay_date;
                        email.email_address = item.email_address;
                        email.header = "Payslip!";

                        sendGridMessage.SetFrom(email_sender, "Aanya HR");
                        sendGridMessage.AddTo(item.email_address, item.display_name);
                        sendGridMessage.SetTemplateId("d-f1a60bc24c554926902778b3226f47c0");
                        sendGridMessage.SetTemplateData(email);

                       
                        byte[] byteData = Encoding.ASCII.GetBytes(File.ReadAllText(path));
                        sendGridMessage.Attachments = new List<SendGrid.Helpers.Mail.Attachment>
                        {
                            new SendGrid.Helpers.Mail.Attachment
                            {
                                Content = Convert.ToBase64String(byteData),
                                //Content = path,
                                Filename = "Payslip-" + item.last_name + ".pdf",
                                Type = "txt/plain",
                                Disposition = "attachment",


                            }
                        };

                        var response = sendGridClient.SendEmailAsync(sendGridMessage).Result;

                        //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "Payslip");

                    }

                    //var fileName = "Payslip.pdf";
                    //var customDir = "ExportPayslip\\";
                    //var directory = Path.Combine(_environment.WebRootPath, customDir);

                    //if (!Directory.Exists(directory))
                    //    Directory.CreateDirectory(directory);

                    //var fullPath = Path.Combine(_environment.WebRootPath, customDir) + $@"{fileName}";

                    //var filepath = customDir + fileName;

                }
            }catch(Exception ex)
            {
                //alert(ex.Message);
                
            }
            finally
            {

            }

        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);



            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }



            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}