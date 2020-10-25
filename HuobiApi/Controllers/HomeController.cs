using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HuobiApi.Controllers {
    [Controller]
    public class HomeController : Controller {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("")]
        public void Home() {
            Response.Redirect("/swagger/index.html");
        }
    }
}