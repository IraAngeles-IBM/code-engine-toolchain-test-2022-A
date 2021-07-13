
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using FileManagerService.Helper;
using FileManagerService.Model;
using System.Reflection;
using System.Data.SqlClient;

namespace FileManagerService.Service
{

    public interface IFileManagerServices
    {
        int file_attachment_in(FileRequest model);
        List<FileResponse> file_attachment_sel(string series_code, string module_id, string transaction_id, int file_type, string created_by);
        int file_attachment_update(FileRequest[] model);

    }

    public class FileManagerServices : IFileManagerServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public FileManagerServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }


        public int file_attachment_update(FileRequest[] model)
        {
            var resp = 0;
            string series_code = Crypto.url_decrypt(model[0].series_code);
            string created_by = Crypto.url_decrypt(model[0].created_by);

            string _con;
            _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";

            DataTable dt = new DataTable();
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;
            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {

                da.SelectCommand = oCmd;
                oCmd.CommandText = "file_attachment_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@module_id", model[0].module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", model[0].transaction_id);
                oCmd.ExecuteNonQuery();
             
                foreach(var item in model)
                {
                    oCmd.CommandText = "file_attachment_in";
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();

                    oCmd.Parameters.AddWithValue("@module_id", item.module_id);
                    oCmd.Parameters.AddWithValue("@created_by", created_by);
                    oCmd.Parameters.AddWithValue("@transaction_id", item.transaction_id);
                    oCmd.Parameters.AddWithValue("@file_name", item.file_name);
                    oCmd.Parameters.AddWithValue("@file_path", item.file_path);
                    oCmd.Parameters.AddWithValue("@file_type", item.file_type);
                    SqlDataReader sdr = oCmd.ExecuteReader();
                    while (sdr.Read())
                    {

                        resp = Convert.ToInt32(sdr["created_by"].ToString());

                    }
                    sdr.Close();
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
        public int file_attachment_in(FileRequest model)
        {
            var resp = 0;
            string series_code = (model.series_code);
            string created_by = (model.created_by);

            string _con;
            _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";

            DataTable dt = new DataTable();
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;
            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {

                da.SelectCommand = oCmd;
                oCmd.CommandText = "file_attachment_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@module_id", model.module_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@transaction_id", model.transaction_id);
                oCmd.Parameters.AddWithValue("@file_name", model.file_name);
                oCmd.Parameters.AddWithValue("@file_path", model.file_path);
                oCmd.Parameters.AddWithValue("@file_type", model.file_type);

                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp = Convert.ToInt32(sdr["created_by"].ToString());

                }
                sdr.Close();


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

        public List<FileResponse> file_attachment_sel(string series_code, string module_id, string transaction_id, int file_type, string created_by)
        {

            if (file_type == 0)
            {

                transaction_id = transaction_id == "0" ? "0" : Crypto.url_decrypt(transaction_id);
            }
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<FileResponse> resp = new List<FileResponse>();
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
                oCmd.CommandText = "file_attachment_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", transaction_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new FileResponse()
                        {
                            module_id       = Convert.ToInt32(dr["module_id"].ToString()),
                            transaction_id  = Convert.ToInt32(dr["transaction_id"].ToString()),
                            file_path       = (dr["file_path"].ToString()),
                            file_name       = (dr["file_name"].ToString()),
                            file_type       = (dr["file_type"].ToString()),
                            file_class      = (dr["file_class"].ToString()),
                            created_by      = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created    = (dr["date_created"].ToString()),
                         



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
