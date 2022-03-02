using AVUP.Fun.Process.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AVUP.Fun.Process.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusController : Controller
    {
        private readonly ILogger<StatusController> _logger;

        public StatusController(ILogger<StatusController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok();
        }

        [HttpGet("count")]
        public ActionResult<ulong> GetCount()
        {
            _logger.LogInformation("Processed messages: {Processed}", PendingWorker.Count);
            return Json(new { count = PendingWorker.Count });
        }
    }
}
