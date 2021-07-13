using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Helper;
using UserManagement.Model;
using UserManagementService;
using UserManagementService.Model;

namespace UserManagement.Services
{

    public interface IUserManagementService
    {
        AuthenticateResponse AuthenticateLogin(AuthenticateRequest model);
        AuthenticateResponse GetById(int userId);
        List<EmployeeResponse> employee_active_view(string series_code, int employee_id);
        List<EmployeeResponse> employee_supervisor_view(string series_code, int employee_id, bool is_supervisor, string created_by);
        List<EmployeeResponse> employee_view_sel(string series_code, string employee_id,string created_by);
        List<EmployeeResponse> employee_fetch(string series_code, string employee_id, string created_by, int row, int index);
        List<EmployeeMovementRequest> employee_in_up(EmployeeRequest model);
        List<EmployeeMovementRequest> employee_profile_up(UserCredentialRequest model);
        List<EmployeeResponse> employee_profile_view(string series_code, string employee_id);

        List<EmployeeMovementResponse> employee_movement_sel(string series_code, string employee_id, string created_by, int movement_type, string date_from, string date_to);
        int employee_movement_in(EmployeeMovementRequest[] model);
        List<EmployeeScheduleResponse> employee_schedule_view(
            string series_code, string shift_id, string total_working_hours, string date_from, string date_to,
            string tag_type, string id, string shift_code_type, string created_by);
        List<EmployeeLeaveResponse> employee_leave_view(
            string series_code, string leave_type_id, string leave_type_code, string leave_name, string gender_to_use, string total_leaves, int tag_type, string id,
             string created_by);


        int employee_in(EmployeeInRequest model);
        int employee_information_temp_in(EmployeeInRequest model);

        List<EmployeeRecurringResponse> employee_recurring_view(string series_code, int adjustment_type_id, int adjustment_id, int timing_id, decimal amount, int tag_type, string id, string created_by);
        List<PayrollContributionResponse> employee_contribution_view(string series_code, int government_type_id, int timing_id,  decimal amount, int tag_type, string id, string created_by);
        List<PayrollAdjustmentResponse> employee_adjustment_view(string series_code, int adjustment_type_id, string adjustment_name, decimal amount, bool taxable, int tag_type, string id, string created_by);
    }
    public class UserManagementServices : IUserManagementService
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        private readonly IWebHostEnvironment _environment;


        public UserManagementServices(IOptions<AppSetting> appSettings, IWebHostEnvironment IWebHostEnvironment, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;
            _environment = IWebHostEnvironment;

        }


        public AuthenticateResponse AuthenticateLogin(AuthenticateRequest model)
        {
            AuthenticateResponse resp = new AuthenticateResponse();


            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + model.series_code + "_userdb;User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
            DataTable dt = new DataTable();
            string UserHash = Crypto.password_encrypt(model.password);
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;

            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {
                oCmd.CommandText = "login_authentication";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@user_name", model.username);
                oCmd.Parameters.AddWithValue("@user_hash", UserHash);
                oCmd.Parameters.AddWithValue("@company_code", model.company_code);
                oCmd.Parameters.AddWithValue("@series_code", model.series_code);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp.id = sdr["employee_id"].ToString() == "0" ? "0" : Crypto.url_encrypt(sdr["employee_id"].ToString());
                    resp.routing = sdr["routing"].ToString();
                    resp.type = sdr["type"].ToString();
                    resp.active = Convert.ToBoolean(sdr["active"].ToString());
                    resp.lock_account = Convert.ToBoolean(sdr["lock_account"].ToString());
                    resp.series = sdr["series_code"].ToString();
                    resp.series_code = Crypto.url_encrypt(sdr["series_code"].ToString());
                    resp.access_level_id = Crypto.url_encrypt(sdr["access_level_id"].ToString());
                    resp.approval_level_id = Crypto.url_encrypt(sdr["approval_level_id"].ToString());
                    resp.category_id = Crypto.url_encrypt(sdr["category_id"].ToString());
                    resp.company_id = Crypto.url_encrypt(sdr["company_id"].ToString());
                    resp.is_admin = Convert.ToBoolean(sdr["is_admin"].ToString());
                    resp.approver = Convert.ToBoolean(sdr["approver"].ToString());
                    resp.is_pw_changed = Convert.ToBoolean(sdr["is_pw_changed"].ToString());
                    resp.display_name = sdr["display_name"].ToString();
                    resp.login_name = sdr["login_name"].ToString();
                    resp.company_name = sdr["company_name"].ToString();
                    resp.image_path = sdr["image_path"].ToString();
                    resp.start = sdr["start"].ToString();
                    resp.end = sdr["end"].ToString();

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


            if (resp.id != "" && resp.id != "0")
            {

                resp.Token = generateJwtToken(resp);
            }

            return resp;
        }

        public List<EmployeeResponse> employee_active_view(string series_code,int employee_id)
        {
            List<EmployeeResponse> resp = new List<EmployeeResponse>();


            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + Crypto.url_decrypt(series_code) + "_userdb;User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
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
                oCmd.CommandText = "employee_active_view";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new EmployeeResponse()
                        {

                            employee_id = Convert.ToInt32(sdr["employee_id"].ToString()),
                            encrypt_employee_id = Crypto.url_encrypt(sdr["employee_id"].ToString()),
                            employee_code = (sdr["employee_code"].ToString()),
                            user_name = (sdr["user_name"].ToString()),
                            user_hash = (sdr["user_hash"].ToString()),
                            decrypted_user_hash = Crypto.password_decrypt(sdr["user_hash"].ToString()),

                            image_path = (sdr["image_path"].ToString()),
                            old_image_path = (sdr["image_path"].ToString()),
                            salutation_id = Convert.ToInt32(sdr["salutation_id"].ToString()),
                            salutation = (sdr["salutation"].ToString()),
                            display_name = sdr["display_name"].ToString(),
                            first_name = (sdr["first_name"].ToString()),
                            middle_name = (sdr["middle_name"].ToString()),
                            last_name = (sdr["last_name"].ToString()),
                            suffix_id = Convert.ToInt32(sdr["suffix_id"].ToString()),
                            suffix = (sdr["suffix"].ToString()),
                            nick_name = (sdr["nick_name"].ToString()),
                            gender_id = Convert.ToInt32(sdr["gender_id"].ToString()),
                            gender = (sdr["gender"].ToString()),
                            nationality_id = Convert.ToInt32(sdr["nationality_id"].ToString()),
                            nationality = (sdr["nationality"].ToString()),
                            birthday = (sdr["birthday"].ToString()),
                            birth_place = (sdr["birth_place"].ToString()),
                            civil_status_id = Convert.ToInt32(sdr["civil_status_id"].ToString()),
                            civil_status = (sdr["civil_status"].ToString()),
                            height = (sdr["height"].ToString()),
                            weight = (sdr["weight"].ToString()),
                            blood_type_id = Convert.ToInt32(sdr["blood_type_id"].ToString()),
                            blood_type = (sdr["blood_type"].ToString()),
                            religion_id = Convert.ToInt32(sdr["religion_id"].ToString()),
                            religion = (sdr["religion"].ToString()),
                            mobile = (sdr["mobile"].ToString()),
                            phone = (sdr["phone"].ToString()),
                            office = (sdr["office"].ToString()),
                            email_address = (sdr["email_address"].ToString()),
                            personal_email_address = (sdr["personal_email_address"].ToString()),
                            alternate_number = (sdr["alternate_number"].ToString()),

                            pre_unit_floor = (sdr["pre_unit_floor"].ToString()),
                            pre_building = (sdr["pre_building"].ToString()),
                            pre_street = (sdr["pre_street"].ToString()),
                            pre_barangay = (sdr["pre_barangay"].ToString()),
                            pre_province_id = Convert.ToInt32(sdr["pre_province_id"].ToString()),
                            pre_province = (sdr["pre_province"].ToString()),
                            pre_city_id = Convert.ToInt32(sdr["pre_city_id"].ToString()),
                            pre_city = (sdr["pre_city"].ToString()),
                            pre_region_id = Convert.ToInt32(sdr["pre_region_id"].ToString()),
                            pre_region = (sdr["pre_region"].ToString()),
                            pre_country_id = Convert.ToInt32(sdr["pre_country_id"].ToString()),
                            pre_country = (sdr["pre_country"].ToString()),
                            pre_zipcode = (sdr["pre_zip_code"].ToString()),
                            per_unit_floor = (sdr["per_unit_floor"].ToString()),
                            per_building = (sdr["per_building"].ToString()),
                            per_street = (sdr["per_street"].ToString()),
                            per_barangay = (sdr["per_barangay"].ToString()),
                            per_province_id = Convert.ToInt32(sdr["per_province_id"].ToString()),
                            per_province = (sdr["per_province"].ToString()),
                            per_city_id = Convert.ToInt32(sdr["per_city_id"].ToString()),
                            per_city = (sdr["per_city"].ToString()),
                            per_region_id = Convert.ToInt32(sdr["per_region_id"].ToString()),
                            per_region = (sdr["per_region"].ToString()),
                            per_country_id = Convert.ToInt32(sdr["per_country_id"].ToString()),
                            per_country = (sdr["per_country"].ToString()),
                            per_zipcode = (sdr["per_zip_code"].ToString()),


                            bio_id = (sdr["bio_id"].ToString()),
                            branch_id = Convert.ToInt32(sdr["branch_id"].ToString()),
                            branch = (sdr["branch_name"].ToString()),
                            employee_status_id = Convert.ToInt32(sdr["employee_status_id"].ToString()),
                            employee_status = (sdr["employee_status"].ToString()),
                            occupation_id = Convert.ToInt32(sdr["occupation_id"].ToString()),
                            occupation = (sdr["occupation"].ToString()),
                            supervisor_id = Convert.ToInt32(sdr["supervisor_id"].ToString()),
                            supervisor = (sdr["supervisor_name"].ToString()),
                            department_id = Convert.ToInt32(sdr["department_id"].ToString()),
                            department = (sdr["department"].ToString()),
                            date_hired = (sdr["date_hired"].ToString()),
                            date_regularized = (sdr["date_regularized"].ToString()),

                            cost_center_id = Convert.ToInt32(sdr["cost_center_id"].ToString()),
                            cost_center = (sdr["cost_center"].ToString()),
                            category_id = Convert.ToInt32(sdr["category_id"].ToString()),
                            category = (sdr["category"].ToString()),
                            division_id = Convert.ToInt32(sdr["division_id"].ToString()),
                            division = (sdr["division"].ToString()),
                            payroll_type_id = Convert.ToInt32(sdr["payroll_type_id"].ToString()),
                            payroll_type = (sdr["payroll_type"].ToString()),
                            monthly_rate = Convert.ToDecimal(sdr["monthly_rate"].ToString()),
                            semi_monthly_rate = Convert.ToDecimal(sdr["semi_monthly_rate"].ToString()),
                            factor_rate = Convert.ToDecimal(sdr["factor_rate"].ToString()),
                            daily_rate = Convert.ToDecimal(sdr["daily_rate"].ToString()),
                            hourly_rate = Convert.ToDecimal(sdr["hourly_rate"].ToString()),
                            bank_id = Convert.ToInt32(sdr["bank_id"].ToString()),
                            bank = (sdr["bank"].ToString()),
                            bank_account = (sdr["bank_account"].ToString()),
                            confidentiality_id = Convert.ToInt32(sdr["confidentiality_id"].ToString()),
                            confidentiality = (sdr["confidentiality"].ToString()),
                            sss = (sdr["sss"].ToString()),
                            pagibig = (sdr["pagibig"].ToString()),
                            tin = (sdr["tin"].ToString()),
                            philhealth = (sdr["philhealth"].ToString()),


                            created_by = Convert.ToInt32(sdr["created_by"].ToString()),
                            created_by_name = (sdr["created_by_name"].ToString()),
                            active = Convert.ToBoolean(sdr["active"].ToString()),
                            date_created = (sdr["date_created"].ToString()),

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

        public List<EmployeeResponse> employee_supervisor_view(string series_code, int employee_id, bool is_supervisor, string created_by)
        {
            List<EmployeeResponse> resp = new List<EmployeeResponse>();


            created_by = Crypto.url_decrypt(created_by);
            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + Crypto.url_decrypt(series_code) + "_userdb;User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
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
                oCmd.CommandText = "employee_supervisor_view";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@is_supervisor", is_supervisor);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new EmployeeResponse()
                        {

                            employee_id = Convert.ToInt32(sdr["employee_id"].ToString()),
                            encrypt_employee_id = Crypto.url_encrypt(sdr["employee_id"].ToString()),
                            employee_code = (sdr["employee_code"].ToString()),
                            user_name = (sdr["user_name"].ToString()),
                            user_hash = (sdr["user_hash"].ToString()),
                            decrypted_user_hash = Crypto.password_decrypt(sdr["user_hash"].ToString()),

                            image_path = (sdr["image_path"].ToString()),
                            old_image_path = (sdr["image_path"].ToString()),
                            salutation_id = Convert.ToInt32(sdr["salutation_id"].ToString()),
                            salutation = (sdr["salutation"].ToString()),
                            display_name = sdr["display_name"].ToString(),
                            first_name = (sdr["first_name"].ToString()),
                            middle_name = (sdr["middle_name"].ToString()),
                            last_name = (sdr["last_name"].ToString()),
                            suffix_id = Convert.ToInt32(sdr["suffix_id"].ToString()),
                            suffix = (sdr["suffix"].ToString()),
                            nick_name = (sdr["nick_name"].ToString()),
                            gender_id = Convert.ToInt32(sdr["gender_id"].ToString()),
                            gender = (sdr["gender"].ToString()),
                            nationality_id = Convert.ToInt32(sdr["nationality_id"].ToString()),
                            nationality = (sdr["nationality"].ToString()),
                            birthday = (sdr["birthday"].ToString()),
                            birth_place = (sdr["birth_place"].ToString()),
                            civil_status_id = Convert.ToInt32(sdr["civil_status_id"].ToString()),
                            civil_status = (sdr["civil_status"].ToString()),
                            height = (sdr["height"].ToString()),
                            weight = (sdr["weight"].ToString()),
                            blood_type_id = Convert.ToInt32(sdr["blood_type_id"].ToString()),
                            blood_type = (sdr["blood_type"].ToString()),
                            religion_id = Convert.ToInt32(sdr["religion_id"].ToString()),
                            religion = (sdr["religion"].ToString()),
                            mobile = (sdr["mobile"].ToString()),
                            phone = (sdr["phone"].ToString()),
                            office = (sdr["office"].ToString()),
                            email_address = (sdr["email_address"].ToString()),
                            personal_email_address = (sdr["personal_email_address"].ToString()),
                            alternate_number = (sdr["alternate_number"].ToString()),

                            pre_unit_floor = (sdr["pre_unit_floor"].ToString()),
                            pre_building = (sdr["pre_building"].ToString()),
                            pre_street = (sdr["pre_street"].ToString()),
                            pre_barangay = (sdr["pre_barangay"].ToString()),
                            pre_province_id = Convert.ToInt32(sdr["pre_province_id"].ToString()),
                            pre_province = (sdr["pre_province"].ToString()),
                            pre_city_id = Convert.ToInt32(sdr["pre_city_id"].ToString()),
                            pre_city = (sdr["pre_city"].ToString()),
                            pre_region_id = Convert.ToInt32(sdr["pre_region_id"].ToString()),
                            pre_region = (sdr["pre_region"].ToString()),
                            pre_country_id = Convert.ToInt32(sdr["pre_country_id"].ToString()),
                            pre_country = (sdr["pre_country"].ToString()),
                            pre_zipcode = (sdr["pre_zip_code"].ToString()),
                            per_unit_floor = (sdr["per_unit_floor"].ToString()),
                            per_building = (sdr["per_building"].ToString()),
                            per_street = (sdr["per_street"].ToString()),
                            per_barangay = (sdr["per_barangay"].ToString()),
                            per_province_id = Convert.ToInt32(sdr["per_province_id"].ToString()),
                            per_province = (sdr["per_province"].ToString()),
                            per_city_id = Convert.ToInt32(sdr["per_city_id"].ToString()),
                            per_city = (sdr["per_city"].ToString()),
                            per_region_id = Convert.ToInt32(sdr["per_region_id"].ToString()),
                            per_region = (sdr["per_region"].ToString()),
                            per_country_id = Convert.ToInt32(sdr["per_country_id"].ToString()),
                            per_country = (sdr["per_country"].ToString()),
                            per_zipcode = (sdr["per_zip_code"].ToString()),


                            bio_id = (sdr["bio_id"].ToString()),
                            branch_id = Convert.ToInt32(sdr["branch_id"].ToString()),
                            branch = (sdr["branch_name"].ToString()),
                            employee_status_id = Convert.ToInt32(sdr["employee_status_id"].ToString()),
                            employee_status = (sdr["employee_status"].ToString()),
                            occupation_id = Convert.ToInt32(sdr["occupation_id"].ToString()),
                            occupation = (sdr["occupation"].ToString()),
                            supervisor_id = Convert.ToInt32(sdr["supervisor_id"].ToString()),
                            supervisor = (sdr["supervisor_name"].ToString()),
                            department_id = Convert.ToInt32(sdr["department_id"].ToString()),
                            department = (sdr["department"].ToString()),
                            date_hired = (sdr["date_hired"].ToString()),
                            date_regularized = (sdr["date_regularized"].ToString()),

                            cost_center_id = Convert.ToInt32(sdr["cost_center_id"].ToString()),
                            cost_center = (sdr["cost_center"].ToString()),
                            category_id = Convert.ToInt32(sdr["category_id"].ToString()),
                            category = (sdr["category"].ToString()),
                            division_id = Convert.ToInt32(sdr["division_id"].ToString()),
                            division = (sdr["division"].ToString()),
                            payroll_type_id = Convert.ToInt32(sdr["payroll_type_id"].ToString()),
                            payroll_type = (sdr["payroll_type"].ToString()),
                            monthly_rate = Convert.ToDecimal(sdr["monthly_rate"].ToString()),
                            semi_monthly_rate = Convert.ToDecimal(sdr["semi_monthly_rate"].ToString()),
                            factor_rate = Convert.ToDecimal(sdr["factor_rate"].ToString()),
                            daily_rate = Convert.ToDecimal(sdr["daily_rate"].ToString()),
                            hourly_rate = Convert.ToDecimal(sdr["hourly_rate"].ToString()),
                            bank_id = Convert.ToInt32(sdr["bank_id"].ToString()),
                            bank = (sdr["bank"].ToString()),
                            bank_account = (sdr["bank_account"].ToString()),
                            confidentiality_id = Convert.ToInt32(sdr["confidentiality_id"].ToString()),
                            confidentiality = (sdr["confidentiality"].ToString()),
                            sss = (sdr["sss"].ToString()),
                            pagibig = (sdr["pagibig"].ToString()),
                            tin = (sdr["tin"].ToString()),
                            philhealth = (sdr["philhealth"].ToString()),


                            created_by = Convert.ToInt32(sdr["created_by"].ToString()),
                            created_by_name = (sdr["created_by_name"].ToString()),
                            active = Convert.ToBoolean(sdr["active"].ToString()),
                            date_created = (sdr["date_created"].ToString()),

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

        public List<EmployeeResponse> employee_view_sel(string series_code, string employee_id,string created_by)
        {
            List<EmployeeResponse> resp = new List<EmployeeResponse>();
            created_by = Crypto.url_decrypt(created_by);
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + Crypto.url_decrypt(series_code) + "_userdb;User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
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
                oCmd.CommandText = "employee_view_sel";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new EmployeeResponse()
                        {

                            employee_id = Convert.ToInt32(sdr["employee_id"].ToString()),
                            encrypt_employee_id = Crypto.url_encrypt(sdr["employee_id"].ToString()),
                            employee_code = (sdr["employee_code"].ToString()),
                            user_name = (sdr["user_name"].ToString()),
                            user_hash = (sdr["user_hash"].ToString()),
                            decrypted_user_hash = Crypto.password_decrypt(sdr["user_hash"].ToString()),

                            image_path = (sdr["image_path"].ToString()),
                            old_image_path = (sdr["image_path"].ToString()),
                            salutation_id = Convert.ToInt32(sdr["salutation_id"].ToString()),
                            salutation = (sdr["salutation"].ToString()),
                            display_name = sdr["display_name"].ToString(),
                            first_name = (sdr["first_name"].ToString()),
                            middle_name = (sdr["middle_name"].ToString()),
                            last_name = (sdr["last_name"].ToString()),
                            suffix_id = Convert.ToInt32(sdr["suffix_id"].ToString()),
                            suffix = (sdr["suffix"].ToString()),
                            nick_name = (sdr["nick_name"].ToString()),
                            gender_id = Convert.ToInt32(sdr["gender_id"].ToString()),
                            gender = (sdr["gender"].ToString()),
                            nationality_id = Convert.ToInt32(sdr["nationality_id"].ToString()),
                            nationality = (sdr["nationality"].ToString()),
                            birthday = (sdr["birthday"].ToString()),
                            birth_place = (sdr["birth_place"].ToString()),
                            civil_status_id = Convert.ToInt32(sdr["civil_status_id"].ToString()),
                            civil_status = (sdr["civil_status"].ToString()),
                            height = (sdr["height"].ToString()),
                            weight = (sdr["weight"].ToString()),
                            blood_type_id = Convert.ToInt32(sdr["blood_type_id"].ToString()),
                            blood_type = (sdr["blood_type"].ToString()),
                            religion_id = Convert.ToInt32(sdr["religion_id"].ToString()),
                            religion = (sdr["religion"].ToString()),
                            mobile = (sdr["mobile"].ToString()),
                            phone = (sdr["phone"].ToString()),
                            office = (sdr["office"].ToString()),
                            email_address = (sdr["email_address"].ToString()),
                            personal_email_address = (sdr["personal_email_address"].ToString()),
                            alternate_number = (sdr["alternate_number"].ToString()),

                            pre_unit_floor = (sdr["pre_unit_floor"].ToString()),
                            pre_building = (sdr["pre_building"].ToString()),
                            pre_street = (sdr["pre_street"].ToString()),
                            pre_barangay = (sdr["pre_barangay"].ToString()),
                            pre_province_id = Convert.ToInt32(sdr["pre_province_id"].ToString()),
                            pre_province = (sdr["pre_province"].ToString()),
                            pre_city_id = Convert.ToInt32(sdr["pre_city_id"].ToString()),
                            pre_city = (sdr["pre_city"].ToString()),
                            pre_region_id = Convert.ToInt32(sdr["pre_region_id"].ToString()),
                            pre_region = (sdr["pre_region"].ToString()),
                            pre_country_id = Convert.ToInt32(sdr["pre_country_id"].ToString()),
                            pre_country = (sdr["pre_country"].ToString()),
                            pre_zipcode = (sdr["pre_zip_code"].ToString()),
                            per_unit_floor = (sdr["per_unit_floor"].ToString()),
                            per_building = (sdr["per_building"].ToString()),
                            per_street = (sdr["per_street"].ToString()),
                            per_barangay = (sdr["per_barangay"].ToString()),
                            per_province_id = Convert.ToInt32(sdr["per_province_id"].ToString()),
                            per_province = (sdr["per_province"].ToString()),
                            per_city_id = Convert.ToInt32(sdr["per_city_id"].ToString()),
                            per_city = (sdr["per_city"].ToString()),
                            per_region_id = Convert.ToInt32(sdr["per_region_id"].ToString()),
                            per_region = (sdr["per_region"].ToString()),
                            per_country_id = Convert.ToInt32(sdr["per_country_id"].ToString()),
                            per_country = (sdr["per_country"].ToString()),
                            per_zipcode = (sdr["per_zip_code"].ToString()),


                            bio_id = (sdr["bio_id"].ToString()),
                            branch_id = Convert.ToInt32(sdr["branch_id"].ToString()),
                            branch = (sdr["branch_name"].ToString()),
                            employee_status_id = Convert.ToInt32(sdr["employee_status_id"].ToString()),
                            employee_status = (sdr["employee_status"].ToString()),
                            occupation_id = Convert.ToInt32(sdr["occupation_id"].ToString()),
                            occupation = (sdr["occupation"].ToString()),
                            supervisor_id = Convert.ToInt32(sdr["supervisor_id"].ToString()),
                            supervisor = (sdr["supervisor_name"].ToString()),
                            department_id = Convert.ToInt32(sdr["department_id"].ToString()),
                            department = (sdr["department"].ToString()),
                            date_hired = (sdr["date_hired"].ToString()),
                            date_regularized = (sdr["date_regularized"].ToString()),

                            cost_center_id = Convert.ToInt32(sdr["cost_center_id"].ToString()),
                            cost_center = (sdr["cost_center"].ToString()),
                            category_id = Convert.ToInt32(sdr["category_id"].ToString()),
                            category = (sdr["category"].ToString()),
                            division_id = Convert.ToInt32(sdr["division_id"].ToString()),
                            division = (sdr["division"].ToString()),
                            payroll_type_id = Convert.ToInt32(sdr["payroll_type_id"].ToString()),
                            payroll_type = (sdr["payroll_type"].ToString()),
                            monthly_rate = Convert.ToDecimal(sdr["monthly_rate"].ToString()),
                            semi_monthly_rate = Convert.ToDecimal(sdr["semi_monthly_rate"].ToString()),
                            factor_rate = Convert.ToDecimal(sdr["factor_rate"].ToString()),
                            daily_rate = Convert.ToDecimal(sdr["daily_rate"].ToString()),
                            hourly_rate = Convert.ToDecimal(sdr["hourly_rate"].ToString()),
                            bank_id = Convert.ToInt32(sdr["bank_id"].ToString()),
                            bank = (sdr["bank"].ToString()),
                            bank_account = (sdr["bank_account"].ToString()),
                            confidentiality_id = Convert.ToInt32(sdr["confidentiality_id"].ToString()),
                            confidentiality = (sdr["confidentiality"].ToString()),
                            sss = (sdr["sss"].ToString()),
                            pagibig = (sdr["pagibig"].ToString()),
                            tin = (sdr["tin"].ToString()),
                            philhealth = (sdr["philhealth"].ToString()),


                            created_by = Convert.ToInt32(sdr["created_by"].ToString()),
                            created_by_name = (sdr["created_by_name"].ToString()),
                            active = Convert.ToBoolean(sdr["active"].ToString()),
                            date_created = (sdr["date_created"].ToString()),

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

        public List<EmployeeResponse> employee_fetch(string series_code, string employee_id, string created_by,int row,int index)
        {
            List<EmployeeResponse> resp = new List<EmployeeResponse>();
            created_by = Crypto.url_decrypt(created_by);
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + Crypto.url_decrypt(series_code) + "_userdb;User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
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
                oCmd.CommandText = "employee_fetch";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@row", row);
                oCmd.Parameters.AddWithValue("@index", index);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new EmployeeResponse()
                        {

                            employee_id = Convert.ToInt32(sdr["employee_id"].ToString()),
                            encrypt_employee_id = Crypto.url_encrypt(sdr["employee_id"].ToString()),
                            employee_code = (sdr["employee_code"].ToString()),
                            user_name = (sdr["user_name"].ToString()),
                            user_hash = (sdr["user_hash"].ToString()),
                            decrypted_user_hash = Crypto.password_decrypt(sdr["user_hash"].ToString()),

                            image_path = (sdr["image_path"].ToString()),
                            old_image_path = (sdr["image_path"].ToString()),
                            salutation_id = Convert.ToInt32(sdr["salutation_id"].ToString()),
                            salutation = (sdr["salutation"].ToString()),
                            display_name = sdr["display_name"].ToString(),
                            first_name = (sdr["first_name"].ToString()),
                            middle_name = (sdr["middle_name"].ToString()),
                            last_name = (sdr["last_name"].ToString()),
                            suffix_id = Convert.ToInt32(sdr["suffix_id"].ToString()),
                            suffix = (sdr["suffix"].ToString()),
                            nick_name = (sdr["nick_name"].ToString()),
                            gender_id = Convert.ToInt32(sdr["gender_id"].ToString()),
                            gender = (sdr["gender"].ToString()),
                            nationality_id = Convert.ToInt32(sdr["nationality_id"].ToString()),
                            nationality = (sdr["nationality"].ToString()),
                            birthday = (sdr["birthday"].ToString()),
                            birth_place = (sdr["birth_place"].ToString()),
                            civil_status_id = Convert.ToInt32(sdr["civil_status_id"].ToString()),
                            civil_status = (sdr["civil_status"].ToString()),
                            height = (sdr["height"].ToString()),
                            weight = (sdr["weight"].ToString()),
                            blood_type_id = Convert.ToInt32(sdr["blood_type_id"].ToString()),
                            blood_type = (sdr["blood_type"].ToString()),
                            religion_id = Convert.ToInt32(sdr["religion_id"].ToString()),
                            religion = (sdr["religion"].ToString()),
                            mobile = (sdr["mobile"].ToString()),
                            phone = (sdr["phone"].ToString()),
                            office = (sdr["office"].ToString()),
                            email_address = (sdr["email_address"].ToString()),
                            personal_email_address = (sdr["personal_email_address"].ToString()),
                            alternate_number = (sdr["alternate_number"].ToString()),

                            pre_unit_floor = (sdr["pre_unit_floor"].ToString()),
                            pre_building = (sdr["pre_building"].ToString()),
                            pre_street = (sdr["pre_street"].ToString()),
                            pre_barangay = (sdr["pre_barangay"].ToString()),
                            pre_province_id = Convert.ToInt32(sdr["pre_province_id"].ToString()),
                            pre_province = (sdr["pre_province"].ToString()),
                            pre_city_id = Convert.ToInt32(sdr["pre_city_id"].ToString()),
                            pre_city = (sdr["pre_city"].ToString()),
                            pre_region_id = Convert.ToInt32(sdr["pre_region_id"].ToString()),
                            pre_region = (sdr["pre_region"].ToString()),
                            pre_country_id = Convert.ToInt32(sdr["pre_country_id"].ToString()),
                            pre_country = (sdr["pre_country"].ToString()),
                            pre_zipcode = (sdr["pre_zip_code"].ToString()),
                            per_unit_floor = (sdr["per_unit_floor"].ToString()),
                            per_building = (sdr["per_building"].ToString()),
                            per_street = (sdr["per_street"].ToString()),
                            per_barangay = (sdr["per_barangay"].ToString()),
                            per_province_id = Convert.ToInt32(sdr["per_province_id"].ToString()),
                            per_province = (sdr["per_province"].ToString()),
                            per_city_id = Convert.ToInt32(sdr["per_city_id"].ToString()),
                            per_city = (sdr["per_city"].ToString()),
                            per_region_id = Convert.ToInt32(sdr["per_region_id"].ToString()),
                            per_region = (sdr["per_region"].ToString()),
                            per_country_id = Convert.ToInt32(sdr["per_country_id"].ToString()),
                            per_country = (sdr["per_country"].ToString()),
                            per_zipcode = (sdr["per_zip_code"].ToString()),


                            bio_id = (sdr["bio_id"].ToString()),
                            branch_id = Convert.ToInt32(sdr["branch_id"].ToString()),
                            branch = (sdr["branch_name"].ToString()),
                            employee_status_id = Convert.ToInt32(sdr["employee_status_id"].ToString()),
                            employee_status = (sdr["employee_status"].ToString()),
                            occupation_id = Convert.ToInt32(sdr["occupation_id"].ToString()),
                            occupation = (sdr["occupation"].ToString()),
                            supervisor_id = Convert.ToInt32(sdr["supervisor_id"].ToString()),
                            supervisor = (sdr["supervisor_name"].ToString()),
                            department_id = Convert.ToInt32(sdr["department_id"].ToString()),
                            department = (sdr["department"].ToString()),
                            date_hired = (sdr["date_hired"].ToString()),
                            date_regularized = (sdr["date_regularized"].ToString()),

                            cost_center_id = Convert.ToInt32(sdr["cost_center_id"].ToString()),
                            cost_center = (sdr["cost_center"].ToString()),
                            category_id = Convert.ToInt32(sdr["category_id"].ToString()),
                            category = (sdr["category"].ToString()),
                            division_id = Convert.ToInt32(sdr["division_id"].ToString()),
                            division = (sdr["division"].ToString()),
                            payroll_type_id = Convert.ToInt32(sdr["payroll_type_id"].ToString()),
                            payroll_type = (sdr["payroll_type"].ToString()),
                            monthly_rate = Convert.ToDecimal(sdr["monthly_rate"].ToString()),
                            semi_monthly_rate = Convert.ToDecimal(sdr["semi_monthly_rate"].ToString()),
                            factor_rate = Convert.ToDecimal(sdr["factor_rate"].ToString()),
                            daily_rate = Convert.ToDecimal(sdr["daily_rate"].ToString()),
                            hourly_rate = Convert.ToDecimal(sdr["hourly_rate"].ToString()),
                            bank_id = Convert.ToInt32(sdr["bank_id"].ToString()),
                            bank = (sdr["bank"].ToString()),
                            bank_account = (sdr["bank_account"].ToString()),
                            confidentiality_id = Convert.ToInt32(sdr["confidentiality_id"].ToString()),
                            confidentiality = (sdr["confidentiality"].ToString()),
                            sss = (sdr["sss"].ToString()),
                            pagibig = (sdr["pagibig"].ToString()),
                            tin = (sdr["tin"].ToString()),
                            philhealth = (sdr["philhealth"].ToString()),


                            created_by = Convert.ToInt32(sdr["created_by"].ToString()),
                            created_by_name = (sdr["created_by_name"].ToString()),
                            active = Convert.ToBoolean(sdr["active"].ToString()),
                            date_created = (sdr["date_created"].ToString()),

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

        public List<EmployeeMovementRequest> employee_in_up(EmployeeRequest model)
        {
            List<EmployeeMovementRequest> resp = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> resp1 = new List<EmployeeMovementRequest>();
            //model.EIRequest.branch_id = Crypto.url_decrypt(model.EIRequest.branch_id);
            model.encrypt_employee_id = model.encrypt_employee_id == "0" ? "0" : Crypto.url_decrypt(model.encrypt_employee_id);
            string series_code = Crypto.url_decrypt(model.series_code);
            string created_by = Crypto.url_decrypt(model.created_by);

            string encrypted_series_code = (model.series_code);
            string encrypted_created_by = (model.created_by);
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
                DateTime birthday = Convert.ToDateTime(model.birthday);
                var userhash = birthday.ToString("MMddyy");

                userhash = model.last_name + "@" + userhash; 
                var folder = series_code + "\\" + created_by + "\\17\\" + model.employee_code;
                var path = "";
                if(model.encrypt_employee_id != "0")
                {
                    if (model.image_path == "")
                    {
                        path = model.old_image_path;
                    }
                    else
                    {
                        path = folder  + "\\" + model.image_path;
                    }
                }
                else
                {
                    if (model.image_path == "")
                    {
                        path = "Default\\Image\\default.png";
                    }
                    else
                    {
                        path = folder + "\\" + model.image_path;
                    }
                }
               
                oCmd.CommandText = "employee_in_up";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@active",                 model.active                );
                oCmd.Parameters.AddWithValue("@created_by",             created_by                  );
                oCmd.Parameters.AddWithValue("@employee_id",            model.encrypt_employee_id   );
                oCmd.Parameters.AddWithValue("@display_name",           model.display_name          );
                oCmd.Parameters.AddWithValue("@employee_code",          model.employee_code         );
                oCmd.Parameters.AddWithValue("@user_name",              model.user_name             );
                oCmd.Parameters.AddWithValue("@user_hash",              model.encrypt_employee_id == "0" ? Crypto.password_encrypt(userhash) : Crypto.password_encrypt(model.user_hash));
                oCmd.Parameters.AddWithValue("@salutation_id",          model.salutation_id         );
                oCmd.Parameters.AddWithValue("@first_name",             model.first_name            );
                oCmd.Parameters.AddWithValue("@middle_name",            model.middle_name           );
                oCmd.Parameters.AddWithValue("@last_name",              model.last_name             );
                oCmd.Parameters.AddWithValue("@suffix_id",              model.suffix_id             );
                oCmd.Parameters.AddWithValue("@nick_name",              model.nick_name             );
                oCmd.Parameters.AddWithValue("@gender_id",              model.gender_id             );
                oCmd.Parameters.AddWithValue("@nationality_id",         model.nationality_id        );
                oCmd.Parameters.AddWithValue("@birthday",               model.birthday              );
                oCmd.Parameters.AddWithValue("@birth_place",            model.birth_place           );
                oCmd.Parameters.AddWithValue("@civil_status_id",        model.civil_status_id       );
                oCmd.Parameters.AddWithValue("@height",                 model.height                );
                oCmd.Parameters.AddWithValue("@weight",                 model.weight                );
                oCmd.Parameters.AddWithValue("@blood_type_id",          model.blood_type_id         );
                oCmd.Parameters.AddWithValue("@religion_id",            model.religion_id           );
                oCmd.Parameters.AddWithValue("@mobile",                 model.mobile                );
                oCmd.Parameters.AddWithValue("@phone",                  model.phone                 );
                oCmd.Parameters.AddWithValue("@office",                 model.office                );
                oCmd.Parameters.AddWithValue("@email_address",          model.email_address         );
                oCmd.Parameters.AddWithValue("@personal_email_address", model.personal_email_address);
                oCmd.Parameters.AddWithValue("@alternate_number",       model.alternate_number      );
                //oCmd.Parameters.AddWithValue("@present_address",        model.present_address       );
                //oCmd.Parameters.AddWithValue("@permanent_address",      model.permanent_address     );
                oCmd.Parameters.AddWithValue("@pre_unit_floor",         model.pre_unit_floor  );
                oCmd.Parameters.AddWithValue("@pre_building",           model.pre_building    );
                oCmd.Parameters.AddWithValue("@pre_street",             model.pre_street      );
                oCmd.Parameters.AddWithValue("@pre_barangay",           model.pre_barangay    );
                oCmd.Parameters.AddWithValue("@pre_province",           model.pre_province_id );
                oCmd.Parameters.AddWithValue("@pre_city",               model.pre_city_id     );
                oCmd.Parameters.AddWithValue("@pre_region",             model.pre_region_id   );
                oCmd.Parameters.AddWithValue("@pre_country",            model.pre_country_id  );
                oCmd.Parameters.AddWithValue("@pre_zip_code",           model.pre_zipcode     );
                oCmd.Parameters.AddWithValue("@per_unit_floor",         model.per_unit_floor  );
                oCmd.Parameters.AddWithValue("@per_building",           model.per_building    );
                oCmd.Parameters.AddWithValue("@per_street",             model.per_street      );
                oCmd.Parameters.AddWithValue("@per_barangay",           model.per_barangay    );
                oCmd.Parameters.AddWithValue("@per_province",           model.per_province_id );
                oCmd.Parameters.AddWithValue("@per_city",               model.per_city_id     );
                oCmd.Parameters.AddWithValue("@per_region",             model.per_region_id   );
                oCmd.Parameters.AddWithValue("@per_country",            model.per_country_id  );
                oCmd.Parameters.AddWithValue("@per_zip_code",           model.per_zipcode     );




                oCmd.Parameters.AddWithValue("@image_path", path);

                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new EmployeeMovementRequest()
                        {

                                employee_id = Convert.ToInt32(sdr["employee_id"].ToString()),
                                movement_type = Convert.ToInt32(sdr["movement_type"].ToString()),
                                is_dropdown = Convert.ToInt32(sdr["is_dropdown"].ToString()),
                                id = Convert.ToInt32(sdr["id"].ToString()),
                                description = (sdr["description"].ToString()),
                                series_code = encrypted_series_code,
                                created_by = encrypted_created_by,
                        }).ToList();
                //SqlDataReader sdr = oCmd.ExecuteReader();
                //while (sdr.Read())
                //{

                //    resp.employee_id = Convert.ToInt32(sdr["employee_id"].ToString());
                //    resp.movement_type = Convert.ToInt32(sdr["movement_type"].ToString());
                //    resp.is_dropdown = Convert.ToInt32(sdr["is_dropdown"].ToString());
                //    resp.id = Convert.ToInt32(sdr["id"].ToString());
                //    resp.description = (sdr["description"].ToString());
                //    resp.series_code = series_code;
                //    resp.created_by = created_by;

                //}
                //sdr.Close();


                if (model.EIRequest != null)
                {
                        oCmd.CommandText = "employee_information_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();

                        oCmd.Parameters.AddWithValue("@employee_id", model.encrypt_employee_id == "0" ? resp[0].employee_id : model.encrypt_employee_id);
                        oCmd.Parameters.AddWithValue("@bio_id", model.EIRequest.bio_id             );
                        oCmd.Parameters.AddWithValue("@branch_id", model.EIRequest.branch_id           );
                        oCmd.Parameters.AddWithValue("@employee_status_id", model.EIRequest.employee_status_id  );
                        oCmd.Parameters.AddWithValue("@occupation_id", model.EIRequest.occupation_id       );
                        oCmd.Parameters.AddWithValue("@supervisor_id", model.EIRequest.supervisor_id       );
                        oCmd.Parameters.AddWithValue("@department_id", model.EIRequest.department_id       );
                        oCmd.Parameters.AddWithValue("@date_hired", model.EIRequest.date_hired          );
                        oCmd.Parameters.AddWithValue("@date_regularized", model.EIRequest.date_regularized    );
                        oCmd.Parameters.AddWithValue("@cost_center_id", model.EIRequest.cost_center_id      );
                        oCmd.Parameters.AddWithValue("@category_id", model.EIRequest.category_id         );
                        oCmd.Parameters.AddWithValue("@division_id", model.EIRequest.division_id         );
                        oCmd.Parameters.AddWithValue("@payroll_type_id", model.EIRequest.payroll_type_id     );
                        oCmd.Parameters.AddWithValue("@monthly_rate", model.EIRequest.monthly_rate        );
                        oCmd.Parameters.AddWithValue("@semi_monthly_rate", model.EIRequest.semi_monthly_rate   );
                        oCmd.Parameters.AddWithValue("@factor_rate", model.EIRequest.factor_rate         );
                        oCmd.Parameters.AddWithValue("@daily_rate", model.EIRequest.daily_rate          );
                        oCmd.Parameters.AddWithValue("@hourly_rate", model.EIRequest.hourly_rate         );
                        oCmd.Parameters.AddWithValue("@bank_id", model.EIRequest.bank_id             );
                        oCmd.Parameters.AddWithValue("@bank_account", model.EIRequest.bank_account        );
                        oCmd.Parameters.AddWithValue("@confidentiality_id", model.EIRequest.confidentiality_id  );
                        oCmd.Parameters.AddWithValue("@sss", model.EIRequest.sss  );
                        oCmd.Parameters.AddWithValue("@pagibig", model.EIRequest.pagibig  );
                        oCmd.Parameters.AddWithValue("@philhealth", model.EIRequest.philhealth  );
                        oCmd.Parameters.AddWithValue("@tin", model.EIRequest.tin  );
                        dt.Clear();
                        da.Fill(dt);
                    resp1 = (from DataRow sdr in dt.Rows
                                select new EmployeeMovementRequest()
                                {

                                    employee_id = Convert.ToInt32(sdr["employee_id"].ToString()),
                                    movement_type = Convert.ToInt32(sdr["movement_type"].ToString()),
                                    is_dropdown = Convert.ToInt32(sdr["is_dropdown"].ToString()),
                                    id = Convert.ToInt32(sdr["id"].ToString()),
                                    description = (sdr["description"].ToString()),
                                    series_code = encrypted_series_code,
                                    created_by = encrypted_created_by,
                                }).ToList();

                }


                foreach (var item in resp1)
                {
                    resp.Add(item);
                }



                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                resp = new List<EmployeeMovementRequest>();
                oTrans.Rollback();
            }
            finally
            {
                oConn.Close();
            }




            return resp;
        }


        public List<EmployeeResponse> employee_profile_view(string series_code, string employee_id)
        {
            List<EmployeeResponse> resp = new List<EmployeeResponse>();
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + Crypto.url_decrypt(series_code) + "_userdb;User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
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
                oCmd.CommandText = "employee_profile_view";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new EmployeeResponse()
                        {

                            employee_id = Convert.ToInt32(sdr["employee_id"].ToString()),
                            encrypt_employee_id = Crypto.url_encrypt(sdr["employee_id"].ToString()),
                            employee_code = (sdr["employee_code"].ToString()),
                            user_name = (sdr["user_name"].ToString()),
                            user_hash = (sdr["user_hash"].ToString()),
                            decrypted_user_hash = Crypto.password_decrypt(sdr["user_hash"].ToString()),

                            image_path = (sdr["image_path"].ToString()),
                            old_image_path = (sdr["image_path"].ToString()),
                            salutation_id = Convert.ToInt32(sdr["salutation_id"].ToString()),
                            salutation = (sdr["salutation"].ToString()),
                            display_name = sdr["display_name"].ToString(),
                            first_name = (sdr["first_name"].ToString()),
                            middle_name = (sdr["middle_name"].ToString()),
                            last_name = (sdr["last_name"].ToString()),
                            suffix_id = Convert.ToInt32(sdr["suffix_id"].ToString()),
                            suffix = (sdr["suffix"].ToString()),
                            nick_name = (sdr["nick_name"].ToString()),
                            gender_id = Convert.ToInt32(sdr["gender_id"].ToString()),
                            gender = (sdr["gender"].ToString()),
                            nationality_id = Convert.ToInt32(sdr["nationality_id"].ToString()),
                            nationality = (sdr["nationality"].ToString()),
                            birthday = (sdr["birthday"].ToString()),
                            birth_place = (sdr["birth_place"].ToString()),
                            civil_status_id = Convert.ToInt32(sdr["civil_status_id"].ToString()),
                            civil_status = (sdr["civil_status"].ToString()),
                            height = (sdr["height"].ToString()),
                            weight = (sdr["weight"].ToString()),
                            blood_type_id = Convert.ToInt32(sdr["blood_type_id"].ToString()),
                            blood_type = (sdr["blood_type"].ToString()),
                            religion_id = Convert.ToInt32(sdr["religion_id"].ToString()),
                            religion = (sdr["religion"].ToString()),
                            mobile = (sdr["mobile"].ToString()),
                            phone = (sdr["phone"].ToString()),
                            office = (sdr["office"].ToString()),
                            email_address = (sdr["email_address"].ToString()),
                            personal_email_address = (sdr["personal_email_address"].ToString()),
                            alternate_number = (sdr["alternate_number"].ToString()),

                            pre_unit_floor = (sdr["pre_unit_floor"].ToString()),
                            pre_building = (sdr["pre_building"].ToString()),
                            pre_street = (sdr["pre_street"].ToString()),
                            pre_barangay = (sdr["pre_barangay"].ToString()),
                            pre_province_id = Convert.ToInt32(sdr["pre_province_id"].ToString()),
                            pre_province = (sdr["pre_province"].ToString()),
                            pre_city_id = Convert.ToInt32(sdr["pre_city_id"].ToString()),
                            pre_city = (sdr["pre_city"].ToString()),
                            pre_region_id = Convert.ToInt32(sdr["pre_region_id"].ToString()),
                            pre_region = (sdr["pre_region"].ToString()),
                            pre_country_id = Convert.ToInt32(sdr["pre_country_id"].ToString()),
                            pre_country = (sdr["pre_country"].ToString()),
                            pre_zipcode = (sdr["pre_zip_code"].ToString()),
                            per_unit_floor = (sdr["per_unit_floor"].ToString()),
                            per_building = (sdr["per_building"].ToString()),
                            per_street = (sdr["per_street"].ToString()),
                            per_barangay = (sdr["per_barangay"].ToString()),
                            per_province_id = Convert.ToInt32(sdr["per_province_id"].ToString()),
                            per_province = (sdr["per_province"].ToString()),
                            per_city_id = Convert.ToInt32(sdr["per_city_id"].ToString()),
                            per_city = (sdr["per_city"].ToString()),
                            per_region_id = Convert.ToInt32(sdr["per_region_id"].ToString()),
                            per_region = (sdr["per_region"].ToString()),
                            per_country_id = Convert.ToInt32(sdr["per_country_id"].ToString()),
                            per_country = (sdr["per_country"].ToString()),
                            per_zipcode = (sdr["per_zip_code"].ToString()),


                            bio_id = (sdr["bio_id"].ToString()),
                            branch_id = Convert.ToInt32(sdr["branch_id"].ToString()),
                            branch = (sdr["branch_name"].ToString()),
                            employee_status_id = Convert.ToInt32(sdr["employee_status_id"].ToString()),
                            employee_status = (sdr["employee_status"].ToString()),
                            occupation_id = Convert.ToInt32(sdr["occupation_id"].ToString()),
                            occupation = (sdr["occupation"].ToString()),
                            supervisor_id = Convert.ToInt32(sdr["supervisor_id"].ToString()),
                            supervisor = (sdr["supervisor_name"].ToString()),
                            department_id = Convert.ToInt32(sdr["department_id"].ToString()),
                            department = (sdr["department"].ToString()),
                            date_hired = (sdr["date_hired"].ToString()),
                            date_regularized = (sdr["date_regularized"].ToString()),

                            cost_center_id = Convert.ToInt32(sdr["cost_center_id"].ToString()),
                            cost_center = (sdr["cost_center"].ToString()),
                            category_id = Convert.ToInt32(sdr["category_id"].ToString()),
                            category = (sdr["category"].ToString()),
                            division_id = Convert.ToInt32(sdr["division_id"].ToString()),
                            division = (sdr["division"].ToString()),
                            payroll_type_id = Convert.ToInt32(sdr["payroll_type_id"].ToString()),
                            payroll_type = (sdr["payroll_type"].ToString()),
                            monthly_rate = Convert.ToDecimal(sdr["monthly_rate"].ToString()),
                            semi_monthly_rate = Convert.ToDecimal(sdr["semi_monthly_rate"].ToString()),
                            factor_rate = Convert.ToDecimal(sdr["factor_rate"].ToString()),
                            daily_rate = Convert.ToDecimal(sdr["daily_rate"].ToString()),
                            hourly_rate = Convert.ToDecimal(sdr["hourly_rate"].ToString()),
                            bank_id = Convert.ToInt32(sdr["bank_id"].ToString()),
                            bank = (sdr["bank"].ToString()),
                            bank_account = (sdr["bank_account"].ToString()),
                            confidentiality_id = Convert.ToInt32(sdr["confidentiality_id"].ToString()),
                            confidentiality = (sdr["confidentiality"].ToString()),
                            sss = (sdr["sss"].ToString()),
                            pagibig = (sdr["pagibig"].ToString()),
                            tin = (sdr["tin"].ToString()),
                            philhealth = (sdr["philhealth"].ToString()),


                            created_by = Convert.ToInt32(sdr["created_by"].ToString()),
                            created_by_name = (sdr["created_by_name"].ToString()),
                            active = Convert.ToBoolean(sdr["active"].ToString()),
                            date_created = (sdr["date_created"].ToString()),

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

        public int employee_movement_in(EmployeeMovementRequest[] model)
        {
            int resp = 0;
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
                        oCmd.CommandText = "employee_movement_in";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        oCmd.Parameters.AddWithValue("@employee_id", item.employee_id);
                        oCmd.Parameters.AddWithValue("@movement_type", item.movement_type);
                        oCmd.Parameters.AddWithValue("@is_dropdown", item.is_dropdown);
                        oCmd.Parameters.AddWithValue("@id", item.id);
                        oCmd.Parameters.AddWithValue("@description", item.description);
                        oCmd.Parameters.AddWithValue("@movement_description", item.movement_description == null ? "" : item.movement_description);
                        SqlDataReader sdr = oCmd.ExecuteReader();
                        while (sdr.Read())
                        {
                            //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                            resp = Convert.ToInt32(sdr["employee_id"].ToString());

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


        public List<EmployeeMovementRequest> employee_profile_up(UserCredentialRequest model)
        {
            model.employee_id = model.employee_id == "0" ? "0" : Crypto.url_decrypt(model.employee_id);
            List<EmployeeMovementRequest> resp = new List<EmployeeMovementRequest>();
            List<EmployeeMovementRequest> loop_resp = new List<EmployeeMovementRequest>();
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
                    oCmd.CommandText = "employee_profile_up";
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    oCmd.Parameters.Clear();
                    oCmd.Parameters.AddWithValue("@employee_id", model.employee_id);
                    oCmd.Parameters.AddWithValue("@user_name", model.user_name);
                    oCmd.Parameters.AddWithValue("@user_hash", Crypto.password_encrypt(model.user_hash));
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



        public List<EmployeeMovementResponse> employee_movement_sel(string series_code, string employee_id, string created_by, int movement_type, string date_from, string date_to)
        {
            List<EmployeeMovementResponse> resp = new List<EmployeeMovementResponse>();
            created_by = Crypto.url_decrypt(created_by);
            employee_id = employee_id == "0" ? "0" : Crypto.url_decrypt(employee_id);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + Crypto.url_decrypt(series_code) + "_userdb;User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
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
                oCmd.CommandText = "employee_movement_sel";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@employee_id", employee_id);
                oCmd.Parameters.AddWithValue("@movement_type", movement_type);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new EmployeeMovementResponse()
                        {


                            employee_code           = (sdr["employee_code"].ToString()),
                            display_name            = (sdr["display_name"].ToString()),  
                            movement_type           = Convert.ToInt32(sdr["movement_type"].ToString()),
                            movement_type_description = (sdr["movement_type_description"].ToString()),  
                            description             = (sdr["description"].ToString()),  
                            created_by              = (sdr["created_by"].ToString()),  
                            date_created            = (sdr["date_created"].ToString()),

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


        public List<EmployeeScheduleResponse> employee_schedule_view(
            string series_code, string shift_id,  string total_working_hours, string date_from, string date_to,
            string tag_type,string id, string shift_code_type, string created_by)
        {
            List<EmployeeScheduleResponse> resp = new List<EmployeeScheduleResponse>();
            created_by = Crypto.url_decrypt(created_by);
            shift_id = shift_id == "0" ? "0" : Crypto.url_decrypt(shift_id);
            id = id == "0" ? "0" : Crypto.url_decrypt(id);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + Crypto.url_decrypt(series_code) + "_userdb;User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
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
                oCmd.CommandText = "employee_schedule_view";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@shift_id", shift_id);
                oCmd.Parameters.AddWithValue("@total_working_hours", total_working_hours);
                oCmd.Parameters.AddWithValue("@tag_type", tag_type);
                oCmd.Parameters.AddWithValue("@shift_code_type", shift_code_type);
                oCmd.Parameters.AddWithValue("@id", id);
                oCmd.Parameters.AddWithValue("@date_from", date_from);
                oCmd.Parameters.AddWithValue("@date_to", date_to);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new EmployeeScheduleResponse()
                        {


                            employee_code = (sdr["employee_code"].ToString()),
                            display_name = (sdr["display_name"].ToString()),
                            //created_by = Convert.ToInt32(sdr["created_by"].ToString()),

                            shift_id                    = Convert.ToInt32(sdr["shift_id"].ToString()),      
                            encrypt_shift_id            = Crypto.url_encrypt(sdr["shift_id"].ToString()),
                            employee_id                 = Convert.ToInt32(sdr["employee_id"].ToString()),
                            date_from                   = (sdr["date_from"].ToString()),
                            date_to                     = (sdr["date_to"].ToString()),
                            total_working_hours         = (sdr["total_working_hours"].ToString()),
                            total_working_hours_decimal = Convert.ToDecimal(sdr["total_working_hours_decimal"].ToString()),

                            int_shift_id                = Convert.ToInt32(sdr["shift_id"].ToString()),
                            shift_code                  = (sdr["shift_code"].ToString()),
                            shift_name                  = (sdr["shift_name"].ToString()),
                            grace_period                = Convert.ToInt32(sdr["grace_period"].ToString()),
                            description                 = (sdr["description"].ToString()),
                            time_in                     = (sdr["time_in"].ToString()),
                            time_out                    = (sdr["time_out"].ToString()),
                            is_flexi                    = Convert.ToBoolean(sdr["is_flexi"].ToString()),
                            shift_code_type             = Convert.ToInt32(sdr["shift_code_type"].ToString()),
                            time_out_days_cover = Convert.ToInt32(sdr["time_out_days_cover"].ToString()),
                            is_rd_mon = Convert.ToBoolean(sdr["is_rd_mon"].ToString()),
                            is_rd_tue = Convert.ToBoolean(sdr["is_rd_tue"].ToString()),
                            is_rd_wed = Convert.ToBoolean(sdr["is_rd_wed"].ToString()),
                            is_rd_thu = Convert.ToBoolean(sdr["is_rd_thu"].ToString()),
                            is_rd_fri = Convert.ToBoolean(sdr["is_rd_fri"].ToString()),
                            is_rd_sat = Convert.ToBoolean(sdr["is_rd_sat"].ToString()),
                            is_rd_sun = Convert.ToBoolean(sdr["is_rd_sun"].ToString()),
                            half_day_in = (sdr["half_day_in"].ToString()),
                            half_day_in_days_cover = Convert.ToInt32(sdr["half_day_in_days_cover"].ToString()),
                            half_day_out = (sdr["half_day_out"].ToString()),
                            half_day_out_days_cover = Convert.ToInt32(sdr["half_day_out_days_cover"].ToString()),
                            night_dif_in = (sdr["night_dif_in"].ToString()),
                            night_dif_in_days_cover = Convert.ToInt32(sdr["night_dif_in_days_cover"].ToString()),
                            night_dif_out = (sdr["night_dif_out"].ToString()),
                            night_dif_out_days_cover = Convert.ToInt32(sdr["night_dif_out_days_cover"].ToString()),
                            first_break_in = (sdr["first_break_in"].ToString()),
                            first_break_in_days_cover = Convert.ToInt32(sdr["first_break_in_days_cover"].ToString()),
                            first_break_out = (sdr["first_break_out"].ToString()),
                            first_break_out_days_cover = Convert.ToInt32(sdr["first_break_out_days_cover"].ToString()),
                            second_break_in = (sdr["second_break_in"].ToString()),
                            second_break_in_days_cover = Convert.ToInt32(sdr["second_break_in_days_cover"].ToString()),
                            second_break_out = (sdr["second_break_out"].ToString()),
                            second_break_out_days_cover = Convert.ToInt32(sdr["second_break_out_days_cover"].ToString()),
                            third_break_in = (sdr["third_break_in"].ToString()),
                            third_break_in_days_cover = Convert.ToInt32(sdr["third_break_in_days_cover"].ToString()),
                            third_break_out = (sdr["third_break_out"].ToString()),
                            third_break_out_days_cover = Convert.ToInt32(sdr["third_break_out_days_cover"].ToString()),
                            //created_by = Convert.ToInt32(sdr["created_by"].ToString()),
                            //date_created = (sdr["date_created"].ToString()),
                            //active = Convert.ToBoolean(sdr["active"].ToString()),
                            //created_by_name = sdr["created_by_name"].ToString(),
                            //status = sdr["status"].ToString(),






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



        public List<EmployeeLeaveResponse> employee_leave_view(
            string series_code, string leave_type_id, string leave_type_code, string leave_name, string gender_to_use, string total_leaves, int tag_type, string id,
             string created_by)
        {
            List<EmployeeLeaveResponse> resp = new List<EmployeeLeaveResponse>();
            created_by = Crypto.url_decrypt(created_by);
            leave_type_id = leave_type_id == "0" ? "0" : Crypto.url_decrypt(leave_type_id);
            id = id == "0" ? "0" : Crypto.url_decrypt(id);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + Crypto.url_decrypt(series_code) + "_userdb;User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
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
                oCmd.CommandText = "employee_leave_view";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@leave_type_id", leave_type_id);
                oCmd.Parameters.AddWithValue("@leave_type_code", leave_type_code);
                oCmd.Parameters.AddWithValue("@leave_name", leave_name);
                oCmd.Parameters.AddWithValue("@total_leaves", total_leaves);
                oCmd.Parameters.AddWithValue("@tag_type", tag_type);
                oCmd.Parameters.AddWithValue("@id", id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                oCmd.Parameters.AddWithValue("@gender_to_use", gender_to_use);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new EmployeeLeaveResponse()
                        {


                            employee_code = (sdr["employee_code"].ToString()),
                            display_name = (sdr["display_name"].ToString()),
                            //created_by = Convert.ToInt32(sdr["created_by"].ToString()),

                            leave_type_id           = Convert.ToInt32(sdr["leave_type_id"].ToString()),
                            encrypt_leave_type_id   = Crypto.url_encrypt(sdr["leave_type_id"].ToString()),
                            employee_id             = Convert.ToInt32(sdr["employee_id"].ToString()),
                            leave_type_code         = (sdr["leave_type_code"].ToString()),
                            leave_name              = (sdr["leave_name"].ToString()),
                            total_leaves            = (sdr["total_leaves"].ToString()),




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



        public int employee_in(EmployeeInRequest model)
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
                oCmd.CommandText = "employee_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
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

        public int employee_information_temp_in(EmployeeInRequest model)
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
                oCmd.CommandText = "employee_information_temp_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
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



        public List<EmployeeRecurringResponse> employee_recurring_view(string series_code, int adjustment_type_id, int adjustment_id, int timing_id, decimal amount, int tag_type,string id, string created_by)
        {
            List<EmployeeRecurringResponse> resp = new List<EmployeeRecurringResponse>();
            created_by = Crypto.url_decrypt(created_by);
            id = Crypto.url_decrypt(id);

            string _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + Crypto.url_decrypt(series_code) + "_userdb;User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
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
                oCmd.CommandText = "employee_recurring_view";
                da.SelectCommand = oCmd;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@adjustment_type_id", adjustment_type_id);
                oCmd.Parameters.AddWithValue("@adjustment_id", adjustment_id);
                oCmd.Parameters.AddWithValue("@timing_id", timing_id);
                oCmd.Parameters.AddWithValue("@amount", amount);
                oCmd.Parameters.AddWithValue("@tag_type", tag_type);
                oCmd.Parameters.AddWithValue("@id", id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow sdr in dt.Rows
                        select new EmployeeRecurringResponse()
                        {
                            employee_id            = Convert.ToInt32(sdr["employee_id"].ToString()),
                            employee_code          = (sdr["employee_code"].ToString()),
                            display_name           = (sdr["display_name"].ToString()),
                            amount                 = Convert.ToDecimal(sdr["amount"].ToString()),
                            timing_id              = Convert.ToInt32(sdr["timing_id"].ToString()),
                            adjustment_type_id     = Convert.ToInt32(sdr["adjustment_type_id"].ToString()),
                            adjustment_id          = Convert.ToInt32(sdr["adjustment_id"].ToString()),
                            timing                 = (sdr["timing"].ToString()),
                            taxable_id             = Convert.ToBoolean(sdr["taxable_id"].ToString()),
                            taxable                = (sdr["taxable"].ToString()),
                            minimum_hour           = Convert.ToDecimal(sdr["minimum_hour"].ToString()),
                            maximum_hour           = Convert.ToDecimal(sdr["maximum_hour"].ToString()),


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


        public List<PayrollContributionResponse> employee_contribution_view(string series_code, int government_type_id,int timing_id, decimal amount,int tag_type,string id, string created_by)
        {

            id = Crypto.url_decrypt(id);
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
                oCmd.CommandText = "employee_contribution_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@government_type_id", government_type_id);
                oCmd.Parameters.AddWithValue("@timing_id", timing_id);
                oCmd.Parameters.AddWithValue("@amount", amount);
                oCmd.Parameters.AddWithValue("@tag_type", tag_type);
                oCmd.Parameters.AddWithValue("@id", id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollContributionResponse()
                        {

                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            government_type_id = Convert.ToInt32(dr["government_type_id"].ToString()),
                            government_type = (dr["government_type"].ToString()),
                            timing_id = Convert.ToInt32(dr["timing_id"].ToString()),
                            timing = (dr["timing"].ToString()),
                            amount = Convert.ToDecimal(dr["amount"].ToString()),
                            taxable_id = Convert.ToBoolean(dr["taxable_id"].ToString()),
                            taxable = (dr["taxable"].ToString()),


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


        public List<PayrollAdjustmentResponse> employee_adjustment_view(string series_code,int adjustment_type_id, string adjustment_name,decimal amount,bool taxable,int tag_type,string id, string created_by)
        {

            id = id == "0" ? "0" : Crypto.url_decrypt(id);
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
                oCmd.CommandText = "employee_adjustment_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@adjustment_type_id", adjustment_type_id);
                oCmd.Parameters.AddWithValue("@adjustment_name", adjustment_name);
                oCmd.Parameters.AddWithValue("@amount", amount);
                oCmd.Parameters.AddWithValue("@taxable", taxable);
                oCmd.Parameters.AddWithValue("@tag_type", tag_type);
                oCmd.Parameters.AddWithValue("@id", id);
                oCmd.Parameters.AddWithValue("@created_by", created_by);
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new PayrollAdjustmentResponse()
                        {
                            employee_id = Convert.ToInt32(dr["employee_id"].ToString()),
                            display_name = (dr["display_name"].ToString()),
                            employee_code = (dr["employee_code"].ToString()),
                            adjustment_type_id = Convert.ToInt32(dr["adjustment_type_id"].ToString()),
                            adjustment_name = (dr["adjustment_name"].ToString()),
                            adjustment_type = (dr["adjustment_type"].ToString()),
                            amount = Convert.ToDecimal(dr["amount"].ToString()),
                            taxable_id = Convert.ToBoolean(dr["taxable_id"].ToString()),
                            taxable = (dr["taxable"].ToString()),


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

        

        private string generateJwtToken(AuthenticateResponse user)
        {
            // generate token that is valid for 1 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public AuthenticateResponse GetById(int userId)
        {
            throw new NotImplementedException();
        }


    }
}
