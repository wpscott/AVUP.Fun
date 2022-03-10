using System;

namespace AVUP.Fun.Models
{
    public sealed record Uper
    {
        public ulong UserId { get; init; }
        public string UserName { get; init; }
        public string UserAvatar { get; init; }
        public string LiveId { get; init; }
        public string Title { get; init; }
        public DateTime CreateTime { get; init; }
    }
}
