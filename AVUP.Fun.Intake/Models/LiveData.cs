using AcFunDanmu;
using System.Text.Json.Serialization;

namespace AVUP.Fun.Intake.Models
{
    public sealed record LiveData
    {
        public long UserId { get; init; }
        public string UserName { get; init; }
        public string Title { get; init; }
        public string LiveId { get; init; }

        [JsonIgnore]
        public Client Client { get; init; }
    }
}
