using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService
{

    public interface IAuthService
    {
        public string GetById(int userId);
    }

    public class AuthService:IAuthService
    {

        public string GetById(int userId)
        {
            return "";
        }


    }
}
