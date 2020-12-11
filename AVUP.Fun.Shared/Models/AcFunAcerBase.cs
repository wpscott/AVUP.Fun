using System.Text.Json;
using System.Text.Json.Serialization;

namespace AVUP.Fun.Shared.Models
{
    public abstract record AcFunAcerBase : Record
    {
        public const string Topic = "acer";

        public long UperId { get; init; }
        public string LiveId { get; init; }
        public abstract string Type { get; }
        public long UserId { get; init; }
        public string UserName { get; init; }
        public string UserAvatar { get; init; }
        public string UserData { get; init; }
        public long UserBadgeUperId => Badge?.UperId ?? 0;
        public string UserBadgeName => Badge?.ClubName ?? string.Empty;
        public int UserBadgeLevel => Badge?.Level ?? 0;
        public int UserManagerType { get; init; }

        [JsonIgnore]
        private Badge Badge => string.IsNullOrEmpty(UserData) ? null : JsonSerializer.Deserialize<MedalInfo>(UserData)?.Badge;
    }
}
