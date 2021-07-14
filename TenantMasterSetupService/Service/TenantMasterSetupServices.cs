using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenantMasterSetupService.Helper;
using TenantMasterSetupService.Model;

namespace TenantMasterSetupService.Service
{

    public interface ITenantMasterSetupServices
    {
        List<ModuleResponse> modules_view(string series_code, string created_by);
        List<ModuleResponse> modules_approval_view();
        List<DropdownEntitlementResponse> dropdown_view_entitlement(string dropdown_type_id);
        List<DropdownTypeResponse> dropdown_type_view();
        List<DropdownTypeResponse> dropdown_type_view_fix(string active);
        List<DropdownResponse> dropdown_fix_view(string dropdown_type_id);
        DropdownIUResponse dropdown_fix_in_up(DropdownIURequest model);
        List<ModuleResponse> module_type_view( string module_type);
        List<PayrollRates> payroll_rates_view();

        List<sssResponse> sss_table_view();
        List<philhealthResponse> philhealth_table_view();
        List<PagibigResponse> pagibig_table_view();
        List<taxResponse> tax_table_view(int payroll_type_id);
    }

    public class TenantMasterSetupServices: ITenantMasterSetupServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;



        public TenantMasterSetupServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }


        public List<ModuleResponse> modules_view(string series_code, string created_by)
        {
            List<ModuleResponse> resp = new List<ModuleResponse>();

            series_code = Crypto.url_decrypt(series_code);
            created_by = Crypto.url_decrypt(created_by);
            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
            DataTable dt = new DataTable();
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;

            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = oCmd;
            try
            {
                oCmd.CommandText = "modules_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@series_code", series_code);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new ModuleResponse()
                        {
                            module_id = Convert.ToInt32(sdr["module_id"].ToString()),
                            module_name = sdr["module_name"].ToString(),
                            module_type = sdr["module_type"].ToString(),
                            has_approval = Convert.ToBoolean(sdr["has_approval"].ToString()),
                            classes = (sdr["class"].ToString()),
                            link = sdr["link"].ToString(),
                            count = Convert.ToInt32(sdr["count"].ToString()),
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

        public List<ModuleResponse> modules_approval_view()
        {
            List<ModuleResponse> resp = new List<ModuleResponse>();


            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
            DataTable dt = new DataTable();
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;

            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = oCmd;
            try
            {
                oCmd.CommandText = "modules_approval_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new ModuleResponse()
                        {
                            module_id = Convert.ToInt32(sdr["module_id"].ToString()),
                            module_name = sdr["module_name"].ToString(),
                            module_type = sdr["module_type"].ToString(),
                            has_approval = Convert.ToBoolean(sdr["has_approval"].ToString()),
                            classes = (sdr["class"].ToString()),
                            link = sdr["link"].ToString(),
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


        public List<ModuleResponse> module_type_view(string module_type)
        {

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog="  + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ModuleResponse> resp = new List<ModuleResponse>();
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
                oCmd.CommandText = "modules_type_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@module_type", module_type);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ModuleResponse()
                        {
                            module_id = Convert.ToInt32(dr["module_id"].ToString()),
                            module_name = dr["module_name"].ToString(),
                            classes = dr["class"].ToString(),
                            module_type = dr["module_type"].ToString(),
                            link = dr["link"].ToString(),
                            has_approval = Convert.ToBoolean(dr["has_approval"].ToString()),

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


        public List<DropdownEntitlementResponse> dropdown_view_entitlement(string dropdown_type_id)
        {
            //DropdownResponse resp = new DropdownResponse();

            List<DropdownEntitlementResponse> resp = new List<DropdownEntitlementResponse>();
            string _con = connection._DB_Master;
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
                oCmd.CommandText = "dropdown_view_entitlement";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@dropdown_type_id", dropdown_type_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new DropdownEntitlementResponse()
                        {
                            id = Convert.ToInt32(dr["id"].ToString()),
                            description = dr["description"].ToString(),
                            type_id = Convert.ToInt32(dr["type_id"].ToString()),

                            to_id = Convert.ToInt32(dr["to_id"].ToString()),
                            to_description = dr["to_description"].ToString(),
                            to_type_id = Convert.ToInt32(dr["to_type_id"].ToString()),

                            id_to = Convert.ToInt32(dr["id_to"].ToString()),
                            description_to = dr["description_to"].ToString(),
                            type_id_to = Convert.ToInt32(dr["type_id_to"].ToString()),

                            to_id_to = Convert.ToInt32(dr["to_id_to"].ToString()),
                            to_description_to = dr["to_description_to"].ToString(),
                            to_type_id_to = Convert.ToInt32(dr["to_type_id_to"].ToString()),

                        }).ToList();
                //while (sdr.Read())
                //{
                //    resp.id = Convert.ToInt32(sdr["id"].ToString());
                //    resp.description = sdr["description"].ToString();

                //}
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

        public List<DropdownTypeResponse> dropdown_type_view()
        {
            //DropdownResponse resp = new DropdownResponse();

            List<DropdownTypeResponse> resp = new List<DropdownTypeResponse>();
            string _con = connection._DB_Master;
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
                oCmd.CommandText = "dropdown_type_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new DropdownTypeResponse()
                        {
                            id = Convert.ToInt32(dr["id"].ToString()),
                            description = dr["description"].ToString(),

                        }).ToList();
                //while (sdr.Read())
                //{
                //    resp.id = Convert.ToInt32(sdr["id"].ToString());
                //    resp.description = sdr["description"].ToString();

                //}
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

        public List<DropdownTypeResponse> dropdown_type_view_fix(string active)
        {
            //DropdownResponse resp = new DropdownResponse();

            List<DropdownTypeResponse> resp = new List<DropdownTypeResponse>();
            string _con = connection._DB_Master;
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
                oCmd.CommandText = "dropdown_type_view_fix";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@active", active);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new DropdownTypeResponse()
                        {
                            id = Convert.ToInt32(dr["id"].ToString()),
                            description = dr["description"].ToString(),

                        }).ToList();
                //while (sdr.Read())
                //{
                //    resp.id = Convert.ToInt32(sdr["id"].ToString());
                //    resp.description = sdr["description"].ToString();

                //}
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

        public List<DropdownResponse> dropdown_fix_view(string dropdown_type_id)
        {


            List<DropdownResponse> resp = new List<DropdownResponse>();
            string _con = connection._DB_Master;
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
                oCmd.CommandText = "dropdown_fix_view";
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

        public DropdownIUResponse dropdown_fix_in_up(DropdownIURequest model)
        {

            //string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + model.series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            DropdownIUResponse resp = new DropdownIUResponse();
            string _con = connection._DB_Master;
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
                oCmd.CommandText = "dropdown_fix_in_up";
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


        public List<PayrollRates> payroll_rates_view()
        {


            List<PayrollRates> resp = new List<PayrollRates>();
            string _con = connection._DB_Master;
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
                oCmd.CommandText = "payroll_rates_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollRates()
                        {
                            payroll_rates_id = Convert.ToInt32(dr["payroll_rates_id"].ToString()),
                            description = dr["description"].ToString(),
                            rates = Convert.ToDecimal(dr["rates"].ToString()),

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



        public List<PagibigResponse> pagibig_table_view()
        {


            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


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
                oCmd.CommandText = "pagibig_table_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PagibigResponse()
                        {
                            range_from = Convert.ToDecimal(dr["range_from"].ToString()),
                            range_to = Convert.ToDecimal(dr["range_to"].ToString()),
                            employer_share = Convert.ToDecimal(dr["employer_share"].ToString()),
                            employee_share = Convert.ToDecimal(dr["employee_share"].ToString()),
                            share_type_id = Convert.ToInt32(dr["share_type_id"].ToString()),

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

        public List<philhealthResponse> philhealth_table_view()
        {



            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" +connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


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
                oCmd.CommandText = "philhealth_table_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new philhealthResponse()
                        {
                            premium_rate = Convert.ToDecimal(dr["premium_rate"].ToString()),
                            minimum = Convert.ToDecimal(dr["minimum"].ToString()),
                            maximum = Convert.ToDecimal(dr["maximum"].ToString()),







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

        public List<sssResponse> sss_table_view()
        {


            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


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
                oCmd.CommandText = "sss_table_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new sssResponse()
                        {


                            range_from = Convert.ToDecimal(dr["range_from"].ToString()),
                            range_to = Convert.ToDecimal(dr["range_to"].ToString()),
                            salary_base = Convert.ToDecimal(dr["salary_base"].ToString()),
                            base_amount = Convert.ToDecimal(dr["base_amount"].ToString()),
                            employer_share = Convert.ToDecimal(dr["employer_share"].ToString()),
                            employee_share = Convert.ToDecimal(dr["employee_share"].ToString()),
                            employer_mpf = Convert.ToDecimal(dr["employer_mpf"].ToString()),
                            employee_mpf = Convert.ToDecimal(dr["employee_mpf"].ToString()),
                            employee_compensation = Convert.ToDecimal(dr["employee_compensation"].ToString()),


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

        public List<taxResponse> tax_table_view(int payroll_type_id)
        {

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" +  connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


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
                oCmd.CommandText = "tax_table_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@payroll_type_id", payroll_type_id);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new taxResponse()
                        {


                            range_from = Convert.ToDecimal(dr["range_from"].ToString()),
                            range_to = Convert.ToDecimal(dr["range_to"].ToString()),
                            salary_base = Convert.ToDecimal(dr["salary_base"].ToString()),
                            base_amount = Convert.ToDecimal(dr["base_amount"].ToString()),
                            payroll_type_id = Convert.ToInt32(dr["payroll_type_id"].ToString()),
                            tax_percentage = Convert.ToDecimal(dr["tax_percentage"].ToString()),


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
