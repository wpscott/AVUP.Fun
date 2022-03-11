using AVUP.Fun.Models;
using AVUP.Fun.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AVUP.Fun.Controllers
{
    [EnableCors]
    [Route("[controller]")]
    [ApiController]
    public class UperController : Controller
    {
        private readonly ILogger<UperController> logger;
        private readonly UperService service;

        public UperController(ILogger<UperController> logger, UperService service)
        {
            this.logger = logger;
            this.service = service;
        }

        [HttpGet]
        public ActionResult GetUper([FromQuery] Query query)
        {
            logger.LogInformation("Get latest uper");
            return Json(service.GetUper(query));
        }

        [HttpGet("{user:long}")]
        public ActionResult GetUperLive(long user, [FromQuery] Query query)
        {
            logger.LogInformation("Get uper live list");
            return Json(service.GetUperLive(user, query));
        }
    }
}
