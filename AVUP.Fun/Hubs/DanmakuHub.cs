using AVUP.Fun.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace AVUP.Fun.Hubs
{
    public sealed class DanmakuHub : Hub<IDanmaku>
    {
        public const string PublicGroup = "public";

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, PublicGroup).ConfigureAwait(false);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.Items["userId"] as string).ConfigureAwait(false);
            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }

        public async Task ConnectToPublic()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, PublicGroup).ConfigureAwait(false);
        }

        public async Task ConnectToRoom(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId).ConfigureAwait(false);
            Context.Items.Add("userId", userId);
        }
    }

    public interface IDanmaku
    {
        public Task SendComment(AcFunComment userComment);
        public Task SendLike(AcFunLike userLike);
        public Task SendFollow(AcFunFollow userFollow);
        public Task SendEnter(AcFunEnter userEnter);
        public Task SendGift(AcFunGift userGift);
        public Task SendBananaCount(AcFunBananaCount bananaCount);
        public Task SendDisplayInfo(AcFunDisplayInfo displayInfo);

        public Task SendMonitor(AcFunLive live);
    }
}
