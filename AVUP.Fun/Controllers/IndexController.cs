using Microsoft.AspNetCore.Mvc;

namespace AVUP.Fun.Controllers
{
    [Route("/")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        [HttpGet]
        [HttpPost]
        [HttpPut]
        public ActionResult Index()
        {
            return Ok();
        }
    }
}
