using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Cleanup.Models;
using Microsoft.Extensions.Logging;
// this is the hubs that are opened in the chatHub.cs file
using Cleanup.Hubs;

namespace Cleanup
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CleanupContext>(options => options.UseMySql(Configuration["DbInfo:ConnectionString"]));
            services.AddSession();
            // to have access to this you need to transfer the signalr.json file from nodemodules -dist-browser to root-lib-signalr
            services.AddSignalR();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole();
            }
            else
            {
                app.UseExceptionHandler("/Error/Default");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseSignalR(routes => {
                // this opens the live hub at /hubs/chat. this is where all connections should link to
                routes.MapHub<ChatHub>("/hubs/chat");
            });
            app.UseMvc();
        }
    }
}
