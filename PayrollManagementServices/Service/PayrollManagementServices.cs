
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using PayrollManagementService.Model;
using PayrollManagementService.Helper;
using PayrollManagementService;
using System.Data.SqlClient;
using System.Reflection;

namespace PayrollManagementService.Service
{

    public interface IPayrollManagementServices
    {

        #region "Approval"
        ApprovalResponse approval_process_in(ApprovalSequenceRequest[] model);
        ApprovalResponse transaction_status_up(TransactionStatusRequest model);
        ApprovalResponse transaction_approval_up(ApprovalRequest model);
        #endregion

        #region "Payroll"
        PayrollHeaderInReponse payroll_header_in(PayrollHeaderRequest model);

        List<PayrollHeaderResponse> payroll_header_view_sel(string series_code, string payroll_header_id, string created_by);
        #endregion

        #region "Payroll Adjustment"
        int payroll_adjustment_in(PayrollAdjustmentRequest[] model);
        List<PayrollAdjustmentResponse> payroll_adjustment_view(string series_code, string payroll_header_id, string created_by);
        #endregion


        #region "Payroll Generation"
        List<PayrollGenerationResponse> payroll_generation_view(string series_code, string payroll_header_id, int category_id, int branch_id, int confidential_id, bool include_tax, bool include_sss, bool include_pagibig, bool include_philhealth, string created_by);

        List<PayslipDetaiResponse> payslip_detail_temp_view(string series_code, string payroll_header_id, int employee_id, string created_by);
        List<PayslipDetaiResponse> payslip_detail_view(string series_code, string payroll_header_id, int payroll_id, int payslip_id, int employee_id, string created_by);

        int payroll_in(PayrollRequest model);
        List<PayrollResponse> payroll_view_sel(string series_code, string payroll_header_id, int payroll_id, string created_by);
        List<PayrollResponse> payroll_view(string series_code, string payroll_header_id, int payroll_id, string created_by);
        #endregion

        #region "Payslip"
        List<PayrollGenerationResponse> payslip_view(string series_code, string payroll_header_id, int payroll_id, int payslip_id, string created_by);
        #endregion

        #region "Posted Payslip"
        int posted_payslip_in(PostedPayrollRequest[] model);
        List<PayrollGenerationResponse> posted_payslip_view(string series_code, string payroll_header_id, int posted_payslip_id, string created_by);
        List<PayslipDetaiResponse> posted_payslip_detail_view(string series_code, string payroll_header_id, int posted_payslip_id, int employee_id, string created_by);
        List<PayrollGenerationResponse> posted_payslip_employee_view(string series_code, string employee_id, string created_by);
        List<PayslipDetaiResponse> posted_payslip_detail_employee_view(string series_code, string payroll_header_id, int posted_payslip_id, int employee_id, string created_by);
        #endregion

        #region"Timekeeping"
        List<TimekeepingGenerationResponse> timekeeping_final_temp_view_sel(string series_code, string payroll_header_id, int employee_id, string created_by);
        List<TimekeepingGenerationResponse> timekeeping_final_view_sel(string series_code, string payroll_header_id, int payroll_id, int payslip_id, int employee_id, int timekeeping_header_id, string created_by);
        List<TimekeepingGenerationResponse> posted_timekeeping_final_view_sel(string series_code, string payroll_header_id, int posted_payslip_id, int employee_id, int timekeeping_header_id, string created_by);
        int timekeeping_upload_in(UploadInRequest model);
        #endregion

        #region"Uploader"
        int timekeeping_upload_del(string series_code, string payroll_header_id, string created_by);
        int timekeeping_upload(List<TimekeepingUpload> model, string series_code, string created_by);
        #endregion

        #region "Reports"
        List<rpt1601cResponse> report_1601c(string series_code, string employee_id, string date_from, string date_to);
        #endregion
    }

    public class PayrollManagementServices : IPayrollManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public PayrollManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
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

                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp.module_id = Convert.ToInt32(sdr["module_id"].ToString());
                    resp.string_transaction_id = (sdr["transaction_id"].ToString());
                    resp.approved = Convert.ToBoolean(sdr["approved"].ToString());

                }
                sdr.Close();


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


        #endregion


        #region "Payroll"

        public PayrollHeaderInReponse payroll_header_in(PayrollHeaderRequest model)
        {
            PayrollHeaderInReponse resp = new PayrollHeaderInReponse();
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
                oCmd.CommandText = "payroll_header_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_code", model.payroll_header_code);
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", model.timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@payroll_basis_id", model.payroll_basis_id);
                oCmd.Parameters.AddWithValue("@payroll_type_id", model.payroll_type_id);
                oCmd.Parameters.AddWithValue("@cutoff_id", model.cutoff_id);
                oCmd.Parameters.AddWithValue("@month_id", model.month_id);
                oCmd.Parameters.AddWithValue("@date_from", model.date_from);
                oCmd.Parameters.AddWithValue("@date_to", model.date_to);
                oCmd.Parameters.AddWithValue("@category_id", model.category_id);
                oCmd.Parameters.AddWithValue("@branch_id", model.branch_id);
                oCmd.Parameters.AddWithValue("@confidentiality_id", model.confidentiality_id);

                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp.payroll_header_id = Convert.ToInt32(sdr["payroll_header_id"].ToString());
                    resp.encrypted_payroll_header_id = Crypto.url_encrypt(sdr["payroll_header_id"].ToString());

                }
                sdr.Close();
                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                resp.payroll_header_id = 0;
                oTrans.Rollback();
            }
            finally
            {
                oConn.Close();
            }

            return resp;
        }

        public List<PayrollHeaderResponse> payroll_header_view_sel(string series_code, string payroll_header_id, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollHeaderResponse> resp = new List<PayrollHeaderResponse>();
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
                oCmd.CommandText = "payroll_header_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollHeaderResponse()
                        {
                            payroll_header_id = Convert.ToInt32(dr["payroll_header_id"].ToString()),
                            encrypted_payroll_header_id = Crypto.url_encrypt(dr["payroll_header_id"].ToString()),
                            payroll_header_code = (dr["payroll_header_code"].ToString()),
                            timekeeping_header_code = (dr["timekeeping_header_code"].ToString()),
                            payroll_type_id = Convert.ToInt32(dr["payroll_type_id"].ToString()),
                            payroll_type = (dr["payroll_type"].ToString()),
                            cutoff_id = Convert.ToInt32(dr["cutoff_id"].ToString()),
                            cutoff = (dr["cutoff"].ToString()),
                            month_id = Convert.ToInt32(dr["month_id"].ToString()),
                            month = (dr["month"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            pay_date = (dr["pay_date"].ToString()),
                            category_id = Convert.ToInt32(dr["category_id"].ToString()),
                            category_name = (dr["category_name"].ToString()),
                            branch_id = Convert.ToInt32(dr["branch_id"].ToString()),
                            branch = (dr["branch"].ToString()),
                            confidentiality_id = Convert.ToInt32(dr["confidentiality_id"].ToString()),
                            confidentiality = (dr["confidentiality"].ToString()),
                            timekeeping_header_id = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            tk_count = Convert.ToInt32(dr["tk_count"].ToString()),
                            payroll_basis_id = Convert.ToInt32(dr["payroll_basis_id"].ToString()),
                            payroll_basis = (dr["payroll_basis"].ToString()),

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

        #region "Payroll Adjustment"

        public int payroll_adjustment_in(PayrollAdjustmentRequest[] model)
        {
            int resp = 0;
            var payroll_header_id = model[0].payroll_header_id == "0" ? "0" : Crypto.url_decrypt(model[0].payroll_header_id);
            var payroll_adjustment_id = model[0].payroll_adjustment_id == "0" ? "0" : Crypto.url_decrypt(model[0].payroll_adjustment_id);
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
            try
            {
                foreach (var item in model)
                {
                    da.SelectCommand = oCmd;
                    oCmd.CommandText = "payroll_adjustment_in";
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();
                    oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                    oCmd.Parameters.AddWithValue("@payroll_adjustment_id", payroll_adjustment_id);
                    oCmd.Parameters.AddWithValue("@employee_id", item.employee_id);
                    oCmd.Parameters.AddWithValue("@adjustment_type_id", item.adjustment_type_id);
                    oCmd.Parameters.AddWithValue("@adjustment_name", item.adjustment_name);
                    oCmd.Parameters.AddWithValue("@amount", item.amount);
                    oCmd.Parameters.AddWithValue("@taxable", item.taxable);
                    oCmd.Parameters.AddWithValue("@active", item.active);
                    oCmd.Parameters.AddWithValue("@created_by", created_by);

                    SqlDataReader sdr = oCmd.ExecuteReader();
                    while (sdr.Read())
                    {

                        resp = Convert.ToInt32(sdr["payroll_adjustment_id"].ToString());

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
        public List<PayrollAdjustmentResponse> payroll_adjustment_view(string series_code, string payroll_header_id, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollAdjustmentResponse> resp = new List<PayrollAdjustmentResponse>();
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
                oCmd.CommandText = "payroll_adjustment_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollAdjustmentResponse()
                        {
                            payroll_adjustment_id = Convert.ToInt32(dr["payroll_adjustment_id"].ToString()),
                            encrypted_payroll_adjustment_id = Crypto.url_encrypt(dr["payroll_adjustment_id"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            adjustment_type_id = Convert.ToInt32(dr["adjustment_type_id"].ToString()),
                            adjustment_type = (dr["adjustment_type"].ToString()),
                            adjustment_name = (dr["adjustment_name"].ToString()),
                            amount = Convert.ToDecimal(dr["amount"].ToString()),
                            taxable_id = Convert.ToBoolean(dr["taxable_id"].ToString()),
                            taxable = (dr["taxable"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            status = (dr["status"].ToString()),


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

        #region "Payroll Generation"

        public List<PayrollGenerationResponse> payroll_generation_view(string series_code, string payroll_header_id, int category_id, int branch_id, int confidential_id, bool include_tax, bool include_sss, bool include_pagibig, bool include_philhealth, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollGenerationResponse> resp = new List<PayrollGenerationResponse>();
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
                oCmd.CommandText = "payroll_generation_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@category_id", category_id);
                oCmd.Parameters.AddWithValue("@branch_id", branch_id);
                oCmd.Parameters.AddWithValue("@confidential_id", confidential_id);
                oCmd.Parameters.AddWithValue("@include_tax", include_tax);
                oCmd.Parameters.AddWithValue("@include_sss", include_sss);
                oCmd.Parameters.AddWithValue("@include_pagibig", include_pagibig);
                oCmd.Parameters.AddWithValue("@include_philhealth", include_philhealth);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollGenerationResponse()
                        {
                            employee_code = (dr["employee_code"].ToString()),
                            last_name = (dr["last_name"].ToString()),
                            first_name = (dr["first_name"].ToString()),
                            file_status = (dr["file_status"].ToString()),
                            tax_status = (dr["tax_status"].ToString()),
                            daily_rate = Convert.ToDecimal(dr["daily_rate"].ToString()),
                            semi_monthly_rate = Convert.ToDecimal(dr["semi_monthly_rate"].ToString()),
                            monthly_rate = Convert.ToDecimal(dr["monthly_rate"].ToString()),
                            hourly_rate = Convert.ToDecimal(dr["hourly_rate"].ToString()),
                            payroll_code = (dr["payroll_code"].ToString()),
                            payroll_header_id = Convert.ToInt32(dr["payroll_header_id"].ToString()),
                            pay_date = (dr["pay_date"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            basic_salary = Convert.ToDecimal(dr["basic_salary"].ToString()),
                            misc_amount = Convert.ToDecimal(dr["misc_amount"].ToString()),
                            leave_amount = Convert.ToDecimal(dr["leave_amount"].ToString()),
                            overtime = Convert.ToDecimal(dr["overtime"].ToString()),
                            overtime_holiday = Convert.ToDecimal(dr["overtime_holiday"].ToString()),
                            other_tax_income = Convert.ToDecimal(dr["other_tax_income"].ToString()),
                            adjustments = Convert.ToDecimal(dr["adjustments"].ToString()),
                            gross_income = Convert.ToDecimal(dr["gross_income"].ToString()),
                            witholding_tax = Convert.ToDecimal(dr["witholding_tax"].ToString()),
                            net_salary_after_tax = Convert.ToDecimal(dr["net_salary_after_tax"].ToString()),
                            employee_sss = Convert.ToDecimal(dr["employee_sss"].ToString()),
                            employee_mcr = Convert.ToDecimal(dr["employee_mcr"].ToString()),
                            employee_pagibig = Convert.ToDecimal(dr["employee_pagibig"].ToString()),
                            other_ntax_income = Convert.ToDecimal(dr["other_ntax_income"].ToString()),
                            loan_payments = Convert.ToDecimal(dr["loan_payments"].ToString()),
                            deductions = Convert.ToDecimal(dr["deductions"].ToString()),
                            net_salary = Convert.ToDecimal(dr["net_salary"].ToString()),
                            employer_sss = Convert.ToDecimal(dr["employer_sss"].ToString()),
                            employer_mcr = Convert.ToDecimal(dr["employer_mcr"].ToString()),
                            employer_ec = Convert.ToDecimal(dr["employer_ec"].ToString()),
                            employer_pagibig = Convert.ToDecimal(dr["employer_pagibig"].ToString()),
                            payroll_cost = Convert.ToDecimal(dr["payroll_cost"].ToString()),
                            ytd_gross = Convert.ToDecimal(dr["ytd_gross"].ToString()),
                            ytd_witholding = Convert.ToDecimal(dr["ytd_witholding"].ToString()),
                            ytd_sss = Convert.ToDecimal(dr["ytd_sss"].ToString()),
                            ytd_mcr = Convert.ToDecimal(dr["ytd_mcr"].ToString()),
                            ytd_pagibig = Convert.ToDecimal(dr["ytd_pagibig"].ToString()),
                            ytd_13ntax = Convert.ToDecimal(dr["ytd_13ntax"].ToString()),
                            ytd_13tax = Convert.ToDecimal(dr["ytd_13tax"].ToString()),
                            payment_type = (dr["payment_type"].ToString()),
                            bank_account = (dr["bank_account"].ToString()),
                            bank_name = (dr["bank_name"].ToString()),
                            comment_field = (dr["comment_field"].ToString()),
                            error_field = (dr["error_field"].ToString()),
                            date_employed = (dr["date_employed"].ToString()),
                            date_terminated = (dr["date_terminated"].ToString()),
                            cost_center = (dr["cost_center"].ToString()),
                            Currency = (dr["Currency"].ToString()),
                            exchange_rate = Convert.ToDecimal(dr["exchange_rate"].ToString()),
                            payment_freq = (dr["payment_freq"].ToString()),
                            mtd_gross = Convert.ToDecimal(dr["mtd_gross"].ToString()),
                            mtd_basic = Convert.ToDecimal(dr["mtd_basic"].ToString()),
                            mtd_sss_employee = Convert.ToDecimal(dr["mtd_sss_employee"].ToString()),
                            mtd_mcr_employee = Convert.ToDecimal(dr["mtd_mcr_employee"].ToString()),
                            mtd_pagibig_employee = Convert.ToDecimal(dr["mtd_pagibig_employee"].ToString()),
                            mtd_sss_employer = Convert.ToDecimal(dr["mtd_sss_employer"].ToString()),
                            mtd_mcr_employer = Convert.ToDecimal(dr["mtd_mcr_employer"].ToString()),
                            mtd_ec_employer = Convert.ToDecimal(dr["mtd_ec_employer"].ToString()),
                            mtd_pagibig_employer = Convert.ToDecimal(dr["mtd_pagibig_employer"].ToString()),
                            mtd_wh_tax = Convert.ToDecimal(dr["mtd_wh_tax"].ToString()),
                            monthly_basic = Convert.ToDecimal(dr["monthly_basic"].ToString()),
                            monthly_allow = Convert.ToDecimal(dr["monthly_allow"].ToString()),
                            mtd_ntax = Convert.ToDecimal(dr["mtd_ntax"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            total_addition = Convert.ToDecimal(dr["total_addition"].ToString()),
                            total_deduction = Convert.ToDecimal(dr["total_deduction"].ToString()),
                            company_name = (dr["company_name"].ToString()),
                            pay_period = (dr["pay_period"].ToString()),
                            cutoff_date = (dr["cutoff_date"].ToString()),
                            department = (dr["department"].ToString()),
                            position = (dr["position"].ToString()),
                            payroll_type = (dr["payroll_type"].ToString()),
                            salary_rate = (dr["salary_rate"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),

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


        public int payroll_in(PayrollRequest model)
        {
            int resp = 0;

            model.payroll_header_id = model.payroll_header_id == "0" ? "0" : Crypto.url_decrypt(model.payroll_header_id);
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
                oCmd.CommandText = "payroll_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", model.payroll_header_id);
                oCmd.Parameters.AddWithValue("@payroll_code", model.payroll_code);
                oCmd.Parameters.AddWithValue("@category_id", model.category_id);
                oCmd.Parameters.AddWithValue("@branch_id", model.branch_id);
                oCmd.Parameters.AddWithValue("@confidential_id", model.confidential_id);
                oCmd.Parameters.AddWithValue("@include_tax", model.include_tax);
                oCmd.Parameters.AddWithValue("@include_sss", model.include_sss);
                oCmd.Parameters.AddWithValue("@include_pagibig", model.include_pagibig);
                oCmd.Parameters.AddWithValue("@include_philhealth", model.include_philhealth);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@active", model.active);

                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp = Convert.ToInt32(sdr["payroll_id"].ToString());

                }
                sdr.Close();
                oTrans.Commit();
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


        public List<PayrollResponse> payroll_view_sel(string series_code, string payroll_header_id, int payroll_id, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollResponse> resp = new List<PayrollResponse>();
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
                oCmd.CommandText = "payroll_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@payroll_id", payroll_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollResponse()
                        {
                            payroll_id = Convert.ToInt32(dr["payroll_id"].ToString()),
                            encrypted_payroll_id = (dr["payroll_id"].ToString()),
                            payroll_code = (dr["payroll_code"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            category_id = Convert.ToInt32(dr["category_id"].ToString()),
                            category_name = (dr["category_name"].ToString()),
                            branch_id = Convert.ToInt32(dr["branch_id"].ToString()),
                            branch = (dr["branch"].ToString()),
                            confidentiality_id = Convert.ToInt32(dr["confidentiality_id"].ToString()),
                            confidentiality = (dr["confidentiality"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            approved = Convert.ToBoolean(dr["approved"].ToString()),
                            status = (dr["status"].ToString()),


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

        public List<PayrollResponse> payroll_view(string series_code, string payroll_header_id, int payroll_id, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollResponse> resp = new List<PayrollResponse>();
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
                oCmd.CommandText = "payroll_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@payroll_id", payroll_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollResponse()
                        {
                            payroll_id = Convert.ToInt32(dr["payroll_id"].ToString()),
                            encrypted_payroll_id = (dr["payroll_id"].ToString()),
                            payroll_code = (dr["payroll_code"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            category_id = Convert.ToInt32(dr["category_id"].ToString()),
                            category_name = (dr["category_name"].ToString()),
                            branch_id = Convert.ToInt32(dr["branch_id"].ToString()),
                            branch = (dr["branch"].ToString()),
                            confidentiality_id = Convert.ToInt32(dr["confidentiality_id"].ToString()),
                            confidentiality = (dr["confidentiality"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            approved = Convert.ToBoolean(dr["approved"].ToString()),
                            status = (dr["status"].ToString()),


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


        public List<PayslipDetaiResponse> payslip_detail_temp_view(string series_code, string payroll_header_id, int employee_id, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayslipDetaiResponse> resp = new List<PayslipDetaiResponse>();
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
                oCmd.CommandText = "payslip_detail_temp_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayslipDetaiResponse()
                        {
                            payroll_header_id = Convert.ToInt32(dr["payroll_header_id"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            detail_group_id = Convert.ToInt32(dr["detail_group_id"].ToString()),
                            detail_type_id = Convert.ToInt32(dr["detail_type_id"].ToString()),
                            detail_id = Convert.ToInt32(dr["detail_id"].ToString()),
                            detail = (dr["detail"].ToString()),
                            total = Convert.ToDecimal(dr["total"].ToString()),
                            amount = Convert.ToDecimal(dr["amount"].ToString()),
                            taxable_id = Convert.ToBoolean(dr["taxable_id"].ToString()),
                            taxable = (dr["taxable"].ToString()),
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



        #endregion


        #region "Payslip"


        public List<PayrollGenerationResponse> payslip_view(string series_code, string payroll_header_id, int payroll_id, int payslip_id, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollGenerationResponse> resp = new List<PayrollGenerationResponse>();
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
                oCmd.CommandText = "payslip_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@payroll_id", payroll_id);
                oCmd.Parameters.AddWithValue("@payslip_id", payslip_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollGenerationResponse()
                        {
                            payslip_id = Convert.ToInt32(dr["payslip_id"].ToString()),
                            payroll_id = Convert.ToInt32(dr["payroll_id"].ToString()),
                            timekeeping_header_id = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            payroll_header_id = Convert.ToInt32(dr["payroll_header_id"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            last_name = (dr["last_name"].ToString()),
                            first_name = (dr["first_name"].ToString()),
                            file_status = (dr["file_status"].ToString()),
                            tax_status = (dr["tax_status"].ToString()),
                            daily_rate = Convert.ToDecimal(dr["daily_rate"].ToString()),
                            semi_monthly_rate = Convert.ToDecimal(dr["semi_monthly_rate"].ToString()),
                            monthly_rate = Convert.ToDecimal(dr["monthly_rate"].ToString()),
                            hourly_rate = Convert.ToDecimal(dr["hourly_rate"].ToString()),
                            payroll_code = (dr["payroll_code"].ToString()),
                            pay_date = (dr["pay_date"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            basic_salary = Convert.ToDecimal(dr["basic_salary"].ToString()),
                            misc_amount = Convert.ToDecimal(dr["misc_amount"].ToString()),
                            leave_amount = Convert.ToDecimal(dr["leave_amount"].ToString()),
                            overtime = Convert.ToDecimal(dr["overtime"].ToString()),
                            overtime_holiday = Convert.ToDecimal(dr["overtime_holiday"].ToString()),
                            other_tax_income = Convert.ToDecimal(dr["other_tax_income"].ToString()),
                            adjustments = Convert.ToDecimal(dr["adjustments"].ToString()),
                            gross_income = Convert.ToDecimal(dr["gross_income"].ToString()),
                            witholding_tax = Convert.ToDecimal(dr["witholding_tax"].ToString()),
                            net_salary_after_tax = Convert.ToDecimal(dr["net_salary_after_tax"].ToString()),
                            employee_sss = Convert.ToDecimal(dr["employee_sss"].ToString()),
                            employee_mcr = Convert.ToDecimal(dr["employee_mcr"].ToString()),
                            employee_pagibig = Convert.ToDecimal(dr["employee_pagibig"].ToString()),
                            other_ntax_income = Convert.ToDecimal(dr["other_ntax_income"].ToString()),
                            loan_payments = Convert.ToDecimal(dr["loan_payments"].ToString()),
                            deductions = Convert.ToDecimal(dr["deductions"].ToString()),
                            net_salary = Convert.ToDecimal(dr["net_salary"].ToString()),
                            employer_sss = Convert.ToDecimal(dr["employer_sss"].ToString()),
                            employer_mcr = Convert.ToDecimal(dr["employer_mcr"].ToString()),
                            employer_ec = Convert.ToDecimal(dr["employer_ec"].ToString()),
                            employer_pagibig = Convert.ToDecimal(dr["employer_pagibig"].ToString()),
                            payroll_cost = Convert.ToDecimal(dr["payroll_cost"].ToString()),
                            ytd_gross = Convert.ToDecimal(dr["ytd_gross"].ToString()),
                            ytd_witholding = Convert.ToDecimal(dr["ytd_witholding"].ToString()),
                            ytd_sss = Convert.ToDecimal(dr["ytd_sss"].ToString()),
                            ytd_mcr = Convert.ToDecimal(dr["ytd_mcr"].ToString()),
                            ytd_pagibig = Convert.ToDecimal(dr["ytd_pagibig"].ToString()),
                            ytd_13ntax = Convert.ToDecimal(dr["ytd_13ntax"].ToString()),
                            ytd_13tax = Convert.ToDecimal(dr["ytd_13tax"].ToString()),
                            payment_type = (dr["payment_type"].ToString()),
                            bank_account = (dr["bank_account"].ToString()),
                            bank_name = (dr["bank_name"].ToString()),
                            comment_field = (dr["comment_field"].ToString()),
                            error_field = (dr["error_field"].ToString()),
                            date_employed = (dr["date_employed"].ToString()),
                            date_terminated = (dr["date_terminated"].ToString()),
                            cost_center = (dr["cost_center"].ToString()),
                            Currency = (dr["Currency"].ToString()),
                            exchange_rate = Convert.ToDecimal(dr["exchange_rate"].ToString()),
                            payment_freq = (dr["payment_freq"].ToString()),
                            mtd_gross = Convert.ToDecimal(dr["mtd_gross"].ToString()),
                            mtd_basic = Convert.ToDecimal(dr["mtd_basic"].ToString()),
                            mtd_sss_employee = Convert.ToDecimal(dr["mtd_sss_employee"].ToString()),
                            mtd_mcr_employee = Convert.ToDecimal(dr["mtd_mcr_employee"].ToString()),
                            mtd_pagibig_employee = Convert.ToDecimal(dr["mtd_pagibig_employee"].ToString()),
                            mtd_sss_employer = Convert.ToDecimal(dr["mtd_sss_employer"].ToString()),
                            mtd_mcr_employer = Convert.ToDecimal(dr["mtd_mcr_employer"].ToString()),
                            mtd_ec_employer = Convert.ToDecimal(dr["mtd_ec_employer"].ToString()),
                            mtd_pagibig_employer = Convert.ToDecimal(dr["mtd_pagibig_employer"].ToString()),
                            mtd_wh_tax = Convert.ToDecimal(dr["mtd_wh_tax"].ToString()),
                            monthly_basic = Convert.ToDecimal(dr["monthly_basic"].ToString()),
                            monthly_allow = Convert.ToDecimal(dr["monthly_allow"].ToString()),
                            mtd_ntax = Convert.ToDecimal(dr["mtd_ntax"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            total_addition = Convert.ToDecimal(dr["total_addition"].ToString()),
                            total_deduction = Convert.ToDecimal(dr["total_deduction"].ToString()),
                            company_name = (dr["company_name"].ToString()),
                            pay_period = (dr["pay_period"].ToString()),
                            cutoff_date = (dr["cutoff_date"].ToString()),
                            department = (dr["department"].ToString()),
                            position = (dr["position"].ToString()),
                            payroll_type = (dr["payroll_type"].ToString()),
                            salary_rate = (dr["salary_rate"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            is_posted = Convert.ToBoolean(dr["is_posted"].ToString()),
                            is_checked = true,
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


        public List<PayslipDetaiResponse> payslip_detail_view(string series_code, string payroll_header_id, int payroll_id, int payslip_id, int employee_id, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayslipDetaiResponse> resp = new List<PayslipDetaiResponse>();
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
                oCmd.CommandText = "payslip_detail_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@payroll_id", payroll_id);
                oCmd.Parameters.AddWithValue("@payslip_id", payslip_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayslipDetaiResponse()
                        {
                            payroll_header_id = Convert.ToInt32(dr["payroll_header_id"].ToString()),
                            payslip_id = Convert.ToInt32(dr["payslip_id"].ToString()),
                            payroll_id = Convert.ToInt32(dr["payroll_id"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            detail_group_id = Convert.ToInt32(dr["detail_group_id"].ToString()),
                            detail_type_id = Convert.ToInt32(dr["detail_type_id"].ToString()),
                            detail_id = Convert.ToInt32(dr["detail_id"].ToString()),
                            detail = (dr["detail"].ToString()),
                            total = Convert.ToDecimal(dr["total"].ToString()),
                            amount = Convert.ToDecimal(dr["amount"].ToString()),
                            taxable_id = Convert.ToBoolean(dr["taxable_id"].ToString()),
                            taxable = (dr["taxable"].ToString()),
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

        #endregion



        #region "Posted Payslip"


        public int posted_payslip_in(PostedPayrollRequest[] model)
        {
            int resp = 0;

            var payroll_header_id = model[0].payroll_header_id == "0" ? "0" : Crypto.url_decrypt(model[0].payroll_header_id);
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
            try
            {
                foreach (var item in model)
                {
                    da.SelectCommand = oCmd;
                    oCmd.CommandText = "posted_payslip_in";
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();
                    oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                    oCmd.Parameters.AddWithValue("@payroll_id", item.payroll_id);
                    oCmd.Parameters.AddWithValue("@payslip_id", item.payslip_id);
                    oCmd.Parameters.AddWithValue("@employee_id", item.employee_id);
                    oCmd.Parameters.AddWithValue("@created_by", created_by);

                    SqlDataReader sdr = oCmd.ExecuteReader();
                    while (sdr.Read())
                    {

                        resp = Convert.ToInt32(sdr["payroll_header_id"].ToString());

                    }
                    sdr.Close();
                }

                oTrans.Commit();
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

        public List<PayrollGenerationResponse> posted_payslip_view(string series_code, string payroll_header_id, int posted_payslip_id, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollGenerationResponse> resp = new List<PayrollGenerationResponse>();
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
                oCmd.CommandText = "posted_payslip_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@posted_payslip_id", posted_payslip_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollGenerationResponse()
                        {

                            posted_payslip_id = Convert.ToInt32(dr["posted_payslip_id"].ToString()),
                            timekeeping_header_id = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            company_logo = (dr["company_logo"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            email_address = (dr["email_address"].ToString()),
                            last_name = (dr["last_name"].ToString()),
                            first_name = (dr["first_name"].ToString()),
                            file_status = (dr["file_status"].ToString()),
                            tax_status = (dr["tax_status"].ToString()),
                            daily_rate = Convert.ToDecimal(dr["daily_rate"].ToString()),
                            semi_monthly_rate = Convert.ToDecimal(dr["semi_monthly_rate"].ToString()),
                            monthly_rate = Convert.ToDecimal(dr["monthly_rate"].ToString()),
                            hourly_rate = Convert.ToDecimal(dr["hourly_rate"].ToString()),
                            late = Convert.ToDecimal(dr["late"].ToString()),
                            undertime = Convert.ToDecimal(dr["undertime"].ToString()),
                            absent = Convert.ToDecimal(dr["absent"].ToString()),
                            sss = Convert.ToDecimal(dr["sss"].ToString()),
                            philhealth = Convert.ToDecimal(dr["philhealth"].ToString()),
                            pagibig = Convert.ToDecimal(dr["pagibig"].ToString()),
                            payroll_code = (dr["payroll_code"].ToString()),
                            payroll_header_id = Convert.ToInt32(dr["payroll_header_id"].ToString()),
                            pay_date = (dr["pay_date"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            basic_salary = Convert.ToDecimal(dr["basic_salary"].ToString()),
                            misc_amount = Convert.ToDecimal(dr["misc_amount"].ToString()),
                            leave_amount = Convert.ToDecimal(dr["leave_amount"].ToString()),
                            overtime = Convert.ToDecimal(dr["overtime"].ToString()),
                            overtime_holiday = Convert.ToDecimal(dr["overtime_holiday"].ToString()),
                            other_tax_income = Convert.ToDecimal(dr["other_tax_income"].ToString()),
                            adjustments = Convert.ToDecimal(dr["adjustments"].ToString()),
                            gross_income = Convert.ToDecimal(dr["gross_income"].ToString()),
                            witholding_tax = Convert.ToDecimal(dr["witholding_tax"].ToString()),
                            net_salary_after_tax = Convert.ToDecimal(dr["net_salary_after_tax"].ToString()),
                            employee_sss = Convert.ToDecimal(dr["employee_sss"].ToString()),
                            employee_mcr = Convert.ToDecimal(dr["employee_mcr"].ToString()),
                            employee_pagibig = Convert.ToDecimal(dr["employee_pagibig"].ToString()),
                            other_ntax_income = Convert.ToDecimal(dr["other_ntax_income"].ToString()),
                            loan_payments = Convert.ToDecimal(dr["loan_payments"].ToString()),
                            deductions = Convert.ToDecimal(dr["deductions"].ToString()),
                            net_salary = Convert.ToDecimal(dr["net_salary"].ToString()),
                            employer_sss = Convert.ToDecimal(dr["employer_sss"].ToString()),
                            employer_mcr = Convert.ToDecimal(dr["employer_mcr"].ToString()),
                            employer_ec = Convert.ToDecimal(dr["employer_ec"].ToString()),
                            employer_pagibig = Convert.ToDecimal(dr["employer_pagibig"].ToString()),
                            payroll_cost = Convert.ToDecimal(dr["payroll_cost"].ToString()),
                            ytd_gross = Convert.ToDecimal(dr["ytd_gross"].ToString()),
                            ytd_witholding = Convert.ToDecimal(dr["ytd_witholding"].ToString()),
                            ytd_sss = Convert.ToDecimal(dr["ytd_sss"].ToString()),
                            ytd_mcr = Convert.ToDecimal(dr["ytd_mcr"].ToString()),
                            ytd_pagibig = Convert.ToDecimal(dr["ytd_pagibig"].ToString()),
                            ytd_13ntax = Convert.ToDecimal(dr["ytd_13ntax"].ToString()),
                            ytd_13tax = Convert.ToDecimal(dr["ytd_13tax"].ToString()),
                            payment_type = (dr["payment_type"].ToString()),
                            bank_account = (dr["bank_account"].ToString()),
                            bank_name = (dr["bank_name"].ToString()),
                            comment_field = (dr["comment_field"].ToString()),
                            error_field = (dr["error_field"].ToString()),
                            date_employed = (dr["date_employed"].ToString()),
                            date_terminated = (dr["date_terminated"].ToString()),
                            cost_center = (dr["cost_center"].ToString()),
                            Currency = (dr["Currency"].ToString()),
                            exchange_rate = Convert.ToDecimal(dr["exchange_rate"].ToString()),
                            payment_freq = (dr["payment_freq"].ToString()),
                            mtd_gross = Convert.ToDecimal(dr["mtd_gross"].ToString()),
                            mtd_basic = Convert.ToDecimal(dr["mtd_basic"].ToString()),
                            mtd_sss_employee = Convert.ToDecimal(dr["mtd_sss_employee"].ToString()),
                            mtd_mcr_employee = Convert.ToDecimal(dr["mtd_mcr_employee"].ToString()),
                            mtd_pagibig_employee = Convert.ToDecimal(dr["mtd_pagibig_employee"].ToString()),
                            mtd_sss_employer = Convert.ToDecimal(dr["mtd_sss_employer"].ToString()),
                            mtd_mcr_employer = Convert.ToDecimal(dr["mtd_mcr_employer"].ToString()),
                            mtd_ec_employer = Convert.ToDecimal(dr["mtd_ec_employer"].ToString()),
                            mtd_pagibig_employer = Convert.ToDecimal(dr["mtd_pagibig_employer"].ToString()),
                            mtd_wh_tax = Convert.ToDecimal(dr["mtd_wh_tax"].ToString()),
                            monthly_basic = Convert.ToDecimal(dr["monthly_basic"].ToString()),
                            monthly_allow = Convert.ToDecimal(dr["monthly_allow"].ToString()),
                            mtd_ntax = Convert.ToDecimal(dr["mtd_ntax"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            total_addition = Convert.ToDecimal(dr["total_addition"].ToString()),
                            total_deduction = Convert.ToDecimal(dr["total_deduction"].ToString()),
                            company_name = (dr["company_name"].ToString()),
                            pay_period = (dr["pay_period"].ToString()),
                            cutoff_date = (dr["cutoff_date"].ToString()),
                            department = (dr["department"].ToString()),
                            position = (dr["position"].ToString()),
                            payroll_type = (dr["payroll_type"].ToString()),
                            salary_rate = (dr["salary_rate"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            regnd_ms = Convert.ToDecimal(dr["regnd_ms"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created = (dr["date_created"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            company_address = (dr["company_address"].ToString()),
                            company_address2 = (dr["company_address2"].ToString()),
                            tin_no = (dr["tin_no"].ToString()),
                            sss_no = (dr["sss_no"].ToString()),
                            philhealth_no = (dr["philhealth_no"].ToString()),
                            pagibig_no = (dr["pagibig_no"].ToString()),
                            total_deduction_attendance = Convert.ToDecimal(dr["total_deduction_attendance"].ToString()),
                            taxable_allowance = Convert.ToDecimal(dr["taxable_allowance"].ToString()),
                            non_taxable_allowance = Convert.ToDecimal(dr["non_taxable_allowance"].ToString()),
                            government_contribution = Convert.ToDecimal(dr["government_contribution"].ToString()),
                            ytd_basic = Convert.ToDecimal(dr["ytd_basic"].ToString()),
                            ytd_allow_t = Convert.ToDecimal(dr["ytd_allow_t"].ToString()),
                            ytd_allow_nt = Convert.ToDecimal(dr["ytd_allow_nt"].ToString()),
                            ytd_ded_att = Convert.ToDecimal(dr["ytd_ded_att"].ToString()),
                            ytd_ded_gov = Convert.ToDecimal(dr["ytd_ded_gov"].ToString()),
                            ytd_ded_tax = Convert.ToDecimal(dr["ytd_ded_tax"].ToString()),
                            ytd_ded_other = Convert.ToDecimal(dr["ytd_ded_other"].ToString()),
                            ytd_net = Convert.ToDecimal(dr["ytd_net"].ToString()),
                            other_deduction = Convert.ToDecimal(dr["other_deduction"].ToString()),

                            is_email = Convert.ToBoolean(dr["is_email"].ToString()),


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


        public List<PayrollGenerationResponse> posted_payslip_employee_view(string series_code, string employee_id, string created_by)
        {

            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollGenerationResponse> resp = new List<PayrollGenerationResponse>();
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
                oCmd.CommandText = "posted_payslip_employee_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollGenerationResponse()
                        {

                            posted_payslip_id = Convert.ToInt32(dr["posted_payslip_id"].ToString()),
                            payroll_code = (dr["payroll_code"].ToString()),
                            posted_payslip_code = (dr["posted_payslip_code"].ToString()),
                            company_logo = (dr["company_logo"].ToString()),
                            timekeeping_header_id = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            last_name = (dr["last_name"].ToString()),
                            first_name = (dr["first_name"].ToString()),
                            file_status = (dr["file_status"].ToString()),
                            tax_status = (dr["tax_status"].ToString()),
                            daily_rate = Convert.ToDecimal(dr["daily_rate"].ToString()),
                            semi_monthly_rate = Convert.ToDecimal(dr["semi_monthly_rate"].ToString()),
                            monthly_rate = Convert.ToDecimal(dr["monthly_rate"].ToString()),
                            hourly_rate = Convert.ToDecimal(dr["hourly_rate"].ToString()),
                            payroll_header_id = Convert.ToInt32(dr["payroll_header_id"].ToString()),
                            pay_date = (dr["pay_date"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            basic_salary = Convert.ToDecimal(dr["basic_salary"].ToString()),
                            misc_amount = Convert.ToDecimal(dr["misc_amount"].ToString()),
                            leave_amount = Convert.ToDecimal(dr["leave_amount"].ToString()),
                            overtime = Convert.ToDecimal(dr["overtime"].ToString()),
                            overtime_holiday = Convert.ToDecimal(dr["overtime_holiday"].ToString()),
                            other_tax_income = Convert.ToDecimal(dr["other_tax_income"].ToString()),
                            adjustments = Convert.ToDecimal(dr["adjustments"].ToString()),
                            gross_income = Convert.ToDecimal(dr["gross_income"].ToString()),
                            witholding_tax = Convert.ToDecimal(dr["witholding_tax"].ToString()),
                            net_salary_after_tax = Convert.ToDecimal(dr["net_salary_after_tax"].ToString()),
                            employee_sss = Convert.ToDecimal(dr["employee_sss"].ToString()),
                            employee_mcr = Convert.ToDecimal(dr["employee_mcr"].ToString()),
                            employee_pagibig = Convert.ToDecimal(dr["employee_pagibig"].ToString()),
                            other_ntax_income = Convert.ToDecimal(dr["other_ntax_income"].ToString()),
                            loan_payments = Convert.ToDecimal(dr["loan_payments"].ToString()),
                            deductions = Convert.ToDecimal(dr["deductions"].ToString()),
                            net_salary = Convert.ToDecimal(dr["net_salary"].ToString()),
                            employer_sss = Convert.ToDecimal(dr["employer_sss"].ToString()),
                            employer_mcr = Convert.ToDecimal(dr["employer_mcr"].ToString()),
                            employer_ec = Convert.ToDecimal(dr["employer_ec"].ToString()),
                            employer_pagibig = Convert.ToDecimal(dr["employer_pagibig"].ToString()),
                            payroll_cost = Convert.ToDecimal(dr["payroll_cost"].ToString()),
                            ytd_gross = Convert.ToDecimal(dr["ytd_gross"].ToString()),
                            ytd_witholding = Convert.ToDecimal(dr["ytd_witholding"].ToString()),
                            ytd_sss = Convert.ToDecimal(dr["ytd_sss"].ToString()),
                            ytd_mcr = Convert.ToDecimal(dr["ytd_mcr"].ToString()),
                            ytd_pagibig = Convert.ToDecimal(dr["ytd_pagibig"].ToString()),
                            ytd_13ntax = Convert.ToDecimal(dr["ytd_13ntax"].ToString()),
                            ytd_13tax = Convert.ToDecimal(dr["ytd_13tax"].ToString()),
                            payment_type = (dr["payment_type"].ToString()),
                            bank_account = (dr["bank_account"].ToString()),
                            bank_name = (dr["bank_name"].ToString()),
                            comment_field = (dr["comment_field"].ToString()),
                            error_field = (dr["error_field"].ToString()),
                            date_employed = (dr["date_employed"].ToString()),
                            date_terminated = (dr["date_terminated"].ToString()),
                            cost_center = (dr["cost_center"].ToString()),
                            Currency = (dr["Currency"].ToString()),
                            exchange_rate = Convert.ToDecimal(dr["exchange_rate"].ToString()),
                            payment_freq = (dr["payment_freq"].ToString()),
                            mtd_gross = Convert.ToDecimal(dr["mtd_gross"].ToString()),
                            mtd_basic = Convert.ToDecimal(dr["mtd_basic"].ToString()),
                            mtd_sss_employee = Convert.ToDecimal(dr["mtd_sss_employee"].ToString()),
                            mtd_mcr_employee = Convert.ToDecimal(dr["mtd_mcr_employee"].ToString()),
                            mtd_pagibig_employee = Convert.ToDecimal(dr["mtd_pagibig_employee"].ToString()),
                            mtd_sss_employer = Convert.ToDecimal(dr["mtd_sss_employer"].ToString()),
                            mtd_mcr_employer = Convert.ToDecimal(dr["mtd_mcr_employer"].ToString()),
                            mtd_ec_employer = Convert.ToDecimal(dr["mtd_ec_employer"].ToString()),
                            mtd_pagibig_employer = Convert.ToDecimal(dr["mtd_pagibig_employer"].ToString()),
                            mtd_wh_tax = Convert.ToDecimal(dr["mtd_wh_tax"].ToString()),
                            monthly_basic = Convert.ToDecimal(dr["monthly_basic"].ToString()),
                            monthly_allow = Convert.ToDecimal(dr["monthly_allow"].ToString()),
                            mtd_ntax = Convert.ToDecimal(dr["mtd_ntax"].ToString()),
                            date_from = (dr["date_from"].ToString()),
                            date_to = (dr["date_to"].ToString()),
                            total_addition = Convert.ToDecimal(dr["total_addition"].ToString()),
                            total_deduction = Convert.ToDecimal(dr["total_deduction"].ToString()),
                            company_name = (dr["company_name"].ToString()),
                            pay_period = (dr["pay_period"].ToString()),
                            cutoff_date = (dr["cutoff_date"].ToString()),
                            department = (dr["department"].ToString()),
                            position = (dr["position"].ToString()),
                            payroll_type = (dr["payroll_type"].ToString()),
                            salary_rate = (dr["salary_rate"].ToString()),
                            display_name = (dr["display_name"].ToString()),
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


        public List<PayslipDetaiResponse> posted_payslip_detail_view(string series_code, string payroll_header_id, int posted_payslip_id, int employee_id, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayslipDetaiResponse> resp = new List<PayslipDetaiResponse>();
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
                oCmd.CommandText = "posted_payslip_detail_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@posted_payslip_id", posted_payslip_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayslipDetaiResponse()
                        {
                            payroll_header_id = Convert.ToInt32(dr["payroll_header_id"].ToString()),
                            posted_payslip_id = Convert.ToInt32(dr["posted_payslip_id"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            detail_group_id = Convert.ToInt32(dr["detail_group_id"].ToString()),
                            detail_type_id = Convert.ToInt32(dr["detail_type_id"].ToString()),
                            detail_id = Convert.ToInt32(dr["detail_id"].ToString()),
                            detail = (dr["detail"].ToString()),
                            total = Convert.ToDecimal(dr["total"].ToString()),
                            amount = Convert.ToDecimal(dr["amount"].ToString()),
                            taxable_id = Convert.ToBoolean(dr["taxable_id"].ToString()),
                            taxable = (dr["taxable"].ToString()),
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


        public List<PayslipDetaiResponse> posted_payslip_detail_employee_view(string series_code, string payroll_header_id, int posted_payslip_id, int employee_id, string created_by)
        {

            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayslipDetaiResponse> resp = new List<PayslipDetaiResponse>();
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
                oCmd.CommandText = "posted_payslip_detail_employee_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@posted_payslip_id", posted_payslip_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayslipDetaiResponse()
                        {
                            payroll_header_id = Convert.ToInt32(dr["payroll_header_id"].ToString()),
                            posted_payslip_id = Convert.ToInt32(dr["posted_payslip_id"].ToString()),
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            detail_group_id = Convert.ToInt32(dr["detail_group_id"].ToString()),
                            detail_type_id = Convert.ToInt32(dr["detail_type_id"].ToString()),
                            detail_id = Convert.ToInt32(dr["detail_id"].ToString()),
                            detail = (dr["detail"].ToString()),
                            total = Convert.ToDecimal(dr["total"].ToString()),
                            amount = Convert.ToDecimal(dr["amount"].ToString()),
                            taxable_id = Convert.ToBoolean(dr["taxable_id"].ToString()),
                            taxable = (dr["taxable"].ToString()),
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

        #endregion

        #region"Timekeeping"

        public List<TimekeepingGenerationResponse> timekeeping_final_temp_view_sel(string series_code, string payroll_header_id, int employee_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            //employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
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
                oCmd.CommandText = "timekeeping_final_temp_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
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

        public List<TimekeepingGenerationResponse> timekeeping_final_view_sel(string series_code, string payroll_header_id, int payroll_id, int payslip_id, int employee_id, int timekeeping_header_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            //employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
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
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@payroll_id", payroll_id);
                oCmd.Parameters.AddWithValue("@payslip_id", payslip_id);
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
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

        public List<TimekeepingGenerationResponse> posted_timekeeping_final_view_sel(string series_code, string payroll_header_id, int posted_payslip_id, int employee_id, int timekeeping_header_id, string created_by)
        {

            created_by = Crypto.url_decrypt(created_by);
            payroll_header_id = payroll_header_id == "0" ? "0" : Crypto.url_decrypt(payroll_header_id);
            //employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
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
                oCmd.CommandText = "posted_timekeeping_final_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@payslip_id", posted_payslip_id);
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
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



        public int timekeeping_upload_in(UploadInRequest model)
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
                oCmd.CommandText = "timekeeping_upload_in";
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



        #region "Uploader"

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

        public int timekeeping_upload(List<TimekeepingUpload> model, string series_code,  string created_by)
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
        #endregion

        #region "Reports"

        public List<rpt1601cResponse> report_1601c(string series_code, string employee_id, string date_from, string date_to)
        {
            int int_emp = 0;
            try
            {
                int_emp = Convert.ToInt32(employee_id);
            }
            catch(Exception e)
            {

                employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            }
            //
            series_code = Crypto.url_decrypt(series_code);
            //created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<rpt1601cResponse> resp = new List<rpt1601cResponse>();
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
                oCmd.CommandText = "report_1601c";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new rpt1601cResponse()
                        {
                            _1  = (dr["_1"].ToString()),
                            _2  = (dr["_2"].ToString()),
                            _3  = (dr["_3"].ToString()),
                            _4  = (dr["_4"].ToString()),
                            _5  = (dr["_5"].ToString()),
                            _6  = (dr["_6"].ToString()),
                            _7  = (dr["_7"].ToString()),
                            _8  = (dr["_8"].ToString()),
                            _9  = (dr["_9"].ToString()),
                            _10 = (dr["_10"].ToString()),
                            _11 = (dr["_11"].ToString()),
                            _12 = (dr["_12"].ToString()),
                            _13 = (dr["_13"].ToString()),
                            _14 = (dr["_14"].ToString()),
                            _15 = (dr["_15"].ToString()),
                            _16 = (dr["_16"].ToString()),
                            _17 = (dr["_17"].ToString()),
                            _18 = (dr["_18"].ToString()),
                            _19 = (dr["_19"].ToString()),
                            _20 = (dr["_20"].ToString()),
                            _21 = (dr["_21"].ToString()),
                            _22 = (dr["_22"].ToString()),
                            _23 = (dr["_23"].ToString()),
                            _24 = (dr["_24"].ToString()),
                            _25 = (dr["_25"].ToString()),
                            _26 = (dr["_26"].ToString()),
                            _27 = (dr["_27"].ToString()),
                            _28 = (dr["_28"].ToString()),
                            _29 = (dr["_29"].ToString()),
                            _30 = (dr["_30"].ToString()),
                            _31 = (dr["_31"].ToString()),
                            _32 = (dr["_32"].ToString()),
                            _33 = (dr["_33"].ToString()),
                            _34 = (dr["_34"].ToString()),
                            _35 = (dr["_35"].ToString()),
                            _36 = (dr["_36"].ToString()),



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
