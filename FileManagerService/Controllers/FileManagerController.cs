using FileManagerService.Helper;
using FileManagerService.Model;
using FileManagerService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileManagerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {

        private IFileManagerServices _FileManagerServices;
        private readonly IWebHostEnvironment _environment;

        private EmailSender email;
        private Default_Url url;

        public FileManagerController(IFileManagerServices FileManagerServices, IWebHostEnvironment IWebHostEnvironment, IOptions<EmailSender> appSettings, IOptions<Default_Url> settings)
        {

            _environment = IWebHostEnvironment;
            _FileManagerServices = FileManagerServices;

            email = appSettings.Value;
            url = settings.Value;
        }

        [HttpPost("UploadAttachment")]
        public ActionResult UploadAttachment(IFormFile formfile, string series_code, string created_by,string code, int module_id, int transaction_id)
        {
            int ret = 0;
            var resp = "";
            var file = formfile;

            series_code = series_code == "code" ? "code" : Crypto.url_decrypt(series_code);
            created_by = created_by == "code" ? "code" : Crypto.url_decrypt(created_by);
            if (file == null)
            {
                return Ok(new { response = 1 });

            }
            if (file.Length > 0)
            {

                foreach (var f in Request.Form.Files)
                {
                    FileRequest fileRequest = new FileRequest();

                    var folder = series_code + "\\" + (created_by) + "\\"+ module_id + "\\" + code; ;

                    //if (code != null)
                    //{
                    //    folder = folder + "\\" + code;
                    //}

                    var fileName = f.FileName;
                    var directory = Path.Combine(_environment.WebRootPath, folder);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    var fullPath = Path.Combine(_environment.WebRootPath, folder) + $@"\{fileName}";

                    using (FileStream fs = System.IO.File.Create(fullPath))
                    {
                        f.CopyTo(fs);
                        fs.Flush();
                    }
                    resp = folder + $@"\{fileName}";
                    ret = 1;
                    fileRequest.series_code = series_code;
                    fileRequest.created_by = created_by;
                    fileRequest.module_id = module_id;
                    fileRequest.file_name = fileName;
                    fileRequest.file_path = resp;
                    fileRequest.transaction_id = transaction_id;
                    fileRequest.file_type = System.IO.Path.GetExtension(f.FileName); ;
                    var a = _FileManagerServices.file_attachment_in(fileRequest);


                }

            }

            //return resp;

            return Ok(new { response = ret });
        }


        [HttpPost("file_attachment_update")]
        public int file_attachment_update(FileRequest[] model)
        {
            var resp = 0;
            if(model.Length > 0)
            {

                resp = _FileManagerServices.file_attachment_update(model);
            }
            //return resp;

            return resp;
        }

        [HttpGet("file_attachment_sel")]
        public List<FileResponse> file_attachment_sel(string series_code, string module_id, string transaction_id, int file_type, string created_by)
        {

            var resp = _FileManagerServices.file_attachment_sel(series_code, module_id, transaction_id,  file_type, created_by);

            return resp;
        }
    }
}
