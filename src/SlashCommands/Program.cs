using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Sinks.RollingFile;

namespace SlashCommands
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(Path.Combine("logs", "log-{Date}.txt"))
                .WriteTo.ColoredConsole()
                .CreateLogger();

            Serilog.Log.Information("Starting SlashCommands");

            var hostingApplication = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            hostingApplication.Run();
        }
    }
}
