using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AVUP.Fun.Controllers
{
    [EnableCors]
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
