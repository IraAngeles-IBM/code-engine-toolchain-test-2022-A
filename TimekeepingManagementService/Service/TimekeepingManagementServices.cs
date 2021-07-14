
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using TimekeepingManagementService.Helper;
using TimekeepingManagementService.Model;
using System.Data.SqlClient;

namespace TimekeepingManagementService.Service
{

    public interface ITimekeepingManagementServices
    {

        #region "Approval"
        ApprovalResponse approval_process_in(ApprovalSequenceRequest[] model);
        ApprovalResponse transaction_status_up(TransactionStatusRequest model);
        #endregion
        List<PayrollCutoffResponse> payroll_cutoff_view(string series_code, int cutoff_id, int month_id, string created_by);
        InsertResponse timekeeping_header_in_up(TimekeepingHeaderRequest model);
        List<TimekeepingHeaderResponse> timekeeping_header_view_sel(string series_code, string timekeeping_header_id, string created_by);
        List<TimekeepingHeaderResponse> timekeeping_header_view(string series_code, string timekeeping_header_id, string created_by);

        List<TimekeepingGenerationResponse> timekeeping_generation_view(string series_code, string timekeeping_header_id, string created_by);
        List<TimekeepingGenerationResponse> timekeeping_generation_employee(string series_code, string employee_id, string date_from, string date_to, string created_by);
        InsertResponse timekeeping_in_up(TimekeepingRequest model);

        List<TimekeepingResponse> timekeeping_view_sel(string series_code, string timekeeping_header_id, string timekeeping_id, string created_by);
        List<TimekeepingGenerationResponse> timekeeping_detail_view_sel(string series_code, string timekeeping_header_id, string timekeeping_id, string created_by);
        List<TimekeepingGenerationResponse> timekeeping_final_view_sel(string series_code, string timekeeping_header_id, string employee_id, string date_from, string date_to, string created_by);

        List<PayrollCutoffSelResponse> payroll_cutoff_sel(string series_code, int payroll_cutoff_id, string created_by);
        InsertResponse payroll_cutoff_up(PayrollCutoffRequest[] model);
    }

    public class TimekeepingManagementServices : ITimekeepingManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public TimekeepingManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }

        #region "Approval"

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

                    SqlDataReader sdr = oCmd.ExecuteReader();
                    while (sdr.Read())
                    {

                        resp.module_id = Convert.ToInt32(sdr["module_id"].ToString());
                        resp.transaction_id = Convert.ToInt32(sdr["transaction_id"].ToString());

                    }

                    sdr.Close();
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
                oCmd.Parameters.AddWithValue("@transaction_id", model.transaction_id);

                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp.module_id = Convert.ToInt32(sdr["module_id"].ToString());
                    resp.transaction_id = Convert.ToInt32(sdr["transaction_id"].ToString());
                    resp.approved = Convert.ToBoolean(sdr["approved"].ToString());

                }
                sdr.Close();


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

         #region "Timekeeping"
        public InsertResponse timekeeping_header_in_up(TimekeepingHeaderRequest model)
        {
            var resp = new InsertResponse();
            model.timekeeping_header_id = model.timekeeping_header_id == "0" ? "0" : Crypto.url_decrypt(model.timekeeping_header_id);
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
                oCmd.CommandText = "timekeeping_header_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", model.timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@timekeeping_header_code", model.timekeeping_header_code);
                oCmd.Parameters.AddWithValue("@date_from", model.date_from);
                oCmd.Parameters.AddWithValue("@date_to", model.date_to);
                oCmd.Parameters.AddWithValue("@payroll_type_id",model.payroll_type_id);
                oCmd.Parameters.AddWithValue("@cutoff_id",model.cutoff_id);
                oCmd.Parameters.AddWithValue("@month_id",model.month_id);
                oCmd.Parameters.AddWithValue("@category_id",model.category_id);
                oCmd.Parameters.AddWithValue("@branch_id",model.branch_id);
                oCmd.Parameters.AddWithValue("@confidentiality_id",model.confidentiality_id);


                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp.id = Convert.ToInt32(sdr["timekeeping_header_id"].ToString());
                    resp.encrypted_id = Crypto.url_encrypt(sdr["timekeeping_header_id"].ToString());
                    resp.code = model.timekeeping_header_code;
                    resp.description = "Saving Successful.";

                }
                sdr.Close();


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

        public List<TimekeepingHeaderResponse> timekeeping_header_view_sel(string series_code, string timekeeping_header_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            timekeeping_header_id = timekeeping_header_id == "0" ? "0" : Crypto.url_decrypt(timekeeping_header_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<TimekeepingHeaderResponse> resp = new List<TimekeepingHeaderResponse>();
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
                oCmd.CommandText = "timekeeping_header_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new TimekeepingHeaderResponse()
                        {
                            timekeeping_header_id = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            encrypt_timekeeping_header_id = Crypto.url_encrypt(dr["timekeeping_header_id"].ToString()),
                            timekeeping_header_code = (dr["timekeeping_header_code"].ToString()),
                            display_name            = (dr["display_name"].ToString()),
                            date_from               = (dr["date_from"].ToString()),
                            date_to                 = (dr["date_to"].ToString()),
                            payroll_type_id         = Convert.ToInt32(dr["payroll_type_id"].ToString()),
                            payroll_type            = (dr["payroll_type"].ToString()),
                            cutoff_id               = Convert.ToInt32(dr["cutoff_id"].ToString()),
                            month_id                = Convert.ToInt32(dr["month_id"].ToString()),
                            category_id             = Convert.ToInt32(dr["category_id"].ToString()),
                            branch_id               = Convert.ToInt32(dr["branch_id"].ToString()),
                            confidentiality_id      = Convert.ToInt32(dr["confidentiality_id"].ToString()),

                            cutoff                  = (dr["cutoff"].ToString()),
                            month                   = (dr["month"].ToString()),
                            category                = (dr["category_name"].ToString()),
                            branch                  = (dr["branch"].ToString()),
                            confidentiality         = (dr["confidentiality"].ToString()),
                            tk_count = Convert.ToInt32(dr["tk_count"].ToString()),

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


        public List<TimekeepingHeaderResponse> timekeeping_header_view(string series_code, string timekeeping_header_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            timekeeping_header_id = timekeeping_header_id == "0" ? "0" : Crypto.url_decrypt(timekeeping_header_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<TimekeepingHeaderResponse> resp = new List<TimekeepingHeaderResponse>();
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
                oCmd.CommandText = "timekeeping_header_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new TimekeepingHeaderResponse()
                        {
                            timekeeping_header_id = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            encrypt_timekeeping_header_id = Crypto.url_encrypt(dr["timekeeping_header_id"].ToString()),
                            timekeeping_header_code = (dr["timekeeping_header_code"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            payroll_type_id = Convert.ToInt32(dr["payroll_type_id"].ToString()),
                            payroll_type = (dr["payroll_type"].ToString()),
                            cutoff_id = Convert.ToInt32(dr["cutoff_id"].ToString()),
                            month_id = Convert.ToInt32(dr["month_id"].ToString()),
                            category_id = Convert.ToInt32(dr["category_id"].ToString()),
                            branch_id = Convert.ToInt32(dr["branch_id"].ToString()),
                            confidentiality_id = Convert.ToInt32(dr["confidentiality_id"].ToString()),

                            cutoff = (dr["cutoff"].ToString()),
                            month = (dr["month"].ToString()),
                            category = (dr["category_name"].ToString()),
                            branch = (dr["branch"].ToString()),
                            confidentiality = (dr["confidentiality"].ToString()),
                            tk_count = Convert.ToInt32(dr["tk_count"].ToString()),

                            approved = Convert.ToBoolean(dr["approved"].ToString()),
                            status = (dr["status"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
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

        public List<TimekeepingGenerationResponse> timekeeping_generation_view(string series_code, string timekeeping_header_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            timekeeping_header_id = timekeeping_header_id == "0" ? "0" : Crypto.url_decrypt(timekeeping_header_id);
            series_code = Crypto.url_decrypt(series_code);
            



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<TimekeepingGenerationResponse> resp = new List<TimekeepingGenerationResponse>();
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
                oCmd.CommandText = "timekeeping_generation_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new TimekeepingGenerationResponse()
                        {
                            timekeeping_header_id       = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            employee_id                 = Convert.ToInt32(dr["employee_id"].ToString()),
                            employee_code               = (dr["employee_code"].ToString()),
                            display_name                = (dr["display_name"].ToString()),
                            date                        = (dr["date"].ToString()),
                            rest_day                    = (dr["rest_day"].ToString()),
                            actual_time_in              = (dr["actual_time_in"].ToString()),
                            actual_time_out             = (dr["actual_time_out"].ToString()),
                            time_in                     = (dr["time_in"].ToString()),
                            time_out                    = (dr["time_out"].ToString()),
                            official_business_in        = (dr["official_business_in"].ToString()),
                            official_business_out       = (dr["official_business_out"].ToString()),
                            overtime_hour               = Convert.ToDecimal(dr["overtime_hour"].ToString()),
                            offset_hour                 = Convert.ToDecimal(dr["offset_hour"].ToString()),
                            vl_hour                     = Convert.ToDecimal(dr["vl_hour"].ToString()),
                            sl_hour                     = Convert.ToDecimal(dr["sl_hour"].ToString()),
                            otherl_hour                 = Convert.ToDecimal(dr["otherl_hour"].ToString()),
                            lwop_hour                   = Convert.ToDecimal(dr["lwop_hour"].ToString()),
                            is_rest_day                 = Convert.ToBoolean(dr["is_rest_day"].ToString()),
                            holiday_type                = Convert.ToInt32(dr["holiday_type"].ToString()),
                            holiday_count                = Convert.ToInt32(dr["holiday_count"].ToString()),
                            schedule_time_in            = (dr["schedule_time_in"].ToString()),
                            schedule_time_out           = (dr["schedule_time_out"].ToString()),
                            schedule_hour               = Convert.ToDecimal(dr["schedule_hour"].ToString()),
                            schedule_break_hour         = Convert.ToDecimal(dr["schedule_break_hour"].ToString()),
                            is_absent                   = Convert.ToBoolean(dr["is_absent"].ToString()),
                            working_hour                = Convert.ToDecimal(dr["working_hour"].ToString()),
                            break_hour                  = Convert.ToDecimal(dr["break_hour"].ToString()),
                            late                        = Convert.ToDecimal(dr["late"].ToString()),
                            undertime                   = Convert.ToDecimal(dr["undertime"].ToString()),
                            first_break_late            = Convert.ToDecimal(dr["first_break_late"].ToString()),
                            first_break_undertime       = Convert.ToDecimal(dr["first_break_undertime"].ToString()),
                            second_break_late           = Convert.ToDecimal(dr["second_break_late"].ToString()),
                            second_break_undertime      = Convert.ToDecimal(dr["second_break_undertime"].ToString()),
                            third_break_late            = Convert.ToDecimal(dr["third_break_late"].ToString()),
                            third_break_undertime       = Convert.ToDecimal(dr["third_break_undertime"].ToString()),
                            total_break_late            = Convert.ToDecimal(dr["total_break_late"].ToString()),
                            total_break_undertime       = Convert.ToDecimal(dr["total_break_undertime"].ToString()),
                            remarks                     = (dr["remarks"].ToString()),
                            first_break_in              = (dr["first_break_in"].ToString()),
                            first_break_out             = (dr["first_break_out"].ToString()),
                            second_break_in             = (dr["second_break_in"].ToString()),
                            second_break_out            = (dr["second_break_out"].ToString()),
                            third_break_in              = (dr["third_break_in"].ToString()),
                            third_break_out             = (dr["third_break_out"].ToString()),
                            reg                         = Convert.ToDecimal(dr["reg"].ToString()),
                            regnd                       = Convert.ToDecimal(dr["regnd"].ToString()),
                            ot                          = Convert.ToDecimal(dr["ot"].ToString()),
                            ot_e8                       = Convert.ToDecimal(dr["ot_e8"].ToString()),
                            otnd                        = Convert.ToDecimal(dr["otnd"].ToString()),
                            otnd_e8                     = Convert.ToDecimal(dr["otnd_e8"].ToString()),
                            otrd                        = Convert.ToDecimal(dr["otrd"].ToString()),
                            otrd_e8                     = Convert.ToDecimal(dr["otrd_e8"].ToString()),
                            otrdnd                      = Convert.ToDecimal(dr["otrdnd"].ToString()),
                            otrdnd_e8                   = Convert.ToDecimal(dr["otrdnd_e8"].ToString()),
                            lh                          = Convert.ToDecimal(dr["lh"].ToString()),
                            lhot                        = Convert.ToDecimal(dr["lhot"].ToString()),
                            lhot_e8                     = Convert.ToDecimal(dr["lhot_e8"].ToString()),
                            lhotnd                      = Convert.ToDecimal(dr["lhotnd"].ToString()),
                            lhotnd_e8                   = Convert.ToDecimal(dr["lhotnd_e8"].ToString()),
                            lhrd                        = Convert.ToDecimal(dr["lhrd"].ToString()),
                            lhrdot                      = Convert.ToDecimal(dr["lhrdot"].ToString()),
                            lhrdot_e8                   = Convert.ToDecimal(dr["lhrdot_e8"].ToString()),
                            lhrdotnd                    = Convert.ToDecimal(dr["lhrdotnd"].ToString()),
                            lhrdotnd_e8                 = Convert.ToDecimal(dr["lhrdotnd_e8"].ToString()),
                            sh                          = Convert.ToDecimal(dr["sh"].ToString()),
                            shot                        = Convert.ToDecimal(dr["shot"].ToString()),
                            shot_e8                     = Convert.ToDecimal(dr["shot_e8"].ToString()),
                            shotnd                      = Convert.ToDecimal(dr["shotnd"].ToString()),
                            shotnd_e8                   = Convert.ToDecimal(dr["shotnd_e8"].ToString()),
                            shrd                        = Convert.ToDecimal(dr["shrd"].ToString()),
                            shrdot                      = Convert.ToDecimal(dr["shrdot"].ToString()),
                            shrdot_e8                   = Convert.ToDecimal(dr["shrdot_e8"].ToString()),
                            shrdotnd                    = Convert.ToDecimal(dr["shrdotnd"].ToString()),
                            shrdotnd_e8                 = Convert.ToDecimal(dr["shrdotnd_e8"].ToString()),
                            dh                          = Convert.ToDecimal(dr["dh"].ToString()),
                            dhot                        = Convert.ToDecimal(dr["dhot"].ToString()),
                            dhot_e8                     = Convert.ToDecimal(dr["dhot_e8"].ToString()),
                            dhotnd                      = Convert.ToDecimal(dr["dhotnd"].ToString()),
                            dhotnd_e8                   = Convert.ToDecimal(dr["dhotnd_e8"].ToString()),
                            dhrd                        = Convert.ToDecimal(dr["dhrd"].ToString()),
                            dhrdot                      = Convert.ToDecimal(dr["dhrdot"].ToString()),
                            dhrdot_e8                   = Convert.ToDecimal(dr["dhrdot_e8"].ToString()),
                            dhrdotnd                    = Convert.ToDecimal(dr["dhrdotnd"].ToString()),
                            dhrdotnd_e8                 = Convert.ToDecimal(dr["dhrdotnd_e8"].ToString()),
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


        public List<TimekeepingGenerationResponse> timekeeping_generation_employee(string series_code, string employee_id,string date_from,string date_to, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);




            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<TimekeepingGenerationResponse> resp = new List<TimekeepingGenerationResponse>();
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
                oCmd.CommandText = "timekeeping_generation_employee";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new TimekeepingGenerationResponse()
                        {
                            timekeeping_header_id = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            date = (dr["date"].ToString()),
                            rest_day = (dr["rest_day"].ToString()),
                            actual_time_in = (dr["actual_time_in"].ToString()),
                            actual_time_out = (dr["actual_time_out"].ToString()),
                            time_in = (dr["time_in"].ToString()),
                            time_out = (dr["time_out"].ToString()),
                            official_business_in = (dr["official_business_in"].ToString()),
                            official_business_out = (dr["official_business_out"].ToString()),
                            overtime_hour = Convert.ToDecimal(dr["overtime_hour"].ToString()),
                            offset_hour = Convert.ToDecimal(dr["offset_hour"].ToString()),
                            vl_hour = Convert.ToDecimal(dr["vl_hour"].ToString()),
                            sl_hour = Convert.ToDecimal(dr["sl_hour"].ToString()),
                            otherl_hour = Convert.ToDecimal(dr["otherl_hour"].ToString()),
                            lwop_hour = Convert.ToDecimal(dr["lwop_hour"].ToString()),
                            is_rest_day = Convert.ToBoolean(dr["is_rest_day"].ToString()),
                            holiday_type = Convert.ToInt32(dr["holiday_type"].ToString()),
                            holiday_count = Convert.ToInt32(dr["holiday_count"].ToString()),
                            schedule_time_in = (dr["schedule_time_in"].ToString()),
                            schedule_time_out = (dr["schedule_time_out"].ToString()),
                            schedule_hour = Convert.ToDecimal(dr["schedule_hour"].ToString()),
                            schedule_break_hour = Convert.ToDecimal(dr["schedule_break_hour"].ToString()),
                            is_absent = Convert.ToBoolean(dr["is_absent"].ToString()),
                            working_hour = Convert.ToDecimal(dr["working_hour"].ToString()),
                            break_hour = Convert.ToDecimal(dr["break_hour"].ToString()),
                            late = Convert.ToDecimal(dr["late"].ToString()),
                            undertime = Convert.ToDecimal(dr["undertime"].ToString()),
                            first_break_late = Convert.ToDecimal(dr["first_break_late"].ToString()),
                            first_break_undertime = Convert.ToDecimal(dr["first_break_undertime"].ToString()),
                            second_break_late = Convert.ToDecimal(dr["second_break_late"].ToString()),
                            second_break_undertime = Convert.ToDecimal(dr["second_break_undertime"].ToString()),
                            third_break_late = Convert.ToDecimal(dr["third_break_late"].ToString()),
                            third_break_undertime = Convert.ToDecimal(dr["third_break_undertime"].ToString()),
                            total_break_late = Convert.ToDecimal(dr["total_break_late"].ToString()),
                            total_break_undertime = Convert.ToDecimal(dr["total_break_undertime"].ToString()),
                            remarks = (dr["remarks"].ToString()),
                            first_break_in = (dr["first_break_in"].ToString()),
                            first_break_out = (dr["first_break_out"].ToString()),
                            second_break_in = (dr["second_break_in"].ToString()),
                            second_break_out = (dr["second_break_out"].ToString()),
                            third_break_in = (dr["third_break_in"].ToString()),
                            third_break_out = (dr["third_break_out"].ToString()),
                            reg = Convert.ToDecimal(dr["reg"].ToString()),
                            regnd = Convert.ToDecimal(dr["regnd"].ToString()),
                            ot = Convert.ToDecimal(dr["ot"].ToString()),
                            ot_e8 = Convert.ToDecimal(dr["ot_e8"].ToString()),
                            otnd = Convert.ToDecimal(dr["otnd"].ToString()),
                            otnd_e8 = Convert.ToDecimal(dr["otnd_e8"].ToString()),
                            otrd = Convert.ToDecimal(dr["otrd"].ToString()),
                            otrd_e8 = Convert.ToDecimal(dr["otrd_e8"].ToString()),
                            otrdnd = Convert.ToDecimal(dr["otrdnd"].ToString()),
                            otrdnd_e8 = Convert.ToDecimal(dr["otrdnd_e8"].ToString()),
                            lh = Convert.ToDecimal(dr["lh"].ToString()),
                            lhot = Convert.ToDecimal(dr["lhot"].ToString()),
                            lhot_e8 = Convert.ToDecimal(dr["lhot_e8"].ToString()),
                            lhotnd = Convert.ToDecimal(dr["lhotnd"].ToString()),
                            lhotnd_e8 = Convert.ToDecimal(dr["lhotnd_e8"].ToString()),
                            lhrd = Convert.ToDecimal(dr["lhrd"].ToString()),
                            lhrdot = Convert.ToDecimal(dr["lhrdot"].ToString()),
                            lhrdot_e8 = Convert.ToDecimal(dr["lhrdot_e8"].ToString()),
                            lhrdotnd = Convert.ToDecimal(dr["lhrdotnd"].ToString()),
                            lhrdotnd_e8 = Convert.ToDecimal(dr["lhrdotnd_e8"].ToString()),
                            sh = Convert.ToDecimal(dr["sh"].ToString()),
                            shot = Convert.ToDecimal(dr["shot"].ToString()),
                            shot_e8 = Convert.ToDecimal(dr["shot_e8"].ToString()),
                            shotnd = Convert.ToDecimal(dr["shotnd"].ToString()),
                            shotnd_e8 = Convert.ToDecimal(dr["shotnd_e8"].ToString()),
                            shrd = Convert.ToDecimal(dr["shrd"].ToString()),
                            shrdot = Convert.ToDecimal(dr["shrdot"].ToString()),
                            shrdot_e8 = Convert.ToDecimal(dr["shrdot_e8"].ToString()),
                            shrdotnd = Convert.ToDecimal(dr["shrdotnd"].ToString()),
                            shrdotnd_e8 = Convert.ToDecimal(dr["shrdotnd_e8"].ToString()),
                            dh = Convert.ToDecimal(dr["dh"].ToString()),
                            dhot = Convert.ToDecimal(dr["dhot"].ToString()),
                            dhot_e8 = Convert.ToDecimal(dr["dhot_e8"].ToString()),
                            dhotnd = Convert.ToDecimal(dr["dhotnd"].ToString()),
                            dhotnd_e8 = Convert.ToDecimal(dr["dhotnd_e8"].ToString()),
                            dhrd = Convert.ToDecimal(dr["dhrd"].ToString()),
                            dhrdot = Convert.ToDecimal(dr["dhrdot"].ToString()),
                            dhrdot_e8 = Convert.ToDecimal(dr["dhrdot_e8"].ToString()),
                            dhrdotnd = Convert.ToDecimal(dr["dhrdotnd"].ToString()),
                            dhrdotnd_e8 = Convert.ToDecimal(dr["dhrdotnd_e8"].ToString()),
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



        public InsertResponse timekeeping_in_up(TimekeepingRequest model)
        {
            var resp = new InsertResponse();
            model.timekeeping_header_id = model.timekeeping_header_id == "0" ? "0" : Crypto.url_decrypt(model.timekeeping_header_id);
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
                oCmd.CommandText = "timekeeping_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", model.timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@timekeeping_header_code", model.timekeeping_header_code);


                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp.id = Convert.ToInt32(sdr["timekeeping_header_id"].ToString());
                    resp.code = model.timekeeping_header_code;
                    resp.description = "Saving Successful.";

                }
                sdr.Close();


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


        public List<TimekeepingResponse> timekeeping_view_sel(string series_code, string timekeeping_header_id, string timekeeping_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            timekeeping_header_id = timekeeping_header_id == "0" ? "0" : Crypto.url_decrypt(timekeeping_header_id);
            timekeeping_id = timekeeping_id == "0" ? "0" : Crypto.url_decrypt(timekeeping_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<TimekeepingResponse> resp = new List<TimekeepingResponse>();
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
                oCmd.CommandText = "timekeeping_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@timekeeping_id", timekeeping_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new TimekeepingResponse()
                        {
                            timekeeping_id = Convert.ToInt32(dr["timekeeping_id"].ToString()),
                            encrypt_timekeeping_id = Crypto.url_encrypt(dr["timekeeping_id"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            timekeeping_code = (dr["timekeeping_code"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
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


        public List<TimekeepingGenerationResponse> timekeeping_detail_view_sel(string series_code, string timekeeping_header_id, string timekeeping_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            timekeeping_header_id = timekeeping_header_id == "0" ? "0" : Crypto.url_decrypt(timekeeping_header_id);
            timekeeping_id = timekeeping_id == "0" ? "0" : Crypto.url_decrypt(timekeeping_id);
            series_code = Crypto.url_decrypt(series_code);




            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<TimekeepingGenerationResponse> resp = new List<TimekeepingGenerationResponse>();
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
                oCmd.CommandText = "timekeeping_detail_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@timekeeping_id", timekeeping_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new TimekeepingGenerationResponse()
                        {
                            timekeeping_header_id = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            timekeeping_id = Convert.ToInt32(dr["timekeeping_id"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            date = (dr["date"].ToString()),
                            rest_day = (dr["rest_day"].ToString()),
                            actual_time_in = (dr["actual_time_in"].ToString()),
                            actual_time_out = (dr["actual_time_out"].ToString()),
                            time_in = (dr["time_in"].ToString()),
                            time_out = (dr["time_out"].ToString()),
                            official_business_in = (dr["official_business_in"].ToString()),
                            official_business_out = (dr["official_business_out"].ToString()),
                            overtime_hour = Convert.ToDecimal(dr["overtime_hour"].ToString()),
                            offset_hour = Convert.ToDecimal(dr["offset_hour"].ToString()),
                            vl_hour = Convert.ToDecimal(dr["vl_hour"].ToString()),
                            sl_hour = Convert.ToDecimal(dr["sl_hour"].ToString()),
                            otherl_hour = Convert.ToDecimal(dr["otherl_hour"].ToString()),
                            lwop_hour = Convert.ToDecimal(dr["lwop_hour"].ToString()),
                            is_rest_day = Convert.ToBoolean(dr["is_rest_day"].ToString()),
                            holiday_type = Convert.ToInt32(dr["holiday_type"].ToString()),
                            holiday_count = Convert.ToInt32(dr["holiday_count"].ToString()),
                            schedule_time_in = (dr["schedule_time_in"].ToString()),
                            schedule_time_out = (dr["schedule_time_out"].ToString()),
                            schedule_hour = Convert.ToDecimal(dr["schedule_hour"].ToString()),
                            schedule_break_hour = Convert.ToDecimal(dr["schedule_break_hour"].ToString()),
                            is_absent = Convert.ToBoolean(dr["is_absent"].ToString()),
                            working_hour = Convert.ToDecimal(dr["working_hour"].ToString()),
                            break_hour = Convert.ToDecimal(dr["break_hour"].ToString()),
                            late = Convert.ToDecimal(dr["late"].ToString()),
                            undertime = Convert.ToDecimal(dr["undertime"].ToString()),
                            first_break_late = Convert.ToDecimal(dr["first_break_late"].ToString()),
                            first_break_undertime = Convert.ToDecimal(dr["first_break_undertime"].ToString()),
                            second_break_late = Convert.ToDecimal(dr["second_break_late"].ToString()),
                            second_break_undertime = Convert.ToDecimal(dr["second_break_undertime"].ToString()),
                            third_break_late = Convert.ToDecimal(dr["third_break_late"].ToString()),
                            third_break_undertime = Convert.ToDecimal(dr["third_break_undertime"].ToString()),
                            total_break_late = Convert.ToDecimal(dr["total_break_late"].ToString()),
                            total_break_undertime = Convert.ToDecimal(dr["total_break_undertime"].ToString()),
                            remarks = (dr["remarks"].ToString()),
                            first_break_in = (dr["first_break_in"].ToString()),
                            first_break_out = (dr["first_break_out"].ToString()),
                            second_break_in = (dr["second_break_in"].ToString()),
                            second_break_out = (dr["second_break_out"].ToString()),
                            third_break_in = (dr["third_break_in"].ToString()),
                            third_break_out = (dr["third_break_out"].ToString()),
                            reg = Convert.ToDecimal(dr["reg"].ToString()),
                            regnd = Convert.ToDecimal(dr["regnd"].ToString()),
                            ot = Convert.ToDecimal(dr["ot"].ToString()),
                            ot_e8 = Convert.ToDecimal(dr["ot_e8"].ToString()),
                            otnd = Convert.ToDecimal(dr["otnd"].ToString()),
                            otnd_e8 = Convert.ToDecimal(dr["otnd_e8"].ToString()),
                            otrd = Convert.ToDecimal(dr["otrd"].ToString()),
                            otrd_e8 = Convert.ToDecimal(dr["otrd_e8"].ToString()),
                            otrdnd = Convert.ToDecimal(dr["otrdnd"].ToString()),
                            otrdnd_e8 = Convert.ToDecimal(dr["otrdnd_e8"].ToString()),
                            lh = Convert.ToDecimal(dr["lh"].ToString()),
                            lhot = Convert.ToDecimal(dr["lhot"].ToString()),
                            lhot_e8 = Convert.ToDecimal(dr["lhot_e8"].ToString()),
                            lhotnd = Convert.ToDecimal(dr["lhotnd"].ToString()),
                            lhotnd_e8 = Convert.ToDecimal(dr["lhotnd_e8"].ToString()),
                            lhrd = Convert.ToDecimal(dr["lhrd"].ToString()),
                            lhrdot = Convert.ToDecimal(dr["lhrdot"].ToString()),
                            lhrdot_e8 = Convert.ToDecimal(dr["lhrdot_e8"].ToString()),
                            lhrdotnd = Convert.ToDecimal(dr["lhrdotnd"].ToString()),
                            lhrdotnd_e8 = Convert.ToDecimal(dr["lhrdotnd_e8"].ToString()),
                            sh = Convert.ToDecimal(dr["sh"].ToString()),
                            shot = Convert.ToDecimal(dr["shot"].ToString()),
                            shot_e8 = Convert.ToDecimal(dr["shot_e8"].ToString()),
                            shotnd = Convert.ToDecimal(dr["shotnd"].ToString()),
                            shotnd_e8 = Convert.ToDecimal(dr["shotnd_e8"].ToString()),
                            shrd = Convert.ToDecimal(dr["shrd"].ToString()),
                            shrdot = Convert.ToDecimal(dr["shrdot"].ToString()),
                            shrdot_e8 = Convert.ToDecimal(dr["shrdot_e8"].ToString()),
                            shrdotnd = Convert.ToDecimal(dr["shrdotnd"].ToString()),
                            shrdotnd_e8 = Convert.ToDecimal(dr["shrdotnd_e8"].ToString()),
                            dh = Convert.ToDecimal(dr["dh"].ToString()),
                            dhot = Convert.ToDecimal(dr["dhot"].ToString()),
                            dhot_e8 = Convert.ToDecimal(dr["dhot_e8"].ToString()),
                            dhotnd = Convert.ToDecimal(dr["dhotnd"].ToString()),
                            dhotnd_e8 = Convert.ToDecimal(dr["dhotnd_e8"].ToString()),
                            dhrd = Convert.ToDecimal(dr["dhrd"].ToString()),
                            dhrdot = Convert.ToDecimal(dr["dhrdot"].ToString()),
                            dhrdot_e8 = Convert.ToDecimal(dr["dhrdot_e8"].ToString()),
                            dhrdotnd = Convert.ToDecimal(dr["dhrdotnd"].ToString()),
                            dhrdotnd_e8 = Convert.ToDecimal(dr["dhrdotnd_e8"].ToString()),
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

        public List<TimekeepingGenerationResponse> timekeeping_final_view_sel(string series_code, string timekeeping_header_id, string employee_id, string date_from,string date_to, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            timekeeping_header_id = timekeeping_header_id == "0" ? "0" : Crypto.url_decrypt(timekeeping_header_id);
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);




            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<TimekeepingGenerationResponse> resp = new List<TimekeepingGenerationResponse>();
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
                oCmd.CommandText = "timekeeping_final_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new TimekeepingGenerationResponse()
                        {
                            timekeeping_header_id = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            date = (dr["date"].ToString()),
                            rest_day = (dr["rest_day"].ToString()),
                            actual_time_in = (dr["actual_time_in"].ToString()),
                            actual_time_out = (dr["actual_time_out"].ToString()),
                            time_in = (dr["time_in"].ToString()),
                            time_out = (dr["time_out"].ToString()),
                            official_business_in = (dr["official_business_in"].ToString()),
                            official_business_out = (dr["official_business_out"].ToString()),
                            overtime_hour = Convert.ToDecimal(dr["overtime_hour"].ToString()),
                            offset_hour = Convert.ToDecimal(dr["offset_hour"].ToString()),
                            vl_hour = Convert.ToDecimal(dr["vl_hour"].ToString()),
                            sl_hour = Convert.ToDecimal(dr["sl_hour"].ToString()),
                            otherl_hour = Convert.ToDecimal(dr["otherl_hour"].ToString()),
                            lwop_hour = Convert.ToDecimal(dr["lwop_hour"].ToString()),
                            is_rest_day = Convert.ToBoolean(dr["is_rest_day"].ToString()),
                            holiday_type = Convert.ToInt32(dr["holiday_type"].ToString()),
                            holiday_count = Convert.ToInt32(dr["holiday_count"].ToString()),
                            schedule_time_in = (dr["schedule_time_in"].ToString()),
                            schedule_time_out = (dr["schedule_time_out"].ToString()),
                            schedule_hour = Convert.ToDecimal(dr["schedule_hour"].ToString()),
                            schedule_break_hour = Convert.ToDecimal(dr["schedule_break_hour"].ToString()),
                            is_absent = Convert.ToBoolean(dr["is_absent"].ToString()),
                            working_hour = Convert.ToDecimal(dr["working_hour"].ToString()),
                            break_hour = Convert.ToDecimal(dr["break_hour"].ToString()),
                            late = Convert.ToDecimal(dr["late"].ToString()),
                            undertime = Convert.ToDecimal(dr["undertime"].ToString()),
                            first_break_late = Convert.ToDecimal(dr["first_break_late"].ToString()),
                            first_break_undertime = Convert.ToDecimal(dr["first_break_undertime"].ToString()),
                            second_break_late = Convert.ToDecimal(dr["second_break_late"].ToString()),
                            second_break_undertime = Convert.ToDecimal(dr["second_break_undertime"].ToString()),
                            third_break_late = Convert.ToDecimal(dr["third_break_late"].ToString()),
                            third_break_undertime = Convert.ToDecimal(dr["third_break_undertime"].ToString()),
                            total_break_late = Convert.ToDecimal(dr["total_break_late"].ToString()),
                            total_break_undertime = Convert.ToDecimal(dr["total_break_undertime"].ToString()),
                            remarks = (dr["remarks"].ToString()),
                            first_break_in = (dr["first_break_in"].ToString()),
                            first_break_out = (dr["first_break_out"].ToString()),
                            second_break_in = (dr["second_break_in"].ToString()),
                            second_break_out = (dr["second_break_out"].ToString()),
                            third_break_in = (dr["third_break_in"].ToString()),
                            third_break_out = (dr["third_break_out"].ToString()),
                            reg = Convert.ToDecimal(dr["reg"].ToString()),
                            regnd = Convert.ToDecimal(dr["regnd"].ToString()),
                            ot = Convert.ToDecimal(dr["ot"].ToString()),
                            ot_e8 = Convert.ToDecimal(dr["ot_e8"].ToString()),
                            otnd = Convert.ToDecimal(dr["otnd"].ToString()),
                            otnd_e8 = Convert.ToDecimal(dr["otnd_e8"].ToString()),
                            otrd = Convert.ToDecimal(dr["otrd"].ToString()),
                            otrd_e8 = Convert.ToDecimal(dr["otrd_e8"].ToString()),
                            otrdnd = Convert.ToDecimal(dr["otrdnd"].ToString()),
                            otrdnd_e8 = Convert.ToDecimal(dr["otrdnd_e8"].ToString()),
                            lh = Convert.ToDecimal(dr["lh"].ToString()),
                            lhot = Convert.ToDecimal(dr["lhot"].ToString()),
                            lhot_e8 = Convert.ToDecimal(dr["lhot_e8"].ToString()),
                            lhotnd = Convert.ToDecimal(dr["lhotnd"].ToString()),
                            lhotnd_e8 = Convert.ToDecimal(dr["lhotnd_e8"].ToString()),
                            lhrd = Convert.ToDecimal(dr["lhrd"].ToString()),
                            lhrdot = Convert.ToDecimal(dr["lhrdot"].ToString()),
                            lhrdot_e8 = Convert.ToDecimal(dr["lhrdot_e8"].ToString()),
                            lhrdotnd = Convert.ToDecimal(dr["lhrdotnd"].ToString()),
                            lhrdotnd_e8 = Convert.ToDecimal(dr["lhrdotnd_e8"].ToString()),
                            sh = Convert.ToDecimal(dr["sh"].ToString()),
                            shot = Convert.ToDecimal(dr["shot"].ToString()),
                            shot_e8 = Convert.ToDecimal(dr["shot_e8"].ToString()),
                            shotnd = Convert.ToDecimal(dr["shotnd"].ToString()),
                            shotnd_e8 = Convert.ToDecimal(dr["shotnd_e8"].ToString()),
                            shrd = Convert.ToDecimal(dr["shrd"].ToString()),
                            shrdot = Convert.ToDecimal(dr["shrdot"].ToString()),
                            shrdot_e8 = Convert.ToDecimal(dr["shrdot_e8"].ToString()),
                            shrdotnd = Convert.ToDecimal(dr["shrdotnd"].ToString()),
                            shrdotnd_e8 = Convert.ToDecimal(dr["shrdotnd_e8"].ToString()),
                            dh = Convert.ToDecimal(dr["dh"].ToString()),
                            dhot = Convert.ToDecimal(dr["dhot"].ToString()),
                            dhot_e8 = Convert.ToDecimal(dr["dhot_e8"].ToString()),
                            dhotnd = Convert.ToDecimal(dr["dhotnd"].ToString()),
                            dhotnd_e8 = Convert.ToDecimal(dr["dhotnd_e8"].ToString()),
                            dhrd = Convert.ToDecimal(dr["dhrd"].ToString()),
                            dhrdot = Convert.ToDecimal(dr["dhrdot"].ToString()),
                            dhrdot_e8 = Convert.ToDecimal(dr["dhrdot_e8"].ToString()),
                            dhrdotnd = Convert.ToDecimal(dr["dhrdotnd"].ToString()),
                            dhrdotnd_e8 = Convert.ToDecimal(dr["dhrdotnd_e8"].ToString()),
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

        #region"Payroll"


        public List<PayrollCutoffResponse> payroll_cutoff_view(string series_code, int cutoff_id, int month_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollCutoffResponse> resp = new List<PayrollCutoffResponse>();
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
                oCmd.CommandText = "payroll_cutoff_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@month_id", month_id);
                oCmd.Parameters.AddWithValue("@cutoff_id", cutoff_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollCutoffResponse()
                        {
                            payroll_cutoff_id = Convert.ToInt32(dr["payroll_cutoff_id"].ToString()),
                            encypted_payroll_cutoff_id = (dr["payroll_cutoff_id"].ToString()),
                            payroll_setup_id = Convert.ToInt32(dr["payroll_setup_id"].ToString()),
                            cutoff_id = Convert.ToInt32(dr["cutoff_id"].ToString()),
                            date_start = (dr["date_start"].ToString()),
                            date_end = (dr["date_end"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created = (dr["date_created"].ToString()),


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


        public List<PayrollCutoffSelResponse> payroll_cutoff_sel(string series_code, int payroll_cutoff_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollCutoffSelResponse> resp = new List<PayrollCutoffSelResponse>();
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
                oCmd.CommandText = "payroll_cutoff_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_cutoff_id ", payroll_cutoff_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollCutoffSelResponse()
                        {
                            payroll_setup_id             = Convert.ToInt32(dr["payroll_setup_id"].ToString()),
                            encypted_payroll_setup_id    = Crypto.url_encrypt(dr["payroll_setup_id"].ToString()),
                            payroll_setup                = (dr["payroll_setup"].ToString()),
                            cutoff                       = (dr["cutoff"].ToString()),
                            cutoff_id                    = Convert.ToInt32(dr["cutoff_id"].ToString()),
                            payroll_cutoff_id                       = Convert.ToInt32(dr["payroll_cutoff_id"].ToString()),
                            date_start                   = (dr["date_start"].ToString()),
                            date_end                     = (dr["date_end"].ToString()),
                            pay_day                      = (dr["pay_day"].ToString()),
                            ds_month                     = Convert.ToInt32(dr["ds_month"].ToString()),
                            de_month                     = Convert.ToInt32(dr["de_month"].ToString()),
                            pd_month                     = Convert.ToInt32(dr["pd_month"].ToString()),
                            lock_id                      = Convert.ToBoolean(dr["lock_id"].ToString()),
                            locked                       = (dr["lock"].ToString()),
                            approval_lock_id = Convert.ToBoolean(dr["approval_lock_id"].ToString()),
                            approval_lock                = (dr["approval_lock"].ToString()),

                         
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created = (dr["date_created"].ToString()),


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


        public InsertResponse payroll_cutoff_up(PayrollCutoffRequest[] model)
        {
            var resp = new InsertResponse();
            //model.timekeeping_header_id = model.timekeeping_header_id == "0" ? "0" : Crypto.url_decrypt(model.timekeeping_header_id);
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
                foreach(var item in model)
                {
                    da.SelectCommand = oCmd;
                    oCmd.CommandText = "payroll_cutoff_up";
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();

                    oCmd.Parameters.AddWithValue("@payroll_cutoff_id", item.payroll_cutoff_id);
                    oCmd.Parameters.AddWithValue("@date_start", item.date_start);
                    oCmd.Parameters.AddWithValue("@date_end", item.date_end);
                    oCmd.Parameters.AddWithValue("@pay_day", item.pay_day);
                    oCmd.Parameters.AddWithValue("@ds_month", item.ds_month);
                    oCmd.Parameters.AddWithValue("@de_month", item.de_month);
                    oCmd.Parameters.AddWithValue("@pd_month", item.pd_month);
                    oCmd.Parameters.AddWithValue("@lock", item.lock_id);
                    oCmd.Parameters.AddWithValue("@approval_lock", item.approval_lock);
                    oCmd.Parameters.AddWithValue("@created_by", created_by);

                    SqlDataReader sdr = oCmd.ExecuteReader();
                    while (sdr.Read())
                    {

                        resp.id = Convert.ToInt32(sdr["payroll_cutoff_id"].ToString());
                        resp.encrypted_id = Crypto.url_encrypt(sdr["payroll_cutoff_id"].ToString());
                        resp.description = "Saving Successful.";

                    }
                    sdr.Close();
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
    }



}
