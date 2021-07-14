
using LogServices.Helper;
using LogServices.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LogServices.Service
{


    public interface ILogService
    {
        string error_log_in(ErrorLogsRequest model);
        List<NotificationResponse> system_notification_view(string series_code, string date_from, string date_to, int module_id, string created_by);
        List<NotificationResponse> system_notification_fetch_view(string series_code, int row, int index, string created_by);
        List<LogResponse> system_log_view(string series_code, string date_from, string date_to, int module_id, int transaction_type_id, string created_by);
        List<LogResponse> system_fetch_view(string series_code, int row, int index, string created_by);
            
    }
    public class LogService : ILogService
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;



        public LogService(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }



        public string error_log_in(ErrorLogsRequest model)
        {
            string resp = "";

            string _con = connection._DB_Master;
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
                oCmd.CommandText = "error_log_in";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", model.module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", model.transaction_id);
                oCmd.Parameters.AddWithValue("@error_log", model.error_log);
                oCmd.Parameters.AddWithValue("@created_by", Crypto.url_decrypt(model.created_by));
                oCmd.ExecuteNonQuery();
               

                oTrans.Commit();
                resp = "OK";
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                resp = "Error: " + e.Message;
            }
            finally
            {
                oConn.Close();
            }

            return resp;
        }


        public string system_log_in(SystemLogsRequest model)
        {
            string resp = "";

            string _con = connection._DB_Master;
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
                oCmd.CommandText = "system_log_in";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", model.module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", model.transaction_id);
                oCmd.Parameters.AddWithValue("@transaction_id", model.transaction_type_id);
                oCmd.Parameters.AddWithValue("@error_log", model.description);
                oCmd.Parameters.AddWithValue("@created_by", Crypto.url_decrypt(model.created_by));
                oCmd.ExecuteNonQuery();


                oTrans.Commit();
                resp = "OK";
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                resp = "Error: " + e.Message;
            }
            finally
            {
                oConn.Close();
            }

            return resp;
        }

        public List<NotificationResponse> system_notification_view(string series_code, string date_from, string date_to,  int module_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<NotificationResponse> resp = new List<NotificationResponse>();
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
                oCmd.CommandText = "system_notification_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@employee_id", created_by);
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new NotificationResponse()
                        {
                            notification_id         = Convert.ToInt32(dr["notification_id"].ToString()),
                            encrypt_notification_id = Crypto.url_encrypt(dr["notification_id"].ToString()),
                            module_id               = Convert.ToInt32(dr["module_id"].ToString()),
                            transaction_id          = Convert.ToInt32(dr["transaction_id"].ToString()),
                            transaction_type        = (dr["transaction_type"].ToString()),
                            description             = (dr["description"].ToString()),
                            is_viewed               = Convert.ToBoolean(dr["is_viewed"].ToString()),
                            created_by              = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created            = (dr["date_created"].ToString()),
                            display_name            = (dr["display_name"].ToString()),
                            image_path              = (dr["image_path"].ToString()),
                            lapse                   = (dr["lapse"].ToString()),
                            icon                    = (dr["icon"].ToString()),

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

        public List<NotificationResponse> system_notification_fetch_view(string series_code, int row, int index, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<NotificationResponse> resp = new List<NotificationResponse>();
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
                oCmd.CommandText = "system_notification_fetch_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@row", row);
                oCmd.Parameters.AddWithValue("@index", index);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new NotificationResponse()
                        {
                            notification_id = Convert.ToInt32(dr["notification_id"].ToString()),
                            encrypt_notification_id = Crypto.url_encrypt(dr["notification_id"].ToString()),
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),
                            transaction_id = Convert.ToInt32(dr["transaction_id"].ToString()),
                            transaction_type = (dr["transaction_type"].ToString()),
                            description = (dr["description"].ToString()),
                            is_viewed = Convert.ToBoolean(dr["is_viewed"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            image_path = (dr["image_path"].ToString()),
                            lapse = (dr["lapse"].ToString()),


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


        public List<LogResponse> system_log_view(string series_code, string date_from, string date_to, int module_id, int transaction_type_id,  string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<LogResponse> resp = new List<LogResponse>();
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
                oCmd.CommandText = "system_log_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@employee_id", created_by);
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@transaction_type_id", transaction_type_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new LogResponse()
                        {
                            log_id                   = Convert.ToInt32(dr["log_id"].ToString()),
                            encrypt_log_id           = Crypto.url_encrypt(dr["log_id"].ToString()),
                            module_id                = Convert.ToInt32(dr["module_id"].ToString()),
                            transaction_id           = Convert.ToInt32(dr["transaction_id"].ToString()),
                            transaction_type_id      = Convert.ToInt32(dr["transaction_type_id"].ToString()),
                            description              = (dr["description"].ToString()),
                            is_viewed                = Convert.ToBoolean(dr["is_viewed"].ToString()),
                            created_by               = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created             = (dr["date_created"].ToString()),
                            display_name             = (dr["display_name"].ToString()),
                            image_path               = (dr["image_path"].ToString()),
                            lapse                    = (dr["lapse"].ToString()),
                            icon                     = (dr["icon"].ToString()),
                            transaction_type = (dr["transaction_type"].ToString()),

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

        
        public List<LogResponse> system_fetch_view(string series_code, int row, int index,  string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<LogResponse> resp = new List<LogResponse>();
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
                oCmd.CommandText = "system_fetch_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@row", row);
                oCmd.Parameters.AddWithValue("@index", index);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new LogResponse()
                        {
                            log_id                   = Convert.ToInt32(dr["log_id"].ToString()),
                            encrypt_log_id           = Crypto.url_encrypt(dr["log_id"].ToString()),
                            module_id                = Convert.ToInt32(dr["module_id"].ToString()),
                            transaction_id           = Convert.ToInt32(dr["transaction_id"].ToString()),
                            transaction_type_id      = Convert.ToInt32(dr["transaction_type_id"].ToString()),
                            description              = (dr["description"].ToString()),
                            is_viewed                = Convert.ToBoolean(dr["is_viewed"].ToString()),
                            created_by               = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created             = (dr["date_created"].ToString()),
                            display_name             = (dr["display_name"].ToString()),
                            image_path               = (dr["image_path"].ToString()),
                            lapse                    = (dr["lapse"].ToString()),
                            icon                     = (dr["icon"].ToString()),

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
