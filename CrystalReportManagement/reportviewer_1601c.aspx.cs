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
    public partial class reportviewer_1601c : System.Web.UI.Page
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
            List<rpt1601cResponse> resp = new List<rpt1601cResponse>();
            COERequest req = new COERequest();


            try
            {


                string series_code = Request.QueryString["series_code"].ToString();
                string employee_id = Request.QueryString["employee_id"].ToString();
                string date_from = Request.QueryString["date_from"].ToString();
                string date_to = Request.QueryString["date_to"].ToString();

                //coe_id = coe_id == "0" ? "0" : Crypto.url_decrypt(coe_id);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                using (var wb = new WebClient())
                {
                    string url = "";

                    url = "http://localhost:1019/api/PayrollManagement/report_1601c?series_code=" + series_code + "&employee_id=" + employee_id + "&date_from=" + date_from + "&date_to=" + date_to;
                    //url = "http://localhost:52549/api/PayrollManagement/report_1601c?series_code=" + series_code + "&employee_id=" + employee_id + "&date_from=" + date_from + "&date_to=" + date_to;



                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Method = "GET";
                    String returnString = String.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        returnString = reader.ReadToEnd();
                        resp = JsonConvert.DeserializeObject<List<rpt1601cResponse>>(returnString);
                        reader.Close();
                        dataStream.Close();
                    }

                }



                dt = ToDataTable(resp);
                dt.TableName = "_1601c";
                ds.Tables.Add(dt);


                ReportDocument crystalReport = new ReportDocument();
                var path = "";
                foreach (var item in resp)
                {
                  

                        //Philealth Application
                        crystalReport.Load(Server.MapPath("~/Reports/rpt1601C.rpt"));

                    

                    //path = Server.MapPath("~/ExportCOE/COE_" + item.last_name + "( " + item.coe_code + ").pdf");

                    path = ("/Export1601C/1601c( " + item._1 + ").pdf");



                    path = Server.MapPath("~/Export1601C/1601c( " + item._1 + ").pdf");
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
                    //crystalReport.SetParameterValue("ipAddress", ipaddress);
                    //var a = CrystalReportViewer1.ParameterFieldInfo;
                    //CrystalReportViewer1.ParameterFieldInfo = paramfs;
                    CrystalReportViewer1.HasToggleGroupTreeButton = false;
                    CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                    CrystalReportViewer1.HasToggleParameterPanelButton = false;
                    Session["rpt"] = crystalReport;
                    Session["rptHeader"] = "1601C";

                crystalReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "1601C");

                crystalReport.ExportToDisk(ExportFormatType.PortableDocFormat, path);


                //var path = "D:\\HR-ILM\\ILMServer\\ILMServer\\CrystalReportManagement\\ExportCOE\\COE" + item.last_name + ".pdf";








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