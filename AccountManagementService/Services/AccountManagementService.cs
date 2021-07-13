
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using AccountManagementService.Model;
using AccountManagementService.Helper;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AccountManagementService.Services
{
    public interface IAccountManagementService
    {
        RegistrationResponse Registration(RegistrationRequest model);
        VerificationResponse Verification(VerificationRequest model);
        CompanyAuthenticateResponse CompanyAuthentication(string company_code);
        AuthenticateResponse AuthenticateLogin(AuthenticateRequest model);
        AuthenticateResponse GetById(int userId);
        CompanyResponse CompanyIU(CompanyIURequest model);
        List<CompanyResponse> company_view_sel(string company_id, string created_by);
        CompanyInActive company_in_active(string company_id);

    }
    public class AccountManagementServices : IAccountManagementService
    {
        public RegistrationResponse resp = new RegistrationResponse();

        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        private readonly IWebHostEnvironment _environment;


        public AccountManagementServices(IOptions<AppSetting> appSettings, IWebHostEnvironment IWebHostEnvironment, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

            _environment = IWebHostEnvironment;
        }

        public RegistrationResponse Registration(RegistrationRequest model)
        {
            // validation
            //if (string.IsNullOrWhiteSpace(model.Password))
            //    throw new AppException("Password is required");

            RegistrationResponse resp = new RegistrationResponse();
            string _con = connection._DB_Master;
            DataTable dt = new DataTable();
            string UserHash = Crypto.password_encrypt(model.Password);
            SqlConnection oConn = new SqlConnection(_con);
            SqlTransaction oTrans;
            oConn.Open();
            oTrans = oConn.BeginTransaction();
            SqlCommand oCmd = new SqlCommand();
            oCmd.Connection = oConn;
            oCmd.Transaction = oTrans;
            try
            {
                oCmd.CommandText = "users_in";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@user_name", model.Username);
                oCmd.Parameters.AddWithValue("@user_hash", UserHash);
                oCmd.Parameters.AddWithValue("@email_address", model.email_address);
                //oCmd.Parameters.AddWithValue("@active", model.active);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {
                    resp.id = sdr["user_id"].ToString() == "0" ? "0" : Crypto.url_encrypt(sdr["user_id"].ToString());  
                    resp.description = (sdr["description"].ToString());
                    resp.email_address = sdr["email_address"].ToString();
                }
                sdr.Close();
                oTrans.Commit();
            }
            catch (Exception e)
            {
                resp.id = "0";
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                oConn.Close();
            }

            return resp;
        }

        public VerificationResponse Verification(VerificationRequest model)
        {


            VerificationResponse resp = new VerificationResponse();
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
                oCmd.CommandText = "users_verification";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@user_id", Crypto.url_decrypt(model.id));

                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {
                    resp.id = sdr["user_id"].ToString() == "0" ? "0" : Crypto.url_encrypt(sdr["user_id"].ToString());
                }
                sdr.Close();
                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                resp.id = "0";
            }
            finally
            {
                oConn.Close();
            }

            return resp;
        }

        public AuthenticateResponse AuthenticateLogin(AuthenticateRequest model)
        {
            AuthenticateResponse resp = new AuthenticateResponse();


            string _con = connection._DB_Master;
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
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp.id = sdr["user_id"].ToString() == "0" ? "0" : Crypto.url_encrypt(sdr["user_id"].ToString());
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
                }
                sdr.Close();
                oConn.Close();
            }
            catch (Exception e)
            {
                resp.id = "0";
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


        public CompanyAuthenticateResponse CompanyAuthentication(string company_code)
        {
            CompanyAuthenticateResponse resp = new CompanyAuthenticateResponse();


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
                oCmd.CommandText = "company_authentication";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@company_code", company_code);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp.company_code = sdr["company_code"].ToString();
                    resp.series_code = sdr["series_code"].ToString();
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

        public CompanyResponse CompanyIU(CompanyIURequest model)
        {

            CompanyResponse resp = new CompanyResponse();
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
                var folder = "code\\" + Crypto.url_decrypt(model.createdBy) + "\\5\\code";
                var path = "";
                var signatory_1 = "";
                var signatory_2 = "";
                var signatory_3 = "";
                if (model.companyID != "0")
                {
                    if (model.img == "")
                    {
                        path = model.old_img;
                    }
                    else
                    {
                        path = folder + "\\" + model.img;
                    }


                }
                else
                {
                    if (model.img == "")
                    {
                        path = "Default\\Image\\default.png";
                    }
                    else
                    {
                        path = folder + "\\" + model.img;
                    }
                }


                if (model.signatory_1_path == "")
                {
                    signatory_1 = model.signatory_1_path_old;
                }
                else
                {
                    signatory_1 = folder + "\\" + model.signatory_1_path;
                }


                if (model.signatory_2_path == "")
                {
                    signatory_2 = model.signatory_2_path_old;
                }
                else
                {
                    signatory_2 = folder + "\\" + model.signatory_2_path;
                }



                if (model.signatory_3_path == "")
                {
                    signatory_3 = model.signatory_3_path_old;
                }
                else
                {
                    signatory_3 = folder + "\\" + model.signatory_3_path;
                }


                oCmd.CommandText = "company_in_up";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@company_id", model.companyID == "0" ? 0 : Crypto.url_decrypt(model.companyID));
                oCmd.Parameters.AddWithValue("@company_code", model.companyCode);
                oCmd.Parameters.AddWithValue("@company_name", model.companyName);
                oCmd.Parameters.AddWithValue("@is_email", model.is_email);
                oCmd.Parameters.AddWithValue("@tk_gen_ref", model.tk_gen_ref);
                oCmd.Parameters.AddWithValue("@unit_floor", model.unit);
                oCmd.Parameters.AddWithValue("@building", model.building);
                oCmd.Parameters.AddWithValue("@street", model.street);
                oCmd.Parameters.AddWithValue("@barangay", model.barangay);
                oCmd.Parameters.AddWithValue("@province", model.province);
                oCmd.Parameters.AddWithValue("@city", model.SelectedCity);
                oCmd.Parameters.AddWithValue("@region", model.SelectedRegion);
                oCmd.Parameters.AddWithValue("@country", model.selectedCompanyCountry);
                oCmd.Parameters.AddWithValue("@zip_code", model.zipCode);
                oCmd.Parameters.AddWithValue("@company_logo", path);
                oCmd.Parameters.AddWithValue("@signatory_1", model.signatory_1);
                oCmd.Parameters.AddWithValue("@signatory_1_title", model.signatory_1_title);
                oCmd.Parameters.AddWithValue("@signatory_1_path", signatory_1);
                oCmd.Parameters.AddWithValue("@signatory_1_file_name", model.signatory_1_file_name);
                oCmd.Parameters.AddWithValue("@signatory_2", model.signatory_2);
                oCmd.Parameters.AddWithValue("@signatory_2_title", model.signatory_2_title);
                oCmd.Parameters.AddWithValue("@signatory_2_path", signatory_2);
                oCmd.Parameters.AddWithValue("@signatory_2_file_name", model.signatory_2_file_name);
                oCmd.Parameters.AddWithValue("@signatory_3", model.signatory_3);
                oCmd.Parameters.AddWithValue("@signatory_3_title", model.signatory_3_title);
                oCmd.Parameters.AddWithValue("@signatory_3_path", signatory_3);
                oCmd.Parameters.AddWithValue("@signatory_3_file_name", model.signatory_3_file_name);
                oCmd.Parameters.AddWithValue("@created_by", Crypto.url_decrypt(model.createdBy));
                oCmd.Parameters.AddWithValue("@email_address", model.email_address);
                oCmd.Parameters.AddWithValue("@telephone", model.telephone);
                oCmd.Parameters.AddWithValue("@philhealth", model.philhealth);
                oCmd.Parameters.AddWithValue("@sss", model.sss);
                oCmd.Parameters.AddWithValue("@pagibig", model.pagibig);
                oCmd.Parameters.AddWithValue("@tin", model.tin);
                //oCmd.Parameters.AddWithValue("@active", model.active);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {
                    resp.companyID = Crypto.url_encrypt(sdr["company_id"].ToString());
                    resp.createdBy = Crypto.url_encrypt(sdr["created_by"].ToString());
                    resp.companyCode = sdr["company_code"].ToString();
                    resp.company_series_code = (sdr["series_code"].ToString());
                }
                sdr.Close();
                oTrans.Commit();

                oCmd.CommandText = "create_database";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@series_code", resp.company_series_code);
                sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {
                    resp.company_series_code = (sdr["series_code"].ToString());
                }

                sdr.Close();

                //oCmd.CommandText = "create_database_userdb";
                //oCmd.CommandType = CommandType.StoredProcedure;
                //oCmd.Parameters.Clear();
                //oCmd.Parameters.AddWithValue("@series_code", resp.company_series_code);
                //sdr = oCmd.ExecuteReader();
                //while (sdr.Read())
                //{
                //    resp.company_series_code = (sdr["series_code"].ToString());
                //}

                //sdr.Close();

                //oCmd.CommandText = "create_database_tenantsetupdb";
                //oCmd.CommandType = CommandType.StoredProcedure;
                //oCmd.Parameters.Clear();
                //oCmd.Parameters.AddWithValue("@series_code", resp.company_series_code);
                //sdr = oCmd.ExecuteReader();
                //while (sdr.Read())
                //{
                //    resp.company_series_code = (sdr["series_code"].ToString());
                //}

                //sdr.Close();

                //oCmd.CommandText = "create_database_branchdb";
                //oCmd.CommandType = CommandType.StoredProcedure;
                //oCmd.Parameters.Clear();
                //oCmd.Parameters.AddWithValue("@series_code", resp.company_series_code);
                //sdr = oCmd.ExecuteReader();
                //while (sdr.Read())
                //{
                //    resp.company_series_code = (sdr["series_code"].ToString());
                //}

                //sdr.Close();

                oCmd.CommandText = "create_table";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@series_code", resp.company_series_code);
                sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {
                    resp.company_series_code = (sdr["series_code"].ToString());
                }

                sdr.Close();


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

        public List<CompanyResponse> company_view_sel(string company_id, string created_by)
        {

            company_id = company_id == "0" ? "0" : Crypto.url_decrypt(company_id);

            List<CompanyResponse> resp = new List<CompanyResponse>();
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
                oCmd.CommandText = "company_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@company_id", company_id);
                oCmd.Parameters.AddWithValue("@created_by", Crypto.url_decrypt(created_by));
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new CompanyResponse()
                        {
                            companyID = Crypto.url_encrypt(dr["company_id"].ToString()),
                            series_code = Crypto.url_encrypt(dr["series_code"].ToString()),
                            companyCode = dr["company_code"].ToString(),
                            createdBy = Crypto.url_encrypt(dr["created_by"].ToString()),
                            street = dr["street"].ToString(),
                            companyName = dr["company_name"].ToString(),
                            barangay = dr["barangay"].ToString(),
                            unit = dr["unit_floor"].ToString(),
                            building = dr["building"].ToString(),
                            province = Convert.ToInt32(dr["province"].ToString()),
                            SelectedCity = Convert.ToInt32(dr["city_id"].ToString()),
                            SelectedRegion = Convert.ToInt32(dr["region_id"].ToString()),
                            selectedCompanyCountry = Convert.ToInt32(dr["country_id"].ToString()),
                            zipCode = dr["zip_code"].ToString(),
                            img = dr["company_logo"].ToString(),
                            old_img = dr["company_logo"].ToString(),
                            active = Convert.ToBoolean(dr["active"].ToString()),
                            created_by_name = dr["created_by_name"].ToString(),
                            date_created = dr["date_created"].ToString(),
                            status = dr["status"].ToString(),
                            is_email = Convert.ToBoolean(dr["is_email"].ToString()),
                            tk_gen_ref_id = Convert.ToInt32(dr["tk_gen_ref_id"].ToString()),
                            tk_gen_ref = (dr["tk_gen_ref"].ToString()),
                            signatory_1 = (dr["signatory_1"].ToString()),
                            signatory_1_title = (dr["signatory_1_title"].ToString()),
                            signatory_1_path = (dr["signatory_1_path"].ToString()),
                            signatory_1_file_name = (dr["signatory_1_file_name"].ToString()),
                            signatory_1_path_old = (dr["signatory_1_path"].ToString()),
                            signatory_2 = (dr["signatory_2"].ToString()),
                            signatory_2_title = (dr["signatory_2_title"].ToString()),
                            signatory_2_path = (dr["signatory_2_path"].ToString()),
                            signatory_2_path_old = (dr["signatory_2_path"].ToString()),
                            signatory_2_file_name = (dr["signatory_2_file_name"].ToString()),
                            signatory_3 = (dr["signatory_3"].ToString()),
                            signatory_3_title = (dr["signatory_3_title"].ToString()),
                            signatory_3_path = (dr["signatory_3_path"].ToString()),
                            signatory_3_path_old = (dr["signatory_3_path"].ToString()),
                            signatory_3_file_name = (dr["signatory_3_file_name"].ToString()),
                            email_address = (dr["email_address"].ToString()),
                            telephone = (dr["telephone"].ToString()),
                            philhealth = (dr["philhealth"].ToString()),
                            sss = (dr["sss"].ToString()),
                            pagibig = (dr["pagibig"].ToString()),
                            tin = (dr["tin"].ToString()),
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


        public CompanyInActive company_in_active(string company_id)
        {


            CompanyInActive resp = new CompanyInActive();
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
                oCmd.CommandText = "company_in_active";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@company_id",Crypto.url_decrypt(company_id));

                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {
                    resp.company_id = sdr["company_id"].ToString();
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
