using AVUP.Fun.Models;
using ClickHouse.Ado;
using ClickHouse.Net;
using System.Collections.Generic;
using System.Data;

namespace AVUP.Fun.Services
{
    public class UperService
    {
        #region Constant Commands
        private const string SELECT_UPER_CMD =
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
        private const string SELECT_UPER_LIVE_CMD =
@"SELECT DISTINCT
    UserId,
    UserName,
    UserAvatar,
    LiveId,
    Title,
    CreateTime
FROM live
WHERE (UserId = @user) AND (Timestamp > @timestamp)
ORDER BY CreateTime DESC
LIMIT @offset, @limit";
        #endregion

        private readonly IClickHouseDatabase database;

        public UperService(IClickHouseDatabase database)
        {
            this.database = database;
        }

        public IEnumerable<Uper> GetUper(Query query) => database.ExecuteQueryMapping<Uper>(
            SELECT_UPER_CMD,
            new ClickHouseParameter[]
            {
                new () { DbType = DbType.UInt32, ParameterName = "offset", Value = query.Offset },
                new () { DbType = DbType.UInt32, ParameterName = "limit", Value = query.Limit }
            }
        );

        public IEnumerable<Live> GetUperLive(long user, Query query) => database.ExecuteQueryMapping<Live>(
            SELECT_UPER_LIVE_CMD,
            new ClickHouseParameter[] {
                new() { DbType = DbType.UInt64, ParameterName = "user", Value = user },
                new() { DbType = DbType.UInt32, ParameterName = "offset", Value = query.Offset },
                new() { DbType = DbType.UInt32, ParameterName = "limit", Value = query.Limit }
            }
        );
    }
}
