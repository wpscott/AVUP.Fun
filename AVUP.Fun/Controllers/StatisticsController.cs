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
    public class StatisticsController : Controller
    {
        private const string SELECT_GIFT_RANK_IN_LIVE_COMMAND =
@"SELECT
    UserId,
    sum(Total) AS Total
FROM
(
    SELECT
        UserId,
        max(GiftCombo) * any(GiftValue) AS Total
    FROM acer
    WHERE (Type = 'gift') AND (GiftId != 1) AND (LiveId = @live)
    GROUP BY
        UserId,
        GiftComboId
    ORDER BY Total DESC
)
GROUP BY UserId
ORDER BY Total DESC";
        private const string TOP_10_LIVE_DD_BY_DAY_COMMAND =
@"SELECT
    UserId,
    countDistinct(LiveId) AS Total
FROM acer
WHERE toYYYYMMDD(Timestamp) = toYYYYMMDD(now())
GROUP BY UserId
ORDER BY
    Total DESC
LIMIT 10";
        private const string TOP_10_COMMENT_DD_BY_DAY_COMMAND =
@"SELECT
    UserId,
    countDistinct(Timestamp) AS Total
FROM acer
WHERE (Type = 'comment') AND (toYYYYMMDD(Timestamp) = toYYYYMMDD(now()))
GROUP BY UserId
ORDER BY
    Total DESC
LIMIT 10";
        private const string TOP_10_LIVE_DD_BY_WEEK_COMMAND =
@"SELECT
    UserId,
    countDistinct(LiveId) AS Total
FROM acer
WHERE week(Timestamp) = week(now())
GROUP BY UserId
ORDER BY
    Total DESC
LIMIT 10";
        private const string TOP_10_COMMENT_DD_BY_WEEK_COMMAND =
@"SELECT
    UserId,
    countDistinct(Timestamp) AS Total
FROM acer
WHERE (Type = 'comment') AND (week(Timestamp) = week(now()))
GROUP BY UserId
ORDER BY
    Total DESC
LIMIT 10";
        private const string TOP_10_LIVE_DD_BY_MONTH_COMMAND =
@"SELECT
    UserId,
    countDistinct(LiveId) AS Total
FROM acer
WHERE toYYYYMM(Timestamp) = toYYYYMM(now())
GROUP BY UserId
ORDER BY
    Total DESC
LIMIT 10";
        private const string TOP_10_COMMENT_DD_BY_MONTH_COMMAND =
@"SELECT
    UserId,
    countDistinct(Timestamp) AS Total
FROM acer
WHERE (Type = 'comment') AND (toYYYYMM(Timestamp) = toYYYYMM(now()))
GROUP BY UserId
ORDER BY
    Total DESC
LIMIT 10";

        private readonly ILogger<StatisticsController> logger;
        private readonly IClickHouseDatabase database;

        public StatisticsController(ILogger<StatisticsController> logger, IClickHouseDatabase database)
        {
            this.logger = logger;
            this.database = database;
        }

        [HttpGet("live")]
        [HttpGet("live/{range}")]
        public ActionResult GetLiveRank(string range)
        {
            return range switch
            {
                "week" => Json(database.ExecuteQueryMapping<TopUser>(TOP_10_LIVE_DD_BY_WEEK_COMMAND)),
                "month" => Json(database.ExecuteQueryMapping<TopUser>(TOP_10_LIVE_DD_BY_MONTH_COMMAND)),
                _ => Json(database.ExecuteQueryMapping<TopUser>(TOP_10_LIVE_DD_BY_DAY_COMMAND)),
            };
        }

        [HttpGet("comment")]
        [HttpGet("comment/{range}")]
        public ActionResult GetCommentRank(string range)
        {
            return range switch
            {
                "week" => Json(database.ExecuteQueryMapping<TopUser>(TOP_10_COMMENT_DD_BY_WEEK_COMMAND)),
                "month" => Json(database.ExecuteQueryMapping<TopUser>(TOP_10_COMMENT_DD_BY_MONTH_COMMAND)),
                _ => Json(database.ExecuteQueryMapping<TopUser>(TOP_10_COMMENT_DD_BY_DAY_COMMAND)),
            };
        }
    }
}
