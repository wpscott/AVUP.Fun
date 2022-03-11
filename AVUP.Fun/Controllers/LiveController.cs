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
    public class LiveController : Controller
    {
        private readonly ILogger<LiveController> logger;
        private readonly UperService uperService;
        private readonly LiveService liveService;

        public LiveController(ILogger<LiveController> logger, UperService userService, LiveService liveService)
        {
            this.logger = logger;
            this.uperService = userService;
            this.liveService = liveService;
        }

        [HttpGet("{user:long}")]
        public ActionResult GetUserLive(long user, [FromQuery] Query query)
        {
            return Json(uperService.GetUperLive(user, query));
        }

        [HttpGet("{user:long}/{live}")]
        public ActionResult GetAll(long user, string live, [FromQuery] Query query)
        {
            logger.LogInformation("Get all {{ user: {UserId}, live: {LiveId} }}", user, live);
            return Json(liveService.GetAllData(user, live, query));
        }

        [HttpGet("{user:long}/{live}/data")]
        public ActionResult GetLiveData(long user, string live)
        {
            logger.LogInformation("Get live data {{ user: {UserId}, live: {LiveId} }}", user, live);
            return Json(liveService.GetLiveCommentData(user, live));
        }

        [HttpGet("{user:long}/{live}/{type}")]
        public ActionResult GetAllByType(long user, string live, string type, [FromQuery] Query query)
        {
            logger.LogInformation("Get all with type {{ user:{UserId}, live: {LiveId}, type: {Type} }}", user, live, type);
            return Json(liveService.GetAllDataByType(user, live, type, query));
        }
    }
}
