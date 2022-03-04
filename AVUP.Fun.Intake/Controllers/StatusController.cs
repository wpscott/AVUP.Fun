using AcFunDanmu;
using AVUP.Fun.Intake.Models;
using AVUP.Fun.Intake.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

        [HttpGet("gift/{uper:long}/{id:long}")]
        public async Task<ActionResult> GetGift(long uper, long id)
        {
            while (true)
            {
                if (Client.Gifts.TryGetValue(id, out var gift))
                {
                    return Json(gift);
                }
                var client = new Client();
                await client.Initialize(uper, true);
            }
        }
    }
}
