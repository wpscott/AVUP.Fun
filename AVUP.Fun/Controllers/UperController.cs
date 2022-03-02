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
    UserName
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
        [HttpGet("{offset:long}/{limit:long}")]
        public ActionResult GetUser(long? offset, long? limit)
        {
            return Json(
                database.ExecuteQueryMapping<Uper>(
                    SELECT_USER_CMD,
                    new ClickHouseParameter[]
                    {
                        new () { DbType = System.Data.DbType.UInt32, ParameterName = "offset", Value = offset ?? 0 },
                        new () { DbType = System.Data.DbType.UInt32, ParameterName = "limit", Value =limit ?? 20 }
                    }
                )
            );
        }

        [HttpGet("{user:long}")]
        [HttpGet("{user:long}/{offset:long}/{limit:long}")]
        public ActionResult GetUserLive(long user, long? offset, long? limit)
        {
            return Json(
                database.ExecuteQueryMapping<Live>(
                    SELECT_LIVE_CMD,
                    new ClickHouseParameter[] {
                        new() { DbType = System.Data.DbType.UInt64, ParameterName = "user", Value = user },
                        new ClickHouseParameter { DbType = System.Data.DbType.UInt32, ParameterName = "offset", Value = offset ?? 0 },
                        new ClickHouseParameter { DbType = System.Data.DbType.UInt32, ParameterName = "limit", Value = limit ?? 20 }
                    }
                )
            );
        }
    }
}
