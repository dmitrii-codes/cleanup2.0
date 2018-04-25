
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cleanup.Hubs
{

    public class ChatHub : Hub
    {
        public ChatHub()
        {}
        public Task Send(string message)
        {
            return Clients.All.SendAsync("SendMessage", message);
        }
    }
}