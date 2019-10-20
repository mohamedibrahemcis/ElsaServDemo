using Elsa.Activities.Email.Extensions;
using Elsa.Activities.Http.Extensions;
using Elsa.Activities.Timers.Extensions;
using Elsa.Dashboard.Extensions;
using Elsa.Extensions;
using Elsa.Persistence.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Guides.Dashboard.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            
            services
                // Add services used for the workflows runtime.
                .AddWorkflows()
                .AddHttpActivities(options => options.Bind(Configuration.GetSection("Elsa:Http")))
                .AddEmailActivities(options => options.Bind(Configuration.GetSection("Elsa:Smtp")))
                .AddTimerActivities(options => options.Bind(Configuration.GetSection("Elsa:Timers")))
                
                // Add services used for the workflows dashboard.
                .AddElsaDashboard(options => options.DiscoverActivities())
                
                // Add services used for persistence. 
                .AddEntityFrameworkCoreWorkflowDefinitionStore()
                .AddEntityFrameworkCoreWorkflowInstanceStore()
                .AddEntityFrameworkCore(
                    options => options
                        .UseSqlite(Configuration.GetConnectionString("Sqlite"))
                );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .UseStaticFiles()
                .UseHttpActivities()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers())
                .UseWelcomePage();
        }
    }
}