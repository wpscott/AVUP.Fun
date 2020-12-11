namespace AVUP.Fun.Shared.Models
{
    public sealed record AcFunLive : Record
    {
        public const string Topic = "live";

        public long UserId { get; init; }
        public string LiveId { get; init; }
        public string Title { get; init; }
        public long Like { get; init; }
        public long Audience { get; init; }
        public int TypeId { get; init; }
        public int TypeCategory { get; init; }
        public string TypeName { get; init; }
        public string TypeCategoryName { get; init; }
        public long UserPost { get; init; }
        public long UserFan { get; init; }
        public long UserFollowing { get; init; }
        public string UserAvatar { get; init; }
        public string UserName { get; init; }
        public long CreateTime { get; init; }
    }
}
