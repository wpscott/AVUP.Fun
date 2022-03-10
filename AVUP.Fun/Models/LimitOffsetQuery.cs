using Microsoft.AspNetCore.Mvc;

namespace AVUP.Fun.Models
{
    public sealed record LimitOffsetQuery
    {
        [FromQuery(Name = "offset")]
        public uint Offset { get; init; } = 0;
        [FromQuery(Name = "limit")]
        public uint Limit { get; init; } = 20;
    }
}
