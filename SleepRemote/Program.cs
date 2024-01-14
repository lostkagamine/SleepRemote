using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using SleepRemote;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Windows Remote Sleep Service";
});

LoggerProviderOptions.RegisterProviderOptions
    <EventLogSettings, EventLogLoggerProvider>(builder.Services);

builder.Services.AddSingleton<SleepService>();
builder.Services.AddHostedService<WindowsBackgroundService>();

var host = builder.Build();
host.Run();