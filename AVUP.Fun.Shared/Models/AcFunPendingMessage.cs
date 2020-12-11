namespace AVUP.Fun.Shared.Models
{
    public sealed record AcFunPendingMessage
    {
        public long UperId { get; init; }
        public string LiveId { get; init; }
        public string MessageType { get; init; }
        public string Payload { get; init; }
    }
}
