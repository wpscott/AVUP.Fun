namespace AVUP.Fun.Shared.Models
{
    public abstract record AcFunRoomRecord : Record
    {
        public const string Topic = "room";

        public long UserId { get; init; }
        public string LiveId { get; init; }
    }
}
