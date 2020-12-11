using AcFunDanmu.Enums;
using AVUP.Fun.Hubs;
using AVUP.Fun.Shared.Models;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AVUP.Fun.Services
{
    public sealed class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHubContext<DanmakuHub, IDanmaku> _hub;

        private static readonly ConsumerConfig consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "broker:9092",
            GroupId = "acfun.hub.worker",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        public Worker(ILogger<Worker> logger, IHubContext<DanmakuHub, IDanmaku> hub)
        {
            _logger = logger;
            _hub = hub;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe("hub");

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);
                switch (result.Message.Key)
                {
                    case PushMessage.ActionSignal.COMMENT:
                        {
                            var message = JsonSerializer.Deserialize<AcFunComment>(result.Message.Value);
                            _hub.Clients.Group($"{message.UperId}").SendComment(message);
                        }
                        break;
                    case PushMessage.ActionSignal.LIKE:
                        {
                            var message = JsonSerializer.Deserialize<AcFunLike>(result.Message.Value);
                            _hub.Clients.Group($"{message.UperId}").SendLike(message);
                        }
                        break;
                    case PushMessage.ActionSignal.ENTER_ROOM:
                        {
                            var message = JsonSerializer.Deserialize<AcFunEnter>(result.Message.Value);
                            _hub.Clients.Group($"{message.UperId}").SendEnter(message);
                        }
                        break;
                    case PushMessage.ActionSignal.FOLLOW:
                        {
                            var message = JsonSerializer.Deserialize<AcFunFollow>(result.Message.Value);
                            _hub.Clients.Group($"{message.UperId}").SendFollow(message);
                        }
                        break;
                    case PushMessage.ActionSignal.GIFT:
                        {
                            var message = JsonSerializer.Deserialize<AcFunGift>(result.Message.Value);
                            _hub.Clients.Group($"{message.UperId}").SendGift(message);
                        }
                        break;
                    case PushMessage.StateSignal.ACFUN_DISPLAY_INFO:
                        {
                            var message = JsonSerializer.Deserialize<AcFunBananaCount>(result.Message.Value);
                            _hub.Clients.Group($"{message.UserId}").SendBananaCount(message);
                        }
                        break;
                    case PushMessage.StateSignal.DISPLAY_INFO:
                        {
                            var message = JsonSerializer.Deserialize<AcFunDisplayInfo>(result.Message.Value);
                            _hub.Clients.Group($"{message.UserId}").SendDisplayInfo(message);
                        }
                        break;
                }
            }

            consumer.Close();
            return Task.CompletedTask;
        }
    }
}
