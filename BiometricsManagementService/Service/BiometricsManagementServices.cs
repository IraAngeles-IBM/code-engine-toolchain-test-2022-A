
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using BiometricsManagementService.Helper;
using BiometricsManagementService.Model;
using System.Data.SqlClient;

namespace BiometricsManagementService.Service
{

    public interface IBiometricsManagementServices
    {
  
    }

    public class BiometricsManagementServices : IBiometricsManagementServices
    {


        private connectionString connection { get; set; }
        private readonly AppSetting _appSettings;

        public BiometricsManagementServices(IOptions<AppSetting> appSettings, IOptions<connectionString> settings)
        {
            _appSettings = appSettings.Value;
            connection = settings.Value;

        }

       
    }



}
