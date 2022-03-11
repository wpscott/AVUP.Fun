using AVUP.Fun.Models;
using ClickHouse.Ado;
using ClickHouse.Net;
using System.Collections.Generic;
using System.Data;

namespace AVUP.Fun.Services
{
    public class LiveService
    {
        #region Constant Commands
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
        #endregion

        private readonly IClickHouseDatabase database;

        public LiveService(IClickHouseDatabase database)
        {
            this.database = database;
        }

        public IEnumerable<ACER> GetAllData(long user, string live, Query query) => database.ExecuteQueryMapping<ACER>(
            SELECT_ALL_CMD,
            new ClickHouseParameter[] {
                new() { DbType = DbType.UInt64, ParameterName = "user", Value = user },
                new() { ParameterName = "live", Value = live },
                new() { DbType = DbType.UInt64, ParameterName = "timestamp", Value = query.Timestamp.ToUnixTimeMilliseconds() },
            }
        );

        public IEnumerable<ACER> GetAllDataByType(long user, string live, string type, Query query) => database.ExecuteQueryMapping<ACER>(
            SELECT_ALL_WITH_TYPE_CMD,
            new ClickHouseParameter[] {
                new() { DbType = DbType.UInt64, ParameterName = "user", Value = user },
                new() { ParameterName = "live", Value = live },
                new() { ParameterName = "type", Value = type },
                new() { DbType = DbType.UInt64, ParameterName = "timestamp", Value = query.Timestamp.ToUnixTimeMilliseconds() },
            }
        );

        public IEnumerable<LiveData> GetLiveCommentData(long user, string live) => database.ExecuteQueryMapping<LiveData>(
            SELECT_LIVE_DATA_CMD,
            new ClickHouseParameter[] {
                new() { DbType = DbType.UInt64, ParameterName = "user", Value = user },
                new() { ParameterName = "live", Value = live },
            }
        );
    }
}
