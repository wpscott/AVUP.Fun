using AVUP.Fun.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AVUP.Fun.Controllers
{
    [EnableCors]
    [Route("[controller]")]
    [ApiController]
    public class StatisticsController : Controller
    {
        private readonly ILogger<StatisticsController> logger;
        private readonly StatisticsService service;

        public StatisticsController(ILogger<StatisticsController> logger, StatisticsService service)
        {
            this.logger = logger;
            this.service = service;
        }

        [HttpGet("live")]
        [HttpGet("live/{range}")]
        public ActionResult GetLiveRank(string range)
        {
            logger.LogInformation("Get live rank: {Range}", range);
            return Json(service.GetLiveRank(range));
        }

        [HttpGet("comment")]
        [HttpGet("comment/{range}")]
        public ActionResult GetCommentRank(string range)
        {
            logger.LogInformation("Get comment rank: {Range}", range);
            return Json(service.GetCommentRank(range));
        }
    }
}
