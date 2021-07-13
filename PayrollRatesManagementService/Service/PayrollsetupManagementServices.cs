
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using PayrollSetupManagementService.Model;
using PayrollSetupManagementService.Helper;
using PayrollSetupManagementService;

namespace PayrollSetupManagementService.Service
{

    public interface IPayrollSetupManagementServices
    {
        int payroll_rates_in(PayrollRatesDetail[] model);
        List<PayrollRatesDetailView> payroll_rates_detail_view_sel(string series_code, int rate_group_id);


        int pagibig_table_in(PagibigRequest[] model);
        int philhealth_table_in(philhealthRequest[] model);
        int sss_table_in(sssRequest[] model);
        int tax_table_in(taxRequest[] model);
        int recurring_in_up(RecurringRequest model);

        List<sssResponse> sss_table_view_sel(string series_code, int contribution_group_id); 
        List<taxResponse> tax_table_view_sel(string series_code, int contribution_group_id, int payroll_type_id);
        List<philhealthResponse> philhealth_table_view_sel(string series_code, int contribution_group_id);
        List<PagibigResponse> pagibig_table_view_sel(string series_code, int contribution_group_id);
        List<RecurringResponse> recurring_view_sel(string series_code, string recurring_id, string created_by);
        List<RecurringResponse> recurring_view(string series_code, string recurring_type, string created_by);
        List<EmployeeMovementRequest> payroll_recurring_in(PayrollRecurringRequest[] model);
        List<PayrollRecurringResponse> payroll_recurring_view(string series_code, string employee_id, string created_by);

        #region "Loan"
        List<EmployeeMovementRequest> loan_in_up(LoanHeaderRequest model);
        List<LoanResponse> loan_view_sel(string series_code, string loan_id, string created_by);
        List<LoanLoadResponse> loan_load(string series_code, decimal total_amount, string loan_start, int terms, int timing_id, string created_by);
        List<LoanDetailResponse> loan_detail_view(string series_code, string loan_id, string created_by);
        #endregion

        #region "Upload Saving"
        int payroll_recurring_upload_in(UploadInRequest model);
        int loan_in(UploadInRequest model);
        #endregion

        #region "Payroll Contribution "
        List<EmployeeMovementRequest> payroll_contribution_in(PayrollContributionRequest[] model);
        List<PayrollContributionResponse> payroll_contribution_view(string series_code, string employee_id, string created_by);
        #endregion

        #region "Payroll"
        int payroll_header_in(PayrollHeaderRequest model);
        List<PayrollHeaderResponse> payroll_header_view(string series_code, string payroll_header_id, string created_by);
        #endregion
    }

    public class PayrollSetupManagementServices : IPayrollSetupManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public PayrollSetupManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }


        public int payroll_rates_in(PayrollRatesDetail[] model)
        {
            int resp = model[0].rate_group_id;
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

                        da.SelectCommand = oCmd;
                        oCmd.CommandText = "payroll_rates_detail_del";
                        
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@rate_group_id", resp);
                        oCmd.ExecuteNonQuery();

                if (model != null)
                    {
                        foreach (var item in model)
                        {
                            oCmd.CommandText = "payroll_rates_detail_in";
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            oCmd.Parameters.Clear();
                            oCmd.Parameters.AddWithValue("@rate_group_id", resp);
                            oCmd.Parameters.AddWithValue("@payroll_rate_id", item.payroll_rate_id);
                            oCmd.Parameters.AddWithValue("@rates", item.rates);
                            oCmd.Parameters.AddWithValue("@created_by", created_by);
                        oCmd.ExecuteNonQuery();
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

        public List<PayrollRatesDetailView> payroll_rates_detail_view_sel(string series_code, int rate_group_id)
        {

            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollRatesDetailView> resp = new List<PayrollRatesDetailView>();
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
                oCmd.CommandText = "payroll_rates_detail_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@rate_group_id", rate_group_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollRatesDetailView()
                        {
                            rate_group_id       = Convert.ToInt32(dr["rate_group_id"].ToString()),
                            payroll_rate_id     = Convert.ToInt32(dr["payroll_rate_id"].ToString()),
                            rates               = Math.Round(Convert.ToDecimal(dr["rates"].ToString()),4),

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


        #region"Government COntribution"
        public int pagibig_table_in(PagibigRequest[] model)
        {
            int resp = model[0].contribution_group_id;
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

                da.SelectCommand = oCmd;
                oCmd.CommandText = "pagibig_table_del";

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@contribution_group_id", resp);
                oCmd.ExecuteNonQuery();

                if (model != null)
                {
                    foreach (var item in model)
                    {
                        oCmd.CommandText = "pagibig_table_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@contribution_group_id", resp);
                        oCmd.Parameters.AddWithValue("@range_from"      , item.range_from);
                        oCmd.Parameters.AddWithValue("@range_to"        , item.range_to);
                        oCmd.Parameters.AddWithValue("@employer_share"  , item.employer_share);
                        oCmd.Parameters.AddWithValue("@employee_share"  , item.employee_share);
                        oCmd.Parameters.AddWithValue("@share_type_id", item.share_type_id);
                        //oCmd.Parameters.AddWithValue("",)

                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        oCmd.ExecuteNonQuery();
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

        public int philhealth_table_in(philhealthRequest[] model)
        {
            int resp = model[0].contribution_group_id;
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

                da.SelectCommand = oCmd;
                oCmd.CommandText = "philhealth_table_del";

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@contribution_group_id", resp);
                oCmd.ExecuteNonQuery();

                if (model != null)
                {
                    foreach (var item in model)
                    {
                        oCmd.CommandText = "philhealth_table_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@contribution_group_id", resp);
                        oCmd.Parameters.AddWithValue("@premium_rate",item.premium_rate);
                        oCmd.Parameters.AddWithValue("@minimum",item.minimum);
                        oCmd.Parameters.AddWithValue("@maximum", item.maximum);
                        //oCmd.Parameters.AddWithValue("",)

                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        oCmd.ExecuteNonQuery();
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

        public int sss_table_in(sssRequest[] model)
        {
            int resp = model[0].contribution_group_id;
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

                da.SelectCommand = oCmd;
                oCmd.CommandText = "sss_table_del";

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@contribution_group_id", resp);
                oCmd.ExecuteNonQuery();

                if (model != null)
                {
                    foreach (var item in model)
                    {
                        oCmd.CommandText = "sss_table_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@contribution_group_id", resp);
                        oCmd.Parameters.AddWithValue("@range_from", item.range_from );
                        oCmd.Parameters.AddWithValue("@range_to", item.range_to);
                        oCmd.Parameters.AddWithValue("@salary_base", item.salary_base );
                        oCmd.Parameters.AddWithValue("@base_amount", item.base_amount);
                        oCmd.Parameters.AddWithValue("@employer_share", item.employer_share );
                        oCmd.Parameters.AddWithValue("@employee_share", item.employee_share );
                        oCmd.Parameters.AddWithValue("@employer_mpf", item.employer_mpf);
                        oCmd.Parameters.AddWithValue("@employee_mpf", item.employee_mpf);
                        oCmd.Parameters.AddWithValue("@employee_compensation", item.employee_compensation);
                        //oCmd.Parameters.AddWithValue("",)

                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        oCmd.ExecuteNonQuery();
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

        public int tax_table_in(taxRequest[] model)
        {
            int resp = model[0].contribution_group_id;
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

                da.SelectCommand = oCmd;
                oCmd.CommandText = "tax_table_del";

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@contribution_group_id", resp);
                oCmd.Parameters.AddWithValue("@payroll_type_id", model[0].payroll_type_id);
                oCmd.ExecuteNonQuery();

                if (model != null)
                {
                    foreach (var item in model)
                    {
                        oCmd.CommandText = "tax_table_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@contribution_group_id", resp);
                        oCmd.Parameters.AddWithValue("@range_from", item.range_from);
                        oCmd.Parameters.AddWithValue("@range_to", item.range_to);
                        oCmd.Parameters.AddWithValue("@salary_base", item.salary_base);
                        oCmd.Parameters.AddWithValue("@base_amount", item.base_amount);
                        oCmd.Parameters.AddWithValue("@tax_percentage", item.tax_percentage);
                        oCmd.Parameters.AddWithValue("@payroll_type_id", item.payroll_type_id);
                        //oCmd.Parameters.AddWithValue("",)

                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        oCmd.ExecuteNonQuery();
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

        public List<PagibigResponse> pagibig_table_view_sel(string series_code, int contribution_group_id)
        {

            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PagibigResponse> resp = new List<PagibigResponse>();
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
                oCmd.CommandText = "pagibig_table_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@contribution_group_id", contribution_group_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PagibigResponse()
                        {
                            contribution_group_id   = Convert.ToInt32(dr["contribution_group_id"].ToString()),
                            share_type_id           = Convert.ToInt32(dr["share_type_id"].ToString()),
                            range_from              = Math.Round(Convert.ToDecimal(dr["range_from"].ToString()), 4),
                            range_to                = Math.Round(Convert.ToDecimal(dr["range_to"].ToString()), 4),
                            employer_share          = Math.Round(Convert.ToDecimal(dr["employer_share"].ToString()), 4),
                            employee_share          = Math.Round(Convert.ToDecimal(dr["employee_share"].ToString()), 4),
                            created_by              = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created            = (dr["date_created"].ToString()),

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

        public List<philhealthResponse> philhealth_table_view_sel(string series_code, int contribution_group_id)
        {

            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<philhealthResponse> resp = new List<philhealthResponse>();
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
                oCmd.CommandText = "philhealth_table_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@contribution_group_id", contribution_group_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new philhealthResponse()
                        {
                            contribution_group_id = Convert.ToInt32(dr["contribution_group_id"].ToString()),
                            premium_rate  =  Math.Round(Convert.ToDecimal(dr["premium_rate"].ToString()),4),
                            minimum       =  Math.Round(Convert.ToDecimal(dr["minimum"].ToString()),4),
                            maximum       = Math.Round(Convert.ToDecimal(dr["maximum"].ToString()), 4),
                            created_by    =  Convert.ToInt32(dr["created_by"].ToString()),
                            date_created  = (dr["date_created"].ToString()) ,







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

        public List<sssResponse> sss_table_view_sel(string series_code, int contribution_group_id)
        {

            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<sssResponse> resp = new List<sssResponse>();
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
                oCmd.CommandText = "sss_table_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@contribution_group_id", contribution_group_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new sssResponse()
                        {


                            contribution_group_id   =   Convert.ToInt32(dr["contribution_group_id"].ToString()),
                            range_from              =   Math.Round(Convert.ToDecimal(dr["range_from"].ToString()),4),
                            range_to                =   Math.Round(Convert.ToDecimal(dr["range_to"].ToString()), 4),
                            salary_base             =   Math.Round(Convert.ToDecimal(dr["salary_base"].ToString()), 4),
                            base_amount             =   Math.Round(Convert.ToDecimal(dr["base_amount"].ToString()), 4),
                            employer_share          =   Math.Round(Convert.ToDecimal(dr["employer_share"].ToString()), 4),
                            employee_share          =   Math.Round(Convert.ToDecimal(dr["employee_share"].ToString()), 4),
                            employer_mpf            =   Math.Round(Convert.ToDecimal(dr["employer_mpf"].ToString()), 4),
                            employee_mpf            =   Math.Round(Convert.ToDecimal(dr["employee_mpf"].ToString()), 4),
                            employee_compensation   =   Math.Round(Convert.ToDecimal(dr["employee_compensation"].ToString()), 4),
                            created_by              =   Convert.ToInt32(dr["created_by"].ToString()),
                            date_created            =   (dr["date_created"].ToString()),


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

        public List<taxResponse> tax_table_view_sel(string series_code, int contribution_group_id, int payroll_type_id)
        {

            series_code = Crypto.url_decrypt(series_code);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<taxResponse> resp = new List<taxResponse>();
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
                oCmd.CommandText = "tax_table_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@contribution_group_id", contribution_group_id);
                oCmd.Parameters.AddWithValue("@payroll_type_id", payroll_type_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new taxResponse()
                        {


                            contribution_group_id = Convert.ToInt32(dr["contribution_group_id"].ToString()),
                            range_from = Math.Round(Convert.ToDecimal(dr["range_from"].ToString()),4),
                            range_to = Math.Round(Convert.ToDecimal(dr["range_to"].ToString()), 4),
                            salary_base = Math.Round(Convert.ToDecimal(dr["salary_base"].ToString()), 4),
                            base_amount = Math.Round(Convert.ToDecimal(dr["base_amount"].ToString()),4),
                            payroll_type_id = Convert.ToInt32(dr["payroll_type_id"].ToString()),
                            tax_percentage = Math.Round(Convert.ToDecimal(dr["tax_percentage"].ToString()), 4),
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


        #region"Recurring"
        public int recurring_in_up(RecurringRequest model)
        {
            int resp = 0;
            model.recurring_id = model.recurring_id == "0" ? "0" : Crypto.url_decrypt(model.recurring_id);
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
                oCmd.CommandText = "recurring_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@recurring_id", model.recurring_id);
                oCmd.Parameters.AddWithValue("@recurring_code", model.recurring_code);
                oCmd.Parameters.AddWithValue("@recurring_name", model.recurring_name);
                oCmd.Parameters.AddWithValue("@recurring_type", model.recurring_type);
                oCmd.Parameters.AddWithValue("@minimum_hour", model.minimum_hour);
                oCmd.Parameters.AddWithValue("@maximum_hour", model.maximum_hour);
                oCmd.Parameters.AddWithValue("@taxable", model.taxable);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.ExecuteNonQuery();
                resp = 1;
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
        public List<RecurringResponse> recurring_view_sel(string series_code, string recurring_id, string created_by)
        {

            recurring_id = recurring_id == "0" ? "0" : Crypto.url_decrypt(recurring_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<RecurringResponse> resp = new List<RecurringResponse>();
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
                oCmd.CommandText = "recurring_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@recurring_id", recurring_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new RecurringResponse()
                        {
                            recurring_id        = Crypto.url_encrypt(dr["recurring_id"].ToString()),
                            int_recurring_id    = Convert.ToInt32(dr["recurring_id"].ToString()),
                            recurring_code      = (dr["recurring_code"].ToString()),
                            recurring_name      = (dr["recurring_name"].ToString()),
                            recurring_type      = Convert.ToInt32(dr["recurring_type"].ToString()),
                            minimum_hour        = Convert.ToDecimal(dr["minimum_hour"].ToString()),
                            maximum_hour        = Convert.ToDecimal(dr["maximum_hour"].ToString()),
                            taxable             = Convert.ToBoolean(dr["taxable"].ToString()),
                            active              = Convert.ToBoolean(dr["active"].ToString()),
                            created_by_name      = (dr["created_by_name"].ToString()),
                            status      = (dr["status"].ToString()),



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
        public List<RecurringResponse> recurring_view(string series_code, string recurring_type, string created_by)
        {

            //recurring_type = recurring_type == "0" ? "0" : Crypto.url_decrypt(recurring_type);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<RecurringResponse> resp = new List<RecurringResponse>();
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
                oCmd.CommandText = "recurring_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@recurring_type", recurring_type);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new RecurringResponse()
                        {
                            recurring_id = Crypto.url_encrypt(dr["recurring_id"].ToString()),
                            int_recurring_id = Convert.ToInt32(dr["recurring_id"].ToString()),
                            recurring_code = (dr["recurring_code"].ToString()),
                            recurring_name = (dr["recurring_name"].ToString()),
                            recurring_type = Convert.ToInt32(dr["recurring_type"].ToString()),
                            minimum_hour = Convert.ToDecimal(dr["minimum_hour"].ToString()),
                            maximum_hour = Convert.ToDecimal(dr["maximum_hour"].ToString()),
                            taxable = Convert.ToBoolean(dr["taxable"].ToString()),
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
        public List<EmployeeMovementRequest> payroll_recurring_in(PayrollRecurringRequest[] model)
        {
            List<EmployeeMovementRequest> resp = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> loop_resp = new List<EmployeeMovementRequest>();
            //model.EIRequest.branch_id = Crypto.url_decrypt(model.EIRequest.branch_id);
            //model.employee_id = model.employee_id == "0" ? "0" : Crypto.url_decrypt(model.employee_id);

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
            da.SelectCommand = oCmd;
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        oCmd.CommandText = "payroll_recurring_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        oCmd.Parameters.AddWithValue("@payroll_recurring_id",item.payroll_recurring_id);
                        oCmd.Parameters.AddWithValue("@employee_id",item.employee_id);
                        oCmd.Parameters.AddWithValue("@amount",item.amount);
                        oCmd.Parameters.AddWithValue("@timing_id",item.timing_id);
                        oCmd.Parameters.AddWithValue("@adjustment_type_id",item.adjustment_type_id);
                        oCmd.Parameters.AddWithValue("@adjustment_id",item.adjustment_id);
                        oCmd.Parameters.AddWithValue("@taxable",item.taxable);
                        oCmd.Parameters.AddWithValue("@minimum_hour",item.minimum_hour);
                        oCmd.Parameters.AddWithValue("@maximum_hour",item.maximum_hour);
                        oCmd.Parameters.AddWithValue("@active", item.active);

                        da.Fill(dt);
                        //resp = (from DataRow sdr in dt.Rows
                        //        select new EmployeeMovementRequest()
                        //        {

                        //            employee_id = Convert.ToInt32(sdr["employee_id"].ToString()),
                        //            movement_type = Convert.ToInt32(sdr["movement_type"].ToString()),
                        //            is_dropdown = Convert.ToInt32(sdr["is_dropdown"].ToString()),
                        //            id = Convert.ToInt32(sdr["id"].ToString()),
                        //            description = (sdr["description"].ToString()),
                        //            series_code = model[0].series_code,
                        //            created_by = model[0].created_by,
                        //        }).ToList();


                        loop_resp = (from DataRow dr in dt.Rows
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


                        foreach (var loop in loop_resp)
                        {
                            resp.Add(loop);
                        }
                    }
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
        public List<PayrollRecurringResponse> payroll_recurring_view(string series_code, string employee_id, string created_by)
        {

            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollRecurringResponse> resp = new List<PayrollRecurringResponse>();
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
                oCmd.CommandText = "payroll_recurring_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollRecurringResponse()
                        {
                            payroll_recurring_id           = Convert.ToInt32(dr["payroll_recurring_id"].ToString()),
                            encrypted_payroll_recurring_id =  Crypto.url_encrypt(dr["payroll_recurring_id"].ToString()),
                            display_name                   =  (dr["display_name"].ToString()),
                            employee_code                  =  (dr["employee_code"].ToString()),
                            employee_id                    = Convert.ToInt32(dr["employee_id"].ToString()),
                            amount                         = Convert.ToDecimal(dr["amount"].ToString()),
                            timing_id                      = Convert.ToInt32(dr["timing_id"].ToString()),
                            timing                         =  (dr["timing"].ToString()),
                            adjustment_type_id             = Convert.ToInt32(dr["adjustment_type_id"].ToString()),
                            adjustment_type                =  (dr["adjustment_type"].ToString()),
                            adjustment_id                  = Convert.ToInt32(dr["adjustment_id"].ToString()),
                            recurring_name                 =  (dr["recurring_name"].ToString()),
                            taxable_id                     = Convert.ToBoolean(dr["taxable_id"].ToString()),
                            taxable                        = (dr["taxable"].ToString()),
                            minimum_hour                   = Convert.ToDecimal(dr["minimum_hour"].ToString()),
                            maximum_hour                   = Convert.ToDecimal(dr["maximum_hour"].ToString()),
                            created_by                     = Convert.ToInt32(dr["created_by"].ToString()),
                            active                         = Convert.ToBoolean(dr["active"].ToString()),
                            date_created                   =  (dr["date_created"].ToString()),
                            status                         =  (dr["status"].ToString()),


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

        #region "Loan"

        public List<EmployeeMovementRequest> loan_in_up(LoanHeaderRequest model)
        {
            var loan_id = model.loan_id == "0" ? "0" : Crypto.url_decrypt(model.loan_id);
            List<EmployeeMovementRequest> resp = new List<EmployeeMovementRequest>();
            List<LoanResponse> loan_res = new List<LoanResponse>();
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
                oCmd.CommandText = "loan_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@loan_id", loan_id );
                oCmd.Parameters.AddWithValue("@loan_code",model.loan_code);
                oCmd.Parameters.AddWithValue("@loan_type_id",model.loan_type_id);
                oCmd.Parameters.AddWithValue("@loan_name",model.loan_name);
                oCmd.Parameters.AddWithValue("@employee_id",model.employee_id);
                oCmd.Parameters.AddWithValue("@total_amount",model.total_amount);
                oCmd.Parameters.AddWithValue("@loan_date",model.loan_date);
                oCmd.Parameters.AddWithValue("@loan_start",model.loan_start);
                oCmd.Parameters.AddWithValue("@terms",model.terms);
                oCmd.Parameters.AddWithValue("@loan_timing_id",model.loan_timing_id);
                oCmd.Parameters.AddWithValue("@active",model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new EmployeeMovementRequest()
                        {
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            movement_type = Convert.ToInt32(dr["movement_type"].ToString()),
                            is_dropdown = Convert.ToInt32(dr["is_dropdown"].ToString()),
                            id = Convert.ToInt32(dr["id"].ToString()),
                            description = (dr["description"].ToString()),
                            movement_description = "",
                            created_by = (model.created_by),
                            series_code = (model.series_code),

                        }).ToList();
                loan_res = (from DataRow dr in dt.Rows
                            select new LoanResponse()
                            {
                                loan_id = Convert.ToInt32(dr["loan_id"].ToString()),
                               

                            }).ToList();
                var loan = loan_res[0].loan_id;

                oCmd.CommandText = "loan_detail_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@loan_id", loan);
                oCmd.ExecuteNonQuery();

                if (model.Detail != null)
                {
                    foreach (var item in model.Detail)
                    {
                        oCmd.CommandText = "loan_detail_in_up";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@loan_id", loan);
                        oCmd.Parameters.AddWithValue("@date", item.date);
                        oCmd.Parameters.AddWithValue("@amount", item.amount);
                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        oCmd.Parameters.AddWithValue("@active", item.active);
                        oCmd.ExecuteNonQuery();
                    }
                }

                oTrans.Commit();

                //if (model.loan_id != "0" )
                //{

                //    oCmd.CommandText = "loan_up";
                //    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                //    oCmd.Parameters.Clear();
                //    oCmd.Parameters.AddWithValue("@loan_id", resp);
                //    oCmd.Parameters.AddWithValue("@created_by", created_by);
                //    oCmd.ExecuteNonQuery();
                //}
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

        public List<LoanResponse> loan_view_sel(string series_code, string loan_id, string created_by)
        {

            loan_id = loan_id == "0" ? "0" : Crypto.url_decrypt(loan_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<LoanResponse> resp = new List<LoanResponse>();
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
                oCmd.CommandText = "loan_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@loan_id", loan_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new LoanResponse()
                        {
                            loan_id         = Convert.ToInt32(dr["loan_id"].ToString()),
                            encrypted_loan_id  = Crypto.url_encrypt(dr["loan_id"].ToString()),
                            loan_code       = (dr["loan_code"].ToString()),
                            loan_type_id    = Convert.ToInt32(dr["loan_type_id"].ToString()),
                            loan_type       = (dr["loan_type"].ToString()),
                            loan_name       = (dr["loan_name"].ToString()),
                            employee_id     = Convert.ToInt32(dr["employee_id"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            total_amount = Convert.ToDecimal(dr["total_amount"].ToString()),
                            loan_date = (dr["loan_date"].ToString()),
                            loan_start = (dr["loan_start"].ToString()),
                            terms = Convert.ToInt32(dr["terms"].ToString()),
                            loan_timing_id = Convert.ToInt32(dr["loan_timing_id"].ToString()),
                            loan_timing = (dr["loan_timing"].ToString()),
                            created_by = Convert.ToInt32(dr["created_by"].ToString()),
                            created_by_name = (dr["created_by_name"].ToString()),
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


        public List<LoanLoadResponse> loan_load(string series_code, decimal total_amount,string loan_start,int terms,int timing_id, string created_by)
        {

            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<LoanLoadResponse> resp = new List<LoanLoadResponse>();
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
                oCmd.CommandText = "loan_load";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@total_amount", total_amount);
                oCmd.Parameters.AddWithValue("@loan_start", loan_start);
                oCmd.Parameters.AddWithValue("@terms", terms);
                oCmd.Parameters.AddWithValue("@loan_timing_id", timing_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new LoanLoadResponse()
                        {
                            date     = (dr["date"].ToString()),
                            amount   = Convert.ToDecimal(dr["amount"].ToString()),
                            paid     = Convert.ToDecimal(dr["paid"].ToString()),
                            balance  = Convert.ToDecimal(dr["balance"].ToString()),
                            payslip  = (dr["payslip"].ToString()),


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


        public List<LoanDetailResponse> loan_detail_view(string series_code, string loan_id, string created_by)
        {

            loan_id = loan_id == "0" ? "0" : Crypto.url_decrypt(loan_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<LoanDetailResponse> resp = new List<LoanDetailResponse>();
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
                oCmd.CommandText = "loan_detail_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@loan_id", loan_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new LoanDetailResponse()
                        {

                            loan_id        = Convert.ToInt32(dr["loan_id"].ToString()),
                            date           = (dr["date"].ToString()),
                            amount         = Convert.ToDecimal(dr["amount"].ToString()),
                            paid           = Convert.ToDecimal(dr["paid"].ToString()),
                            balance        = Convert.ToDecimal(dr["balance"].ToString()),
                            payslip        = (dr["payslip"].ToString()),
                            created_by     = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created   = (dr["date_created"].ToString()),



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

        #region "Upload Saving"

        public int payroll_recurring_upload_in(UploadInRequest model)
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
                oCmd.CommandText = "payroll_recurring_upload_in";
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

        public int loan_in(UploadInRequest model)
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
                oCmd.CommandText = "loan_in";
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

        #region "Payroll Contribution"

        public List<EmployeeMovementRequest> payroll_contribution_in(PayrollContributionRequest[] model)
        {
            var payroll_contribution_id = model[0].payroll_contribution_id == "0" ? "0" : Crypto.url_decrypt(model[0].payroll_contribution_id);
            List<EmployeeMovementRequest> resp = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> loop_resp = new List<EmployeeMovementRequest>();
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
                    loop_resp = new List<EmployeeMovementRequest>();
                    dt.Clear();
                    da.SelectCommand = oCmd;
                    oCmd.CommandText = "payroll_contribution_in";
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();
                    oCmd.Parameters.AddWithValue("@payroll_contribution_id", item.payroll_contribution_id == "0" ? "0" : Crypto.url_decrypt(item.payroll_contribution_id));
                    oCmd.Parameters.AddWithValue("@employee_id", item.employee_id);
                    oCmd.Parameters.AddWithValue("@government_type_id", item.government_type_id);
                    oCmd.Parameters.AddWithValue("@timing_id", item.timing_id);
                    oCmd.Parameters.AddWithValue("@amount", item.amount);
                    oCmd.Parameters.AddWithValue("@taxable", item.taxable);
                    oCmd.Parameters.AddWithValue("@active", item.active);
                    oCmd.Parameters.AddWithValue("@created_by", created_by);
                    da.Fill(dt);

                    loop_resp = (from DataRow dr in dt.Rows
                            select new EmployeeMovementRequest()
                            {
                                employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                                movement_type = Convert.ToInt32(dr["movement_type"].ToString()),
                                is_dropdown = Convert.ToInt32(dr["is_dropdown"].ToString()),
                                id = Convert.ToInt32(dr["id"].ToString()),
                                description = (dr["description"].ToString()),
                                movement_description = "",
                                created_by = (item.created_by),
                                series_code = (item.series_code),

                            }).ToList();

                    foreach (var loop in loop_resp)
                    {
                        resp.Add(loop);
                    }
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

        public List<PayrollContributionResponse> payroll_contribution_view(string series_code, string employee_id, string created_by)
        {

            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);
            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<PayrollContributionResponse> resp = new List<PayrollContributionResponse>();
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
                oCmd.CommandText = "payroll_contribution_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollContributionResponse()
                        {

                            payroll_contribution_id  = Convert.ToInt32(dr["payroll_contribution_id"].ToString()),
                            encrypted_payroll_contribution_id  = (dr["payroll_contribution_id"].ToString()),
                            employee_id              = Convert.ToInt32(dr["employee_id"].ToString()),
                            display_name             = (dr["display_name"].ToString()),
                            employee_code            = (dr["employee_code"].ToString()),
                            government_type_id       = Convert.ToInt32(dr["government_type_id"].ToString()),
                            adjustment_type          = (dr["adjustment_type"].ToString()),
                            timing_id                = Convert.ToInt32(dr["timing_id"].ToString()),
                            timing                   = (dr["timing"].ToString()),
                            amount                   = Convert.ToDecimal(dr["amount"].ToString()),
                            taxable_id               = Convert.ToBoolean(dr["taxable_id"].ToString()),
                            taxable                  = (dr["taxable"].ToString()),
                            created_by               = Convert.ToInt32(dr["created_by"].ToString()),
                            date_created             = (dr["date_created"].ToString()),
                            status                   = (dr["status"].ToString()),

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

        #region "Payroll"

        public int payroll_header_in(PayrollHeaderRequest model)
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
                oCmd.CommandText = "payroll_header_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_code", model.payroll_header_code);
                oCmd.Parameters.AddWithValue("@timekeeping_header_id", model.timekeeping_header_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@active", model.active);

                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp = Convert.ToInt32(sdr["payroll_header_id"].ToString());

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

        public List<PayrollHeaderResponse> payroll_header_view(string series_code, string payroll_header_id, string created_by)
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
                oCmd.CommandText = "payroll_header_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_header_id", payroll_header_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollHeaderResponse()
                        {
                            payroll_header_id            = Convert.ToInt32(dr["payroll_header_id"].ToString()),
                            encrypted_payroll_header_id  = Crypto.url_encrypt(dr["payroll_header_id"].ToString()),
                            payroll_header_code          = (dr["payroll_header_code"].ToString()),
                            payroll_type_id              = Convert.ToInt32(dr["payroll_type_id"].ToString()),
                            payroll_type                 = (dr["payroll_type"].ToString()),
                            cutoff_id                    = Convert.ToInt32(dr["cutoff_id"].ToString()),
                            cutoff                       = (dr["cutoff"].ToString()),
                            month_id                     = Convert.ToInt32(dr["month_id"].ToString()),
                            month                        = (dr["month"].ToString()),
                            date_from                    = (dr["date_from"].ToString()),
                            date_to                      = (dr["date_to"].ToString()),
                            pay_date                     = (dr["pay_date"].ToString()),
                            category_id                  = Convert.ToInt32(dr["category_id"].ToString()),
                            category_name                = (dr["category_name"].ToString()),
                            branch_id                    = Convert.ToInt32(dr["branch_id"].ToString()),
                            branch                       = (dr["branch"].ToString()),
                            confidentiality_id           = Convert.ToInt32(dr["confidentiality_id"].ToString()),
                            confidentiality              = (dr["confidentiality"].ToString()),
                            timekeeping_header_id        = Convert.ToInt32(dr["timekeeping_header_id"].ToString()),
                            created_by                   = Convert.ToInt32(dr["created_by"].ToString()),
                            display_name                 = (dr["display_name"].ToString()),
                            active                       = Convert.ToBoolean(dr["active"].ToString()),
                            date_created                 = (dr["date_created"].ToString()),
                            approved                     = Convert.ToBoolean(dr["approved"].ToString()),
                            status                       = (dr["status"].ToString()),
                            tk_count                     = Convert.ToInt32(dr["tk_count"].ToString()),

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
