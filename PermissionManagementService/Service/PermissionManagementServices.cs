using Microsoft.Extensions.Options;
using PermissionManagementService.Helper;
using PermissionManagementService.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PermissionManagementService.Service
{


    public interface IPermissionManagementServices
    {
        List<MenuViewResponse> dynamic_menu_view(string access_level_id, string series_code, string created_by);


        ModuleResponse module_access_in(ModuleRequest[] model);
        ReportAccessResponse report_access_in(ReportAccessRequest[] model);
        ConfidentialityAccessResponse confidentiality_access_in(ConfidentialityAccessRequest[] model);
        DataUploadAccessResponse data_upload_access_in(DataUploadAccessRequest[] model);


        List<ModuleResponse> module_access_view(string series_code, string access_level_id);
        List<ReportAccessResponse> report_access_view(string series_code, string access_level_id);
        List<ReportAccessResponse> report_access_list_view(string series_code, string access_level_id, string created_by);

        List<ConfidentialityAccessResponse> confidentiality_access_view(string series_code, string access_level_id);
        List<ConfidentialityAccessResponse> confidentiality_access_sel(string series_code, string access_level_id);
        List<ConfidentialityAccessResponse> confidentiality_access_list_view(string series_code, string access_level_id, string created_by);
        List<DataUploadAccessResponse> data_upload_access_view(string series_code, string access_level_id);
        List<DataUploadAccessResponse> data_upload_access_list_view(string series_code, string access_level_id, string created_by);

        List<ApprovalAccessResponse> approval_access_view(string series_code, string access_level_id, string date_from, string date_to, string created_by);
    }

    public class PermissionManagementServices : IPermissionManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public PermissionManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }


        public List<MenuViewResponse> dynamic_menu_view(string access_level_id,string series_code, string created_by)
        {
            access_level_id = Crypto.url_decrypt(access_level_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);


            List<MenuViewResponse> resp = new List<MenuViewResponse>();


            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog="+ series_code  + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
            DataTable dt = new DataTable();
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;

            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = oCmd;
            try
            {
                oCmd.CommandText = "dynamic_menu_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new MenuViewResponse()
                        {
                            module_id = Convert.ToInt32(sdr["module_id"].ToString()),
                            module_name = sdr["module_name"].ToString(),
                            classes = sdr["class"].ToString(),
                            link = sdr["link"].ToString(),
                            has_approval = Convert.ToBoolean(sdr["has_approval"].ToString()),
                            count = Convert.ToInt32(sdr["count"].ToString()),
                            parent_module_id = Convert.ToInt32(sdr["parent_module_id"].ToString()),
                            module_type = sdr["module_type"].ToString(),
                            order_by = (sdr["order_by"].ToString()),
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



        public ModuleResponse module_access_in(ModuleRequest[] model)
        {
            int access_level_id = (model[0].access_level_id);
            string created_by = Crypto.url_decrypt(model[0].created_by);
            string series_code = Crypto.url_decrypt(model[0].series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";

             ModuleResponse resp = new ModuleResponse();

            resp.module_id = 0;
            //string _con = connection._DB_Master;
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
                oCmd.CommandText = "module_access_del";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                oCmd.ExecuteNonQuery();
                if (model != null)
                {
                    foreach (var item in model)
                    {

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = oCmd;
                        oCmd.CommandText = "module_access_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@module_id", item.module_id);
                        oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        da.Fill(dt);
                        
                    }
                }
                oTrans.Commit();

                resp.module_id = 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
            }
            finally
            {
                oConn.Close();
            }

            return resp;
        }

        public ReportAccessResponse report_access_in(ReportAccessRequest[] model)
        {

            int access_level_id = (model[0].access_level_id);
            string created_by = Crypto.url_decrypt(model[0].created_by);
            string series_code = Crypto.url_decrypt(model[0].series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            ReportAccessResponse resp = new ReportAccessResponse();
            resp.report_id = 0;
            //string _con = connection._DB_Master;
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
                oCmd.CommandText = "report_access_del";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                oCmd.ExecuteNonQuery();


                if (model != null)
                {
                    foreach (var item in model)
                    {

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = oCmd;
                        oCmd.CommandText = "report_access_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@report_id", item.report_id);
                        oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        da.Fill(dt);

                    }
                }
                oTrans.Commit();

                resp.report_id = 1;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);

                oTrans.Rollback();
            }
            finally
            {
                oConn.Close();
            }

            return resp;
        }


        public ConfidentialityAccessResponse confidentiality_access_in(ConfidentialityAccessRequest[] model)
        {

            int access_level_id = (model[0].access_level_id);
            string created_by = Crypto.url_decrypt(model[0].created_by);
            string series_code = Crypto.url_decrypt(model[0].series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            ConfidentialityAccessResponse resp = new ConfidentialityAccessResponse();
            resp.confidentiality_id = 0;
            //string _con = connection._DB_Master;
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
                oCmd.CommandText = "confidentiality_access_del";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                oCmd.ExecuteNonQuery();


                if (model != null)
                {
                    foreach (var item in model)
                    {

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = oCmd;
                        oCmd.CommandText = "confidentiality_access_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@confidentiality_id", item.confidentiality_id);
                        oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        da.Fill(dt);

                    }
                }
                oTrans.Commit();

                resp.confidentiality_id = 1;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);

                resp.confidentiality_id = 0;
                oTrans.Rollback();
            }
            finally
            {
                oConn.Close();
            }

            return resp;
        }

        public DataUploadAccessResponse data_upload_access_in(DataUploadAccessRequest[] model)
        {
            int access_level_id = (model[0].access_level_id);
            string created_by = Crypto.url_decrypt(model[0].created_by);
            string series_code = Crypto.url_decrypt(model[0].series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            DataUploadAccessResponse resp = new DataUploadAccessResponse();
            resp.data_upload_id = 0;
            //string _con = connection._DB_Master;
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
                oCmd.CommandText = "data_upload_access_del";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                oCmd.ExecuteNonQuery();

                if (model != null)
                {
                    foreach (var item in model)
                    {

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = oCmd;
                        oCmd.CommandText = "data_upload_access_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@data_upload_id", item.data_upload_id);
                        oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        da.Fill(dt);

                    }
                }
                oTrans.Commit();

                resp.data_upload_id = 1;
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
            }
            finally
            {
                oConn.Close();
            }

            return resp;
        }



        public List<ModuleResponse> module_access_view(string series_code, string access_level_id)
        {

            series_code = Crypto.url_decrypt(series_code);

            int int_access_level_id = 0;
            try
            {
                int_access_level_id = Convert.ToInt32(access_level_id);
            }
            catch
            {
                access_level_id = Crypto.url_decrypt(access_level_id);
            }



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ModuleResponse> resp = new List<ModuleResponse>();
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
                oCmd.CommandText = "module_access_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ModuleResponse()
                        {
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),
                            module_name = dr["module_name"].ToString(),
                            access_level_id = dr["access_level_id"].ToString(),

                            int_access_level_id = Convert.ToInt32(dr["access_level_id"].ToString()),
                            created_by = dr["created_by"].ToString(),
                            active = Convert.ToBoolean(dr["active"].ToString()),
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

        public List<ReportAccessResponse> report_access_view(string series_code, string access_level_id)
        {

            series_code = Crypto.url_decrypt(series_code);


            int int_access_level_id = 0;
            try
            {
                int_access_level_id = Convert.ToInt32(access_level_id);
            }
            catch
            {
                access_level_id = Crypto.url_decrypt(access_level_id);
            }



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ReportAccessResponse> resp = new List<ReportAccessResponse>();
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
                oCmd.CommandText = "report_access_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);

                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ReportAccessResponse()
                        {
                            report_id = Convert.ToInt32(dr["report_id"].ToString()),
                            report_name = dr["report_name"].ToString(),
                            access_level_id = dr["access_level_id"].ToString(),
                            int_access_level_id = Convert.ToInt32(dr["access_level_id"].ToString()),
                            created_by = dr["created_by"].ToString(),

                            active = Convert.ToBoolean(dr["active"].ToString()),
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


        public List<ConfidentialityAccessResponse> confidentiality_access_view(string series_code, string access_level_id)
        {

            series_code = Crypto.url_decrypt(series_code);


            int int_access_level_id = 0;
            try
            {
                int_access_level_id = Convert.ToInt32(access_level_id);
            }
            catch
            {
                access_level_id = Crypto.url_decrypt(access_level_id);
            }



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ConfidentialityAccessResponse> resp = new List<ConfidentialityAccessResponse>();
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
                oCmd.CommandText = "confidentiality_access_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);

                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ConfidentialityAccessResponse()
                        {
                            confidentiality_id = Convert.ToInt32(dr["confidentiality_id"].ToString()),
                            confidentiality_name = dr["confidentiality_name"].ToString(),
                            access_level_id = dr["access_level_id"].ToString(),
                            int_access_level_id = Convert.ToInt32(dr["access_level_id"].ToString()),
                            created_by = dr["created_by"].ToString(),

                            active = Convert.ToBoolean(dr["active"].ToString()),
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

        public List<ConfidentialityAccessResponse> confidentiality_access_sel(string series_code, string access_level_id)
        {

            series_code = Crypto.url_decrypt(series_code);


            int int_access_level_id = 0;
            try
            {
                int_access_level_id = Convert.ToInt32(access_level_id);
            }
            catch
            {
                access_level_id = Crypto.url_decrypt(access_level_id);
            }



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ConfidentialityAccessResponse> resp = new List<ConfidentialityAccessResponse>();
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
                oCmd.CommandText = "confidentiality_access_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);

                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ConfidentialityAccessResponse()
                        {
                            confidentiality_id = Convert.ToInt32(dr["confidentiality_id"].ToString()),
                            confidentiality_name = dr["confidentiality_name"].ToString(),
                            access_level_id = dr["access_level_id"].ToString(),
                            int_access_level_id = Convert.ToInt32(dr["access_level_id"].ToString()),
                            created_by = dr["created_by"].ToString(),

                            active = Convert.ToBoolean(dr["active"].ToString()),
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

        public List<DataUploadAccessResponse> data_upload_access_view(string series_code, string access_level_id)
        {

             series_code = Crypto.url_decrypt(series_code);

            int int_access_level_id = 0;
            try
            {
                int_access_level_id = Convert.ToInt32(access_level_id);
            }
            catch
            {
                access_level_id = Crypto.url_decrypt(access_level_id);
            }


            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<DataUploadAccessResponse> resp = new List<DataUploadAccessResponse>();
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
                oCmd.CommandText = "data_upload_access_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new DataUploadAccessResponse()
                        {
                            data_upload_id = Convert.ToInt32(dr["data_upload_id"].ToString()),
                            access_level_id = dr["access_level_id"].ToString(),
                            int_access_level_id = Convert.ToInt32(dr["access_level_id"].ToString()),
                            data_upload_name = dr["data_upload_name"].ToString(),
                            export = dr["export"].ToString(),
                            upload = dr["upload"].ToString(),
                            save = dr["save"].ToString(),
                            service = dr["service"].ToString(),
                            created_by = dr["created_by"].ToString(),
                            active = Convert.ToBoolean(dr["active"].ToString()),

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


        public List<DataUploadAccessResponse> data_upload_access_list_view(string series_code, string access_level_id, string created_by)
        {

            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

            int int_access_level_id = 0;
            try
            {
                int_access_level_id = Convert.ToInt32(access_level_id);
            }
            catch
            {
                access_level_id = Crypto.url_decrypt(access_level_id);
            }


            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<DataUploadAccessResponse> resp = new List<DataUploadAccessResponse>();
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
                oCmd.CommandText = "data_upload_access_list_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new DataUploadAccessResponse()
                        {
                            data_upload_id = Convert.ToInt32(dr["data_upload_id"].ToString()),
                            access_level_id = dr["access_level_id"].ToString(),
                            int_access_level_id = Convert.ToInt32(dr["access_level_id"].ToString()),
                            data_upload_name = dr["data_upload_name"].ToString(),
                            export = dr["export"].ToString(),
                            upload = dr["upload"].ToString(),
                            save = dr["save"].ToString(),
                            service = dr["service"].ToString(),
                            created_by = dr["created_by"].ToString(),
                            active = Convert.ToBoolean(dr["active"].ToString()),

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

        public List<ReportAccessResponse> report_access_list_view(string series_code, string access_level_id, string created_by)
        {

            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);


            int int_access_level_id = 0;
            try
            {
                int_access_level_id = Convert.ToInt32(access_level_id);
            }
            catch
            {
                access_level_id = Crypto.url_decrypt(access_level_id);
            }



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ReportAccessResponse> resp = new List<ReportAccessResponse>();
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
                oCmd.CommandText = "report_access_list_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ReportAccessResponse()
                        {
                            report_id = Convert.ToInt32(dr["report_id"].ToString()),
                            report_name = dr["report_name"].ToString(),
                            access_level_id = dr["access_level_id"].ToString(),
                            int_access_level_id = Convert.ToInt32(dr["access_level_id"].ToString()),
                            report_type_id = Convert.ToInt32(dr["report_type_id"].ToString()),
                            report_type = dr["report_type"].ToString(),
                            link = dr["link"].ToString(),
                            created_by = dr["created_by"].ToString(),

                            active = Convert.ToBoolean(dr["active"].ToString()),
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

        public List<ConfidentialityAccessResponse> confidentiality_access_list_view(string series_code, string access_level_id, string created_by)
        {

            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);


            int int_access_level_id = 0;
            try
            {
                int_access_level_id = Convert.ToInt32(access_level_id);
            }
            catch
            {
                access_level_id = Crypto.url_decrypt(access_level_id);
            }



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ConfidentialityAccessResponse> resp = new List<ConfidentialityAccessResponse>();
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
                oCmd.CommandText = "confidentiality_access_list_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ConfidentialityAccessResponse()
                        {
                            confidentiality_id = Convert.ToInt32(dr["confidentiality_id"].ToString()),
                            confidentiality_name = dr["confidentiality_name"].ToString(),
                            access_level_id = dr["access_level_id"].ToString(),
                            int_access_level_id = Convert.ToInt32(dr["access_level_id"].ToString()),
                            created_by = dr["created_by"].ToString(),

                            active = Convert.ToBoolean(dr["active"].ToString()),
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


        public List<ApprovalAccessResponse> approval_access_view(string series_code, string access_level_id, string date_from, string date_to, string created_by)
        {

            series_code = Crypto.url_decrypt(series_code);
            access_level_id = Crypto.url_decrypt(access_level_id);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ApprovalAccessResponse> resp = new List<ApprovalAccessResponse>();
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
                oCmd.CommandText = "approval_access_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@access_level_id", access_level_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ApprovalAccessResponse()
                        {
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),
                            module_name = dr["module_name"].ToString(),
                            microservice = Convert.ToInt32(dr["microservice"].ToString()),
                            count = Convert.ToInt32(dr["count"].ToString()),

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
