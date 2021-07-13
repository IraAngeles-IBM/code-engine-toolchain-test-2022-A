using AuthService.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // GET: api/<AuthController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AuthController>
        [HttpPost("AuthenticateLogin")]
        public AuthenticationResponse AuthenticateLogin(AuthenticationRequest model)
        {
            AuthenticationResponse resp = new AuthenticationResponse();
            try
            {
               

                try
                {
                    CompanyAuthenticationResponse records = new CompanyAuthenticationResponse();
                    string HostURI = "https://accountmanagementservices.azurewebsites.net/api/AccountManagement/CompanyAuthentication?company_code=" + model.company_code;
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(HostURI);
                    request.Method = "GET";
                    String responseInString = String.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        responseInString = reader.ReadToEnd();
                        records = JsonConvert.DeserializeObject<CompanyAuthenticationResponse>(responseInString);
                        //resp = access_deduction_in(records);
                        reader.Close();
                        dataStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    string cont = ex.Message;
                }
            }
            catch (Exception e)
            {
               string error = "Error: " + e.Message;
            }





            return resp;


        }

       
    }
}
