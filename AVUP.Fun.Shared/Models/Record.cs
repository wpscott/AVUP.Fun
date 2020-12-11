using System.Text.Json.Serialization;

namespace AVUP.Fun.Shared.Models
{
    public abstract record Record
    {
        public long Timestamp { get; init; }
    }
}
