using AcFunDanmu;
using AVUP.Fun.Intake.Models;
using AVUP.Fun.Shared.Models;
using Confluent.Kafka;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AVUP.Fun.Intake.Services
{
    public sealed class IntakeWorker : IHostedService, IDisposable
    {
        private const string Url = "https://live.acfun.cn/api/channel/list";

        private static readonly ConcurrentDictionary<long, LiveData> _Monitors = new();
        public static ReadOnlyDictionary<long, LiveData> Monitors => new(_Monitors);

        private static readonly ProducerConfig _config = new() { BootstrapServers = "broker:9092" };
        private readonly IProducer<Null, string> producer;

        private readonly ILogger<IntakeWorker> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private Timer _timer;

        public IntakeWorker(ILogger<IntakeWorker> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;

            producer = new ProducerBuilder<Null, string>(_config).Build();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(StartMonitor, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            foreach (var (_, data) in _Monitors)
            {
                data.Client.Stop("dispose");
            }
            _Monitors.Clear();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            producer?.Flush();
            producer?.Dispose();
        }

        private async void StartMonitor(object state)
        {
            await Client.Prepare();
            _logger.LogInformation("Fetching live list");
            using var client = _clientFactory.CreateClient("acfun");
            try
            {
                var pcursor = "";
                do
                {
                    using var resp = await client.GetAsync(new Uri($"{Url}?pcursor={pcursor}"));
                    if (resp.IsSuccessStatusCode)
                    {
                        var channel = await JsonSerializer.DeserializeAsync<Channel>(await resp.Content.ReadAsStreamAsync());
                        Parallel.ForEach(channel?.LiveList ?? Array.Empty<Live>(), live =>
                        {
                            PushLive(live);
                            if (!_Monitors.ContainsKey(live.AuthorId))
                            {
                                Monitor(live);
                            }
                        });
                        pcursor = channel?.ChannelListData?.Pcursor ?? "no_more";
                    }
                    else { break; }
                } while (pcursor != "no_more");
            }
            catch (HttpRequestException) { }
            catch (TaskCanceledException) { }
            catch (JsonException) { }
        }

        private async void Monitor(Live live)
        {
            _logger.LogInformation("Connect to {AuthorId}", live.AuthorId);
            var client = new Client();
            client.Handler += HandleSignal;

            var data = new LiveData
            {
                UserId = live.AuthorId,
                UserName = live.User.Name,
                LiveId = live.LiveId,
                Title = live.Title,
                Client = client
            };
            if (_Monitors.TryAdd(live.AuthorId, data))
            {
                await client.Initialize($"{live.AuthorId}");
                _logger.LogInformation("Start monitoring {AuthorId}", live.AuthorId);
                int retry = 0;
                using var resetTimer = new System.Timers.Timer(10000);
                resetTimer.Elapsed += (s, e) =>
                {
                    retry = 0;
                };
                try
                {
                    while (!await client.Start() && retry < 6)
                    {
                        if (retry > 0)
                        {
                            resetTimer.Stop();
                        }
                        retry++;
                        resetTimer.Start();
                    }
                }
                catch (NullReferenceException ex) { _logger.LogError(ex, "Monitor failed"); }
                finally
                {
                    _logger.LogInformation("End monitoring {AuthorId}", live.AuthorId);
                    _Monitors.TryRemove(live.AuthorId, out _);
                }
            }
        }

        private void PushLive(Live live)
        {
            var data = new AcFunLive
            {
                UserId = live.AuthorId,
                LiveId = live.LiveId,
                Title = live.Title,
                Like = live.LikeCount,
                Audience = live.OnlineCount,
                TypeId = live.Type?.Id ?? 0,
                TypeCategory = live.Type?.CategoryId ?? 0,
                TypeName = live.Type?.Name ?? string.Empty,
                TypeCategoryName = live.Type?.CategoryName ?? string.Empty,
                UserPost = live.User.ContributeCountValue,
                UserFan = live.User.FanCountValue,
                UserFollowing = live.User.FollowingCountValue,
                UserAvatar = $"{live.User.HeadUrl.Scheme}://{live.User.HeadUrl.Host}{live.User.HeadUrl.AbsolutePath}",
                UserName = live.User.Name,
                CreateTime = live.CreateTime,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            producer?.Produce(AcFunLive.Topic, new Message<Null, string> { Value = JsonSerializer.Serialize(data) });
        }

        private void HandleSignal(Client sender, string messagetType, ByteString payload)
        {
            if (!_Monitors.TryGetValue(sender.HostId, out var data))
            {
                producer?.Produce("missing",
                    new Message<Null, string>
                    {
                        Value = JsonSerializer.Serialize(
                            new AcFunPendingMessage
                            {
                                UperId = sender.HostId,
                                LiveId = "missing",
                                MessageType = messagetType,
                                Payload = payload.ToBase64()
                            })
                    });
            }
            else
            {
                producer?.Produce("pending",
                    new Message<Null, string>
                    {
                        Value = JsonSerializer.Serialize(
                        new AcFunPendingMessage
                        {
                            UperId = data.UserId,
                            LiveId = data.LiveId,
                            MessageType = messagetType,
                            Payload = payload.ToBase64()
                        })
                    });
            }
        }
    }
}
