using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantMasterSetupService.Helper;
using TenantMasterSetupService.Model;
using TenantMasterSetupService.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TenantMasterSetupService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantMasterSetup : ControllerBase
    {
        private ITenantMasterSetupServices _TenantMasterSetupManagement;

        private EmailSender email;
        private Default_Url url;

        public TenantMasterSetup(ITenantMasterSetupServices TenantMasterSetupManagement, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _TenantMasterSetupManagement = TenantMasterSetupManagement;

            email = appSettings.Value;
            url = settings.Value;
        }


        [HttpGet("modules_view")]
        public List<ModuleResponse> modules_view(string series_code,string created_by)
        {

            var resp = _TenantMasterSetupManagement.modules_view(series_code,created_by);

            return resp;
        }


        [HttpGet("payroll_rates_view")]
        public List<PayrollRates> payroll_rates_view()
        {

            var resp = _TenantMasterSetupManagement.payroll_rates_view();

            return resp;
        }



        [HttpGet("sss_table_view")]
        public List<sssResponse> sss_table_view()
        {

            var resp = _TenantMasterSetupManagement.sss_table_view();

            return resp;
        }


        [HttpGet("philhealth_table_view")]
        public List<philhealthResponse> philhealth_table_view()
        {

            var resp = _TenantMasterSetupManagement.philhealth_table_view();

            return resp;
        }


        [HttpGet("pagibig_table_view")]
        public List<PagibigResponse> pagibig_table_view()
        {

            var resp = _TenantMasterSetupManagement.pagibig_table_view();

            return resp;
        }


        [HttpGet("tax_table_view")]
        public List<taxResponse> tax_table_view(int payroll_type_id)
        {

            var resp = _TenantMasterSetupManagement.tax_table_view(payroll_type_id);

            return resp;
        }

        [HttpGet("modules_approval_view")]
        public List<ModuleResponse> modules_approval_view()
        {

            var resp = _TenantMasterSetupManagement.modules_approval_view();

            return resp;
        }


        [HttpGet("dropdown_view_entitlement")]
        public List<DropdownEntitlementResponse> dropdown_view_entitlement(string dropdowntype_id)
        {
            //dropdowntype_id = dropdowntype_id == "null" ? "0" : dropdowntype_id;
            //dropdown_type = dropdown_type == "null" ? "" : dropdown_type;

            var result = _TenantMasterSetupManagement.dropdown_view_entitlement(dropdowntype_id);
            return result;
        }


        [HttpGet("dropdown_type_view")]
        public List<DropdownTypeResponse> dropdown_type_view()
        {

            var result = _TenantMasterSetupManagement.dropdown_type_view();
            return result;
        }


        [HttpGet("dropdown_type_view_fix")]
        public List<DropdownTypeResponse> dropdown_type_view_fix(string active)
        {

            var result = _TenantMasterSetupManagement.dropdown_type_view_fix(active);
            return result;
        }


        [HttpGet("dropdown_fix_view")]
        public List<DropdownResponse> dropdown_fix_view(string dropdowntype_id)
        {

            var result = _TenantMasterSetupManagement.dropdown_fix_view(dropdowntype_id);
            return result;
        }


        [HttpPost("dropdown_fix_in_up")]
        public DropdownIUResponse dropdown_fix_in_up(DropdownIURequest model)
        {

            var result = _TenantMasterSetupManagement.dropdown_fix_in_up(model);
            return result;
        }

        [HttpGet("module_type_view")]
        public List<ModuleResponse> module_type_view(string module_type)
        {
            var resp = _TenantMasterSetupManagement.module_type_view(module_type);

            return resp;
        }

    }
}
