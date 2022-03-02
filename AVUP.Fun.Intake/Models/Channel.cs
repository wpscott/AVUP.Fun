using System;
using System.Text.Json.Serialization;

namespace AVUP.Fun.Intake.Models
{
    public sealed record Channel
    {
        [JsonPropertyName("channelListData")]
        public ChannelListData ChannelListData { get; set; }

        [JsonPropertyName("totalCount")]
        public long TotalCount { get; set; }

        [JsonPropertyName("channelData")]
        public ChannelData ChannelData { get; set; }

        [JsonPropertyName("liveList")]
        public Live[] LiveList { get; set; }

        [JsonPropertyName("recommendAuthorsData")]
        public object[] RecommendAuthorsData { get; set; }

        [JsonPropertyName("channelFilters")]
        public ChannelFilters ChannelFilters { get; set; }
    }

    public sealed record ChannelListData
    {
        [JsonPropertyName("result")]
        public long Result { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("liveList")]
        public Live[] LiveList { get; set; }

        [JsonPropertyName("count")]
        public long Count { get; set; }

        [JsonPropertyName("pcursor")]
        public string Pcursor { get; set; }

        [JsonPropertyName("host-name")]
        public string HostName { get; set; }

        [JsonPropertyName("totalCount")]
        public long TotalCount { get; set; }
    }

    public sealed record ChannelData { }

    public sealed record ChannelFilters
    {
        [JsonPropertyName("liveChannelDisplayFilters")]
        public LiveChannelDisplayFilter[] LiveChannelDisplayFilters { get; set; }
    }

    public sealed record LiveChannelDisplayFilter
    {
        [JsonPropertyName("displayFilters")]
        public DisplayFilter[] DisplayFilters { get; set; }
    }

    public sealed record DisplayFilter
    {
        [JsonPropertyName("filterType")]
        public int FilterType { get; set; }

        [JsonPropertyName("filterId")]
        public int FilterId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public sealed record Live
    {
        [JsonPropertyName("disableDanmakuShow")]
        public bool DisableDanmakuShow { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("groupId")]
        public string GroupId { get; set; }

        [JsonPropertyName("action")]
        public long Action { get; set; }

        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("createTime")]
        public long CreateTime { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("onlineCount")]
        public long OnlineCount { get; set; }

        [JsonPropertyName("liveId")]
        public string LiveId { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("coverUrls")]
        public Uri[] CoverUrls { get; set; }

        [JsonPropertyName("likeCount")]
        public long LikeCount { get; set; }

        [JsonPropertyName("streamName")]
        public string StreamName { get; set; }

        [JsonPropertyName("formatLikeCount")]
        public string FormatLikeCount { get; set; }

        [JsonPropertyName("formatOnlineCount")]
        public string FormatOnlineCount { get; set; }

        [JsonPropertyName("portrait")]
        public bool Portrait { get; set; }

        [JsonPropertyName("panoramic")]
        public bool Panoramic { get; set; }

        [JsonPropertyName("bizCustomData")]
        public string BizCustomData { get; set; }

        [JsonPropertyName("authorId")]
        public long AuthorId { get; set; }

        [JsonPropertyName("type")]
        public LiveType Type { get; set; }
    }

    public sealed record User
    {
        [JsonPropertyName("action")]
        public long Action { get; set; }

        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("gender")]
        public int Gender { get; set; }

        [JsonPropertyName("verifiedTypes")]
        public long[] VerifiedTypes { get; set; }

        [JsonPropertyName("nameColor")]
        public int NameColor { get; set; }

        [JsonPropertyName("followingStatus")]
        public int FollowingStatus { get; set; }

        [JsonPropertyName("liveId")]
        public string LiveId { get; set; }

        [JsonPropertyName("avatarFrame")]
        public long AvatarFrame { get; set; }

        [JsonPropertyName("contributeCount")]
        public string ContributeCount { get; set; }

        [JsonPropertyName("headUrl")]
        public Uri HeadUrl { get; set; }

        [JsonPropertyName("fanCountValue")]
        public long FanCountValue { get; set; }

        [JsonPropertyName("verifiedType")]
        public long? VerifiedType { get; set; }

        [JsonPropertyName("verifiedText")]
        public string VerifiedText { get; set; }

        [JsonPropertyName("followingCount")]
        public string FollowingCount { get; set; }

        [JsonPropertyName("avatarFrameMobileImg")]
        public string AvatarFrameMobileImg { get; set; }

        [JsonPropertyName("avatarFramePcImg")]
        public string AvatarFramePcImg { get; set; }

        [JsonPropertyName("fanCount")]
        public string FanCount { get; set; }

        [JsonPropertyName("isFollowing")]
        public bool IsFollowing { get; set; }

        [JsonPropertyName("headCdnUrls")]
        public HeadCdnUrl[] HeadCdnUrls { get; set; }

        [JsonPropertyName("isJoinUpCollege")]
        public bool? IsJoinUpCollege { get; set; }

        [JsonPropertyName("followingCountValue")]
        public long FollowingCountValue { get; set; }

        [JsonPropertyName("contributeCountValue")]
        public long ContributeCountValue { get; set; }

        [JsonPropertyName("sexTrend")]
        public int SexTrend { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("comeFrom")]
        public string ComeFrom { get; set; }
    }

    public sealed record LiveType
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }
    }

    public sealed record HeadCdnUrl
    {
        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        [JsonPropertyName("freeTrafficCdn")]
        public bool FreeTrafficCdn { get; set; }
    }
}
