
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using LeaveManagementService.Helper;
using LeaveManagementService.Model;
using System.Data.SqlClient;

namespace LeaveManagementService.Service
{

    public interface ILeaveManagementServices
    {
        List<LeaveTypeResponse> leave_type_view_sel(string series_code, string leave_type_id,string created_by);
        int leave_type_in_up(LeaveTypeRequest model);
        List<LeaveTypeResponse> leave_type_view(string series_code, string leave_type_id, string created_by);

        List<EmployeeMovementRequest> leave_entitlement_in(LeaveEntitlementRequest[] model);

        int leave_balance_in(UploadInRequest model);

        List<EmployeeLeaveResponse> leave_employee_view(string series_code, string leave_type_id, string employee_id, string created_by);
        InsertResponse leave_restriction(string series_code,string leave_id, string employee_id, int leave_type_id, bool with_pay, bool is_half_day, bool with_attachment, string date_from, string date_to, string created_by);
    }

    public class LeaveManagementServices : ILeaveManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public LeaveManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }


        public int leave_type_in_up(LeaveTypeRequest model)
        {
            int resp = 0;
            model.leave_type_id = model.leave_type_id == "0" ? "0" : Crypto.url_decrypt(model.leave_type_id);
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
                oCmd.CommandText = "leave_type_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);

                oCmd.Parameters.AddWithValue("@leave_type_id", model.leave_type_id);
                oCmd.Parameters.AddWithValue("@leave_type_code", model.leave_type_code);
                oCmd.Parameters.AddWithValue("@leave_name", model.leave_name);
                oCmd.Parameters.AddWithValue("@description", model.description);
                oCmd.Parameters.AddWithValue("@gender_to_use", model.gender_to_use);
                oCmd.Parameters.AddWithValue("@required_attachment", model.required_attachment);
                oCmd.Parameters.AddWithValue("@filed_by", model.filed_by);
                oCmd.Parameters.AddWithValue("@leave_start", model.leave_start);
                oCmd.Parameters.AddWithValue("@total_leaves", model.total_leaves);
                oCmd.Parameters.AddWithValue("@leave_accrued", model.leave_accrued);
                oCmd.Parameters.AddWithValue("@accrued_credits", model.accrued_credits);
                oCmd.Parameters.AddWithValue("@leave_per_month", model.leave_per_month);
                oCmd.Parameters.AddWithValue("@convertible_to_cash", model.convertible_to_cash);
                oCmd.Parameters.AddWithValue("@taxable_credits", model.taxable_credits);
                oCmd.Parameters.AddWithValue("@non_taxable_credits", model.non_taxable_credits);
                oCmd.Parameters.AddWithValue("@priority_to_convert", model.priority_to_convert);
                oCmd.Parameters.AddWithValue("@leave_before", model.leave_before);
                oCmd.Parameters.AddWithValue("@leave_after", model.leave_after);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                    resp = Convert.ToInt32(sdr["leave_type_id"].ToString());

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


        public List<LeaveTypeResponse> leave_type_view_sel(string series_code, string leave_type_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            leave_type_id = leave_type_id == "0" ? "0" : Crypto.url_decrypt(leave_type_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<LeaveTypeResponse> resp = new List<LeaveTypeResponse>();
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
                oCmd.CommandText = "leave_type_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@leave_type_id", leave_type_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new LeaveTypeResponse()
                        {
                            leave_type_id = Convert.ToInt32(dr["leave_type_id"].ToString()),
                            encrypted_leave_type_id = Crypto.url_encrypt(dr["leave_type_id"].ToString()),
                            leave_type_code = (dr["leave_type_code"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            filed_by_name = (dr["filed_by_name"].ToString()),
                            leave_name = (dr["leave_name"].ToString()),
                            description = (dr["description"].ToString()),
                            gender_to_use = Convert.ToInt32(dr["gender_to_use"].ToString()),
                            required_attachment = Convert.ToBoolean(dr["required_attachment"].ToString()),
                            filed_by = Convert.ToInt32(dr["filed_by"].ToString()),
                            leave_start = Convert.ToInt32(dr["leave_start"].ToString()),
                            total_leaves = Convert.ToDecimal(dr["total_leaves"].ToString()),
                            leave_accrued = Convert.ToInt32(dr["leave_accrued"].ToString()),
                            accrued_credits = Convert.ToDecimal(dr["accrued_credits"].ToString()),
                            leave_per_month = Convert.ToDecimal(dr["leave_per_month"].ToString()),
                            convertible_to_cash = Convert.ToDecimal(dr["convertible_to_cash"].ToString()),
                            taxable_credits = Convert.ToDecimal(dr["taxable_credits"].ToString()),
                            non_taxable_credits = Convert.ToDecimal(dr["non_taxable_credits"].ToString()),
                            priority_to_convert = Convert.ToInt32(dr["priority_to_convert"].ToString()),
                            leave_before = Convert.ToInt32(dr["leave_before"].ToString()),
                            leave_after = Convert.ToInt32(dr["leave_after"].ToString()),

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


        public List<LeaveTypeResponse> leave_type_view(string series_code, string leave_type_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            leave_type_id = leave_type_id == "0" ? "0" : Crypto.url_decrypt(leave_type_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<LeaveTypeResponse> resp = new List<LeaveTypeResponse>();
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
                oCmd.CommandText = "leave_type_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@leave_type_id", leave_type_id);
                //oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new LeaveTypeResponse()
                        {
                            leave_type_id = Convert.ToInt32(dr["leave_type_id"].ToString()),
                            encrypted_leave_type_id = Crypto.url_encrypt(dr["leave_type_id"].ToString()),
                            leave_type_code = (dr["leave_type_code"].ToString()),
                            leave_name = (dr["leave_name"].ToString()),
                            description = (dr["description"].ToString()),
                            gender_to_use = Convert.ToInt32(dr["gender_to_use"].ToString()),
                            required_attachment = Convert.ToBoolean(dr["required_attachment"].ToString()),
                            filed_by = Convert.ToInt32(dr["filed_by"].ToString()),
                            leave_start = Convert.ToInt32(dr["leave_start"].ToString()),
                            total_leaves = Convert.ToDecimal(dr["total_leaves"].ToString()),
                            leave_accrued = Convert.ToInt32(dr["leave_accrued"].ToString()),
                            accrued_credits = Convert.ToDecimal(dr["accrued_credits"].ToString()),
                            leave_per_month = Convert.ToDecimal(dr["leave_per_month"].ToString()),
                            convertible_to_cash = Convert.ToDecimal(dr["convertible_to_cash"].ToString()),
                            taxable_credits = Convert.ToDecimal(dr["taxable_credits"].ToString()),
                            non_taxable_credits = Convert.ToDecimal(dr["non_taxable_credits"].ToString()),
                            priority_to_convert = Convert.ToInt32(dr["priority_to_convert"].ToString()),
                            leave_before = Convert.ToInt32(dr["leave_before"].ToString()),
                            leave_after = Convert.ToInt32(dr["leave_after"].ToString()),

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


        public List<EmployeeMovementRequest> leave_entitlement_in(LeaveEntitlementRequest[] model)
        {
            List<EmployeeMovementRequest> resp = new List<EmployeeMovementRequest>();
            string leave_type_id = model[0].leave_type_id == "0" ? "0" : Crypto.url_decrypt(model[0].leave_type_id);
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



                    oCmd.CommandText = "leave_entitlement_in";
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();
                    oCmd.Parameters.AddWithValue("@created_by", created_by);
                    oCmd.Parameters.AddWithValue("@leave_type_id", leave_type_id);
                    oCmd.Parameters.AddWithValue("@employee_id", item.employee_id);
                    da.Fill(dt);
                    resp = (from DataRow dr in dt.Rows
                            select new EmployeeMovementRequest()
                            {
                                employee_id             = Convert.ToInt32(dr["employee_id"].ToString()),
                                movement_type           = Convert.ToInt32(dr["movement_type"].ToString()),
                                is_dropdown             = Convert.ToInt32(dr["is_dropdown"].ToString()),
                                id                      = Convert.ToInt32(dr["id"].ToString()),
                                description             = (dr["description"].ToString()),
                                movement_description    = "",                          
                                created_by              = (model[0].created_by),
                                series_code             = (model[0].series_code),

                            }).ToList();
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



        public int leave_balance_in(UploadInRequest model)
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
                oCmd.CommandText = "leave_balance_in";
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


        public List<EmployeeLeaveResponse> leave_employee_view(string series_code, string leave_type_id, string employee_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            leave_type_id = leave_type_id == "0" ? "0" : Crypto.url_decrypt(leave_type_id);
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<EmployeeLeaveResponse> resp = new List<EmployeeLeaveResponse>();
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
                oCmd.CommandText = "leave_employee_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@leave_type_id", leave_type_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new EmployeeLeaveResponse()
                        {

                            employee_id                 = Convert.ToInt32(dr["employee_id"].ToString()),
                            leave_type_id               = Convert.ToInt32(dr["leave_type_id"].ToString()),
                            leave_type_code             = (dr["leave_type_code"].ToString()),
                            leave_name                  = (dr["leave_name"].ToString()),
                            description                 = (dr["description"].ToString()),
                            gender_to_use               = Convert.ToInt32(dr["gender_to_use"].ToString()),
                            required_attachment         = Convert.ToBoolean(dr["required_attachment"].ToString()),
                            filed_by                    = Convert.ToInt32(dr["filed_by"].ToString()),
                            leave_start                 = (dr["leave_start"].ToString()),
                            total_leaves                = Convert.ToDecimal(dr["total_leaves"].ToString()),
                            leave_accrued               = Convert.ToDecimal(dr["leave_accrued"].ToString()),
                            leave_accrued_description   = (dr["leave_accrued_description"].ToString()),
                            accrued_credits             = Convert.ToDecimal(dr["accrued_credits"].ToString()),
                            leave_total                 = Convert.ToDecimal(dr["leave_total"].ToString()),
                            leave_used                  = Convert.ToDecimal(dr["leave_used"].ToString()),
                            leave_balance               = Convert.ToDecimal(dr["leave_balance"].ToString()),
                            leave_per_month             = Convert.ToDecimal(dr["leave_per_month"].ToString()),
                            convertible_to_cash         = Convert.ToDecimal(dr["convertible_to_cash"].ToString()),
                            taxable_credits             = Convert.ToDecimal(dr["taxable_credits"].ToString()),
                            non_taxable_credits         = Convert.ToDecimal(dr["non_taxable_credits"].ToString()),
                            priority_to_convert         = Convert.ToInt32(dr["priority_to_convert"].ToString()),
                            created_by                  = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created                = (dr["date_created"].ToString()),
                            active                      = Convert.ToBoolean(dr["active"].ToString()),



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


        public InsertResponse leave_restriction(string series_code, string leave_id, string employee_id, int leave_type_id, bool with_pay, bool is_half_day, bool with_attachment, string date_from, string date_to, string created_by)
        {
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            leave_id = leave_id == "0" ? "0" : Crypto.url_decrypt(leave_id);
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);
            InsertResponse resp = new InsertResponse();


            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
            DataTable dt = new DataTable();
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;

            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            oCmd.Connection = oConn;
            SqlDataAdapter da = new SqlDataAdapter();
            oCmd.Transaction = oTrans;
            try
            {
                oCmd.CommandText = "leave_restriction";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@leave_id", leave_id);
                oCmd.Parameters.AddWithValue("@leave_type_id", leave_type_id);
                oCmd.Parameters.AddWithValue("@with_pay", with_pay);
                oCmd.Parameters.AddWithValue("@is_half_day", is_half_day);
                oCmd.Parameters.AddWithValue("@with_attachment", with_attachment);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                    resp.id = 0;
                    resp.late_filing = Convert.ToBoolean(sdr["late_filing"].ToString());
                    resp.is_restricted = Convert.ToBoolean(sdr["is_restricted"].ToString());
                    resp.description = (sdr["remarks"].ToString());
                    resp.is_save = Convert.ToBoolean(sdr["is_save"].ToString());

                }
                sdr.Close();
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
