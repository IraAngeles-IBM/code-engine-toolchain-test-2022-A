
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using AttendanceManagementService.Helper;
using AttendanceManagementService.Model;
using System.Data.SqlClient;

namespace AttendanceManagementService.Service
{

    public interface IAttendanceManagementServices
    {
        int attendance_log_approval_in(AttendanceLogApprovalRequest model);
        int employee_schedule_in(EmployeeScheduleRequest model);
        List<EmployeeScheduleResponse> employee_schedule_view_sel(string series_code, string shift_id, string employee_id, string date_from, string date_to, string created_by);
        List<EmployeeMovementRequest> employee_schedule_detail_in(EmployeeScheduleDetailRequest[] model);
        List<EmployeeScheduleDetailResponse> employee_schedule_detail_view_sel(string series_code, string shift_id, string created_by);
        int attendance_log_temp_in(LogRequest model);
        int attendance_log_in_up(AttendanceLogRequest model);
        List<AttendanceLog> attendance_log_sel(string series_code, string date_from, string date_to, string bio_id, string created_by);
        int attendance_log_deleted_in(AttendanceLogRequest model);
        List<AttendanceCLResponse> employee_attendance_cl_view(string series_code, string date_from, string date_to, string employee_id, string created_by);
        List<AttendanceDashboardResponse> employee_attendance_dashboard_view(string series_code, string date_from, string date_to, string employee_id, string created_by);
        int attendance_log_cl_in(AttendanceCLRequest model);
        int attendance_log_in(AttendanceRequest model);
        int employee_schedule_detail_auto(AttendanceLogApprovalRequest model);
        List<EmployeeAttendanceResponse> employee_attendance_view(string series_code, string date_from, string date_to, string employee_id, bool missing_logs_only, bool is_supervisor, string created_by);
        int employee_attendance_up(EmployeeAttendanceRequest model);


    }

    public class AttendanceManagementServices : IAttendanceManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public AttendanceManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }


        public int employee_schedule_in(EmployeeScheduleRequest model)
        {
            int resp = 0;
            model.encrypt_shift_id = model.encrypt_shift_id == "0" ? "0" : Crypto.url_decrypt(model.encrypt_shift_id);
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
            SqlDataAdapter da = new SqlDataAdapter();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {

                da.SelectCommand = oCmd;
                oCmd.CommandText = "employee_schedule_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();


                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@shift_id",model.encrypt_shift_id);
                oCmd.Parameters.AddWithValue("@employee_id",model.employee_id);
                oCmd.Parameters.AddWithValue("@date_from",model.date_from);
                oCmd.Parameters.AddWithValue("@date_to",model.date_to);
                oCmd.Parameters.AddWithValue("@grace_period",model.grace_period);
                oCmd.Parameters.AddWithValue("@is_flexi",model.is_flexi);
                oCmd.Parameters.AddWithValue("@time_in",model.time_in);
                oCmd.Parameters.AddWithValue("@time_out",model.time_out);
                oCmd.Parameters.AddWithValue("@time_out_days_cover",model.time_out_days_cover);
                oCmd.Parameters.AddWithValue("@total_working_hours",model.total_working_hours);
                oCmd.Parameters.AddWithValue("@half_day_in",model.half_day_in);
                oCmd.Parameters.AddWithValue("@half_day_in_days_cover",model.half_day_in_days_cover);
                oCmd.Parameters.AddWithValue("@half_day_out",model.half_day_out);
                oCmd.Parameters.AddWithValue("@half_day_out_days_cover",model.half_day_out_days_cover);
                oCmd.Parameters.AddWithValue("@night_dif_in",model.night_dif_in);
                oCmd.Parameters.AddWithValue("@night_dif_in_days_cover",model.night_dif_in_days_cover);
                oCmd.Parameters.AddWithValue("@night_dif_out",model.night_dif_out);
                oCmd.Parameters.AddWithValue("@night_dif_out_days_cover",model.night_dif_out_days_cover);
                oCmd.Parameters.AddWithValue("@first_break_in",model.first_break_in);
                oCmd.Parameters.AddWithValue("@first_break_in_days_cover",model.first_break_in_days_cover);
                oCmd.Parameters.AddWithValue("@first_break_out",model.first_break_out);
                oCmd.Parameters.AddWithValue("@first_break_out_days_cover",model.first_break_out_days_cover);
                oCmd.Parameters.AddWithValue("@second_break_in",model.second_break_in);
                oCmd.Parameters.AddWithValue("@second_break_in_days_cover",model.second_break_in_days_cover);
                oCmd.Parameters.AddWithValue("@second_break_out",model.second_break_out);
                oCmd.Parameters.AddWithValue("@second_break_out_days_cover",model.second_break_out_days_cover);
                oCmd.Parameters.AddWithValue("@third_break_in",model.third_break_in);
                oCmd.Parameters.AddWithValue("@third_break_in_days_cover",model.third_break_in_days_cover);
                oCmd.Parameters.AddWithValue("@third_break_out",model.third_break_out);
                oCmd.Parameters.AddWithValue("@.third_break_out_days_cover",model.third_break_out_days_cover);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                    resp = Convert.ToInt32(sdr["shift_id"].ToString());

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


        public List<EmployeeScheduleResponse> employee_schedule_view_sel(string series_code, string shift_id, string employee_id, string date_from, string date_to, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            shift_id = shift_id == "0" ? "0" : Crypto.url_decrypt(shift_id);
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<EmployeeScheduleResponse> resp = new List<EmployeeScheduleResponse>();
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
                oCmd.CommandText = "employee_schedule_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@shift_id", shift_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new EmployeeScheduleResponse()
                        {
                            shift_id                    = Convert.ToInt32(dr["shift_id"].ToString()),
                            encrypt_shift_id            = Crypto.url_encrypt(dr["shift_id"].ToString()),
                            employee_id                 = Convert.ToInt32(dr["employee_id"].ToString()),
                            shift_code                        = (dr["shift_code"].ToString()),
                            shift_name                        = (dr["shift_name"].ToString()),
                            date                        = (dr["date"].ToString()),
                            grace_period                = Convert.ToInt32(dr["grace_period"].ToString()),
                            is_flexi                    = Convert.ToBoolean(dr["is_flexi"].ToString()),
                            time_in                     = (dr["time_in"].ToString()),
                            time_out                    = (dr["time_out"].ToString()),
                            time_out_days_cover         = Convert.ToInt32(dr["time_out_days_cover"].ToString()),
                            total_working_hours         = Convert.ToDecimal(dr["total_working_hours"].ToString()),
                            half_day_in                 = (dr["half_day_in"].ToString()),
                            half_day_in_days_cover      = Convert.ToInt32(dr["half_day_in_days_cover"].ToString()),
                            half_day_out                = (dr["half_day_out"].ToString()),
                            half_day_out_days_cover     = Convert.ToInt32(dr["half_day_out_days_cover"].ToString()),
                            night_dif_in                = (dr["night_dif_in"].ToString()),
                            night_dif_in_days_cover     = Convert.ToInt32(dr["night_dif_in_days_cover"].ToString()),
                            night_dif_out               = (dr["night_dif_out"].ToString()),
                            night_dif_out_days_cover    = Convert.ToInt32(dr["night_dif_out_days_cover"].ToString()),
                            first_break_in              = (dr["first_break_in"].ToString()),
                            first_break_in_days_cover   = Convert.ToInt32(dr["first_break_in_days_cover"].ToString()),
                            first_break_out             = (dr["first_break_out"].ToString()),
                            first_break_out_days_cover  = Convert.ToInt32(dr["first_break_out_days_cover"].ToString()),
                            second_break_in             = (dr["second_break_in"].ToString()),
                            second_break_in_days_cover  = Convert.ToInt32(dr["second_break_in_days_cover"].ToString()),
                            second_break_out            = (dr["second_break_out"].ToString()),
                            second_break_out_days_cover = Convert.ToInt32(dr["second_break_out_days_cover"].ToString()),
                            third_break_in              = (dr["third_break_in"].ToString()),
                            third_break_in_days_cover   = Convert.ToInt32(dr["third_break_in_days_cover"].ToString()),
                            third_break_out             = (dr["third_break_out"].ToString()),
                            third_break_out_days_cover  = Convert.ToInt32(dr["third_break_out_days_cover"].ToString()),
                            created_by                  = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created                = (dr["date_created"].ToString()),

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



        public List<EmployeeMovementRequest> employee_schedule_detail_in(EmployeeScheduleDetailRequest[] model)
        {


            List<EmployeeMovementRequest> movement_resp = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> resp = new List<EmployeeMovementRequest>();
            string encrypt_shift_id = model[0].encrypt_shift_id == "0" ? "0" : Crypto.url_decrypt(model[0].encrypt_shift_id);
            string series_code = Crypto.url_decrypt(model[0].series_code);
            string created_by = Crypto.url_decrypt(model[0].created_by);
            //BranchResponse resp = new BranchResponse();
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
                foreach (var item in model)
                {

                    oCmd.CommandText = "employee_schedule_detail_in";
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();
                    oCmd.Parameters.AddWithValue("@created_by", created_by);
                    oCmd.Parameters.AddWithValue("@shift_id", encrypt_shift_id);
                    oCmd.Parameters.AddWithValue("@employee_id", item.employee_id);
                    oCmd.Parameters.AddWithValue("@date_from", item.date_from);
                    oCmd.Parameters.AddWithValue("@date_to", item.date_to);

                    oCmd.Parameters.AddWithValue("@shift_code", item.shift_code);
                    oCmd.Parameters.AddWithValue("@shift_code_type", item.shift_code_type);
                    oCmd.Parameters.AddWithValue("@shift_name", item.shift_name);
                    oCmd.Parameters.AddWithValue("@grace_period", item.grace_period);
                    oCmd.Parameters.AddWithValue("@description", item.description);
                    oCmd.Parameters.AddWithValue("@is_flexi", item.is_flexi);
                    oCmd.Parameters.AddWithValue("@time_in", item.time_in);
                    oCmd.Parameters.AddWithValue("@time_out", item.time_out);
                    oCmd.Parameters.AddWithValue("@time_out_days_cover", item.time_out_days_cover);
                    oCmd.Parameters.AddWithValue("@total_working_hours", item.total_working_hours);
                    oCmd.Parameters.AddWithValue("@is_rd_mon", item.is_rd_mon);
                    oCmd.Parameters.AddWithValue("@is_rd_tue", item.is_rd_tue);
                    oCmd.Parameters.AddWithValue("@is_rd_wed", item.is_rd_wed);
                    oCmd.Parameters.AddWithValue("@is_rd_thu", item.is_rd_thu);
                    oCmd.Parameters.AddWithValue("@is_rd_fri", item.is_rd_fri);
                    oCmd.Parameters.AddWithValue("@is_rd_sat", item.is_rd_sat);
                    oCmd.Parameters.AddWithValue("@is_rd_sun", item.is_rd_sun);
                    oCmd.Parameters.AddWithValue("@half_day_in", item.half_day_in);
                    oCmd.Parameters.AddWithValue("@half_day_in_days_cover", item.half_day_in_days_cover);
                    oCmd.Parameters.AddWithValue("@half_day_out", item.half_day_out);
                    oCmd.Parameters.AddWithValue("@half_day_out_days_cover", item.half_day_out_days_cover);
                    oCmd.Parameters.AddWithValue("@night_dif_in", item.night_dif_in);
                    oCmd.Parameters.AddWithValue("@night_dif_in_days_cover", item.night_dif_in_days_cover);
                    oCmd.Parameters.AddWithValue("@night_dif_out", item.night_dif_out);
                    oCmd.Parameters.AddWithValue("@night_dif_out_days_cover", item.night_dif_out_days_cover);
                    oCmd.Parameters.AddWithValue("@first_break_in", item.first_break_in);
                    oCmd.Parameters.AddWithValue("@first_break_in_days_cover", item.first_break_in_days_cover);
                    oCmd.Parameters.AddWithValue("@first_break_out", item.first_break_out);
                    oCmd.Parameters.AddWithValue("@first_break_out_days_cover", item.first_break_out_days_cover);
                    oCmd.Parameters.AddWithValue("@second_break_in", item.second_break_in);
                    oCmd.Parameters.AddWithValue("@second_break_in_days_cover", item.second_break_in_days_cover);
                    oCmd.Parameters.AddWithValue("@second_break_out", item.second_break_out);
                    oCmd.Parameters.AddWithValue("@second_break_out_days_cover", item.second_break_out_days_cover);
                    oCmd.Parameters.AddWithValue("@third_break_in", item.third_break_in);
                    oCmd.Parameters.AddWithValue("@third_break_in_days_cover", item.third_break_in_days_cover);
                    oCmd.Parameters.AddWithValue("@third_break_out", item.third_break_out);
                    oCmd.Parameters.AddWithValue("@third_break_out_days_cover", item.third_break_out_days_cover);

                    da.Fill(dt);
                    movement_resp = (from DataRow dr in dt.Rows
                            select new EmployeeMovementRequest()
                            {
                                employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                                movement_type = Convert.ToInt32(dr["movement_type"].ToString()),
                                is_dropdown = Convert.ToInt32(dr["is_dropdown"].ToString()),
                                id = Convert.ToInt32(dr["id"].ToString()),
                                description = (dr["description"].ToString()),
                                movement_description = "",
                                created_by = (model[0].created_by),
                                series_code = (model[0].series_code),

                            }).ToList();

                    foreach (var x in movement_resp)
                    {
                        resp.Add(x);
                    }
                }
                oTrans.Commit();

                oCmd.CommandText = "employee_schedule_detail_job";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@series_code", series_code);
                oCmd.ExecuteNonQuery();

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



        public List<EmployeeScheduleDetailResponse> employee_schedule_detail_view_sel(string series_code, string shift_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            shift_id = shift_id == "0" ? "0" : Crypto.url_decrypt(shift_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<EmployeeScheduleDetailResponse> resp = new List<EmployeeScheduleDetailResponse>();
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
                oCmd.CommandText = "employee_schedule_detail_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@shift_id", shift_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new EmployeeScheduleDetailResponse()
                        {
                            shift_id = Convert.ToInt32(dr["leave_type_id"].ToString()),
                            encrypt_shift_id = Crypto.url_encrypt(dr["shift_id"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            process = Convert.ToBoolean(dr["process"].ToString()),
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



        public int attendance_log_temp_in(LogRequest model)
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
            SqlDataAdapter da = new SqlDataAdapter();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "attendance_log_temp_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@series_code", series_code);
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


        public int attendance_log_in_up(AttendanceLogRequest model)
        {
            int resp = 0;
            string series_code = Crypto.url_decrypt(model.series_code);
            string created_by = Crypto.url_decrypt(model.created_by_new);
            //BranchResponse resp = new BranchResponse();
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

                oCmd.CommandText = "attendance_log_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@bio_id", model.bio_id          );
                oCmd.Parameters.AddWithValue("@date_time", model.date_time       );
                oCmd.Parameters.AddWithValue("@in_out", model.in_out          );
                oCmd.Parameters.AddWithValue("@terminal_id", model.terminal_id     );
                oCmd.Parameters.AddWithValue("@created_by", model.created_by      );
                oCmd.Parameters.AddWithValue("@date_created", model.date_created    );
                oCmd.Parameters.AddWithValue("@date_time_new", model.date_time_new   );
                oCmd.Parameters.AddWithValue("@in_out_new", model.in_out_new      );
                oCmd.Parameters.AddWithValue("@created_by_new", created_by  );
             

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


        public List<AttendanceLog> attendance_log_sel(string series_code, string date_from, string date_to, string bio_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<AttendanceLog> resp = new List<AttendanceLog>();
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
                oCmd.CommandText = "attendance_log_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@bio_id", bio_id == null ? "" : bio_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new AttendanceLog()
                        {

                            bio_id             = (dr["bio_id"].ToString()),
                            date_time          = (dr["date_time"].ToString()),
                            in_out             = Convert.ToInt32(dr["in_out"].ToString()),
                            terminal_id        = (dr["terminal_id"].ToString()),
                            created_by         = Convert.ToInt32(dr["created_by"].ToString()),
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



        public int attendance_log_deleted_in(AttendanceLogRequest model)
        {
            int resp = 0;
            string series_code = Crypto.url_decrypt(model.series_code);
            string created_by = Crypto.url_decrypt(model.created_by_new);
            //BranchResponse resp = new BranchResponse();
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

                oCmd.CommandText = "attendance_log_deleted_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@bio_id", model.bio_id);
                oCmd.Parameters.AddWithValue("@date_time", model.date_time);
                oCmd.Parameters.AddWithValue("@in_out", model.in_out);
                oCmd.Parameters.AddWithValue("@terminal_id", model.terminal_id);
                oCmd.Parameters.AddWithValue("@created_by", model.created_by);
                oCmd.Parameters.AddWithValue("@date_created", model.date_created);
                oCmd.Parameters.AddWithValue("@created_by_new", created_by);


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


        public List<AttendanceCLResponse> employee_attendance_cl_view(string series_code, string date_from, string date_to, string employee_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);
            employee_id = Crypto.url_decrypt(employee_id);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<AttendanceCLResponse> resp = new List<AttendanceCLResponse>();
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
                oCmd.CommandText = "employee_attendance_cl_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new AttendanceCLResponse()
                        {

                            employee_id   = Convert.ToInt32(dr["employee_id"].ToString()),
                            date          = (dr["date"].ToString()),
                            time_in       = (dr["time_in"].ToString()),
                            time_out      = (dr["time_out"].ToString()),
                            sked_time_in  = (dr["sked_time_in"].ToString()),
                            sked_time_out = (dr["sked_time_out"].ToString()),
                            remarks       = (dr["remarks"].ToString()),
                            is_update     = Convert.ToBoolean(dr["is_update"].ToString()),



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


        public List<AttendanceDashboardResponse> employee_attendance_dashboard_view(string series_code, string date_from, string date_to, string employee_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);
            employee_id = Crypto.url_decrypt(employee_id);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<AttendanceDashboardResponse> resp = new List<AttendanceDashboardResponse>();
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
                oCmd.CommandText = "employee_attendance_dashboard_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new AttendanceDashboardResponse()
                        {

                            title            = (dr["title"].ToString()),
                            start            = (dr["start"].ToString()),
                            end              = (dr["end"].ToString()),
                            description      = (dr["description"].ToString()),
                            backgroundColor  = (dr["backgroundcolor"].ToString()),
                            borderColor      = (dr["bordercolor"].ToString()),
                            id               = Convert.ToInt32(dr["id"].ToString()),



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


        public int attendance_log_cl_in(AttendanceCLRequest model)
        {
            int resp = 0;
            model.employee_id = model.employee_id == "0" ? "0" : Crypto.url_decrypt(model.employee_id);
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
            SqlDataAdapter da = new SqlDataAdapter();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "attendance_log_cl_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", model.employee_id);
                oCmd.Parameters.AddWithValue("@date", model.date);
                oCmd.Parameters.AddWithValue("@time_in", model.time_in);
                oCmd.Parameters.AddWithValue("@time_out", model.time_out);
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

        public int employee_schedule_detail_auto(AttendanceLogApprovalRequest model)
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
            SqlDataAdapter da = new SqlDataAdapter();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "employee_schedule_detail_auto";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@transaction_id", model.transaction_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@series_code", series_code);


                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {
                    resp = Convert.ToInt32(sdr["module_id"].ToString());

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

        public int attendance_log_approval_in(AttendanceLogApprovalRequest model)
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
            SqlDataAdapter da = new SqlDataAdapter();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {

                da.SelectCommand = oCmd;
                oCmd.CommandText = "attendance_log_approval_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();


                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@transaction_id", model.transaction_id);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
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

        public int attendance_log_in(AttendanceRequest model)
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
            SqlDataAdapter da = new SqlDataAdapter();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            da.SelectCommand = oCmd;
            try
            {

                oCmd.CommandText = "attendance_log_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@in_out", model.in_out);
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


        public List<EmployeeAttendanceResponse> employee_attendance_view(string series_code, string date_from, string date_to, string employee_id, bool missing_logs_only, bool is_supervisor, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);


            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<EmployeeAttendanceResponse> resp = new List<EmployeeAttendanceResponse>();
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
                oCmd.CommandText = "employee_attendance_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@missing_logs_only", missing_logs_only);
                oCmd.Parameters.AddWithValue("@is_supervisor", is_supervisor);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new EmployeeAttendanceResponse()
                        {

                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            encrypt_employee_id = Crypto.url_encrypt(dr["employee_id"].ToString()),
                            date = (dr["date"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            time_in = (dr["time_in"].ToString()),
                            time_out = (dr["time_out"].ToString()),
                            sked_in = (dr["sked_in"].ToString()),
                            sked_out = (dr["sked_out"].ToString()),
                            remarks = (dr["remarks"].ToString()),
                            missing_logs = Convert.ToBoolean(dr["missing_logs"].ToString()),
                            is_add = Convert.ToBoolean(dr["is_add"].ToString()),



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


        public int employee_attendance_up(EmployeeAttendanceRequest model)
        {
            int resp = 0;
            string series_code = Crypto.url_decrypt(model.series_code);
            string created_by = Crypto.url_decrypt(model.created_by);
            string employee_id = Crypto.url_decrypt(model.employee_id);
            //BranchResponse resp = new BranchResponse();
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

                oCmd.CommandText = "employee_attendance_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id",employee_id);
                oCmd.Parameters.AddWithValue("@date",model.date);
                oCmd.Parameters.AddWithValue("@time_in",model.time_in);
                oCmd.Parameters.AddWithValue("@time_out",model.time_out);
                oCmd.Parameters.AddWithValue("@sked_time_in",model.sked_time_in);
                oCmd.Parameters.AddWithValue("@sked_time_out",model.sked_time_out);
                oCmd.Parameters.AddWithValue("@created_by",created_by);


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
    }




}
