using AcFunDanmu;
using AcFunDanmu.Enums;
using AcFunDanmu.Models.Client;
using AVUP.Fun.Shared.Models;
using Confluent.Kafka;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AVUP.Fun.Process.Services
{
    public sealed class PendingWorker : BackgroundService
    {
        private static readonly ProducerConfig producerConfig = new() { BootstrapServers = "broker:9092" };

        private static ulong _count;
        public static ulong Count => _count;

        private static readonly ConsumerConfig consumerConfig = new()
        {
            BootstrapServers = "broker:9092",
            GroupId = "acfun.pending.worker",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        private readonly ILogger<PendingWorker> _logger;
        private readonly IHttpClientFactory _clientFactory;

        private static readonly ConcurrentDictionary<long, GiftInfo> Gifts = new(6, 64);

        public PendingWorker(ILogger<PendingWorker> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(Enumerable.Range(0, 6).Select(_ =>
            {
                using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
                using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
                consumer.Subscribe("pending");

                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(stoppingToken);
                    var message = JsonSerializer.Deserialize<AcFunPendingMessage>(result.Message.Value);
                    HandleSignal(producer, message);
                    producer?.Produce("processed", new Message<Null, string> { Value = result.Message.Value });
                    Interlocked.Increment(ref _count);
                }

                producer.Flush(stoppingToken);
                consumer.Close();
                return Task.CompletedTask;
            }));
        }

        internal static ulong ConvertNumber(string number)
        {
            if (number.EndsWith("万", StringComparison.Ordinal))
            {
                return Convert.ToUInt64(Convert.ToSingle(number[0..^1], CultureInfo.InvariantCulture) * 10000);
            }
            else
            {
                return Convert.ToUInt64(number, CultureInfo.InvariantCulture);
            }
        }

        private async void HandleSignal(IProducer<Null, string> producer, AcFunPendingMessage pendingMessage)
        {
            var payload = Convert.FromBase64String(pendingMessage.Payload);
            switch (pendingMessage.MessageType)
            {
                // Includes comment, gift, enter room, like, follower
                case PushMessage.ACTION_SIGNAL:
                    var actionSignal = ZtLiveScActionSignal.Parser.ParseFrom(payload);

                    foreach (var item in actionSignal.Item)
                    {
                        switch (item.SignalType)
                        {
                            case PushMessage.ActionSignal.COMMENT:
                                foreach (var pl in item.Payload)
                                {
                                    var comment = CommonActionSignalComment.Parser.ParseFrom(pl);
                                    var acComment = new AcFunComment
                                    {
                                        UperId = pendingMessage.UperId,
                                        LiveId = pendingMessage.LiveId,
                                        UserId = comment.UserInfo.UserId,
                                        UserName = comment.UserInfo.Nickname,
                                        UserAvatar = comment.UserInfo.Avatar.First().UrlPattern,
                                        UserData = comment.UserInfo.Badge,
                                        UserManagerType = (int)(comment.UserInfo.UserIdentity?.ManagerType ?? ZtLiveUserIdentity.Types.ManagerType.UnknownManagerType),
                                        Comment = comment.Content,
                                        Timestamp = comment.SendTimeMs
                                    };
                                    var message = JsonSerializer.Serialize(acComment);
                                    producer?.Produce(AcFunAcerBase.Topic,
                                                      new Message<Null, string> { Value = message });
                                }
                                break;
                            case PushMessage.ActionSignal.LIKE:
                                foreach (var pl in item.Payload)
                                {
                                    var like = CommonActionSignalLike.Parser.ParseFrom(pl);
                                    var acLike = new AcFunLike
                                    {
                                        UperId = pendingMessage.UperId,
                                        LiveId = pendingMessage.LiveId,
                                        UserId = like.UserInfo.UserId,
                                        UserName = like.UserInfo.Nickname,
                                        UserAvatar = like.UserInfo.Avatar.First().UrlPattern,
                                        UserData = like.UserInfo.Badge,
                                        UserManagerType = (int)(like.UserInfo.UserIdentity?.ManagerType ?? ZtLiveUserIdentity.Types.ManagerType.UnknownManagerType),
                                        Timestamp = like.SendTimeMs
                                    };
                                    var message = JsonSerializer.Serialize(acLike);
                                    producer?.Produce(AcFunAcerBase.Topic,
                                                      new Message<Null, string> { Value = message });
                                }
                                break;
                            case PushMessage.ActionSignal.ENTER_ROOM:
                                foreach (var pl in item.Payload)
                                {
                                    var enter = CommonActionSignalUserEnterRoom.Parser.ParseFrom(pl);
                                    var acEnter = new AcFunEnter
                                    {
                                        UperId = pendingMessage.UperId,
                                        LiveId = pendingMessage.LiveId,
                                        UserId = enter.UserInfo.UserId,
                                        UserName = enter.UserInfo.Nickname,
                                        UserAvatar = enter.UserInfo.Avatar.First().UrlPattern,
                                        UserData = enter.UserInfo.Badge,
                                        UserManagerType = (int)(enter.UserInfo.UserIdentity?.ManagerType ?? ZtLiveUserIdentity.Types.ManagerType.UnknownManagerType),
                                        Timestamp = enter.SendTimeMs
                                    };
                                    var message = JsonSerializer.Serialize(acEnter);
                                    producer?.Produce(AcFunAcerBase.Topic,
                                                      new Message<Null, string> { Value = message });
                                }
                                break;
                            case PushMessage.ActionSignal.FOLLOW:
                                foreach (var pl in item.Payload)
                                {
                                    var follower = CommonActionSignalUserFollowAuthor.Parser.ParseFrom(pl);
                                    var acFollow = new AcFunFollow
                                    {
                                        UperId = pendingMessage.UperId,
                                        LiveId = pendingMessage.LiveId,
                                        UserId = follower.UserInfo.UserId,
                                        UserName = follower.UserInfo.Nickname,
                                        UserAvatar = follower.UserInfo.Avatar.First().UrlPattern,
                                        UserData = follower.UserInfo.Badge,
                                        UserManagerType = (int)(follower.UserInfo.UserIdentity?.ManagerType ?? ZtLiveUserIdentity.Types.ManagerType.UnknownManagerType),
                                        Timestamp = follower.SendTimeMs
                                    };
                                    var message = JsonSerializer.Serialize(acFollow);
                                    producer?.Produce(AcFunAcerBase.Topic,
                                                      new Message<Null, string> { Value = message });
                                }
                                break;
                            case PushMessage.ActionSignal.THROW_BANANA:
                                break;
                            case PushMessage.ActionSignal.GIFT:
                                foreach (var pl in item.Payload)
                                {
                                    var gift = CommonActionSignalGift.Parser.ParseFrom(pl);
                                    if (!Gifts.TryGetValue(gift.GiftId, out var info))
                                    {
                                        using var client = _clientFactory.CreateClient("acfun");
                                        var uri = new Uri($"http://intake/status/gift/{gift.GiftId}");
                                        HttpResponseMessage resp;
                                        do
                                        {
                                            resp = await client.GetAsync(uri).ConfigureAwait(false);
                                            if (resp.IsSuccessStatusCode)
                                            {
                                                info = await JsonSerializer.DeserializeAsync<GiftInfo>(await resp.Content.ReadAsStreamAsync().ConfigureAwait(false)).ConfigureAwait(false);
                                                Gifts.AddOrUpdate(gift.GiftId, info, (k, v) => info);
                                                break;
                                            }
                                        } while (true);
                                    }
                                    var acGift = new AcFunGift
                                    {
                                        UperId = pendingMessage.UperId,
                                        LiveId = pendingMessage.LiveId,
                                        UserId = gift.User.UserId,
                                        UserName = gift.User.Nickname,
                                        UserAvatar = gift.User.Avatar.First().UrlPattern,
                                        UserData = gift.User.Badge,
                                        UserManagerType = (int)(gift.User.UserIdentity?.ManagerType ?? ZtLiveUserIdentity.Types.ManagerType.UnknownManagerType),
                                        Timestamp = gift.SendTimeMs,
                                        GiftId = gift.GiftId,
                                        GiftName = info.Name,
                                        GiftComboId = gift.ComboId,
                                        GiftCount = gift.Count,
                                        GiftCombo = gift.Combo,
                                        GiftValue = gift.Value,
                                        Pic = info.Pic.OriginalString,
                                    };
                                    var message = JsonSerializer.Serialize(acGift);
                                    producer?.Produce(AcFunAcerBase.Topic,
                                                      new Message<Null, string> { Value = message });
                                }
                                break;
                            case PushMessage.ActionSignal.RICH_TEXT:
                                break;
                            case PushMessage.ActionSignal.JOIN_CLUB:
                                break;
                            default:
                                _logger.LogWarning("Unhandled action signal: {SignalType}", item.SignalType);
                                _logger.LogDebug("{Detail}", item.ToByteString().ToBase64());
                                break;
                        }
                    }
                    break;
                //Includes current banana counts, watching count, like count and top 3 users sent gifts
                case PushMessage.STATE_SIGNAL:
                    var signal = ZtLiveScStateSignal.Parser.ParseFrom(payload);

                    foreach (var item in signal.Item)
                    {
                        switch (item.SignalType)
                        {
                            case PushMessage.StateSignal.ACFUN_DISPLAY_INFO:
                                {
                                    var acInfo = AcfunStateSignalDisplayInfo.Parser.ParseFrom(item.Payload);
                                    var acBananaCount = new AcFunBananaCount
                                    {
                                        UserId = pendingMessage.UperId,
                                        LiveId = pendingMessage.LiveId,
                                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                                        Banana = ConvertNumber(acInfo.BananaCount),
                                    };

                                    var message = JsonSerializer.Serialize(acBananaCount);
                                    producer?.Produce(AcFunRoomRecord.Topic,
                                                      new Message<Null, string> { Value = message });
                                }
                                break;
                            case PushMessage.StateSignal.DISPLAY_INFO:
                                {
                                    var stateInfo = CommonStateSignalDisplayInfo.Parser.ParseFrom(item.Payload);
                                    var acDisplayInfo = new AcFunDisplayInfo
                                    {
                                        UserId = pendingMessage.UperId,
                                        LiveId = pendingMessage.LiveId,
                                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                                        Like = ConvertNumber(stateInfo.LikeCount),
                                        LikeDelta = stateInfo.LikeDelta,
                                        Audience = ConvertNumber(stateInfo.WatchingCount),
                                    };

                                    var message = JsonSerializer.Serialize(acDisplayInfo);
                                    producer?.Produce(AcFunRoomRecord.Topic,
                                                      new Message<Null, string> { Value = message });
                                }
                                break;
                            case PushMessage.StateSignal.TOP_USRES:
                                break;
                            case PushMessage.StateSignal.RECENT_COMMENT:
                                {
                                    var comments = CommonStateSignalRecentComment.Parser.ParseFrom(item.Payload);
                                    foreach (var comment in comments.Comment)
                                    {
                                        var acComment = new AcFunComment
                                        {
                                            UperId = pendingMessage.UperId,
                                            LiveId = pendingMessage.LiveId,
                                            UserId = comment.UserInfo.UserId,
                                            UserName = comment.UserInfo.Nickname,
                                            UserAvatar = comment.UserInfo.Avatar.First().UrlPattern,
                                            UserData = comment.UserInfo.Badge,
                                            UserManagerType = (int)(comment.UserInfo.UserIdentity?.ManagerType ?? ZtLiveUserIdentity.Types.ManagerType.UnknownManagerType),
                                            Timestamp = comment.SendTimeMs,
                                            Comment = comment.Content,
                                        };

                                        var message = JsonSerializer.Serialize(acComment);
                                        producer?.Produce(AcFunAcerBase.Topic,
                                                          new Message<Null, string> { Value = message });
                                    }
                                }
                                break;
                            case PushMessage.StateSignal.AUTHOR_CHAT_ACCEPT:
                            case PushMessage.StateSignal.AUTHOR_CHAT_CALL:
                            case PushMessage.StateSignal.AUTHOR_CHAT_READY:
                            case PushMessage.StateSignal.AUTHOR_CHAT_CHANGE_SOUND_CONFIG:
                            case PushMessage.StateSignal.AUTHOR_CHAT_END:
                            case PushMessage.StateSignal.CHAT_ACCEPT:
                            case PushMessage.StateSignal.CHAT_CALL:
                            case PushMessage.StateSignal.CHAT_READY:
                            case PushMessage.StateSignal.CHAT_END:
                                break;
                            case PushMessage.StateSignal.CURRENT_RED_PACK_LIST:
                                var list = CommonStateSignalCurrentRedpackList.Parser.ParseFrom(item.Payload);
                                foreach (var redpack in list.Redpacks)
                                {

                                }
                                break;
                            default:
                                _logger.LogWarning("Unhandled state signal: {SignalType}", item.SignalType);
                                _logger.LogDebug("{Detail}", item.ToByteString().ToBase64());
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
