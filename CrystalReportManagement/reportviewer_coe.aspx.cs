using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalReportManagement.Model;

namespace CrystalReportManagement
{
    public partial class reportviewer_coe : System.Web.UI.Page
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
            List<COEResponse> resp = new List<COEResponse>();
            COERequest req = new COERequest();


            try
            {


                string series_code = Request.QueryString["series_code"].ToString();
                string coe_id = Request.QueryString["coe_id"].ToString();
                string created_by = Request.QueryString["created_by"].ToString();
                string approval_level_id = Request.QueryString["approval_level_id"].ToString();

                //coe_id = coe_id == "0" ? "0" : Crypto.url_decrypt(coe_id);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                using (var wb = new WebClient())
                {
                    string url = "";

                    url = "http://localhost:1016/api/FilingManagement/coe_request_view_sel?series_code=" + series_code + "&coe_id=" + coe_id +  "&created_by=" + created_by;
                    //url = "http://localhost:7012/api/FilingManagement/coe_request_view_sel?series_code=" + series_code + "&coe_id=" + coe_id +  "&created_by=" + created_by;



                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Method = "GET";
                    String returnString = String.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        returnString = reader.ReadToEnd();
                        resp = JsonConvert.DeserializeObject<List<COEResponse>>(returnString);
                        reader.Close();
                        dataStream.Close();
                    }

                }



                dt = ToDataTable(resp);
                dt.TableName = "COE";
                ds.Tables.Add(dt);


                ReportDocument crystalReport = new ReportDocument();
                var path = "";
                foreach (var item in resp)
                {
                    if (item.purpose_id == 12657)
                    {
                        //COE Purposes
                        crystalReport.Load(Server.MapPath("~/Reports/COELegalPurpose_NoSalary.rpt"));
                    }
                    else if (item.purpose_id == 12660)
                    {

                        //Legal Purpose 
                        if(item.with_pay == true)
                        {
                            crystalReport.Load(Server.MapPath("~/Reports/COELegalPurpose.rpt"));
                        }
                        else
                        {

                            crystalReport.Load(Server.MapPath("~/Reports/COELegalPurpose_NoSalary.rpt"));
                        }

                    }
                    else if (item.purpose_id == 12659)
                    {
                        //Loan Purpose 
                        if (item.with_pay == true)
                        {
                            crystalReport.Load(Server.MapPath("~/Reports/COELoan.rpt"));
                        }
                        else
                        {

                            crystalReport.Load(Server.MapPath("~/Reports/COELoan_NoSalary.rpt"));
                        }
                        

                    }
                    else if (item.purpose_id == 12661)
                    {

                        //Mobile Purposes
                        if (item.with_pay == true)
                        {
                            crystalReport.Load(Server.MapPath("~/Reports/COEMobilePlan.rpt"));
                        }
                        else
                        {

                            crystalReport.Load(Server.MapPath("~/Reports/COEMobilePlan_NoSalary.rpt"));
                        }

                    }
                    else if (item.purpose_id == 12658)
                    {

                        //Visa Application
                        if (item.with_pay == true)
                        {
                            crystalReport.Load(Server.MapPath("~/Reports/COEVisa.rpt"));
                        }
                        else
                        {

                            crystalReport.Load(Server.MapPath("~/Reports/COEVisa_NoSalary.rpt"));
                        }


                    }
                    else
                    {

                        //Philealth Application
                        if (item.with_pay == true)
                        {
                            crystalReport.Load(Server.MapPath("~/Reports/COE_NoSalary.rpt"));
                        }
                        else
                        {

                            crystalReport.Load(Server.MapPath("~/Reports/COE.rpt"));
                        }

                    }

                    //path = Server.MapPath("~/ExportCOE/COE_" + item.last_name + "( " + item.coe_code + ").pdf");

                    path = ("/ExportCOE/COE_" + item.last_name + "( " + item.coe_code + ").pdf");

                    req.coe_id = Crypto.url_encrypt(item.coe_id.ToString());
                    req.coe_code = item.coe_code;
                    req.reason = item.reason;
                    req.purpose_id = item.purpose_id; 
                    req.with_pay            = item.with_pay;   
                    req.coe_path            = path;
                    req.active              = item.active;
                    req.approval_level_id   = approval_level_id;
                    req.created_by          = created_by;
                    req.series_code         = series_code;



                    path = Server.MapPath("~/ExportCOE/COE_" + item.last_name + "( " + item.coe_code + ").pdf");
                }

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
                    Session["rptHeader"] = "COE";
                //crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "COE");



                //var path = "D:\\HR-ILM\\ILMServer\\ILMServer\\CrystalReportManagement\\ExportCOE\\COE" + item.last_name + ".pdf";



                COEIUResponse insert_resp = new COEIUResponse();
                string responseInString =  "";
                    crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, path);


                        using (var wb = new WebClient())
                        {


                    string url = "http://localhost:1016/api/FilingManagement/coe_request_in_up";
                    //string url = "http://localhost:7012/api/FilingManagement/coe_request_in_up";


                            wb.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string Stringdata = JsonConvert.SerializeObject(req);
                            responseInString = wb.UploadString(url, Stringdata);
                            //string HtmlResult = wb.UploadValues(url, data);

                            //var response = wb.UploadValues(url, "POST", data);
                            //responseInString = Encoding.UTF8.GetString(response);

                        }
                insert_resp = JsonConvert.DeserializeObject<COEIUResponse>(responseInString);



            }
            catch (Exception ex)
            {

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