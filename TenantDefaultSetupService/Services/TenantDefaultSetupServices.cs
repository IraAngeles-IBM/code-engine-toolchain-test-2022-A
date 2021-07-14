
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenantDefaultSetupService.Helper;
using TenantDefaultSetupService.Model;

namespace TenantDefaultSetupService.Services
{

    public interface ITenantDefaultSetupService
    {
        SeriesResponse series_in(string module_id,string series_code);
        List<SeriesResponse> series_multiple_in(string module_id, string series_code, int count);
        List<SeriesResponse> series_multiple_del(string module_id, string series_code, int count);
        List<Series> series_view(string series_code, string created_by);
        List<Series> series_view_next(string series_code, string created_by, int module_id);
        DropdownIUResponse DropdownIU(DropdownIURequest model);
        List<DropdownResponse> dropdown_view(string dropdown_type_id, string series_code);
        List<DropdownResponse> dropdown_view_all(string dropdown_type_id, string series_code);
        List<ApprovalSequenceResponse> approval_sequence_view(string series_code, int module_id, int approval_level_id);
        List<ApprovalSequenceResponse> transaction_approval_sequence_view(string series_code, int module_id,string employee_id, string approval_level_id);
        List<ApprovalEmailSequenceResponse> approval_sequence_email_view(string series_code, int module_id, string employee_id, string approval_level_id);
        List<ApprovalSequenceResponse> approval_sequence_status_view(string series_code, int module_id, int approval_level_id);
        int approval_sequence_in(ApprovalSequenceHeaderRequest[] model);
        List<ApprovalSequenceHeaderRequest> approval_sequence_module_view(string series_code, string modules, int approval_level_id);



        List<ApprovalHeaderResponse> transaction_approval_header(string series_code, int module_id, string created_by);
        DataTable transaction_approval_view(string series_code, int module_id, string series, string date_from, string date_to, string created_by);

        int series_up(SeriesUp model);
        int series_temp_in(SeriesTemp model);

        int dropdown_in(DropdownInRequest model);

    }
    public class TenantDefaultSetupServices : ITenantDefaultSetupService
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;



        public TenantDefaultSetupServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }



        public SeriesResponse series_in(string module_id, string series_code)
        {
            SeriesResponse resp = new SeriesResponse();

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


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
                oCmd.CommandText = "series_in";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp.series_code = sdr["series_code"].ToString();
                }
                sdr.Close();
                oTrans.Commit();
                oConn.Close();
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


        public List<SeriesResponse> series_multiple_del (string module_id, string series_code, int count)
        {
            List<SeriesResponse> resp = new List<SeriesResponse>();

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


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
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "series_multiple_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@count", count);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new SeriesResponse()
                        {
                            series_code = sdr["series_code"].ToString()

                        }).ToList();
                oTrans.Commit();
                oConn.Close();
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


        public List<SeriesResponse> series_multiple_in(string module_id, string series_code,int count)
        {
            List<SeriesResponse> resp = new List<SeriesResponse>();

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


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
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "series_multiple_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@count", count);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new SeriesResponse()
                        {
                            series_code = sdr["series_code"].ToString()

                        }).ToList();
                oTrans.Commit();
                oConn.Close();
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

        public List<Series> series_view(string series_code, string created_by)
        {
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);
            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<Series> resp = new List<Series>();
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

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "series_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new Series()
                        {
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),
                            module_name = (dr["module_name"].ToString()),
                            series = Convert.ToInt32(dr["series"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            prefix = (dr["prefix"].ToString()),
                            year = Convert.ToInt32(dr["year"].ToString()),
                            length = Convert.ToInt32(dr["length"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            series_code = (dr["series_code"].ToString()),


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


        public List<Series> series_view_next(string series_code, string created_by,int module_id)
        {
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);
            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<Series> resp = new List<Series>();
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

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "series_view_next";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new Series()
                        {
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),
                            series = Convert.ToInt32(dr["series"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            prefix = (dr["prefix"].ToString()),
                            year = Convert.ToInt32(dr["year"].ToString()),
                            length = Convert.ToInt32(dr["length"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            series_code = (dr["series_code"].ToString()),


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

        public int series_up(SeriesUp model)
        {

            var series_code = Crypto.url_decrypt(model.series_code);
            var created_by = Crypto.url_decrypt(model.created_by);
            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";

            int resp = 0;
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
                oCmd.CommandText = "series_up";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", model.module_id);
                oCmd.Parameters.AddWithValue("@series", model.series);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@prefix", model.prefix);
                oCmd.Parameters.AddWithValue("@year", model.year);
                oCmd.Parameters.AddWithValue("@length", model.length);
                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.ExecuteNonQuery();

                oTrans.Commit();
                oConn.Close();
                resp = 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);

                resp = 0;
                oTrans.Rollback();
            }
            finally
            {
                oConn.Close();
            }



            return resp;
        }


        public int series_temp_in(SeriesTemp model)
        {

            var series_code = Crypto.url_decrypt(model.series_code);
            var created_by = Crypto.url_decrypt(model.created_by);
            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";

            int resp = 0;
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
                oCmd.CommandText = "series_temp_in";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", model.module_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp = Convert.ToInt32(sdr["module_id"].ToString());

                }
                sdr.Close();


                oTrans.Commit();
                oConn.Close();
                resp = 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);

                resp = 0;
                oTrans.Rollback();
            }
            finally
            {
                oConn.Close();
            }



            return resp;
        }



        public DropdownIUResponse DropdownIU(DropdownIURequest model)
        {
            string series_code = Crypto.url_decrypt(model.series_code);
            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            DropdownIUResponse resp = new DropdownIUResponse();
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
                oCmd.CommandText = "dropdown_in_up";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", model.dropdown_id);
                oCmd.Parameters.AddWithValue("@dropdown_type_id", model.dropdown_type_id);
                oCmd.Parameters.AddWithValue("@dropdown_description", model.dropdown_description);
                oCmd.Parameters.AddWithValue("@created_by", Crypto.url_decrypt(model.created_by));
                oCmd.Parameters.AddWithValue("@active", model.active);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {
                    resp.dropdown_id = Convert.ToInt32(sdr["dropdown_id"].ToString());
                    resp.dropdown_type_id = Convert.ToInt32(sdr["dropdown_type_id"].ToString());
                }
                sdr.Close();
                oTrans.Commit();
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


        public List<DropdownResponse> dropdown_view(string dropdown_type_id,string series_code)
        {
            series_code = Crypto.url_decrypt(series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";



            List<DropdownResponse> resp = new List<DropdownResponse>();
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

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "dropdown_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_type_id", dropdown_type_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new DropdownResponse()
                        {
                            id = Convert.ToInt32(dr["id"].ToString()),
                            encrypted_id = Crypto.url_encrypt(dr["id"].ToString()),
                            string_id = (dr["id"].ToString()),
                            description = dr["description"].ToString(),
                            type_description = dr["type_description"].ToString(),
                            type_id = Convert.ToInt32(dr["type_id"].ToString()),
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


        public List<DropdownResponse> dropdown_view_all(string dropdown_type_id, string series_code)
        {
            series_code = Crypto.url_decrypt(series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";



            List<DropdownResponse> resp = new List<DropdownResponse>();
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

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "dropdown_view_all";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_type_id", dropdown_type_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new DropdownResponse()
                        {
                            id = Convert.ToInt32(dr["id"].ToString()),
                            string_id = (dr["id"].ToString()),
                            description = dr["description"].ToString(),
                            type_description = dr["type_description"].ToString(),
                            type_id = Convert.ToInt32(dr["type_id"].ToString()),
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


        public int dropdown_in(DropdownInRequest model)
        {
            var resp = 0;
            string series_code = Crypto.url_decrypt(model.series_code);
            string created_by = Crypto.url_decrypt(model.created_by);
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
                oCmd.CommandText = "dropdown_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", created_by);

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


        public List<ApprovalSequenceResponse> approval_sequence_view(string series_code,int module_id,int approval_level_id)
        {
            series_code = Crypto.url_decrypt(series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";



            List<ApprovalSequenceResponse> resp = new List<ApprovalSequenceResponse>();
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

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "approval_sequence_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@approval_level_id", approval_level_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ApprovalSequenceResponse()
                        {
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),
                            approval_level_id = Convert.ToInt32(dr["approval_level_id"].ToString()),
                            status = dr["status"].ToString(),
                            seqn = Convert.ToInt32(dr["seqn"].ToString()),
                            approver_id = Convert.ToInt32(dr["approver_id"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),

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

        public List<ApprovalSequenceResponse> transaction_approval_sequence_view(string series_code, int module_id,string employee_id, string approval_level_id)
        {
            employee_id = Crypto.url_decrypt(employee_id);
            approval_level_id = approval_level_id == "0" ? "0" : Crypto.url_decrypt(approval_level_id);
            series_code = Crypto.url_decrypt(series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";



            List<ApprovalSequenceResponse> resp = new List<ApprovalSequenceResponse>();
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

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "approval_sequence_transaction_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@approval_level_id", approval_level_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ApprovalSequenceResponse()
                        {
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),
                            approval_level_id = Convert.ToInt32(dr["approval_level_id"].ToString()),
                            status = dr["status"].ToString(),
                            seqn = Convert.ToInt32(dr["seqn"].ToString()),
                            approver_id = Convert.ToInt32(dr["approver_id"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),

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


        public List<ApprovalEmailSequenceResponse> approval_sequence_email_view(string series_code, int module_id, string employee_id, string approval_level_id)
        {
            employee_id = Crypto.url_decrypt(employee_id);
            approval_level_id = approval_level_id == "0" ? "0" : Crypto.url_decrypt(approval_level_id);
            series_code = Crypto.url_decrypt(series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";



            List<ApprovalEmailSequenceResponse> resp = new List<ApprovalEmailSequenceResponse>();
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

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "approval_sequence_email_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@approval_level_id", approval_level_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ApprovalEmailSequenceResponse()
                        {
                            is_email = Convert.ToBoolean(dr["is_email"].ToString()),
                            approver_name = dr["approver_name"].ToString(),
                            email_address = dr["email_address"].ToString(),
                            date_created = dr["date_created"].ToString(),
                            module_name = dr["module_name"].ToString(),


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


        public List<ApprovalSequenceResponse> initial_approval_sequence_view(string series_code, int module_id, string approval_level_id)
        {
            approval_level_id = approval_level_id == "0" ? "0" : Crypto.url_decrypt(approval_level_id);
            series_code = Crypto.url_decrypt(series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";



            List<ApprovalSequenceResponse> resp = new List<ApprovalSequenceResponse>();
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

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "approval_sequence_initial_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@approval_level_id", approval_level_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ApprovalSequenceResponse()
                        {
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),
                            approval_level_id = Convert.ToInt32(dr["approval_level_id"].ToString()),
                            status = dr["status"].ToString(),
                            seqn = Convert.ToInt32(dr["seqn"].ToString()),
                            approver_id = Convert.ToInt32(dr["approver_id"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),

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

        public List<ApprovalSequenceResponse> approval_sequence_status_view(string series_code, int module_id, int approval_level_id)
        {
            series_code = Crypto.url_decrypt(series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";



            List<ApprovalSequenceResponse> resp = new List<ApprovalSequenceResponse>();
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

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "approval_sequence_status_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@approval_level_id", approval_level_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ApprovalSequenceResponse()
                        {
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),
                            approval_level_id = Convert.ToInt32(dr["approval_level_id"].ToString()),
                            status = dr["type_description"].ToString()

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

        public int approval_sequence_in(ApprovalSequenceHeaderRequest[] model)
        {
            //List<BranchResponse> resp = new List<BranchResponse>();
            int resp = 0;
            string series_code = Crypto.url_decrypt(model[0].series_code);
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

                int module_id = 0;
                int approval_level_id = 0;
                string status = "";
                int seqn = 0;
                int approver_id = 0;
                var created_by = Crypto.url_decrypt(model[0].created_by);
                if (model != null)
                {

                    foreach (var item in model)
                    {
                        approval_level_id = item.approval_level_id;
                        module_id = item.module_id;

                        oCmd.CommandText = "approval_sequence_del";
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@approval_level_id", approval_level_id);
                        oCmd.Parameters.AddWithValue("@module_id", module_id);
                        oCmd.ExecuteNonQuery();

                        
                        if (item.StatusRequest != null)
                        {
                            foreach (var stat in item.StatusRequest)
                            {
                                status = stat.status;
                                seqn = stat.index;

                                if (stat.DetailRequest != null)
                                {
                                    foreach (var detail in stat.DetailRequest)
                                    {

                                        SqlDataAdapter da = new SqlDataAdapter();
                                        da.SelectCommand = oCmd;
                                        oCmd.CommandText = "approval_sequence_in";
                                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                                        oCmd.Parameters.Clear();
                                        oCmd.Parameters.AddWithValue("@module_id", module_id);
                                        oCmd.Parameters.AddWithValue("@approval_level_id", approval_level_id);
                                        oCmd.Parameters.AddWithValue("@status", status);
                                        oCmd.Parameters.AddWithValue("@seqn", seqn);
                                        oCmd.Parameters.AddWithValue("@approver_id", detail.approver_id);
                                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                                        oCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                    }
                }
                oTrans.Commit();

                resp = 1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                //resp = new List<BranchResponse>();
                oTrans.Rollback();
                resp = 0;
            }
            finally
            {
                oConn.Close();
            }



            return resp;
        }


        public List<ApprovalSequenceHeaderRequest> approval_sequence_module_view(string series_code,string modules, int approval_level_id)
        {
            List<ApprovalSequenceHeaderRequest> resp = new List<ApprovalSequenceHeaderRequest>();

           
            List<ApprovalSequenceStatusRequest> status_resp = new List<ApprovalSequenceStatusRequest>();
            List<ApprovalSequenceDetailRequest> detail_resp = new List<ApprovalSequenceDetailRequest>();


            series_code = Crypto.url_decrypt(series_code);
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
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = oCmd;
                oCmd.CommandText = "approval_sequence_module_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", modules);
                oCmd.Parameters.AddWithValue("@approval_level_id", approval_level_id);
                da.Fill(dt);
                //for(int x = 0; x < dt.Rows.Count; x++)
                //{
                //    status_resp.Add(dt.Rows[x]["type_description"].ToString(), Convert.ToInt32(dt.Rows[x]["type_id"].ToString()));
                //    resp.Add();
                //}

                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    int max = dt.Rows.Count - 1;
                    int module_id = Convert.ToInt32(dt.Rows[x]["module_id"].ToString());
                    string status = (dt.Rows[x]["status"].ToString());
                    int seqn = Convert.ToInt32(dt.Rows[x]["seqn"].ToString());
                    int approver_id = Convert.ToInt32(dt.Rows[x]["approver_id"].ToString());

                    detail_resp.Add(new ApprovalSequenceDetailRequest
                    {
                        approver_id = approver_id
                    });

                    if (x == max)
                    {
                        status_resp.Add(new ApprovalSequenceStatusRequest
                        {
                            status = (dt.Rows[x]["status"].ToString()),
                            index = Convert.ToInt32(dt.Rows[x]["seqn"].ToString()),
                            DetailRequest = detail_resp.ToArray(),
                        });

                        detail_resp = new List<ApprovalSequenceDetailRequest>();

                        resp.Add(new ApprovalSequenceHeaderRequest
                        {
                            approval_level_id = Convert.ToInt32(dt.Rows[x]["approval_level_id"].ToString()),
                            module_id = Convert.ToInt32(dt.Rows[x]["module_id"].ToString()),
                            StatusRequest = status_resp.ToArray(),

                        });
                        status_resp = new List<ApprovalSequenceStatusRequest>();

                    }
                    else
                    {

                        if (  status != (dt.Rows[x + 1]["status"].ToString()))
                        {
                            status_resp.Add(new ApprovalSequenceStatusRequest
                            {
                                status = (dt.Rows[x]["status"].ToString()),
                                index = Convert.ToInt32(dt.Rows[x]["seqn"].ToString()),
                                DetailRequest = detail_resp.ToArray(),
                            });

                            detail_resp = new List<ApprovalSequenceDetailRequest>();
                        }else if(module_id != Convert.ToInt32(dt.Rows[x + 1]["module_id"].ToString()))
                        {
                            status_resp.Add(new ApprovalSequenceStatusRequest
                            {
                                status = (dt.Rows[x]["status"].ToString()),
                                index = Convert.ToInt32(dt.Rows[x]["seqn"].ToString()),
                                DetailRequest = detail_resp.ToArray(),
                            });

                            detail_resp = new List<ApprovalSequenceDetailRequest>();
                        }


                        if (module_id != Convert.ToInt32(dt.Rows[x + 1]["module_id"].ToString()))
                        {
                            resp.Add(new ApprovalSequenceHeaderRequest
                            {
                                approval_level_id = Convert.ToInt32(dt.Rows[x]["approval_level_id"].ToString()),
                                module_id = Convert.ToInt32(dt.Rows[x]["module_id"].ToString()),
                                StatusRequest = status_resp.ToArray(),

                            });
                            status_resp = new List<ApprovalSequenceStatusRequest>();
                        }
                    }
                      
                }

                


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


        public List<ApprovalHeaderResponse> transaction_approval_header(string series_code, int module_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            //employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ApprovalHeaderResponse> resp = new List<ApprovalHeaderResponse>();
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
                oCmd.CommandText = "transaction_approval_header";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ApprovalHeaderResponse()
                        {
                            seqn = Convert.ToInt32(dr["seqn"].ToString()),
                            columns = (dr["columns"].ToString()),
                            colname = (dr["colname"].ToString()),
                            is_view = Convert.ToBoolean(dr["is_view"].ToString()),



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

        public DataTable transaction_approval_view(string series_code, int module_id,string series, string date_from, string date_to, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            //employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";

            DataSet ds = new DataSet();
            DataTable resp = new DataTable();
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
                oCmd.CommandText = "transaction_approval_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@series", series);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);

                resp = dt;
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
