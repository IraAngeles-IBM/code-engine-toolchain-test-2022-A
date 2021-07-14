using EmployeeCategoryManagementService.Helper;
using EmployeeCategoryManagementService.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCategoryManagementService.Services
{

    public interface IEmployeeCategoryManagementServices
    {
        List<CategoryResponse> employee_category_view(string series_code, string category_id, string created_by); 
        List<CategoryResponse> employee_category_view_sel(string series_code, string category_id, string created_by);
        int employee_category_in_up(CategoryRequest model);
        InsertResponse employee_category_restriction(string series_code, string transaction_id, string module_id, string category_id, string date_from, string date_to, string created_by);
    }
    public class EmployeeCategoryManagementServices : IEmployeeCategoryManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;



        public EmployeeCategoryManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }



        public List<CategoryResponse> employee_category_view(string series_code, string category_id,string created_by)
        {
            category_id = category_id == "0" ? "0" : Crypto.url_decrypt(category_id);
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);
            List<CategoryResponse> resp = new List<CategoryResponse>();


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
                oCmd.CommandText = "employee_category_view";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@category_id", category_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new CategoryResponse()
                        {
                            category_id                     = Convert.ToInt32(sdr["category_id"].ToString()),
                            category_name                   = (sdr["category_name"].ToString()),
                            encrypt_category_id             = (sdr["category_id"].ToString()), 
                            category_description            = (sdr["category_description"].ToString()),
                            access_level_id                 = Convert.ToInt32(sdr["access_level_id"].ToString()),
                            approval_level_id               = Convert.ToInt32(sdr["approval_level_id"].ToString()),
                            change_schedule_before          = Convert.ToInt32(sdr["change_schedule_before"].ToString()),
                            change_schedule_after           = Convert.ToInt32(sdr["change_schedule_after"].ToString()),
                            change_log_before               = Convert.ToInt32(sdr["change_log_before"].ToString()),
                            change_log_after                = Convert.ToInt32(sdr["change_log_after"].ToString()),
                            
                            official_business_before        = Convert.ToInt32(sdr["official_business_before"].ToString()),
                            official_business_after         = Convert.ToInt32(sdr["official_business_after"].ToString()),
                            overtime_before                 = Convert.ToInt32(sdr["overtime_before"].ToString()),
                            overtime_after                  = Convert.ToInt32(sdr["overtime_after"].ToString()),
                            offset_before                   = Convert.ToInt32(sdr["offset_before"].ToString()),
                            offset_after                    = Convert.ToInt32(sdr["offset_after"].ToString()),
                            allow_overtime                  = Convert.ToBoolean(sdr["allow_overtime"].ToString()),
                            holiday_based_id                = Convert.ToInt32(sdr["holiday_based_id"].ToString()),
                            enable_tardiness                = Convert.ToBoolean(sdr["enable_tardiness"].ToString()),
                            fixed_salary                    = Convert.ToBoolean(sdr["fixed_salary"].ToString()),
                            basis_sss_deduction_id          = Convert.ToInt32(sdr["basis_sss_deduction_id"].ToString()),
                            basis_philhealth_deduction_id   = Convert.ToInt32(sdr["basis_philhealth_deduction_id"].ToString()),
                            basis_pagibig_deduction_id      = Convert.ToInt32(sdr["basis_pagibig_deduction_id"].ToString()),
                            created_by                      = Convert.ToInt32(sdr["created_by"].ToString()),
                            date_created                    = (sdr["date_created"].ToString()),
                            active                          = Convert.ToBoolean(sdr["active"].ToString()),

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

        public List<CategoryResponse> employee_category_view_sel(string series_code, string category_id, string created_by)
        {
            category_id = category_id == "0" ? "0" : Crypto.url_decrypt(category_id);
            created_by = Crypto.url_decrypt(created_by);
            series_code = Crypto.url_decrypt(series_code);
            List<CategoryResponse> resp = new List<CategoryResponse>();


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
                oCmd.CommandText = "employee_category_view_sel";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@category_id", category_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new CategoryResponse()
                        {
                            category_id = Convert.ToInt32(sdr["category_id"].ToString()),
                            category_name = (sdr["category_name"].ToString()),
                            category_code = (sdr["category_code"].ToString()),
                            encrypt_category_id = Crypto.url_encrypt(sdr["category_id"].ToString()),
                            category_description = (sdr["category_description"].ToString()),
                            access_level_id = Convert.ToInt32(sdr["access_level_id"].ToString()),
                            approval_level_id = Convert.ToInt32(sdr["approval_level_id"].ToString()),
                            change_schedule_before = Convert.ToInt32(sdr["change_schedule_before"].ToString()),
                            change_schedule_after = Convert.ToInt32(sdr["change_schedule_after"].ToString()),
                            change_log_before = Convert.ToInt32(sdr["change_log_before"].ToString()),
                            change_log_after = Convert.ToInt32(sdr["change_log_after"].ToString()),
                           
                            official_business_before = Convert.ToInt32(sdr["official_business_before"].ToString()),
                            official_business_after = Convert.ToInt32(sdr["official_business_after"].ToString()),
                            overtime_before = Convert.ToInt32(sdr["overtime_before"].ToString()),
                            overtime_after = Convert.ToInt32(sdr["overtime_after"].ToString()),
                            offset_before = Convert.ToInt32(sdr["offset_before"].ToString()),
                            offset_after = Convert.ToInt32(sdr["offset_after"].ToString()),
                            allow_overtime = Convert.ToBoolean(sdr["allow_overtime"].ToString()),
                            holiday_based_id = Convert.ToInt32(sdr["holiday_based_id"].ToString()),
                            enable_tardiness = Convert.ToBoolean(sdr["enable_tardiness"].ToString()),
                            fixed_salary = Convert.ToBoolean(sdr["fixed_salary"].ToString()),
                            basis_sss_deduction_id = Convert.ToInt32(sdr["basis_sss_deduction_id"].ToString()),
                            basis_philhealth_deduction_id = Convert.ToInt32(sdr["basis_philhealth_deduction_id"].ToString()),
                            basis_pagibig_deduction_id = Convert.ToInt32(sdr["basis_pagibig_deduction_id"].ToString()),
                            created_by = Convert.ToInt32(sdr["created_by"].ToString()),
                            date_created = (sdr["date_created"].ToString()),
                            active = Convert.ToBoolean(sdr["active"].ToString()),
                            
                            rate_group_id                  = Convert.ToInt32(sdr["rate_group_id"].ToString()),
                            contribution_group_id                  = Convert.ToInt32(sdr["contribution_group_id"].ToString()),
                            approval_level                  = (sdr["approval_level"].ToString()),
                            access_level                  = (sdr["access_level"].ToString()),
                            holiday_based                   = (sdr["holiday_based"].ToString()),
                            basis_sss_deduction             = (sdr["basis_sss_deduction"].ToString()),
                            basis_philhealth_deduction      = (sdr["basis_philhealth_deduction"].ToString()),
                            basis_pagibig_deduction         = (sdr["basis_pagibig_deduction"].ToString()),
                            rate_group                      = (sdr["rate_group"].ToString()),
                            contribution_group              = (sdr["contribution_group"].ToString()),
                            created_by_name                 = (sdr["created_by_name"].ToString()),
                            status                          = (sdr["status"].ToString()),

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

        public int employee_category_in_up(CategoryRequest model)
        {
            int resp = 0;
            model.category_id = model.category_id == "0" ? "0" : Crypto.url_decrypt(model.category_id);
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
                oCmd.CommandText = "employee_category_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();

                oCmd.Parameters.AddWithValue("@active", model.active);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@category_id", model.category_id);
                oCmd.Parameters.AddWithValue("@category_code", model.category_code);
                oCmd.Parameters.AddWithValue("@category_name", model.category_name);
                oCmd.Parameters.AddWithValue("@category_description", model.category_description);
                oCmd.Parameters.AddWithValue("@access_level_id", model.access_level_id);
                oCmd.Parameters.AddWithValue("@approval_level_id", model.approval_level_id);
                oCmd.Parameters.AddWithValue("@change_schedule_before", model.change_schedule_before);
                oCmd.Parameters.AddWithValue("@change_schedule_after", model.change_schedule_after);
                oCmd.Parameters.AddWithValue("@change_log_before", model.change_log_before);
                oCmd.Parameters.AddWithValue("@change_log_after", model.change_log_after);
                oCmd.Parameters.AddWithValue("@official_business_before", model.official_business_before);
                oCmd.Parameters.AddWithValue("@official_business_after", model.official_business_after);
                oCmd.Parameters.AddWithValue("@overtime_before", model.overtime_before);
                oCmd.Parameters.AddWithValue("@overtime_after", model.overtime_after);
                oCmd.Parameters.AddWithValue("@offset_before", model.offset_before);
                oCmd.Parameters.AddWithValue("@offset_after", model.offset_after);
                oCmd.Parameters.AddWithValue("@allow_overtime", model.allow_overtime);
                oCmd.Parameters.AddWithValue("@holiday_based_id", model.holiday_based_id);
                oCmd.Parameters.AddWithValue("@enable_tardiness", model.enable_tardiness);
                oCmd.Parameters.AddWithValue("@fixed_salary", model.fixed_salary);
                oCmd.Parameters.AddWithValue("@basis_sss_deduction_id", model.basis_sss_deduction_id);
                oCmd.Parameters.AddWithValue("@basis_philhealth_deduction_id",model.basis_philhealth_deduction_id);
                oCmd.Parameters.AddWithValue("@basis_pagibig_deduction_id", model.basis_pagibig_deduction_id);
                oCmd.Parameters.AddWithValue("@rate_group_id", model.rate_group_id);
                oCmd.Parameters.AddWithValue("@contribution_group_id", model.contribution_group_id);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                    resp = Convert.ToInt32(sdr["category_id"].ToString());

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



        public InsertResponse employee_category_restriction(string series_code, string transaction_id, string module_id, string category_id, string date_from, string date_to, string created_by)
        {
            category_id = category_id == "0" ? "0" : Crypto.url_decrypt(category_id);
            transaction_id = transaction_id == "0" ? "0" : Crypto.url_decrypt(transaction_id);
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
                oCmd.CommandText = "employee_category_restriction";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_id", module_id);
                oCmd.Parameters.AddWithValue("@transaction_id", transaction_id);
                oCmd.Parameters.AddWithValue("@category_id", category_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                    resp.id = 0;
                    resp.is_restricted = Convert.ToBoolean(sdr["is_restricted"].ToString());
                    resp.late_filing = Convert.ToBoolean(sdr["late_filing"].ToString());
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
