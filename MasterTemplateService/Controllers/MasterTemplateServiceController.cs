using MasterTemplateService.Helper;
using MasterTemplateService.Model;
using MasterTemplateService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MasterTemplateService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterTemplateServiceController : ControllerBase
    {

        private IMasterTemplateServices _MasterTemplateServices;

        private EmailSender email;
        private Default_Url url;

        public MasterTemplateServiceController(IMasterTemplateServices MasterTemplateServices, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _MasterTemplateServices = MasterTemplateServices;

            email = appSettings.Value;
            url = settings.Value;
        }


        [HttpPost("DropdownIU")]
        public DropdownIUResponse DropdownIU(DropdownIURequest model)
        {

            var result = _MasterTemplateServices.DropdownIU(model);
            return result;
        }


        [HttpPost("dropdown_fix_in_up")]
        public DropdownIUResponse dropdown_fix_in_up(DropdownIURequest model)
        {

            var result = _MasterTemplateServices.dropdown_fix_in_up(model);
            return result;
        }


        [HttpGet("dropdown_view")]
        public List<DropdownResponse> dropdown_view(string dropdowntype_id)
        {
            var result = _MasterTemplateServices.dropdown_view(dropdowntype_id);
            return result;
        }



        [HttpGet("dropdown_view_all")]
        public List<DropdownResponse> dropdown_view_all(string dropdowntype_id)
        {
            //dropdowntype_id = dropdowntype_id == "null" ? "0" : dropdowntype_id;
            //dropdown_type = dropdown_type == "null" ? "" : dropdown_type;

            var result = _MasterTemplateServices.dropdown_view_all(dropdowntype_id);
            return result;
        }


        [HttpGet("dropdown_view_entitlement")]
        public List<DropdownResponse> dropdown_view_entitlement(string dropdowntype_id)
        {
            //dropdowntype_id = dropdowntype_id == "null" ? "0" : dropdowntype_id;
            //dropdown_type = dropdown_type == "null" ? "" : dropdown_type;

            var result = _MasterTemplateServices.dropdown_view_entitlement(dropdowntype_id);
            return result;
        }


        [HttpGet("dropdown_type_view")]
        public List<DropdownTypeResponse> dropdown_type_view()
        {

            var result = _MasterTemplateServices.dropdown_type_view();
            return result;
        }


        [HttpGet("dropdown_type_view_fix")]
        public List<DropdownTypeResponse> dropdown_type_view_fix(string active)
        {

            var result = _MasterTemplateServices.dropdown_type_view_fix(active);
            return result;
        }


        [HttpGet("dropdown_fix_view")]
        public List<DropdownResponse> dropdown_fix_view(string active)
        {

            var result = _MasterTemplateServices.dropdown_fix_view(active);
            return result;
        }

    }
}
