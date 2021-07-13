using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PermissionManagementService.Helper;
using PermissionManagementService.Model;
using PermissionManagementService.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PermissionManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionManagement : ControllerBase
    {


        private IPermissionManagementServices _PermissionManagement;

        private EmailSender email;
        private Default_Url url;

        public PermissionManagement(IPermissionManagementServices PermissionManagement, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _PermissionManagement = PermissionManagement;

            email = appSettings.Value;
            url = settings.Value;
        }

        // GET: api/<PermissionManagementService>
        [HttpGet("dynamic_menu_view")]
        public List<MenuViewResponse> dynamic_menu_view(string access_level_id, string series_code, string created_by)
        {
            var resp = _PermissionManagement.dynamic_menu_view(access_level_id, series_code,created_by);
            return resp;
        }


        // GET: api/<PermissionManagementService>
        [HttpGet("module_access_view")]
        public List<ModuleResponse> module_access_view(string series_code, string access_level_id)
        {
            var resp = _PermissionManagement.module_access_view(series_code, access_level_id);
            return resp;
        }

        // GET: api/<PermissionManagementService>
        [HttpGet("report_access_view")]
        public List<ReportAccessResponse> report_access_view(string series_code, string access_level_id)
        {
            var resp = _PermissionManagement.report_access_view(series_code, access_level_id);
            return resp;
        }

        // GET: api/<PermissionManagementService>
        [HttpGet("report_access_list_view")]
        public List<ReportAccessResponse> report_access_list_view(string series_code, string access_level_id, string created_by)
        {
            var resp = _PermissionManagement.report_access_list_view(series_code, access_level_id, created_by);
            return resp;
        }


        // GET: api/<PermissionManagementService>
        [HttpGet("confidentiality_access_view")]
        public List<ConfidentialityAccessResponse> confidentiality_access_view(string series_code, string access_level_id)
        {
            var resp = _PermissionManagement.confidentiality_access_view(series_code, access_level_id);
            return resp;
        }

        // GET: api/<PermissionManagementService>
        [HttpGet("confidentiality_access_sel")]
        public List<ConfidentialityAccessResponse> confidentiality_access_sel(string series_code, string access_level_id)
        {
            var resp = _PermissionManagement.confidentiality_access_sel(series_code, access_level_id);
            return resp;
        }

        // GET: api/<PermissionManagementService>
        [HttpGet("confidentiality_access_list_view")]
        public List<ConfidentialityAccessResponse> confidentiality_access_list_view(string series_code, string access_level_id, string created_by)
        {
            var resp = _PermissionManagement.confidentiality_access_list_view(series_code, access_level_id, created_by);
            return resp;
        }

        // GET: api/<PermissionManagementService>
        [HttpGet("data_upload_access_list_view")]
        public List<DataUploadAccessResponse> data_upload_access_list_view(string series_code, string access_level_id,string created_by)
        {
            var resp = _PermissionManagement.data_upload_access_list_view(series_code,access_level_id, created_by);
            return resp;
        }

        // GET: api/<PermissionManagementService>
        [HttpGet("data_upload_access_view")]
        public List<DataUploadAccessResponse> data_upload_access_view(string series_code, string access_level_id)
        {
            var resp = _PermissionManagement.data_upload_access_view(series_code, access_level_id);
            return resp;
        }



        // GET: api/<PermissionManagementService>
        [HttpPost("module_access_in")]
        public ModuleResponse module_access_in(ModuleRequest[] model)
        {
            var resp = _PermissionManagement.module_access_in(model);
            return resp;
        }

        // GET: api/<PermissionManagementService>
        [HttpPost("report_access_in")]
        public ReportAccessResponse report_access_in(ReportAccessRequest[] model)
        {
            var resp = _PermissionManagement.report_access_in(model);
            return resp;
        }

        // GET: api/<PermissionManagementService>
        [HttpPost("confidentiality_access_in")]
        public ConfidentialityAccessResponse confidentiality_access_in(ConfidentialityAccessRequest[] model)
        {
            var resp = _PermissionManagement.confidentiality_access_in(model);
            return resp;
        }


        // GET: api/<PermissionManagementService>
        [HttpPost("data_upload_access_in")]
        public DataUploadAccessResponse data_upload_access_in(DataUploadAccessRequest[] model)
        {
            var resp = _PermissionManagement.data_upload_access_in(model);
            return resp;
        }



        // GET: api/<PermissionManagementService>
        [HttpGet("approval_access_view")]
        public List<ApprovalAccessResponse> approval_access_view(string series_code, string access_level_id, string date_from, string date_to, string created_by)
        {
            var resp = _PermissionManagement.approval_access_view(series_code, access_level_id, date_from, date_to, created_by);
            return resp;
        }


    }
}
