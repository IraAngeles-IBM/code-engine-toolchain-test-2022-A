
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using DataUploadManagementService.Helper;
using DataUploadManagementService.Model;
using System.Data.SqlClient;
using System.Reflection;

namespace DataUploadManagementService.Service
{

    public interface IDataUploadManagementServices
    {
        List<AttendanceLog> attendance_log_temp(List<AttendanceLog> model, string series_code);
        List<DataUploadHeaderResponse> data_upload_header(string series_code, int dropdown_id, string created_by);
        DataTable data_upload_view(string series_code, int dropdown_id, string created_by);
        int data_upload_del(string series_code, int dropdown_id, string created_by);

        int dropdown_upload(List<DropdownUpload> model, string series_code, int dropdown_id, string created_by);
        int employee_upload(List<EmployeeUpload> model, string series_code, int dropdown_id, string created_by);
        int employee_information_upload(List<EmployeeInformationUpload> model, string series_code, int dropdown_id, string created_by);

        int changelog_upload(List<ChangelogUpload> model, string series_code, int dropdown_id, string created_by);
        int changeschedule_upload(List<ChangeScheduleUpload> model, string series_code, int dropdown_id, string created_by);
        int leave_upload(List<LeaveUpload> model, string series_code, int dropdown_id, string created_by);
        int leave_balance_upload(List<LeaveBalanceUpload> model, string series_code, int dropdown_id, string created_by);
        int ob_upload(List<OBUpload> model, string series_code, int dropdown_id, string created_by);
        int overtime_upload(List<OvertimeUpload> model, string series_code, int dropdown_id, string created_by);
        int offset_upload(List<OffsetUpload> model, string series_code, int dropdown_id, string created_by);

        int loan_upload(List<LoanUpload> model, string series_code, int dropdown_id, string created_by);
        int ad_upload(List<ADUpload> model, string series_code, int dropdown_id, string created_by);


        #region"Uploader"
        int timekeeping_upload_del(string series_code, string payroll_header_id, string created_by);
        int timekeeping_upload(List<TimekeepingUpload> model, string series_code, string created_by);
        #endregion

    }

    public class DataUploadManagementServices : IDataUploadManagementServices
    {


        private connectionString connection { get; set; }

        private readonly AppSetting _appSettings;

        public DataUploadManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }

        public List<DataUploadHeaderResponse> data_upload_header(string series_code, int dropdown_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            //employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<DataUploadHeaderResponse> resp = new List<DataUploadHeaderResponse>();
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
                oCmd.CommandText = "data_upload_header";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new DataUploadHeaderResponse()
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

        public DataTable data_upload_view(string series_code, int dropdown_id,  string created_by)
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
                oCmd.CommandText = "data_upload_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
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



        #region "Uploading"

        public int data_upload_del( string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();



                oTrans.Commit();
                resp = 1;
                oConn.Close();
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

        public List<AttendanceLog> attendance_log_temp(List<AttendanceLog> model, string series_code)
        {
            List<AttendanceLog> resp = new List<AttendanceLog>();
            series_code = Crypto.url_decrypt(series_code);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "attendance_log_temp_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", model[0].created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "attendance_log_temp";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();

                da.SelectCommand = oCmd;
                oCmd.CommandText = "attendance_log_temp_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", model[0].created_by);
                dt.Clear();
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new AttendanceLog()
                        {
                            bio_id = (dr["bio_id"].ToString()),
                            date_time = (dr["date_time"].ToString()),
                            in_out = Convert.ToInt32(dr["in_out"].ToString()),
                            terminal_id = (dr["terminal_id"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),


                        }).ToList();



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

        public int dropdown_upload(List<DropdownUpload> model, string series_code, int dropdown_id,string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "dropdown";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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

        public int employee_upload(List<EmployeeUpload> model, string series_code, int dropdown_id,string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "employee";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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

        public int employee_information_upload(List<EmployeeInformationUpload> model, string series_code, int dropdown_id,string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "employee_information";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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

        public int changelog_upload(List<ChangelogUpload> model, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "change_log";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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
        public int changeschedule_upload(List<ChangeScheduleUpload> model, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "change_schedule";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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

        public int leave_upload(List<LeaveUpload> model, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "leave";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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
        public int leave_balance_upload(List<LeaveBalanceUpload> model, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "leave_balance";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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
        public int ob_upload(List<OBUpload> model, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "official_business";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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
        public int overtime_upload(List<OvertimeUpload> model, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "overtime";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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
        public int offset_upload(List<OffsetUpload> model, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "offset";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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


        public int loan_upload(List<LoanUpload> model, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "loan";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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
        public int ad_upload(List<ADUpload> model, string series_code, int dropdown_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "data_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();


                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "payroll_recurring";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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



        public int timekeeping_upload_del(string series_code, string payroll_header_id, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "timekeeping_upload_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id ", payroll_header_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.ExecuteNonQuery();



                oTrans.Commit();
                resp = 1;
                oConn.Close();
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

        public int timekeeping_upload(List<TimekeepingUpload> model, string series_code, string created_by)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);

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
            da.SelectCommand = oCmd;
            try
            {

                //oCmd.CommandText = "data_upload_del";
                //da.SelectCommand.CommandType = CommandType.StoredProcedure;
                //oCmd.Parameters.Clear();
                //oCmd.Parameters.AddWithValue("@dropdown_id", dropdown_id);
                //oCmd.Parameters.AddWithValue("@created_by", created_by);
                //oCmd.ExecuteNonQuery();

                DataSet ds = new DataSet();
                dt = ToDataTable(model);

                ds.Tables.Add(dt);
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "timekeeping_upload";
                    bulkCopy.WriteToServer(dt);
                }

                oTrans.Commit();
                resp = 1;
                oConn.Close();
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

        #endregion







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
