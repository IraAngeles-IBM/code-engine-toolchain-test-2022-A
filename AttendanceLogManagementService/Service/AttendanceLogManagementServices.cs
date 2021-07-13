
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using AttendanceLogManagementService.Helper;
using AttendanceLogManagementService.Model;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Reflection;

namespace AttendanceLogManagementService.Service
{

    public interface IAttendanceLogManagementServices
    {
        int attendance_log_in_up(AttendanceLogRequest model);
        AuthenticateResponse TokenRequest(AuthenticateRequest model);
        int attendance_log_temp(List<AttendanceLog> model, string series_code);

    }

    public class AttendanceLogManagementServices : IAttendanceLogManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public AttendanceLogManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }


        public int attendance_log_in_up(AttendanceLogRequest model)
        {
            int resp = 0;
            string series_code = Crypto.url_decrypt(model.series_code);
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

                oCmd.CommandText = "attendance_log_in_up_client";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@bio_id", model.bio_id);
                oCmd.Parameters.AddWithValue("@date_time", model.date_time);
                oCmd.Parameters.AddWithValue("@in_out", model.in_out);
                oCmd.Parameters.AddWithValue("@terminal_id", model.terminal_id);
                oCmd.Parameters.AddWithValue("@created_by", 0);


                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {
                    resp = Convert.ToInt32(sdr["created_by"].ToString());

                }
                sdr.Close();
                resp = 1;
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


        public int attendance_log_temp(List<AttendanceLog> model, string series_code)
        {
            int resp = 0;
            series_code = Crypto.url_decrypt(series_code);

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

                oCmd.CommandText = "attendance_log_client_del";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.ExecuteNonQuery();

                dt = ToDataTable(model);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(oConn, SqlBulkCopyOptions.Default, oTrans))
                {
                    bulkCopy.DestinationTableName = "attendance_log_client";
                    bulkCopy.WriteToServer(dt);
                }


                oCmd.CommandText = "attendance_log_client_in";
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.ExecuteNonQuery();

                resp = 1;
                oTrans.Commit();
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




        public AuthenticateResponse TokenRequest(AuthenticateRequest model)
        {
            AuthenticateResponse resp = new AuthenticateResponse();

            model.series_code = Crypto.url_decrypt(model.series_code);

            string _con;
            _con = "Data Source=" + connection.instance_name + ";Initial Catalog=" + model.series_code + connection.catalog + ";User ID=" + connection.user_name + ";Password=" + connection.user_hash + ";MultipleActiveResultSets=True;";
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
                oCmd.CommandText = "token_for_client";
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.Parameters.Clear();
                oCmd.Parameters.AddWithValue("@user_name", model.username);
                oCmd.Parameters.AddWithValue("@user_hash", UserHash);
                oCmd.Parameters.AddWithValue("@series_code", model.series_code);
                SqlDataReader sdr = oCmd.ExecuteReader();
                while (sdr.Read())
                {

                    resp.id = sdr["employee_id"].ToString() == "0" ? "0" : Crypto.url_encrypt(sdr["employee_id"].ToString());
                    resp.series_code = sdr["series_code"].ToString() == "0" ? "0" : Crypto.url_encrypt(sdr["series_code"].ToString());

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
        public AuthenticateResponse GetById(int userId)
        {
            throw new NotImplementedException();
        }
    }


}
