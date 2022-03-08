using Microsoft.AspNetCore.Mvc;

namespace AVUP.Fun.Models
{
    public sealed record LimitOffsetQuery
    {
        [FromQuery(Name = "offset")]
        public long Offset { get; init; } = 0;
        [FromQuery(Name = "limit")]
        public long Limit { get; init; } = 20;
    }
}
