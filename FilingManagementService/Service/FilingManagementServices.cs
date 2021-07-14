
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using FilingManagementService.Helper;
using FilingManagementService.Model;
using System.Data.SqlClient;

namespace FilingManagementService.Service
{

    public interface IFilingManagementServices
    {
        #region "Approval"
        ApprovalResponse approval_process_in(ApprovalSequenceRequest[] model);
        ApprovalResponse transaction_status_up(TransactionStatusRequest model);
        ApprovalResponse transaction_status_approver_up(TransactionStatusRequest model);
        ApprovalResponse transaction_approval_up(ApprovalRequest model);

        List<ApprovalEmailResponse> transaction_approval_email(string series_code, int module_id, string transaction_id, string created_by);

        #endregion

        InsertResponse official_business_in_up(OBRequest model);
        List<OBResponse> official_business_view_sel(string series_code, string official_business_id, int file_type, string created_by);

        List<OTResponse> overtime_view_sel(string series_code, string overtime_id, int file_type, string created_by);
        InsertResponse overtime_in_up(OTRequest model);
        List<OTRenderResponse> overtime_render_view(string series_code, string employee_id, string date_from, string date_to, string created_by);
        InsertResponse overtime_render_up(OTRenderRequest[] model);

        List<LeaveResponse> leave_view_sel(string series_code, string leave_id, int file_type, string created_by);
        InsertResponse leave_in_up(LeaveRequest model);


        InsertResponse offset_in_up(OffsetHeaderRequest model);
        List<OffsetHeaderResponse> offset_view_sel(string series_code, string offset_id, int file_type, string created_by);
        List<OffsetDetailResponse> offset_detail_view(string series_code, string offset_id, int file_type, string employee_id, string created_by);

        InsertResponse change_log_in_up(ChangelogHeaderRequest model);
        List<ChangelogHeaderResponse> change_log_view_sel(string series_code, string change_log_id, int file_type, string created_by);
        List<ChangelogDetailResponse> change_log_detail_view_sel(string series_code, string change_log_id, int file_type, string created_by);


        InsertResponse change_schedule_in_up(ChangeScheduleRequest model);
        List<ChangeScheduleResponse> change_schedule_view_sel(string series_code, string change_schedule_id, int file_type, string created_by);
        List<ScheduleShiftResponse> change_schedule_shift_view(string series_code, string employee_id, string shift_id, string date_from, string date_to, string created_by);
        List<EmployeeScheduleDetailRequest> change_schedule_detail_in(ChangeScheduleRequest model);
        List<ScheduleShiftResponse> change_schedule_detail_view(string series_code, string change_schedule_id, int file_type, string created_by);
        
        
        List<DashboardResponse> filing_dashboard_view(string series_code, string employee_id, bool approver, int count, string created_by);

        uploadResponse change_log_in(UploadInRequest model);
        uploadResponse change_schedule_in(UploadInRequest model);
        int leave_in(UploadInRequest model);
        int official_business_in(UploadInRequest model);
        int offset_in(UploadInRequest model);
        int overtime_in(UploadInRequest model);

        int transaction_cancel_up(CancelTransactionRequest model);


        #region "COE"
        COEIUResponse coe_request_in_up(COERequest model);
        List<COEResponse> coe_request_view_sel(string series_code, string coe_id, int file_type, string created_by);
        #endregion
    }

    public class FilingManagementServices : IFilingManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public FilingManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }

        #region "Approval and Status Change"

        public ApprovalResponse approval_process_in(ApprovalSequenceRequest[] model)
        {
            var resp = new ApprovalResponse();
            string series_code = Crypto.url_decrypt(model[0].series_code);
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
                oCmd.CommandText = "approval_process_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", model[0].module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", model[0].transaction_id);
                oCmd.ExecuteNonQuery();


                foreach (var item in model)
                {

                    da.SelectCommand = oCmd;
                    oCmd.CommandText = "approval_process_in";
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();


                    oCmd.Parameters.AddWithValue("@module_id", item.module_id);
                    oCmd.Parameters.AddWithValue("@transaction_id", item.transaction_id);
                    oCmd.Parameters.AddWithValue("@status", item.status);
                    oCmd.Parameters.AddWithValue("@seqn", item.seqn);
                    oCmd.Parameters.AddWithValue("@approver_id", item.approver_id);

                    SqlDataReader dr = oCmd.ExecuteReader();
                    while (dr.Read())
                    {

                        resp.module_id = Convert.ToInt32(dr["module_id"].ToString());
                        resp.transaction_id = Convert.ToInt32(dr["transaction_id"].ToString());

                    }

                    dr.Close();
                }


                oTrans.Commit();
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

        public ApprovalResponse transaction_status_up(TransactionStatusRequest model)
        {
            var resp = new ApprovalResponse();
            string series_code = Crypto.url_decrypt(model.series_code);
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
                oCmd.CommandText = "transaction_status_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();


                oCmd.Parameters.AddWithValue("@module_id", model.module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", model.int_transaction_id);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.module_id = Convert.ToInt32(dr["module_id"].ToString());
                    resp.transaction_id = Convert.ToInt32(dr["transaction_id"].ToString());
                    resp.approved = Convert.ToBoolean(dr["approved"].ToString());

                }
                dr.Close();


                oTrans.Commit();
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



        public ApprovalResponse transaction_approval_up(ApprovalRequest model)
        {
            var resp = new ApprovalResponse();
            string series_code = Crypto.url_decrypt(model.series_code);
            string approved_by = Crypto.url_decrypt(model.approved_by);
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
                oCmd.CommandText = "transaction_approval_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();


                oCmd.Parameters.AddWithValue("@module_id", model.module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", model.transaction_id);
                oCmd.Parameters.AddWithValue("@action", model.action);
                oCmd.Parameters.AddWithValue("@remarks", model.remarks);
                oCmd.Parameters.AddWithValue("@approved_by", approved_by);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.module_id = Convert.ToInt32(dr["module_id"].ToString());
                    resp.string_transaction_id = (dr["transaction_id"].ToString());
                    resp.approved = Convert.ToBoolean(dr["approved"].ToString());

                }
                dr.Close();


                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.module_id = 0;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }


        public List<ApprovalEmailResponse> transaction_approval_email(string series_code, int module_id, string transaction_id, string created_by)
        {
            created_by = Crypto.url_decrypt(created_by);
            //transaction_id = transaction_id == "0" ? "0" : Crypto.url_decrypt(transaction_id);
            series_code = Crypto.url_decrypt(series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";



            List<ApprovalEmailResponse> resp = new List<ApprovalEmailResponse>();
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
                oCmd.CommandText = "transaction_approval_email";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", transaction_id);
                oCmd.Parameters.AddWithValue("@approval_level_id", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ApprovalEmailResponse()
                        {
                            is_email = Convert.ToBoolean(dr["is_email"].ToString()),
                            approver_name = dr["approver_name"].ToString(),
                            email_address = dr["email_address"].ToString(),
                            date_created = dr["date_created"].ToString(),
                            module_name = dr["module_name"].ToString(),
                            code = dr["module_name"].ToString(),
                            code_date = dr["module_name"].ToString(),


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


        public ApprovalResponse transaction_status_approver_up(TransactionStatusRequest model)
        {
            var resp = new ApprovalResponse();
            string series_code = Crypto.url_decrypt(model.series_code);
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
                oCmd.CommandText = "transaction_status_approver_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();


                oCmd.Parameters.AddWithValue("@module_id", model.module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", model.transaction_id);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.module_id = Convert.ToInt32(dr["module_id"].ToString());
                    resp.string_transaction_id = (dr["transaction_id"].ToString());
                    resp.approved = Convert.ToBoolean(dr["approved"].ToString());

                }
                dr.Close();


                oTrans.Commit();
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




        #endregion

        #region "Official Business"
        public InsertResponse official_business_in_up(OBRequest model)
        {
            var resp = new InsertResponse();
            model.official_business_id = model.official_business_id == "0" ? "0" : Crypto.url_decrypt(model.official_business_id);
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
                oCmd.CommandText = "official_business_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@official_business_id", model.official_business_id);
                oCmd.Parameters.AddWithValue("@official_business_code", model.official_business_code);
                oCmd.Parameters.AddWithValue("@date_from", model.date_from);
                oCmd.Parameters.AddWithValue("@date_to", model.date_to);
                oCmd.Parameters.AddWithValue("@company_to_visit", model.company_to_visit);
                oCmd.Parameters.AddWithValue("@location", model.location);
                oCmd.Parameters.AddWithValue("@description", model.description);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.id = Convert.ToInt32(dr["official_business_id"].ToString());
                    resp.code = model.official_business_code;
                    resp.description = "Saving Successful.";

                }
                dr.Close();


                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.id = 0;
                resp.error_message = "Error: " + e.Message;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }

        public List<OBResponse> official_business_view_sel(string series_code, string official_business_id, int file_type, string created_by)
        {
            
            
            if (file_type == 0)
            {

                official_business_id = official_business_id == "0" ? "0" : Crypto.url_decrypt(official_business_id);
            }
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<OBResponse> resp = new List<OBResponse>();
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
                oCmd.CommandText = "official_business_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@official_business_id", official_business_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new OBResponse()
                        {
                            official_business_id = Convert.ToInt32(dr["official_business_id"].ToString()),
                            encrypt_official_business_id = Crypto.url_encrypt(dr["official_business_id"].ToString()),
                            official_business_code = (dr["official_business_code"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            company_to_visit = (dr["company_to_visit"].ToString()),
                            location = (dr["location"].ToString()),
                            description = (dr["description"].ToString()),
                            approved = Convert.ToBoolean(dr["approved"].ToString()),
                            status = (dr["status"].ToString()),

                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            created_by_name = (dr["created_by_name"].ToString()),
                            date_created = (dr["date_created"].ToString()),
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

        #endregion

        #region "Overtime"
        public InsertResponse overtime_in_up(OTRequest model)
        {
            var resp = new InsertResponse();
            model.overtime_id = model.overtime_id == "0" ? "0" : Crypto.url_decrypt(model.overtime_id);
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
                oCmd.CommandText = "overtime_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@overtime_id", model.overtime_id);
                oCmd.Parameters.AddWithValue("@overtime_code", model.overtime_code);
                oCmd.Parameters.AddWithValue("@overtime_type_id", model.overtime_type_id);
                oCmd.Parameters.AddWithValue("@date_from", model.date_from);
                oCmd.Parameters.AddWithValue("@date_to", model.date_to);
                oCmd.Parameters.AddWithValue("@with_break", model.with_break);
                oCmd.Parameters.AddWithValue("@break_in", model.break_in);
                oCmd.Parameters.AddWithValue("@break_out", model.break_out);
                oCmd.Parameters.AddWithValue("@description", model.description);
                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.id = Convert.ToInt32(dr["overtime_id"].ToString());
                    resp.code = model.overtime_code;
                    resp.description = "Saving Successful.";

                }
                dr.Close();


                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.id = 0;
                resp.error_message = "Error: " + e.Message;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }

        public List<OTResponse> overtime_view_sel(string series_code, string overtime_id, int file_type, string created_by)
        {
            
            if (file_type == 0)
            {

                overtime_id = overtime_id == "0" ? "0" : Crypto.url_decrypt(overtime_id);
            }
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<OTResponse> resp = new List<OTResponse>();
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
                oCmd.CommandText = "overtime_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@overtime_id", overtime_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new OTResponse()
                        {


                            overtime_id = Convert.ToInt32(dr["overtime_id"].ToString()),
                            encrypted_overtime_id = Crypto.url_encrypt(dr["overtime_id"].ToString()),
                            overtime_code = (dr["overtime_code"].ToString()),
                            overtime_type = (dr["overtime_type"].ToString()),
                            overtime_type_id = Convert.ToInt32(dr["overtime_type_id"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            render_date_from = (dr["render_date_from"].ToString()),
                            render_date_to = (dr["render_date_to"].ToString()),
                            with_break = Convert.ToBoolean(dr["with_break"].ToString()),
                            break_in = (dr["break_in"].ToString()),
                            break_out = (dr["break_out"].ToString()),


                            description = (dr["description"].ToString()),
                            approved = Convert.ToBoolean(dr["approved"].ToString()),
                            status = (dr["status"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            created_by_name = (dr["created_by_name"].ToString()),
                            date_created = (dr["date_created"].ToString()),
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


        public List<OTRenderResponse> overtime_render_view(string series_code, string employee_id, string date_from,string date_to, string created_by)
        {


            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<OTRenderResponse> resp = new List<OTRenderResponse>();
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
                oCmd.CommandText = "overtime_render_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new OTRenderResponse()
                        {
                            overtime_id     = Convert.ToInt32(dr["overtime_id"].ToString()),
                            employee_id     = Convert.ToInt32(dr["employee_id"].ToString()),
                            employee_code   = (dr["employee_code"].ToString()),
                            display_name    = (dr["display_name"].ToString()),
                            overtime_code   = (dr["overtime_code"].ToString()),
                            schedule_in     = (dr["schedule_in"].ToString()),
                            schedule_out    = (dr["schedule_out"].ToString()),
                            time_in         = (dr["time_in"].ToString()),
                            time_out        = (dr["time_out"].ToString()),
                            file_ot_in      = (dr["file_ot_in"].ToString()),
                            file_ot_out     = (dr["file_ot_out"].ToString()),
                            final_in        = (dr["final_in"].ToString()),
                            final_out       = (dr["final_out"].ToString()),
                            is_edit         = Convert.ToBoolean(dr["is_edit"].ToString()),

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


        public InsertResponse overtime_render_up(OTRenderRequest[] model)
        {
            var resp = new InsertResponse();
            //model.overtime_id = model.overtime_id == "0" ? "0" : Crypto.url_decrypt(model.overtime_id);
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
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        da.SelectCommand = oCmd;
                        oCmd.CommandText = "overtime_render_up";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();

                        oCmd.Parameters.AddWithValue("@overtime_id", item.overtime_id);
                        oCmd.Parameters.AddWithValue("@final_in", item.final_in);
                        oCmd.Parameters.AddWithValue("@final_out", item.final_out);
                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        SqlDataReader dr = oCmd.ExecuteReader();
                        while (dr.Read())
                        {

                            resp.id = Convert.ToInt32(dr["overtime_id"].ToString());
                            resp.description = "Saving Successful.";

                        }
                        dr.Close();
                    }
                }
               


                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.id = 0;
                resp.error_message = "Error: " + e.Message;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }

        #endregion

        #region "Leave"
        public InsertResponse leave_in_up(LeaveRequest model)
        {
            var resp = new InsertResponse();
            model.leave_id = model.leave_id == "0" ? "0" : Crypto.url_decrypt(model.leave_id);
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
                oCmd.CommandText = "leave_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@leave_id", model.leave_id);
                oCmd.Parameters.AddWithValue("@leave_code", model.leave_code);
                oCmd.Parameters.AddWithValue("@leave_type_id", model.leave_type_id);
                oCmd.Parameters.AddWithValue("@date_from", model.date_from);
                oCmd.Parameters.AddWithValue("@date_to", model.date_to);
                oCmd.Parameters.AddWithValue("@is_half_day", model.is_half_day);
                oCmd.Parameters.AddWithValue("@is_paid", model.is_paid);


                oCmd.Parameters.AddWithValue("@description", model.description);
                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.id = Convert.ToInt32(dr["leave_id"].ToString());
                    resp.code = model.leave_code;
                    resp.description = "Saving Successful.";

                }
                dr.Close();


                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.id = 0;
                resp.error_message = "Error: " + e.Message;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }

        public List<LeaveResponse> leave_view_sel(string series_code, string leave_id, int file_type, string created_by)
        {
            
            
            if (file_type == 0)
            {

                leave_id = leave_id == "0" ? "0" : Crypto.url_decrypt(leave_id);
            }
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<LeaveResponse> resp = new List<LeaveResponse>();
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
                oCmd.CommandText = "leave_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@leave_id", leave_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new LeaveResponse()
                        {



                            leave_id = Convert.ToInt32(dr["leave_id"].ToString()),
                            encrypted_leave_id = Crypto.url_encrypt(dr["leave_id"].ToString()),
                            leave_name = (dr["leave_name"].ToString()),
                            leave_code = (dr["leave_code"].ToString()),
                            leave_type_id = Convert.ToInt32(dr["leave_type_id"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            is_half_day = Convert.ToBoolean(dr["is_half_day"].ToString()),
                            is_paid = Convert.ToBoolean(dr["is_paid"].ToString()),
                            leave_balance = Convert.ToDecimal(dr["leave_balance"].ToString()),

                            description = (dr["description"].ToString()),
                            approved = Convert.ToBoolean(dr["approved"].ToString()),
                            status = (dr["status"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            encrypted_created_by = Crypto.url_encrypt(dr["created_by"].ToString()),
                            created_by_name = (dr["created_by_name"].ToString()),
                            date_created = (dr["date_created"].ToString()),
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

        #endregion

        #region "Offset
        public InsertResponse offset_in_up(OffsetHeaderRequest model)
        {
            var resp = new InsertResponse();
            model.offset_id = model.offset_id == "0" ? "0" : Crypto.url_decrypt(model.offset_id);
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
                oCmd.CommandText = "offset_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@offset_id", model.offset_id);
                oCmd.Parameters.AddWithValue("@offset_code", model.offset_code);
                oCmd.Parameters.AddWithValue("@date", model.date);
                oCmd.Parameters.AddWithValue("@offset_hour", model.offset_hour);
                oCmd.Parameters.AddWithValue("@reason", model.reason);

                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.id = Convert.ToInt32(dr["offset_id"].ToString());
                    resp.code = model.offset_code;
                    resp.description = "Saving Successful.";

                }
                dr.Close();

                if (model.OffsetDetail != null)
                {
                    oCmd.CommandText = "offset_detail_del";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();
                    oCmd.Parameters.AddWithValue("@offset_id", resp.id);
                    oCmd.ExecuteNonQuery();

                    foreach (var item in model.OffsetDetail)
                    {
                        oCmd.CommandText = "offset_detail_in";
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@offset_id", resp.id);
                        oCmd.Parameters.AddWithValue("@overtime_id", item.overtime_id);
                        oCmd.Parameters.AddWithValue("@offset_hour", item.offset_hour);
                        oCmd.ExecuteNonQuery();
                    }
                }

                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.id = 0;
                resp.error_message = "Error: " + e.Message;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }

        public List<OffsetHeaderResponse> offset_view_sel(string series_code, string offset_id, int file_type, string created_by)
        {
            
            if (file_type == 0)
            {

                offset_id = offset_id == "0" ? "0" : Crypto.url_decrypt(offset_id);
            }
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<OffsetHeaderResponse> resp = new List<OffsetHeaderResponse>();
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
                oCmd.CommandText = "offset_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@offset_id", offset_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new OffsetHeaderResponse()
                        {



                            offset_id = Convert.ToInt32(dr["offset_id"].ToString()),
                            encrypted_offset_id = Crypto.url_encrypt(dr["offset_id"].ToString()),
                            offset_code = (dr["offset_code"].ToString()),
                            reason = (dr["reason"].ToString()),
                            offset_hour = Convert.ToDecimal(dr["offset_hour"].ToString()),
                            date = (dr["date"].ToString()),


                            approved = Convert.ToBoolean(dr["approved"].ToString()),
                            status = (dr["status"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            created_by_name = (dr["created_by_name"].ToString()),
                            date_created = (dr["date_created"].ToString()),
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


        public List<OffsetDetailResponse> offset_detail_view(string series_code, string offset_id, int file_type, string employee_id, string created_by)
        {
            
            
            if (file_type == 0)
            {

                offset_id = offset_id == "0" ? "0" : Crypto.url_decrypt(offset_id);
            }
            employee_id = Crypto.url_decrypt(employee_id);
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<OffsetDetailResponse> resp = new List<OffsetDetailResponse>();
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
                oCmd.CommandText = "offset_detail_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@offset_id", offset_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new OffsetDetailResponse()
                        {



                            offset_id = Convert.ToInt32(dr["offset_id"].ToString()),
                            overtime_id = Convert.ToInt32(dr["overtime_id"].ToString()),
                            overtime_code = (dr["overtime_code"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            offset_hour = Convert.ToDecimal(dr["offset_hour"].ToString()),
                            overtime_hour = Convert.ToDecimal(dr["overtime_hour"].ToString()),
                            balance_hour = Convert.ToDecimal(dr["balance_hour"].ToString()),
                            is_overtime_used = Convert.ToBoolean(dr["is_overtime_used"].ToString()),
                            offset_used = Convert.ToDecimal(dr["offset_used"].ToString()),
                            //total_hour      = Convert.ToDecimal(dr["total_hour"].ToString()),
                            //used_hour       = Convert.ToDecimal(dr["used_hour"].ToString()),




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

        #endregion

        #region "Change Log"
        public InsertResponse change_log_in_up(ChangelogHeaderRequest model)
        {
            var resp = new InsertResponse();
            model.change_log_id = model.change_log_id == "0" ? "0" : Crypto.url_decrypt(model.change_log_id);
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
                oCmd.CommandText = "change_log_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@change_log_id", model.change_log_id);
                oCmd.Parameters.AddWithValue("@change_log_code", model.change_log_code);
                oCmd.Parameters.AddWithValue("@date_from", model.date_from);
                oCmd.Parameters.AddWithValue("@date_to", model.date_to);
                oCmd.Parameters.AddWithValue("@reason", model.reason);

                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.id = Convert.ToInt32(dr["change_log_id"].ToString());
                    resp.code = model.change_log_code;
                    resp.description = "Saving Successful.";

                }
                dr.Close();

                if (model.Detail != null)
                {
                    oCmd.CommandText = "change_log_detail_del";
                    oCmd.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();
                    oCmd.Parameters.AddWithValue("@change_log_id", resp.id);
                    oCmd.ExecuteNonQuery();

                    foreach (var item in model.Detail)
                    {
                        oCmd.CommandText = "change_log_detail_in";
                        oCmd.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@change_log_id", resp.id);
                        oCmd.Parameters.AddWithValue("@date", item.date);
                        oCmd.Parameters.AddWithValue("@time_in", item.time_in);
                        oCmd.Parameters.AddWithValue("@time_out", item.time_out);
                        oCmd.Parameters.AddWithValue("@remarks", item.remarks);
                        oCmd.ExecuteNonQuery();
                    }
                }

                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.id = 0;
                resp.error_message = "Error: " + e.Message;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }

        public List<ChangelogHeaderResponse> change_log_view_sel(string series_code, string change_log_id, int file_type, string created_by)
        {

            if (file_type == 0)
            {

                change_log_id = change_log_id == "0" ? "0" : Crypto.url_decrypt(change_log_id);
            }
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ChangelogHeaderResponse> resp = new List<ChangelogHeaderResponse>();
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
                oCmd.CommandText = "change_log_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@change_log_id", change_log_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ChangelogHeaderResponse()
                        {



                            encrypted_change_log_id = Crypto.url_encrypt(dr["change_log_id"].ToString()),
                            change_log_id = Convert.ToInt32(dr["change_log_id"].ToString()),
                            change_log_code = (dr["change_log_code"].ToString()),
                            reason = (dr["reason"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),



                            approved = Convert.ToBoolean(dr["approved"].ToString()),
                            status = (dr["status"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            created_by_name = (dr["created_by_name"].ToString()),
                            date_created = (dr["date_created"].ToString()),
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


        public List<ChangelogDetailResponse> change_log_detail_view_sel(string series_code, string change_log_id, int file_type, string created_by)
        {
            
            if (file_type == 0)
            {

                change_log_id = change_log_id == "0" ? "0" : Crypto.url_decrypt(change_log_id);
            }
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ChangelogDetailResponse> resp = new List<ChangelogDetailResponse>();
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
                oCmd.CommandText = "change_log_detail_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@change_log_id", change_log_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ChangelogDetailResponse()
                        {


                            change_log_id = Convert.ToInt32(dr["change_log_id"].ToString()),

                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            date = (dr["date"].ToString()),
                            time_in = (dr["time_in"].ToString()),
                            time_out = (dr["time_out"].ToString()),
                            sked_time_in = (dr["sked_time_in"].ToString()),
                            sked_time_out = (dr["sked_time_out"].ToString()),
                            remarks = (dr["remarks"].ToString()),
                            is_update = Convert.ToBoolean(dr["is_update"].ToString()),



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

        #endregion

        #region "Change Schedule"

        public InsertResponse change_schedule_in_up(ChangeScheduleRequest model)
        {
            var resp = new InsertResponse();
            model.change_schedule_id = model.change_schedule_id == "0" ? "0" : Crypto.url_decrypt(model.change_schedule_id);
            model.employee_id = Crypto.url_decrypt(model.employee_id);
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
                oCmd.CommandText = "change_schedule_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@change_schedule_id", model.change_schedule_id);
                oCmd.Parameters.AddWithValue("@change_schedule_code", model.change_schedule_code);
                oCmd.Parameters.AddWithValue("@shift_id", model.shift_id);
                oCmd.Parameters.AddWithValue("@date_from", model.date_from);
                oCmd.Parameters.AddWithValue("@date_to", model.date_to);
                oCmd.Parameters.AddWithValue("@employee_id", model.employee_id);
                oCmd.Parameters.AddWithValue("@reason", model.reason);

                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.id = Convert.ToInt32(dr["change_schedule_id"].ToString());
                    resp.code = model.change_schedule_code;
                    resp.description = "Saving Successful.";

                }
                dr.Close();

                //oCmd.CommandText = "change_schedule_detail_in";
                //da.SelectCommand.CommandType = CommandType.StoredProcedure;
                //oCmd.Parameters.Clear();

                //oCmd.Parameters.AddWithValue("@change_schedule_id", model.change_schedule_id);
                //oCmd.Parameters.AddWithValue("@shift_id", model.shift_id);
                //oCmd.Parameters.AddWithValue("@date_from", model.date_from);
                //oCmd.Parameters.AddWithValue("@date_to", model.date_to);
                //oCmd.Parameters.AddWithValue("@employee_id", model.employee_id);
                //oCmd.Parameters.AddWithValue("@created_by", created_by);
                //oCmd.ExecuteNonQuery();
                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.id = 0;
                resp.error_message = "Error: " + e.Message;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }

        public List<EmployeeScheduleDetailRequest> change_schedule_detail_in(ChangeScheduleRequest model)
        {
            var resp = new List<EmployeeScheduleDetailRequest>();
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
                oCmd.CommandText = "change_schedule_detail_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@change_schedule_id", model.change_schedule_id);
                oCmd.Parameters.AddWithValue("@shift_id", model.shift_id);
                oCmd.Parameters.AddWithValue("@date_from", model.date_from);
                oCmd.Parameters.AddWithValue("@date_to", model.date_to);
                oCmd.Parameters.AddWithValue("@employee_id", model.employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new EmployeeScheduleDetailRequest()
                        {

                            shift_id = Convert.ToInt32(dr["shift_id"].ToString()),
                            encrypt_shift_id = Crypto.url_encrypt(dr["shift_id"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            shift_code = (dr["shift_code"].ToString()),
                            shift_name = (dr["shift_name"].ToString()),
                            grace_period = Convert.ToInt32(dr["grace_period"].ToString()),
                            description = (dr["description"].ToString()),
                            is_flexi = Convert.ToBoolean(dr["is_flexi"].ToString()),
                            time_in = (dr["time_in"].ToString()),
                            time_out = (dr["time_out"].ToString()),
                            time_out_days_cover = Convert.ToInt32(dr["time_out_days_cover"].ToString()),
                            total_working_hours = Convert.ToDecimal(dr["total_working_hours"].ToString()),
                            is_rd_mon = Convert.ToBoolean(dr["is_rd_mon"].ToString()),
                            is_rd_tue = Convert.ToBoolean(dr["is_rd_tue"].ToString()),
                            is_rd_wed = Convert.ToBoolean(dr["is_rd_wed"].ToString()),
                            is_rd_thu = Convert.ToBoolean(dr["is_rd_thu"].ToString()),
                            is_rd_fri = Convert.ToBoolean(dr["is_rd_fri"].ToString()),
                            is_rd_sat = Convert.ToBoolean(dr["is_rd_sat"].ToString()),
                            is_rd_sun = Convert.ToBoolean(dr["is_rd_sun"].ToString()),
                            half_day_in = (dr["half_day_in"].ToString()),
                            half_day_in_days_cover = Convert.ToInt32(dr["half_day_in_days_cover"].ToString()),
                            half_day_out = (dr["half_day_out"].ToString()),
                            half_day_out_days_cover = Convert.ToInt32(dr["half_day_out_days_cover"].ToString()),
                            night_dif_in = (dr["night_dif_in"].ToString()),
                            night_dif_in_days_cover = Convert.ToInt32(dr["night_dif_in_days_cover"].ToString()),
                            night_dif_out = (dr["night_dif_out"].ToString()),
                            night_dif_out_days_cover = Convert.ToInt32(dr["night_dif_out_days_cover"].ToString()),
                            first_break_in = (dr["first_break_in"].ToString()),
                            first_break_in_days_cover = Convert.ToInt32(dr["first_break_in_days_cover"].ToString()),
                            first_break_out = (dr["first_break_out"].ToString()),
                            first_break_out_days_cover = Convert.ToInt32(dr["first_break_out_days_cover"]),
                            second_break_in = (dr["second_break_in"].ToString()),
                            second_break_in_days_cover = Convert.ToInt32(dr["second_break_in_days_cover"].ToString()),
                            second_break_out = (dr["second_break_out"].ToString()),
                            second_break_out_days_cover = Convert.ToInt32(dr["second_break_out_days_cover"].ToString()),
                            third_break_in = (dr["third_break_in"].ToString()),
                            third_break_in_days_cover = Convert.ToInt32(dr["third_break_in_days_cover"].ToString()),
                            third_break_out = (dr["third_break_out"].ToString()),
                            third_break_out_days_cover = Convert.ToInt32(dr["third_break_out_days_cover"].ToString()),
                            created_by = model.created_by,
                            series_code = model.series_code,


                        }).ToList();
                //SqlDataReader dr = oCmd.ExecuteReader();
                //while (dr.Read())
                //{

                //    resp.shift_id = Convert.ToInt32(dr["shift_id"].ToString());
                //    resp.encrypt_shift_id =  Crypto.url_encrypt(dr["shift_id"].ToString());
                //    resp.employee_id = Convert.ToInt32(dr["employee_id"].ToString());
                //    resp.date_from =  (dr["date_from"].ToString());
                //    resp.date_to =  (dr["date_to"].ToString());
                //    resp.shift_code =  (dr["shift_code"].ToString());
                //    resp.shift_name =  (dr["shift_name"].ToString());
                //    resp.grace_period = Convert.ToInt32(dr["grace_period"].ToString());
                //    resp.description =  (dr["description"].ToString());
                //    resp.is_flexi = Convert.ToBoolean(dr["is_flexi"].ToString());
                //    resp.time_in =  (dr["time_in"].ToString());
                //    resp.time_out =  (dr["time_out"].ToString());
                //    resp.time_out_days_cover = Convert.ToInt32(dr["time_out_days_cover"].ToString());
                //    resp.total_working_hours = Convert.ToDecimal(dr["total_working_hours"].ToString());
                //    resp.is_rd_mon =  Convert.ToBoolean(dr["is_rd_mon"].ToString());
                //    resp.is_rd_tue =  Convert.ToBoolean(dr["is_rd_tue"].ToString());
                //    resp.is_rd_wed =  Convert.ToBoolean(dr["is_rd_wed"].ToString());
                //    resp.is_rd_thu =  Convert.ToBoolean(dr["is_rd_thu"].ToString());
                //    resp.is_rd_fri =  Convert.ToBoolean(dr["is_rd_fri"].ToString());
                //    resp.is_rd_sat =  Convert.ToBoolean(dr["is_rd_sat"].ToString());
                //    resp.is_rd_sun = Convert.ToBoolean(dr["is_rd_sun"].ToString());
                //    resp.half_day_in =  (dr["half_day_in"].ToString());
                //    resp.half_day_in_days_cover = Convert.ToInt32(dr["half_day_in_days_cover"].ToString());
                //    resp.half_day_out =  (dr["half_day_out"].ToString());
                //    resp.half_day_out_days_cover = Convert.ToInt32(dr["half_day_out_days_cover"].ToString());
                //    resp.night_dif_in =  (dr["night_dif_in"].ToString());
                //    resp.night_dif_in_days_cover = Convert.ToInt32(dr["night_dif_in_days_cover"].ToString());
                //    resp.night_dif_out =  (dr["night_dif_out"].ToString());
                //    resp.night_dif_out_days_cover = Convert.ToInt32(dr["night_dif_out_days_cover"].ToString());
                //    resp.first_break_in =  (dr["first_break_in"].ToString());
                //    resp.first_break_in_days_cover = Convert.ToInt32(dr["first_break_in_days_cover"].ToString());
                //    resp.first_break_out =  (dr["first_break_out"].ToString());
                //    resp.first_break_out_days_cover = Convert.ToInt32(dr["first_break_out_days_cover"]);
                //    resp.second_break_in =  (dr["second_break_in"].ToString());
                //    resp.second_break_in_days_cover = Convert.ToInt32(dr["second_break_in_days_cover"].ToString());
                //    resp.second_break_out =  (dr["second_break_out"].ToString());
                //    resp.second_break_out_days_cover = Convert.ToInt32(dr["second_break_out_days_cover"].ToString());
                //    resp.third_break_in =  (dr["third_break_in"].ToString());
                //    resp.third_break_in_days_cover = Convert.ToInt32(dr["third_break_in_days_cover"].ToString());
                //    resp.third_break_out =  (dr["third_break_out"].ToString());
                //    resp.third_break_out_days_cover = Convert.ToInt32(dr["third_break_out_days_cover"].ToString());
                //    resp.created_by =  model.created_by;
                //    resp.series_code =  model.series_code;

                //}
                //dr.Close();
                oTrans.Commit();
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

        public List<ChangeScheduleResponse> change_schedule_view_sel(string series_code, string change_schedule_id, int file_type, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            if (file_type == 0)
            {

                change_schedule_id = change_schedule_id == "0" ? "0" : Crypto.url_decrypt(change_schedule_id);
            }


            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ChangeScheduleResponse> resp = new List<ChangeScheduleResponse>();
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
                oCmd.CommandText = "change_schedule_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@change_schedule_id", change_schedule_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ChangeScheduleResponse()
                        {



                            change_schedule_id = Convert.ToInt32(dr["change_schedule_id"].ToString()),
                            encrypted_change_schedule_id = Crypto.url_encrypt(dr["change_schedule_id"].ToString()),
                            change_schedule_code = (dr["change_schedule_code"].ToString()),
                            shift_id = Convert.ToInt32(dr["shift_id"].ToString()),
                            reason = (dr["reason"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),

                            approved = Convert.ToBoolean(dr["approved"].ToString()),
                            status = (dr["status"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            created_by_name = (dr["created_by_name"].ToString()),
                            date_created = (dr["date_created"].ToString()),
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

        public List<ScheduleShiftResponse> change_schedule_shift_view(string series_code, string employee_id, string shift_id, string date_from, string date_to, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ScheduleShiftResponse> resp = new List<ScheduleShiftResponse>();
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
                oCmd.CommandText = "change_schedule_shift_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@shift_id", shift_id);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ScheduleShiftResponse()
                        {

                            date = (dr["date"].ToString()),
                            sked_time_in = (dr["sked_time_in"].ToString()),
                            sked_time_out = (dr["sked_time_out"].ToString()),
                            time_in = (dr["time_in"].ToString()),
                            time_out = (dr["time_out"].ToString()),



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


        public List<ScheduleShiftResponse> change_schedule_detail_view(string series_code, string change_schedule_id, int file_type, string created_by)
        {
            if (file_type == 0)
            {

                change_schedule_id = change_schedule_id == "0" ? "0" : Crypto.url_decrypt(change_schedule_id);
            }

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ScheduleShiftResponse> resp = new List<ScheduleShiftResponse>();
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
                oCmd.CommandText = "change_schedule_detail_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@change_schedule_id", change_schedule_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ScheduleShiftResponse()
                        {

                            date = (dr["date"].ToString()),
                            sked_time_in = (dr["sked_time_in"].ToString()),
                            sked_time_out = (dr["sked_time_out"].ToString()),
                            time_in = (dr["time_in"].ToString()),
                            time_out = (dr["time_out"].ToString()),



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


        #endregion


        #region "COE"
        public COEIUResponse coe_request_in_up(COERequest model)
        {
            COEIUResponse resp = new COEIUResponse();
            model.coe_id = model.coe_id == "0" ? "0" : Crypto.url_decrypt(model.coe_id);
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
                oCmd.CommandText = "coe_request_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@coe_id", model.coe_id);
                oCmd.Parameters.AddWithValue("@coe_code", model.coe_code);
                oCmd.Parameters.AddWithValue("@reason", model.reason);
                oCmd.Parameters.AddWithValue("@with_pay", model.with_pay);
                oCmd.Parameters.AddWithValue("@purpose_id", model.purpose_id);
                oCmd.Parameters.AddWithValue("@coe_path", model.coe_path);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.id = Convert.ToInt32(dr["coe_id"].ToString());
                    resp.encrypt_id = Crypto.url_encrypt(dr["coe_id"].ToString());
                    resp.code = model.coe_code;
                    resp.description = "Saving Successful.";
                    resp.company_name          = (dr["company_name"].ToString());
                    resp.company_logo          = (dr["company_logo"].ToString());
                    resp.company_address       = (dr["company_address"].ToString());
                    resp.company_address2      = (dr["company_address2"].ToString());
                    resp.company_address3      = (dr["company_address3"].ToString());
                    resp.email_address         = (dr["email_address"].ToString());
                    resp.monthly_rate          = Convert.ToDecimal(dr["monthly_rate"].ToString());
                    resp.annual_compensation   = Convert.ToDecimal(dr["annual_compensation"].ToString());
                    resp.salutation            = (dr["salutation"].ToString());
                    resp.first_name            = (dr["first_name"].ToString());
                    resp.middle_name           = (dr["middle_name"].ToString());
                    resp.last_name             = (dr["last_name"].ToString());
                    resp.suffix                = (dr["suffix"].ToString());
                    resp.position              = (dr["position"].ToString());
                    resp.philhealth            = (dr["philhealth"].ToString());
                    resp.is_email              = Convert.ToBoolean(dr["is_email"].ToString());
                    resp.purpose_id            = Convert.ToInt32(dr["purpose_id"].ToString());
                    resp.purpose               = (dr["purpose"].ToString());
                    resp.signatory_1 = (dr["signatory_1"].ToString());
                    resp.signatory_1_path = (dr["signatory_1_path"].ToString());
                    resp.signatory_2 = (dr["signatory_2"].ToString());
                    resp.signatory_2_path = (dr["signatory_2_path"].ToString());
                    resp.signatory_3 = (dr["signatory_3"].ToString());
                    resp.signatory_3_path = (dr["signatory_3_path"].ToString());
                }
                dr.Close();


                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.id = 0;
                resp.description = "Error: " + e.Message;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }

        public List<COEResponse> coe_request_view_sel(string series_code, string coe_id,int file_type,  string created_by)
        {

            if (file_type == 0)
            {

                coe_id = coe_id == "0" ? "0" : Crypto.url_decrypt(coe_id);
            }
            
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<COEResponse> resp = new List<COEResponse>();
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
                oCmd.CommandText = "coe_request_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@coe_id", coe_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new COEResponse()
                        {
                            approved = (dr["approved"].ToString()),
                            approved_bit = Convert.ToBoolean(dr["approved_bit"].ToString()),
                            status = (dr["status"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            created_by_name = (dr["created_by_name"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            with_pay = Convert.ToBoolean(dr["with_pay"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),

                            coe_id               = Convert.ToInt32(dr["coe_id"].ToString()),
                            encrypted_coe_id     = Crypto.url_encrypt(dr["coe_id"].ToString()),
                            coe_code             = (dr["coe_code"].ToString()),
                            reason               = (dr["reason"].ToString()),
                            purpose              = (dr["purpose"].ToString()),
                            purpose_id           = Convert.ToInt32(dr["purpose_id"].ToString()),
                            coe_path             = (dr["coe_path"].ToString()),
                            company_name          = (dr["company_name"].ToString()),
                            company_logo          = (dr["company_logo"].ToString()),
                            company_address       = (dr["company_address"].ToString()),
                            company_address2      = (dr["company_address2"].ToString()),
                            company_address3      = (dr["company_address3"].ToString()),
                            email_address         = (dr["email_address"].ToString()),
                            monthly_rate          = Convert.ToDecimal(dr["monthly_rate"].ToString()),
                            annual_compensation   = Convert.ToDecimal(dr["annual_compensation"].ToString()),
                            salutation            = (dr["salutation"].ToString()),
                            first_name            = (dr["first_name"].ToString()),
                            middle_name           = (dr["middle_name"].ToString()),
                            last_name             = (dr["last_name"].ToString()),
                            suffix                = (dr["suffix"].ToString()),
                            position              = (dr["position"].ToString()),
                            philhealth            = (dr["philhealth"].ToString()),
                            corporate_philhealth  = (dr["corporate_philhealth"].ToString()),
                            signatory_1           = (dr["signatory_1"].ToString()),
                            signatory_1_path      = (dr["signatory_1_path"].ToString()),
                            signatory_1_title     = (dr["signatory_1_title"].ToString()),
                            signatory_1_file_name = (dr["signatory_1_file_name"].ToString()),
                            signatory_2           = (dr["signatory_2"].ToString()),
                            signatory_2_path      = (dr["signatory_2_path"].ToString()),
                            signatory_2_title     = (dr["signatory_2_title"].ToString()),
                            signatory_2_file_name = (dr["signatory_2_file_name"].ToString()),
                            signatory_3           = (dr["signatory_3"].ToString()),
                            signatory_3_path      = (dr["signatory_3_path"].ToString()),
                            signatory_3_title     = (dr["signatory_3_title"].ToString()),
                            signatory_3_file_name = (dr["signatory_3_file_name"].ToString()),
                            telephone             = (dr["telephone"].ToString()),
                            date_hired            = (dr["date_hired"].ToString()),



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

        #endregion

        #region "upload saving"

        public uploadResponse change_log_in(UploadInRequest model)
        {
            uploadResponse resp = new uploadResponse();
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
                oCmd.CommandText = "change_log_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.created_by = Convert.ToInt32(dr["created_by"].ToString());
                    resp.transaction_id = (dr["transaction_id"].ToString());

                }
                dr.Close();


                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.created_by = 0;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }


        public uploadResponse change_schedule_in(UploadInRequest model)
        {
            uploadResponse resp = new uploadResponse();
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
                oCmd.CommandText = "change_schedule_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp.created_by = Convert.ToInt32(dr["created_by"].ToString());
                    resp.transaction_id = (dr["transaction_id"].ToString());

                }
                dr.Close();


                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                oTrans.Rollback();
                resp.created_by = 0;
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }


        public int leave_in(UploadInRequest model)
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
                oCmd.CommandText = "leave_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp = Convert.ToInt32(dr["created_by"].ToString());

                }
                dr.Close();


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


        public int official_business_in(UploadInRequest model)
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
                oCmd.CommandText = "official_business_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp = Convert.ToInt32(dr["created_by"].ToString());

                }
                dr.Close();


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


        public int offset_in(UploadInRequest model)
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
                oCmd.CommandText = "offset_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp = Convert.ToInt32(dr["created_by"].ToString());

                }
                dr.Close();


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


        public int overtime_in(UploadInRequest model)
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
                oCmd.CommandText = "overtime_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp = Convert.ToInt32(dr["created_by"].ToString());

                }
                dr.Close();


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
        #endregion



        public int transaction_cancel_up(CancelTransactionRequest model)
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
                oCmd.CommandText = "transaction_cancel_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", model.module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", model.transaction_id);
                oCmd.Parameters.AddWithValue("@remarks", model.remarks);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                SqlDataReader dr = oCmd.ExecuteReader();
                while (dr.Read())
                {

                    resp = Convert.ToInt32(dr["module_id"].ToString());

                }
                dr.Close();


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

        #region "Dashboard"

        public List<DashboardResponse> filing_dashboard_view(string series_code, string employee_id,bool approver, int count, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<DashboardResponse> resp = new List<DashboardResponse>();
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
                oCmd.CommandText = "filing_dashboard_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@approver", approver);
                oCmd.Parameters.AddWithValue("@count", count);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new DashboardResponse()
                        {
                            title = (dr["title"].ToString()),
                            backgroundColor = (dr["backgroundcolor"].ToString()),
                            id = Crypto.url_encrypt(dr["id"].ToString()),
                            status = (dr["status"].ToString()),
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),



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

        #endregion
    }



}
