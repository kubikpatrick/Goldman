using Goldman.Daemon.Extensions;
using Goldman.Daemon.Services;
using Goldman.Daemon.Workers;
using Goldman.Models.Devices;

namespace Goldman.Daemon;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddLogging();
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton(new Device());
        builder.Services.AddSingleton<DeviceIdAccessor>();
        builder.Services.AddSingleton<JwtService>();
        builder.Services.AddHostedService<JwtRefreshWorker>();
        builder.Services.AddHostedService<LocationWorker>();

        var host = builder.Build();
        
        host.Run();
    }
}