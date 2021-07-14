using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Model
{
    public class AuthenticationRequest
    {

        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }


        public string company_code { get; set; }
    }

    public class AuthenticationResponse
    {
        public string id { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string email_address { get; set; }

        public string type { get; set; }

        public string guid { get; set; }

        public string routing { get; set; }

        public bool email_verified { get; set; }

        public bool lock_account { get; set; }

        public string Token { get; set; }

        public string json { get; set; }

        public bool active { get; set; }

        public string company_id { get; set; }

        public string company_code { get; set; }

        public string instance_name { get; set; }

        public string company_user_name { get; set; }

        public string company_user_hash { get; set; }

        public string access_level_id { get; set; }



    }

    public class CompanyAuthenticationResponse
    {
        public string series_code { get; set; }
        public string company_code { get; set; }
    }
}
