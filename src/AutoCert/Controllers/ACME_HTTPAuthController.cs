using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCert.Controllers
{
    [Route(".well-know/acme-challenge")]
    [ApiController]
    public class ACME_HTTPAuthController : ControllerBase
    {
        [Route(".well-know/acme-challenge/{FileName}")]
        public IActionResult Get(string FileName)
        {
            if (System.IO.File.Exists(@$"D:/{HttpContext.Request.Host}/{FileName}"))
            {
                return File(@$"D:/{HttpContext.Request.Host}/{FileName}", "application/octet-stream", FileName);
            }
            return NotFound();
        }
    }
}
