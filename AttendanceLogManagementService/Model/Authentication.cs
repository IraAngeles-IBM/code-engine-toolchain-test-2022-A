using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceLogManagementService.Model
{
    public class CompanyAuthenticateRequest
    {

        public string company_code { get; set; }
    }

    public class CompanyAuthenticateResponse
    {

        public string series_code { get; set; }
        public string company_code { get; set; }
    }

    public class AuthenticateRequest
    {

        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }


        public string series_code { get; set; }
    }

    public class AuthenticateResponse
    {
        public string id { get; set; }

        public string Token { get; set; }
        public string series_code { get; set; }



    }
}
