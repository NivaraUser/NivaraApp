using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Nivara.Core.ChatModule;
using Nivara.Models;
using Nivara.Web.ChatHub.UserConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Web.ChatHub
{
    public class MessengerHub : Hub
    {
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IChatService _chatService;
        private readonly IHubContext<MessengerHub> _notificationUserHubContext;
        public MessengerHub(IUserConnectionManager userConnectionManager, IChatService chatService, IHubContext<MessengerHub> notificationUserHubContext)
        {
            _userConnectionManager = userConnectionManager;
            this._chatService = chatService;
            _notificationUserHubContext = notificationUserHubContext;
        }
        public string GetConnectionId()
        {
            var httpContext = this.Context.GetHttpContext();
            var userId = httpContext.Request.Query["userId"];
            _userConnectionManager.KeepUserConnection(userId, Context.ConnectionId);

            return Context.ConnectionId;
        }
        //Called when a connection with the hub is terminated.
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            //get the connectionId
            var connectionId = Context.ConnectionId;
            _userConnectionManager.RemoveUserConnection(connectionId);
            var value = await Task.FromResult(0);
        }

        public async Task SendMessage(string model)
        {
            var SerializeResp = JsonConvert.DeserializeObject<ChatModel>(model);
            await _chatService.CreateChat(SerializeResp);
            var connections = _userConnectionManager.GetUserConnections(SerializeResp.ReceiverUserId);
            if (connections != null && connections.Count > 0)
            {
                foreach (var connectionId in connections)
                {
                    await _notificationUserHubContext.Clients.Client(connectionId).SendAsync("sendToUser", SerializeResp);
                }
            }
        }
    }
}
