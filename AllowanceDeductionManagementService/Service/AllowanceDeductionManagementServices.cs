
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using AllowanceDeductionManagementService.Helper;
using AllowanceDeductionManagementService.Model;

namespace AllowanceDeductionManagementService.Service
{

    public interface IAllowanceDeductionManagementServices
    {
    }

    public class AllowanceDeductionManagementServices : IAllowanceDeductionManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public AllowanceDeductionManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }


        public int leave_type_in_up(LeaveTypeRequest model)
        {
            int resp = model.leave_type_id;
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

                oCmd.Parameters.AddWithValue("leave_type_id", model.leave_type_id);
                oCmd.Parameters.AddWithValue("leave_type_code", model.leave_type_code);
                oCmd.Parameters.AddWithValue("leave_name", model.leave_name);
                oCmd.Parameters.AddWithValue("description", model.description);
                oCmd.Parameters.AddWithValue("gender_to_use", model.gender_to_use);
                oCmd.Parameters.AddWithValue("required_attachment", model.required_attachment);
                oCmd.Parameters.AddWithValue("filed_by", model.filed_by);
                oCmd.Parameters.AddWithValue("leave_start", model.leave_start);
                oCmd.Parameters.AddWithValue("total_leaves", model.total_leaves);
                oCmd.Parameters.AddWithValue("leave_accrued", model.leave_accrued);
                oCmd.Parameters.AddWithValue("accrued_credits", model.accrued_credits);
                oCmd.Parameters.AddWithValue("leave_per_month", model.leave_per_month);
                oCmd.Parameters.AddWithValue("convertible_to_cash", model.convertible_to_cash);
                oCmd.Parameters.AddWithValue("taxable_credits", model.taxable_credits);
                oCmd.Parameters.AddWithValue("non_taxable_credits", model.non_taxable_credits);
                oCmd.Parameters.AddWithValue("priority_to_convert", model.priority_to_convert);

                oCmd.ExecuteNonQuery();
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


        public List<LeaveTypeResponse> leave_type_view_sel(string series_code, int leave_type_id)
        {

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
                da.Fill(dt);
                resp = (from DataRow dr in dt.Rows
                        select new LeaveTypeResponse()
                        {
                            leave_type_id = Convert.ToInt32(dr["leave_type_id"].ToString()),
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
                            priority_to_convert = (dr["priority_to_convert"].ToString()),

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


    }



}
