using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace SlashCommands
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        public Startup(IHostingEnvironment env)
        {
        }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            var serverAddresses = app.ServerFeatures.Get<IServerAddressesFeature>();
            serverAddresses.Addresses.Clear();
            serverAddresses.Addresses.Add($"http://*:8462/");
            Console.WriteLine($"Magic happens at http://*:8462/");

            app.UseMvc();
        }
    }
}
