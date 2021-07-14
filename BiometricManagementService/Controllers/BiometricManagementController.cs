using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BiometricManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BiometricManagementController : ControllerBase
    {
        //private IPayrollManagementServices _IPayrollManagementServices;

        private EmailSender email;
        private Default_Url url;
    }
}
