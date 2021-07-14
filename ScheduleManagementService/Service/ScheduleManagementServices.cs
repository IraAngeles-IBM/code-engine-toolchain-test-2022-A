
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using ScheduleManagementService.Helper;
using System.Data.SqlClient;
using ScheduleManagementService.Model;

namespace ScheduleManagementService.Service
{

    public interface IScheduleManagementServices
    {
        int shift_code_in_up(ShiftCodeRequest model);
        List<ShiftCodeResponse> shift_code_view_sel(string series_code, string shift_id,string created_by);

        List<ShiftCodeResponse> shift_code_view(string series_code, string shift_id, string created_by);
        int shift_code_per_day_in(ShiftCodePerDayHeaderlRequest model);

        List<ShiftPerDayHeaderResponse> shift_code_per_day_view_sel(string series_code, string shift_per_day_id, string created_by);
        List<ShiftPerDayDetailResponse> shift_code_per_day_detail_view_sel(string series_code, string shift_per_day_id, string created_by);
    }

    public class ScheduleManagementServices : IScheduleManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public ScheduleManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }


        public int shift_code_in_up(ShiftCodeRequest model)
        {
            int resp = 0;
            model.shift_id = model.shift_id == "0" ? "0" : Crypto.url_decrypt(model.shift_id);
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
              
                        oCmd.CommandText = "shift_code_in_up";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@shift_id", model.shift_id);
                        oCmd.Parameters.AddWithValue("@shift_code", model.shift_code);
                        oCmd.Parameters.AddWithValue("@shift_name", model.shift_name);
                        oCmd.Parameters.AddWithValue("@grace_period", model.grace_period);
                        oCmd.Parameters.AddWithValue("@description", model.description);
                        oCmd.Parameters.AddWithValue("@is_flexi", model.is_flexi);
                        oCmd.Parameters.AddWithValue("@time_in", model.time_in);
                        oCmd.Parameters.AddWithValue("@time_out", model.time_out);
                        oCmd.Parameters.AddWithValue("@time_out_days_cover", model.time_out_days_cover);
                        oCmd.Parameters.AddWithValue("@total_working_hours", model.total_working_hours);
                        oCmd.Parameters.AddWithValue("@is_rd_mon", model.is_rd_mon);
                        oCmd.Parameters.AddWithValue("@is_rd_tue", model.is_rd_tue);
                        oCmd.Parameters.AddWithValue("@is_rd_wed", model.is_rd_wed);
                        oCmd.Parameters.AddWithValue("@is_rd_thu", model.is_rd_thu);
                        oCmd.Parameters.AddWithValue("@is_rd_fri", model.is_rd_fri);
                        oCmd.Parameters.AddWithValue("@is_rd_sat", model.is_rd_sat);
                        oCmd.Parameters.AddWithValue("@is_rd_sun", model.is_rd_sun);
                        oCmd.Parameters.AddWithValue("@half_day_in", model.half_day_in);
                        oCmd.Parameters.AddWithValue("@half_day_in_days_cover", model.half_day_in_days_cover);
                        oCmd.Parameters.AddWithValue("@half_day_out", model.half_day_out);
                        oCmd.Parameters.AddWithValue("@half_day_out_days_cover", model.half_day_out_days_cover);
                        oCmd.Parameters.AddWithValue("@night_dif_in", model.night_dif_in);
                        oCmd.Parameters.AddWithValue("@night_dif_in_days_cover", model.night_dif_in_days_cover);
                        oCmd.Parameters.AddWithValue("@night_dif_out", model.night_dif_out);
                        oCmd.Parameters.AddWithValue("@night_dif_out_days_cover", model.night_dif_out_days_cover);
                        oCmd.Parameters.AddWithValue("@first_break_in", model.first_break_in);
                        oCmd.Parameters.AddWithValue("@first_break_in_days_cover", model.first_break_in_days_cover);
                        oCmd.Parameters.AddWithValue("@first_break_out", model.first_break_out);
                        oCmd.Parameters.AddWithValue("@first_break_out_days_cover", model.first_break_out_days_cover);
                        oCmd.Parameters.AddWithValue("@second_break_in", model.second_break_in);
                        oCmd.Parameters.AddWithValue("@second_break_in_days_cover", model.second_break_in_days_cover);
                        oCmd.Parameters.AddWithValue("@second_break_out", model.second_break_out);
                        oCmd.Parameters.AddWithValue("@second_break_out_days_cover", model.second_break_out_days_cover);
                        oCmd.Parameters.AddWithValue("@third_break_in", model.third_break_in);
                        oCmd.Parameters.AddWithValue("@third_break_in_days_cover", model.third_break_in_days_cover);
                        oCmd.Parameters.AddWithValue("@third_break_out", model.third_break_out);
                        oCmd.Parameters.AddWithValue("@third_break_out_days_cover", model.third_break_out_days_cover);
                        oCmd.Parameters.AddWithValue("@active", model.active);
                        oCmd.Parameters.AddWithValue("@created_by", created_by);

                //oCmd.ExecuteNonQuery();
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


        public List<ShiftCodeResponse> shift_code_view_sel(string series_code, string shift_id, string created_by)
        {

            series_code = Crypto.url_decrypt(series_code);
            shift_id = shift_id == "0" ? "0" : Crypto.url_decrypt(shift_id);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ShiftCodeResponse> resp = new List<ShiftCodeResponse>();
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
                oCmd.CommandText = "shift_code_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@shift_id", shift_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ShiftCodeResponse()
                        {
                            shift_id = Crypto.url_encrypt(dr["shift_id"].ToString()),
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
                            first_break_out_days_cover = Convert.ToInt32(dr["first_break_out_days_cover"].ToString()),
                            second_break_in = (dr["second_break_in"].ToString()),
                            second_break_in_days_cover = Convert.ToInt32(dr["second_break_in_days_cover"].ToString()),
                            second_break_out = (dr["second_break_out"].ToString()),
                            second_break_out_days_cover = Convert.ToInt32(dr["second_break_out_days_cover"].ToString()),
                            third_break_in = (dr["third_break_in"].ToString()),
                            third_break_in_days_cover = Convert.ToInt32(dr["third_break_in_days_cover"].ToString()),
                            third_break_out = (dr["third_break_out"].ToString()),
                            third_break_out_days_cover = Convert.ToInt32(dr["third_break_out_days_cover"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            created_by_name = dr["created_by_name"].ToString(),
                            status = dr["status"].ToString(),

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


        public List<ShiftCodeResponse> shift_code_view(string series_code, string shift_id, string created_by)
        {

            series_code = Crypto.url_decrypt(series_code);
            shift_id = shift_id == "0" ? "0" : Crypto.url_decrypt(shift_id);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ShiftCodeResponse> resp = new List<ShiftCodeResponse>();
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
                oCmd.CommandText = "shift_code_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@shift_id", shift_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ShiftCodeResponse()
                        {
                            shift_id = Crypto.url_encrypt(dr["shift_id"].ToString()),
                            int_shift_id = Convert.ToInt32(dr["shift_id"].ToString()),
                            shift_code = (dr["shift_code"].ToString()),
                            shift_name = (dr["shift_name"].ToString()),
                            grace_period = Convert.ToInt32(dr["grace_period"].ToString()),
                            description = (dr["description"].ToString()),
                            is_flexi = Convert.ToBoolean(dr["is_flexi"].ToString()),
                            shift_code_type = Convert.ToInt32(dr["shift_code_type"].ToString()),
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
                            first_break_out_days_cover = Convert.ToInt32(dr["first_break_out_days_cover"].ToString()),
                            second_break_in = (dr["second_break_in"].ToString()),
                            second_break_in_days_cover = Convert.ToInt32(dr["second_break_in_days_cover"].ToString()),
                            second_break_out = (dr["second_break_out"].ToString()),
                            second_break_out_days_cover = Convert.ToInt32(dr["second_break_out_days_cover"].ToString()),
                            third_break_in = (dr["third_break_in"].ToString()),
                            third_break_in_days_cover = Convert.ToInt32(dr["third_break_in_days_cover"].ToString()),
                            third_break_out = (dr["third_break_out"].ToString()),
                            third_break_out_days_cover = Convert.ToInt32(dr["third_break_out_days_cover"].ToString()),
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




        public int shift_code_per_day_in(ShiftCodePerDayHeaderlRequest model)
        {
            int resp = 0;
            model.shift_per_day_id = model.shift_per_day_id == "0" ? "0" : Crypto.url_decrypt(model.shift_per_day_id);
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

                oCmd.CommandText = "shift_code_per_day_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@shift_per_day_id", model.shift_per_day_id);
                oCmd.Parameters.AddWithValue("@shift_per_day_code", model.shift_per_day_code);
                oCmd.Parameters.AddWithValue("@shift_name", model.shift_name);
                oCmd.Parameters.AddWithValue("@grace_period", model.grace_period);
                oCmd.Parameters.AddWithValue("@description", model.description);
                oCmd.Parameters.AddWithValue("@is_flexi", model.is_flexi);
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                //oCmd.ExecuteNonQuery();
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                    resp = Convert.ToInt32(sdr["shift_per_day_id"].ToString());

                }
                sdr.Close();



                if (model.Detail != null)
                {
                    foreach (var item in model.Detail)
                    {

                        oCmd.CommandText = "shift_code_per_day_detail_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@shift_per_day_id", resp);
                        oCmd.Parameters.AddWithValue("@shift_per_day_code", model.shift_per_day_code);
                        oCmd.Parameters.AddWithValue("@shift_name", model.shift_name);
                        oCmd.Parameters.AddWithValue("@grace_period", model.grace_period);
                        oCmd.Parameters.AddWithValue("@description", model.description);
                        oCmd.Parameters.AddWithValue("@is_flexi", model.is_flexi);
                        oCmd.Parameters.AddWithValue("@time_in", item.time_in);
                        oCmd.Parameters.AddWithValue("@time_out", item.time_out);
                        oCmd.Parameters.AddWithValue("@total_working_hours", item.total_working_hours);
                        oCmd.Parameters.AddWithValue("@half_day_in", item.half_day_in);
                        oCmd.Parameters.AddWithValue("@half_day_out", item.half_day_out);
                        oCmd.Parameters.AddWithValue("@night_dif_in", item.night_dif_in);
                        oCmd.Parameters.AddWithValue("@night_dif_out", item.night_dif_out);
                        oCmd.Parameters.AddWithValue("@first_break_in", item.first_break_in);
                        oCmd.Parameters.AddWithValue("@first_break_out", item.first_break_out);
                        oCmd.Parameters.AddWithValue("@second_break_in", item.second_break_in);
                        oCmd.Parameters.AddWithValue("@second_break_out", item.second_break_out);
                        oCmd.Parameters.AddWithValue("@third_break_in", item.third_break_in);
                        oCmd.Parameters.AddWithValue("@third_break_out", item.third_break_out);
                        oCmd.Parameters.AddWithValue("@day", item.day);
                        oCmd.Parameters.AddWithValue("@is_rest_day", item.is_rest_day);
                        oCmd.Parameters.AddWithValue("@active", item.active);
                        oCmd.Parameters.AddWithValue("@break_count", item.break_count);
                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                         sdr = oCmd.ExecuteReader();
                        while (sdr.Read())
                        {

                            //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                            resp = Convert.ToInt32(sdr["shift_per_day_id"].ToString());

                        }
                        sdr.Close();


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



        public List<ShiftPerDayHeaderResponse> shift_code_per_day_view_sel(string series_code, string shift_per_day_id, string created_by)
        {

            series_code = Crypto.url_decrypt(series_code);
            shift_per_day_id = shift_per_day_id == "0" ? "0" : Crypto.url_decrypt(shift_per_day_id);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ShiftPerDayHeaderResponse> resp = new List<ShiftPerDayHeaderResponse>();
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
                oCmd.CommandText = "shift_code_per_day_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@shift_per_day_id ", shift_per_day_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ShiftPerDayHeaderResponse()
                        {
                            int_shift_per_day_id = Convert.ToInt32(dr["shift_per_day_id"].ToString()),
                            shift_per_day_id = Crypto.url_encrypt(dr["shift_per_day_id"].ToString()),
                            shift_per_day_code = (dr["shift_per_day_code"].ToString()),
                            shift_name = (dr["shift_name"].ToString()),
                            grace_period = Convert.ToInt32(dr["grace_period"].ToString()),
                            description = (dr["description"].ToString()),
                            is_flexi = Convert.ToBoolean(dr["is_flexi"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            created_by_name = dr["created_by_name"].ToString(),
                            status = dr["status"].ToString(),

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


        public List<ShiftPerDayDetailResponse> shift_code_per_day_detail_view_sel(string series_code, string shift_per_day_id, string created_by)
        {

            series_code = Crypto.url_decrypt(series_code);
            shift_per_day_id = shift_per_day_id == "0" ? "0" : Crypto.url_decrypt(shift_per_day_id);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ShiftPerDayDetailResponse> resp = new List<ShiftPerDayDetailResponse>();
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
                oCmd.CommandText = "shift_code_per_day_detail_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@shift_per_day_id", shift_per_day_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ShiftPerDayDetailResponse()
                        {
                            int_shift_per_day_id = Convert.ToInt32(dr["shift_per_day_id"].ToString()),
                            shift_per_day_id = Crypto.url_encrypt(dr["shift_per_day_id"].ToString()),
                            shift_per_day_code = (dr["shift_per_day_code"].ToString()),
                            shift_name = (dr["shift_name"].ToString()),
                            grace_period = Convert.ToInt32(dr["grace_period"].ToString()),
                            description = (dr["description"].ToString()),
                            is_flexi = Convert.ToBoolean(dr["is_flexi"].ToString()),
                            time_in = (dr["time_in"].ToString()),
                            time_out = (dr["time_out"].ToString()),
                            time_out_days_cover = Convert.ToInt32(dr["time_out_days_cover"].ToString()),
                            total_working_hours = Convert.ToDecimal(dr["total_working_hours"].ToString()),
                            day = Convert.ToInt32(dr["day"].ToString()),
                            day_name = (dr["day_name"].ToString()),
                            is_rest_day = Convert.ToBoolean(dr["is_rest_day"].ToString()),
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
                            first_break_out_days_cover = Convert.ToInt32(dr["first_break_out_days_cover"].ToString()),
                            second_break_in = (dr["second_break_in"].ToString()),
                            second_break_in_days_cover = Convert.ToInt32(dr["second_break_in_days_cover"].ToString()),
                            second_break_out = (dr["second_break_out"].ToString()),
                            second_break_out_days_cover = Convert.ToInt32(dr["second_break_out_days_cover"].ToString()),
                            third_break_in = (dr["third_break_in"].ToString()),
                            third_break_in_days_cover = Convert.ToInt32(dr["third_break_in_days_cover"].ToString()),
                            third_break_out = (dr["third_break_out"].ToString()),
                            third_break_out_days_cover = Convert.ToInt32(dr["third_break_out_days_cover"].ToString()),
                            break_count = Convert.ToInt32(dr["break_count"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            created_by_name = dr["created_by_name"].ToString(),
                            status = dr["status"].ToString(),

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
