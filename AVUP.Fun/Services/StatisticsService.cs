using AVUP.Fun.Models;
using ClickHouse.Net;
using System.Collections.Generic;

namespace AVUP.Fun.Services
{
    public class StatisticsService
    {
        #region Constant Commands
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
        #endregion

        private readonly IClickHouseDatabase database;

        public StatisticsService(IClickHouseDatabase database)
        {
            this.database = database;
        }

        public IEnumerable<TopUser> GetLiveRank(string range) => database.ExecuteQueryMapping<TopUser>(
            range switch
            {
                "week" => TOP_10_LIVE_DD_BY_WEEK_COMMAND,
                "month" => TOP_10_LIVE_DD_BY_MONTH_COMMAND,
                _ => TOP_10_LIVE_DD_BY_DAY_COMMAND,
            });

        public IEnumerable<TopUser> GetCommentRank(string range) => database.ExecuteQueryMapping<TopUser>(
            range switch
            {
                "week" => TOP_10_COMMENT_DD_BY_WEEK_COMMAND,
                "month" => TOP_10_COMMENT_DD_BY_MONTH_COMMAND,
                _ => TOP_10_COMMENT_DD_BY_DAY_COMMAND,
            });
    }
}
