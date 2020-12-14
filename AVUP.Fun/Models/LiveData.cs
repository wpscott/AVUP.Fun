using System;

namespace AVUP.Fun.Models
{
    public sealed record LiveData
    {
        public DateTime Timestamp { get; init; }
        public long Total { get; init; }
        public long AccumulateTotal { get; init; }
    }
}
