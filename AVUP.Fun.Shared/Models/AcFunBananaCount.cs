namespace AVUP.Fun.Shared.Models
{
    public sealed record AcFunBananaCount : AcFunRoomRecord
    {
        public new const string Topic = "banana_count";

        public ulong Banana { get; init; }
    }
}
