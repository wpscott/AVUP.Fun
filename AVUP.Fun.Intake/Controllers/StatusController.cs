using AcFunDanmu;
using AVUP.Fun.Intake.Models;
using AVUP.Fun.Intake.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AVUP.Fun.Intake.Controllers
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

        [HttpGet("live")]
        public ActionResult<LiveData[]> Get()
        {
            return Json(IntakeWorker.Monitors.Values);
        }

        [HttpGet("gift")]
        public ActionResult GetGiftList()
        {
            return Json(Client.Gifts);
        }

        [HttpGet("gift/{id:long}")]
        public ActionResult GetGift(long id)
        {
            if (Client.Gifts.TryGetValue(id, out var gift)) { return Json(gift); }
            else { return NotFound(); }
        }
    }
}
