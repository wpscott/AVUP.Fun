using AVUP.Fun.Models;
using ClickHouse.Ado;
using ClickHouse.Net;
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
        private const string SELECT_USER_CMD =
@"SELECT DISTINCT
    UserId,
    UserName,
    UserAvatar,
    LiveId,
    Title,
    CreateTime
FROM live
ORDER BY
    CreateTime DESC,
    UserId ASC
LIMIT @offset, @limit";
        private const string SELECT_LIVE_CMD =
@"SELECT DISTINCT
    LiveId,
    Title,
    CreateTime
FROM live
WHERE UserId = @user
ORDER BY CreateTime DESC
LIMIT @offset, @limit";

        private readonly ILogger<UperController> logger;
        private readonly IClickHouseDatabase database;

        public UperController(ILogger<UperController> logger, IClickHouseDatabase database)
        {
            this.logger = logger;
            this.database = database;
        }

        [HttpGet]
        public ActionResult GetUser([FromQuery] LimitOffsetQuery query)
        {
            return Json(
                database.ExecuteQueryMapping<Uper>(
                    SELECT_USER_CMD,
                    new ClickHouseParameter[]
                    {
                        new () { DbType = System.Data.DbType.UInt32, ParameterName = "offset", Value = query.Offset },
                        new () { DbType = System.Data.DbType.UInt32, ParameterName = "limit", Value = query.Limit }
                    }
                )
            );
        }

        [HttpGet("{user:long}")]
        public ActionResult GetUserLive(long user, [FromQuery] LimitOffsetQuery query)
        {
            return Json(
                database.ExecuteQueryMapping<Live>(
                    SELECT_LIVE_CMD,
                    new ClickHouseParameter[] {
                        new() { DbType = System.Data.DbType.UInt64, ParameterName = "user", Value = user },
                        new ClickHouseParameter { DbType = System.Data.DbType.UInt32, ParameterName = "offset", Value = query.Offset },
                        new ClickHouseParameter { DbType = System.Data.DbType.UInt32, ParameterName = "limit", Value = query.Limit }
                    }
                )
            );
        }
    }
}
