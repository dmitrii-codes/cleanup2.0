using Microsoft.EntityFrameworkCore;
using Cleanup.Models;

namespace Cleanup.Models
{
    public class CleanupContext : DbContext
    {
        public CleanupContext(DbContextOptions<CleanupContext> options) : base(options) {}
        public DbSet<User> users {get;set;}
        public DbSet<CleanupEvent> cleanups {get;set;}
        public DbSet<PrivateMessage> privatemessages {get;set;}
        public DbSet<BoardMessage> boardmessages {get;set;}
        public DbSet<Image> images {get;set;}
    }
}