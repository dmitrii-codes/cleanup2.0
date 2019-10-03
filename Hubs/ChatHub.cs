using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Cleanup.Models;

namespace Cleanup.Hubs
{

    public class ChatHub : Hub
    {
        private CleanupContext _context;

        public ChatHub (CleanupContext context)
        {
            _context = context;
        }

        public Task Send(string message)
        {
            Live newmsg = new Live {
                Messages = message
            };
            _context.Add(newmsg);
            _context.SaveChanges();
            List<Live> allmsgs = _context.livemessages.OrderByDescending(m => m.CreatedAt).ToList();
            if (allmsgs.Count > 20)
            {
                _context.livemessages.Remove(allmsgs.Last());
                _context.SaveChanges();
            }
            return Clients.All.SendAsync("SendMessage", message);
        }
    }
}