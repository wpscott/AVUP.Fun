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
    public class LiveController : Controller
    {
        private const string SELECT_ALL_CMD =
@"SELECT DISTINCT *
FROM acer
WHERE (UperId = @user) AND (LiveId = @live) AND (Timestamp > @timestamp)
ORDER BY Timestamp ASC";
        private const string SELECT_ALL_WITH_TYPE_CMD =
@"SELECT DISTINCT *
FROM acer
WHERE (UperId = @user) AND (LiveId = @live) AND (Type = @type) AND (Timestamp > @timestamp)
ORDER BY Timestamp ASC";
        private const string SELECT_LIVE_DATA_CMD =
@"SELECT
    Timestamp,
    finalizeAggregation(state) AS Total,
    runningAccumulate(state) AS AccumulateTotal
FROM
(
    SELECT
        Timestamp,
        uniqState(Timestamp, UserId) AS state
    FROM acer
    WHERE (UperId = @user) AND (LiveId = @live) AND (Type = 'comment')
    GROUP BY Timestamp
    ORDER BY Timestamp ASC
)";

        private readonly ILogger<LiveController> logger;
        private readonly IClickHouseDatabase database;

        public LiveController(ILogger<LiveController> logger, IClickHouseDatabase database)
        {
            this.logger = logger;
            this.database = database;
        }

        [HttpGet("{user:long}/{live}")]
        [HttpGet("{user:long}/{live}/{timestamp:long}")]
        public ActionResult GetAll(long user, string live, long? timestamp)
        {
            logger.LogInformation("Get all {{ user: {UserId}, live: {LiveId} }}", user, live);
            return Json(
                database.ExecuteQueryMapping<ACER>(
                    SELECT_ALL_CMD,
                    new ClickHouseParameter[] {
                        new () { DbType = System.Data.DbType.UInt64, ParameterName = "user", Value = user },
                        new() { DbType = System.Data.DbType.String, ParameterName = "live", Value = live },
                        new() { ParameterName = "timestamp", Value = timestamp ?? 0 },
                    }
                )
            );
        }

        [HttpGet("{user:long}/{live}/data")]
        public ActionResult GetLiveData(long user, string live)
        {
            logger.LogInformation("Get live data {{ user: {UserId}, live: {LiveId} }}", user, live);
            return Json(
                database.ExecuteQueryMapping<LiveData>(
                    SELECT_LIVE_DATA_CMD,
                    new ClickHouseParameter[] {
                        new  () { DbType = System.Data.DbType.UInt64, ParameterName = "user", Value = user },
                        new () { DbType = System.Data.DbType.String, ParameterName = "live", Value = live },
                    }
                )
            );
        }

        [HttpGet("{user:long}/{live}/{type}")]
        [HttpGet("{user:long}/{live}/{type}/{timestamp:long}")]
        public ActionResult GetAllWithType(long user, string live, string type, long? timestamp)
        {
            logger.LogInformation("Get all with type {{ user:{UserId}, live: {LiveId}, type: {Type} }}", user, live, type);
            return Json(
                database.ExecuteQueryMapping<ACER>(
                    SELECT_ALL_WITH_TYPE_CMD,
                    new ClickHouseParameter[] {
                        new () { DbType = System.Data.DbType.UInt64, ParameterName = "user", Value = user },
                        new () { DbType = System.Data.DbType.String, ParameterName = "live", Value = live },
                        new () { DbType = System.Data.DbType.String, ParameterName = "type", Value = type },
                        new () { ParameterName = "timestamp", Value = timestamp ?? 0 },
                    }
                )
            );
        }
    }
}
