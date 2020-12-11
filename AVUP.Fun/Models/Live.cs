using System;

namespace AVUP.Fun.Models
{
    public record Live
    {
        public string LiveId { get; init; }
        public string Title { get; init; }
        public DateTime CreateTime { get; init; }
    }
}
