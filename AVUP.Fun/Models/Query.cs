using Microsoft.AspNetCore.Mvc;
using System;

namespace AVUP.Fun.Models
{
    public sealed record Query
    {
        [FromQuery(Name = "timestamp")]
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.MinValue;
        [FromQuery(Name = "offset")]
        public uint Offset { get; init; } = 0;
        [FromQuery(Name = "limit")]
        public uint Limit { get; init; } = 20;
    }
}
