using System;

namespace AVUP.Fun.Models
{
    public sealed record ACER
    {
        public ulong UperId { get; init; }
        public string LiveId { get; init; }
        public string Type { get; init; }
        public ulong UserId { get; init; }
        public string UserName { get; init; }
        public string UserAvatar { get; init; }
        public string UserData { get; init; }
        public ulong UserBadgeUperId { get; init; }
        public string UserBadgeName { get; init; }
        public byte UserBadgeLevel { get; init; }
        public byte UserManagerType { get; init; }
        public DateTime Timestamp { get; init; }
        public string Comment { get; init; }
        public uint GiftId { get; init; }
        public string GiftName { get; init; }
        public uint GiftCount { get; init; }
        public uint GiftCombo { get; init; }
        public string GiftComboId { get; init; }
        public ulong GiftValue { get; init; }
    }
}
