using HolidayManagementService.Helper;
using HolidayManagementService.Model;
using Microsoft.Extensions.Options;
using HolidayManagementService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace HolidayManagementService.Service
{

    public interface IHolidayManagementServices
    {
        int holiday_in_up(HolidayHeader model);

        List<HolidayHeaderResponse> holiday_view_sel(string series_code, string holiday_id);
        List<HolidayBranchView>     holiday_branch_view(string series_code, string holiday_id);
        List<HolidayDetailView>     holiday_detail_view(string series_code, string holiday_id);
    }

    public class HolidayManagementServices : IHolidayManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public HolidayManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }


        public int holiday_in_up(HolidayHeader model)
        {
            int resp = 0;
            string series_code = Crypto.url_decrypt(model.series_code);
            string created_by = Crypto.url_decrypt(model.created_by);
            //BranchResponse resp = new BranchResponse();
            string _con;
            _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";

            DataTable dt = new DataTable();
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;
            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {
                if (model != null)
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = oCmd;
                    oCmd.CommandText = "holiday_in_up";
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();
                    oCmd.Parameters.AddWithValue("@holiday_id",             model.holiday_id == "0" ? 0 : Crypto.url_decrypt(model.holiday_id));
                    oCmd.Parameters.AddWithValue("@holiday_code",           model.holiday_code);
                    oCmd.Parameters.AddWithValue("@holiday_header_name",    model.holiday_header_name);
                    oCmd.Parameters.AddWithValue("@holiday_description",    model.holiday_description);
                    oCmd.Parameters.AddWithValue("@active",                 model.active );
                    oCmd.Parameters.AddWithValue("@created_by",             created_by);
                    //oCmd.ExecuteNonQuery();
                    SqlDataReader sdr = oCmd.ExecuteReader();
                    while (sdr.Read())
                    {
                        resp = Convert.ToInt32(sdr["holiday_id"].ToString());
                    }
                    sdr.Close();
                    
                    if (model.Detail != null)
                    {
                        foreach (var item in model.Detail)
                        {
                            oCmd.CommandText = "holiday_detail_in";
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            oCmd.Parameters.Clear();
                            oCmd.Parameters.AddWithValue("@holiday_id", resp);
                            oCmd.Parameters.AddWithValue("@holiday_name", item.holiday_name);
                            oCmd.Parameters.AddWithValue("@holiday_type_id", item.holiday_type_id);
                            oCmd.Parameters.AddWithValue("@holiday_date", item.holiday_date);
                            oCmd.Parameters.AddWithValue("@created_by", created_by);
                            oCmd.ExecuteNonQuery();
                        }
                    }


                    if (model.Branch != null)
                    {
                        foreach (var br in model.Branch)
                        {
                            oCmd.CommandText = "holiday_branch_in";
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            oCmd.Parameters.Clear();
                            oCmd.Parameters.AddWithValue("@holiday_id", resp);
                            oCmd.Parameters.AddWithValue("@branch_id", br.branch_id);
                            oCmd.Parameters.AddWithValue("@created_by", created_by);
                            oCmd.ExecuteNonQuery();
                        }
                    }

                }

                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp = 0;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }

        public List<HolidayHeaderResponse> holiday_view_sel(string series_code, string holiday_id)
        {
            holiday_id = holiday_id == "0" ? "0" : Crypto.url_decrypt(holiday_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<HolidayHeaderResponse> resp = new List<HolidayHeaderResponse>();
            DataTable dt = new DataTable();
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;
            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "holiday_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@holiday_id", holiday_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new HolidayHeaderResponse()
                        {
                            holiday_id              = Convert.ToInt32(dr["holiday_id"].ToString()),
                            holiday_id_encrypted    = Crypto.url_encrypt(dr["holiday_id"].ToString()),
                            holiday_code            = dr["holiday_code"].ToString(),
                            holiday_header_name     = dr["holiday_header_name"].ToString(),
                            holiday_description     = dr["holiday_description"].ToString(),
                            created_by              = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created            = dr["date_created"].ToString(),
                            active                  = Convert.ToBoolean(dr["active"].ToString()),
                            created_by_name         = dr["created_by_name"].ToString(),
                            status                  = dr["status"].ToString(),

                        }).ToList();
                oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                oConn.Close();
            }


            return resp;
        }

        public List<HolidayBranchView> holiday_branch_view(string series_code, string holiday_id)
        {

            holiday_id = holiday_id == "0" ? "0" : Crypto.url_decrypt(holiday_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<HolidayBranchView> resp = new List<HolidayBranchView>();
            DataTable dt = new DataTable();
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;
            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "holiday_branch_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@holiday_id", holiday_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new HolidayBranchView()
                        {
                            holiday_id = Convert.ToInt32(dr["holiday_id"].ToString()),
                            branch_id = Convert.ToInt32(dr["branch_id"].ToString()),
                         
                        }).ToList();
                oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                oConn.Close();
            }


            return resp;
        }

        public List<HolidayDetailView> holiday_detail_view(string series_code, string holiday_id)
        {

            holiday_id = holiday_id == "0" ? "0" : Crypto.url_decrypt(holiday_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<HolidayDetailView> resp = new List<HolidayDetailView>();
            DataTable dt = new DataTable();
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;
            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "holiday_detail_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@holiday_id", holiday_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new HolidayDetailView()
                        {
                            holiday_id = Convert.ToInt32(dr["holiday_id"].ToString()),
                            holiday_name = dr["holiday_name"].ToString(),
                            description = dr["description"].ToString(),
                            holiday_type_id = Convert.ToInt32(dr["holiday_type_id"].ToString()),
                            holiday_date = dr["holiday_date"].ToString(),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created = dr["date_created"].ToString(),

                        }).ToList();
                oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                oConn.Close();
            }


            return resp;
        }
    }
}
