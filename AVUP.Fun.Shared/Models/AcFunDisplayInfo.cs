namespace AVUP.Fun.Shared.Models
{
    public sealed record AcFunDisplayInfo : AcFunRoomRecord
    {
        public new const string Topic = "display_info";

        public ulong Like { get; init; }
        public int LikeDelta { get; init; }
        public ulong Audience { get; init; }
    }
}
