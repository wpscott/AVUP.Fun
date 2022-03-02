namespace AVUP.Fun.Models
{
    public sealed record TopUser
    {
        public ulong UserId { get; init; }
        public ulong Total { get; init; }
    }
}
