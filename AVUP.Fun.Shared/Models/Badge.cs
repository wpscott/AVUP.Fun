using System.Text.Json.Serialization;

namespace AVUP.Fun.Shared.Models
{
    public record MedalInfo
    {
        [JsonPropertyName("medalInfo")]
        public Badge Badge { get; init; }
    }

    public record Badge
    {
        [JsonPropertyName("uperId")]
        public long UperId { get; init; }
        [JsonPropertyName("userId")]
        public long UserId { get; init; }
        [JsonPropertyName("clubName")]
        public string ClubName { get; init; }
        [JsonPropertyName("level")]
        public int Level { get; init; }
    }
}
