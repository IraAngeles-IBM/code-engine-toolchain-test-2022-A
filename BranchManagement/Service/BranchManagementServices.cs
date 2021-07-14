using BranchManagementService.Helper;
using BranchManagementService.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BranchManagementService.Service
{


    public interface IBranchManagementServices
    {
        List<BranchResponse> MultipleBranchIU(BranchIURequest[] model);
        BranchResponse BranchIU(BranchIURequest model);
        List<BranchViewResponse> branch_list(string series_code, string created_by);
        List<BranchResponse> branch_view(string company_series_code, string company_id, string branch_id, string created_by);
        List<IPResponse> branch_ip_view(string company_series_code, string company_id, string branch_id, string created_by);
        List<ContactResponse> branch_contact_view(string company_series_code, string company_id, string branch_id, string created_by);
        List<EmailResponse> branch_email_view(string company_series_code, string company_id, string branch_id, string created_by);

    }
    public class BranchManagementServices : IBranchManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;



        public BranchManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }



        public List<BranchResponse> MultipleBranchIU(BranchIURequest[] model)
        {
            List<BranchResponse> resp = new List<BranchResponse>();

            string company_id = Crypto.url_decrypt(model[0].company_id);
            //BranchResponse resp = new BranchResponse();
            string _con;
                _con = "Data Source=" + connection.instance_name + ";Initial Catalog="+ model[0].company_series_code + connection.catalog +";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";

            int branch = 0;
            int x = 0;
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
                if (model != null)
                {
                    foreach (var item in model)
                    {

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = oCmd;
                        oCmd.CommandText = "branch_in_up";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear(); 
                        oCmd.Parameters.AddWithValue("@branch_id", item.branchID == "0"? "0": Crypto.url_decrypt(item.branchID));
                        oCmd.Parameters.AddWithValue("@branch_code", item.branch_series_code);
                        oCmd.Parameters.AddWithValue("@bank_account", item.bankAccount);
                        oCmd.Parameters.AddWithValue("@barangay", item.barangay);
                        oCmd.Parameters.AddWithValue("@branch_name", item.branchName);
                        oCmd.Parameters.AddWithValue("@building", item.building);
                        oCmd.Parameters.AddWithValue("@province", item.province);
                        oCmd.Parameters.AddWithValue("@pagibig", item.pagibig);
                        oCmd.Parameters.AddWithValue("@philhealth", item.philhealth);
                        oCmd.Parameters.AddWithValue("@bank_id", item.SelectedBank);
                        oCmd.Parameters.AddWithValue("@country", item.SelectedBranchCountry);
                        oCmd.Parameters.AddWithValue("@city", item.SelectedCity);
                        oCmd.Parameters.AddWithValue("@industry", item.SelectedIndustry);
                        oCmd.Parameters.AddWithValue("@pagibig_branch", item.SelectedPCity);
                        oCmd.Parameters.AddWithValue("@pagibig_code", item.SelectedPCode);
                        oCmd.Parameters.AddWithValue("@pagibig_region", item.SelectedPRegion);
                        oCmd.Parameters.AddWithValue("@rdo", item.SelectedRdoOffice);
                        oCmd.Parameters.AddWithValue("@rdo_branch", item.SelectedRdoBranch);
                        oCmd.Parameters.AddWithValue("@region", item.SelectedRegion);
                        oCmd.Parameters.AddWithValue("@sss", item.sss);
                        oCmd.Parameters.AddWithValue("@street", item.street);
                        oCmd.Parameters.AddWithValue("@tin", item.tin);
                        oCmd.Parameters.AddWithValue("@unit_floor", item.unit);
                        oCmd.Parameters.AddWithValue("@zip_code", item.zipCode);
                        oCmd.Parameters.AddWithValue("@company_id", company_id);
                        oCmd.Parameters.AddWithValue("@created_by", Crypto.url_decrypt(item.CreatedBy));
                        //oCmd.ExecuteNonQuery();
                        //SqlDataReader sdr = oCmd.ExecuteReader();
                        //while (sdr.Read())
                        //{
                            
                        //    //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                        //    branch = Convert.ToInt32(sdr["branch_id"].ToString());
                        //    x = x + 1;
                        //}

                        da.Fill(dt);
                        resp = (from DataRow dr in dt.Rows
                                select new BranchResponse()
                                {
                                    branch_id = Crypto.url_encrypt(dr["branch_id"].ToString()),

                                }).ToList();

                        branch = Convert.ToInt32(Crypto.url_decrypt(resp[0].branch_id));
                        if (item.iP_IU != null)
                        {
                            foreach (var ip in item.iP_IU)
                            {
                                oCmd.CommandText = "branch_ip_in";
                                oCmd.CommandType = CommandType.StoredProcedure;
                                oCmd.Parameters.Clear();
                                oCmd.Parameters.AddWithValue("@branch_id", branch);
                                oCmd.Parameters.AddWithValue("@ip_address", ip.description);
                                oCmd.Parameters.AddWithValue("@created_by", Crypto.url_decrypt(item.CreatedBy));
                                oCmd.ExecuteNonQuery();
                            }
                        }


                        if (item.Contact_IU != null)
                        {
                            foreach (var contact in item.Contact_IU)
                            {
                                oCmd.CommandText = "branch_contact_in";
                                oCmd.CommandType = CommandType.StoredProcedure;
                                oCmd.Parameters.Clear();
                                oCmd.Parameters.AddWithValue("@branch_id", branch);
                                oCmd.Parameters.AddWithValue("@contact_type_id", contact.id);
                                oCmd.Parameters.AddWithValue("@contact_number", contact.number);
                                oCmd.Parameters.AddWithValue("@created_by", Crypto.url_decrypt(item.CreatedBy));
                                oCmd.ExecuteNonQuery();
                            }
                        }


                        if (item.Email_IU != null)
                        {
                            foreach (var email in item.Email_IU)
                            {
                                oCmd.CommandText = "branch_email_in";
                                oCmd.CommandType = CommandType.StoredProcedure;
                                oCmd.Parameters.Clear();
                                oCmd.Parameters.AddWithValue("@branch_id", branch);
                                oCmd.Parameters.AddWithValue("@email_type_id", email.id);
                                oCmd.Parameters.AddWithValue("@email_address", email.email_address);
                                oCmd.Parameters.AddWithValue("@created_by", Crypto.url_decrypt(item.CreatedBy));
                                oCmd.ExecuteNonQuery();
                            }
                        }

                    }
                }

                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                resp = new List<BranchResponse>();
                oTrans.Rollback();
            }
            finally
            {
                oConn.Close();
            }



            return resp;
        }


        public BranchResponse BranchIU(BranchIURequest model)
        {
            BranchResponse resp = new BranchResponse();

            string company_id = Crypto.url_decrypt(model.company_id);
            string company_series_code = Crypto.url_decrypt(model.company_series_code);
            string created_by = Crypto.url_decrypt(model.CreatedBy);
            //BranchResponse resp = new BranchResponse();
            string _con;
            _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + company_series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";

            int branch = 0;
            int x = 0;
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
                if (model != null)
                {
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = oCmd;
                        oCmd.CommandText = "branch_in_up";
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddWithValue("@branch_id", model.branchID == "0" ? "0" : Crypto.url_decrypt(model.branchID));
                        oCmd.Parameters.AddWithValue("@branch_code", model.branch_series_code);
                        oCmd.Parameters.AddWithValue("@bank_account", model.bankAccount);
                        oCmd.Parameters.AddWithValue("@barangay", model.barangay);
                        oCmd.Parameters.AddWithValue("@branch_name", model.branchName);
                        oCmd.Parameters.AddWithValue("@building", model.building);
                        oCmd.Parameters.AddWithValue("@province", model.province);
                        oCmd.Parameters.AddWithValue("@pagibig", model.pagibig);
                        oCmd.Parameters.AddWithValue("@philhealth", model.philhealth);
                        oCmd.Parameters.AddWithValue("@bank_id", model.SelectedBank);
                        oCmd.Parameters.AddWithValue("@country", model.SelectedBranchCountry);
                        oCmd.Parameters.AddWithValue("@city", model.SelectedCity);
                        oCmd.Parameters.AddWithValue("@industry", model.SelectedIndustry);
                        oCmd.Parameters.AddWithValue("@pagibig_branch", model.SelectedPCity);
                        oCmd.Parameters.AddWithValue("@pagibig_code", model.SelectedPCode);
                        oCmd.Parameters.AddWithValue("@pagibig_region", model.SelectedPRegion);
                        oCmd.Parameters.AddWithValue("@rdo", model.SelectedRdoOffice);
                        oCmd.Parameters.AddWithValue("@rdo_branch", model.SelectedRdoBranch);
                        oCmd.Parameters.AddWithValue("@region", model.SelectedRegion);
                        oCmd.Parameters.AddWithValue("@sss", model.sss);
                        oCmd.Parameters.AddWithValue("@street", model.street);
                        oCmd.Parameters.AddWithValue("@tin", model.tin);
                        oCmd.Parameters.AddWithValue("@unit_floor", model.unit);
                        oCmd.Parameters.AddWithValue("@zip_code", model.zipCode);
                        oCmd.Parameters.AddWithValue("@company_id", company_id);
                        oCmd.Parameters.AddWithValue("@created_by", created_by);
                        //oCmd.ExecuteNonQuery();
                        SqlDataReader sdr = oCmd.ExecuteReader();
                        while (sdr.Read())
                        {

                            //resp[0].branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                            branch = Convert.ToInt32(sdr["branch_id"].ToString());

                        resp.branch_id = Crypto.url_encrypt(sdr["branch_id"].ToString());
                        }
                    sdr.Close();
                    //da.Fill(dt);
                    //    resp = (from DataRow dr in dt.Rows
                    //            select new BranchResponse()
                    //            {
                    //                branch_id = Crypto.url_encrypt(dr["branch_id"].ToString()),

                    //            }).ToList();

                        //branch = Convert.ToInt32(Crypto.url_decrypt(resp.branch_id));
                        if (model.iP_IU != null)
                        {
                            foreach (var ip in model.iP_IU)
                            {
                                oCmd.CommandText = "branch_ip_in";
                                oCmd.CommandType = CommandType.StoredProcedure;
                                oCmd.Parameters.Clear();
                                oCmd.Parameters.AddWithValue("@branch_id", branch);
                                oCmd.Parameters.AddWithValue("@ip_address", ip.description);
                                oCmd.Parameters.AddWithValue("@created_by", created_by);
                                oCmd.ExecuteNonQuery();
                            }
                        }


                        if (model.Contact_IU != null)
                        {
                            foreach (var contact in model.Contact_IU)
                            {
                                oCmd.CommandText = "branch_contact_in";
                                oCmd.CommandType = CommandType.StoredProcedure;
                                oCmd.Parameters.Clear();
                                oCmd.Parameters.AddWithValue("@branch_id", branch);
                                oCmd.Parameters.AddWithValue("@contact_type_id", contact.id);
                                oCmd.Parameters.AddWithValue("@contact_number", contact.number);
                                oCmd.Parameters.AddWithValue("@created_by", created_by);
                                oCmd.ExecuteNonQuery();
                            }
                        }


                        if (model.Email_IU != null)
                        {
                            foreach (var email in model.Email_IU)
                            {
                                oCmd.CommandText = "branch_email_in";
                                oCmd.CommandType = CommandType.StoredProcedure;
                                oCmd.Parameters.Clear();
                                oCmd.Parameters.AddWithValue("@branch_id", branch);
                                oCmd.Parameters.AddWithValue("@email_type_id", email.id);
                                oCmd.Parameters.AddWithValue("@email_address", email.email_address);
                                oCmd.Parameters.AddWithValue("@created_by", created_by);
                                oCmd.ExecuteNonQuery();
                            }
                        }

                    
                }
                

                oTrans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                resp = new BranchResponse();
                oTrans.Rollback();
            }
            finally
            {
                oConn.Close();
            }



            return resp;
        }


        public List<BranchResponse> branch_view(string company_series_code, string company_id, string branch_id, string created_by)
        {

            company_id = Crypto.url_decrypt(company_id);
            company_series_code = Crypto.url_decrypt(company_series_code);
            //BranchResponse resp = new BranchResponse();
            string _con;
            _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + company_series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<BranchResponse> resp = new List<BranchResponse>();
            DataTable dt = new DataTable();
            DataTable dt_ip = new DataTable();
            DataTable dt_contact = new DataTable();
            DataTable dt_email = new DataTable();
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


                oCmd.CommandText = "branch_view_sel";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                //oCmd.Parameters.AddWithValue("@company_id", company_id);
                oCmd.Parameters.AddWithValue("@branch_id", branch_id == "0" ? 0 : Crypto.url_decrypt(branch_id));
                //oCmd.Parameters.AddWithValue("@created_by", Crypto.url_decrypt(created_by));
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new BranchResponse()
                        {
                            branch_id = Crypto.url_encrypt(dr["branch_id"].ToString()),
                            encrypted_branch_id = Crypto.url_encrypt(dr["branch_id"].ToString()),
                            CreatedBy = Crypto.url_encrypt(dr["created_by"].ToString()),
                            branch_code = dr["branch_code"].ToString(),
                            branchName = dr["branch_name"].ToString(),
                            unit = dr["unit_floor"].ToString(),
                            building = dr["building"].ToString(),
                            street = dr["street"].ToString(),
                            barangay = dr["barangay"].ToString(),
                            province = Convert.ToInt32(dr["province"].ToString()),
                            SelectedCity = Convert.ToInt32(dr["city_id"].ToString()),
                            SelectedRegion = Convert.ToInt32(dr["region_id"].ToString()),
                            SelectedBranchCountry = Convert.ToInt32(dr["country_id"].ToString()),
                            zipCode = dr["zip_code"].ToString(),
                            sss = dr["sss"].ToString(),
                            philhealth = dr["philhealth"].ToString(),
                            pagibig = dr["pagibig"].ToString(),
                            tin = dr["tin"].ToString(),

                            SelectedRdoBranch = Convert.ToInt32(dr["rdo_branch_id"].ToString()),
                            SelectedRdoOffice = Convert.ToInt32(dr["rdo_id"].ToString()),
                            SelectedPCity = Convert.ToInt32(dr["pagibig_branch_id"].ToString()),
                            SelectedPCode = Convert.ToInt32(dr["pagibig_code"].ToString()),
                            SelectedPRegion = Convert.ToInt32(dr["pagibig_region_id"].ToString()),
                            SelectedIndustry = Convert.ToInt32(dr["industry_id"].ToString()),
                            SelectedBank = Convert.ToInt32(dr["bank_id"].ToString()),
                            bankAccount = dr["bank_account"].ToString(),
                            company_id = Crypto.url_encrypt(dr["company_id"].ToString()),

                            //instance_name = Crypto.url_encrypt(dr["instance_name"].ToString()),
                            //username = Crypto.url_encrypt(dr["user_name"].ToString()),
                            //password = Crypto.url_encrypt(dr["user_hash"].ToString()),
                            active = Convert.ToBoolean(dr["active"].ToString()),

                            //iP_IU = ipresp

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

        public List<BranchViewResponse> branch_list(string series_code,string created_by)
        {

            series_code = Crypto.url_decrypt(series_code);
            //BranchResponse resp = new BranchResponse();
            string _con;
            _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<BranchViewResponse> resp = new List<BranchViewResponse>();
            DataTable dt = new DataTable();
            DataTable dt_ip = new DataTable();
            DataTable dt_contact = new DataTable();
            DataTable dt_email = new DataTable();
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


                oCmd.CommandText = "branch_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new BranchViewResponse()
                        {
                            branch_id = Convert.ToInt32(dr["branch_id"].ToString()),
                            encrypted_branch_id = Crypto.url_encrypt(dr["branch_id"].ToString()),
                            branch_name = (dr["branch_name"].ToString()),



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


        public List<IPResponse> branch_ip_view(string company_series_code, string company_id, string branch_id, string created_by)
        {

            company_id = Crypto.url_decrypt(company_id);
            company_series_code = Crypto.url_decrypt(company_series_code);
            //BranchResponse resp = new BranchResponse();
            string _con;
            _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + company_series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<IPResponse> resp = new List<IPResponse>();
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

                oCmd.CommandText = "branch_ip_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@branch_id", branch_id == "0" ? 0 : Crypto.url_decrypt(branch_id));
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new IPResponse()
                        {
                            branch_id = Crypto.url_encrypt(dr["branch_id"].ToString()),
                            createdBy = Crypto.url_encrypt(dr["created_by"].ToString()),
                            description = dr["ip_address"].ToString(),

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

        public List<ContactResponse> branch_contact_view(string company_series_code, string company_id, string branch_id, string created_by)
        {

            company_id = Crypto.url_decrypt(company_id);
            company_series_code = Crypto.url_decrypt(company_series_code);
            //BranchResponse resp = new BranchResponse();
            string _con;
            _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + company_series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<ContactResponse> resp = new List<ContactResponse>();
            //List<BranchResponse> resp = new List<BranchResponse>();
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

                oCmd.CommandText = "branch_contact_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@branch_id", branch_id == "0" ? 0 : Crypto.url_decrypt(branch_id));
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new ContactResponse()
                        {
                            branch_id = Crypto.url_encrypt(dr["branch_id"].ToString()),
                            createdBy = Crypto.url_encrypt(dr["created_by"].ToString()),
                            id = (dr["contact_type_id"].ToString()),
                            description = (dr["contact_type"].ToString()),
                            number = dr["contact_number"].ToString(),

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


        public List<EmailResponse> branch_email_view(string company_series_code, string company_id, string branch_id, string created_by)
        {

            company_id = Crypto.url_decrypt(company_id);
            company_series_code = Crypto.url_decrypt(company_series_code);
            //BranchResponse resp = new BranchResponse();
            string _con;
            _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + company_series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";


            List<EmailResponse> resp = new List<EmailResponse>();
            //List<BranchResponse> resp = new List<BranchResponse>();
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
                oCmd.CommandText = "branch_email_view";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@branch_id", branch_id == "0" ? 0 : Crypto.url_decrypt(branch_id));
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new EmailResponse()
                        {
                            branch_id = Crypto.url_encrypt(dr["branch_id"].ToString()),
                            createdBy = Crypto.url_encrypt(dr["created_by"].ToString()),
                            id = Convert.ToInt32(dr["email_type_id"].ToString()),
                            description = (dr["email_type"].ToString()),
                            email_address = dr["email_address"].ToString(),

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
