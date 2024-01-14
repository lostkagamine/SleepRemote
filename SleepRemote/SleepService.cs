using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using EmbedIO;
using EmbedIO.WebApi;

namespace SleepRemote;

public class SleepService
{
    private static IPAddress GetTailscaleIpv4()
    {
        var netIfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (var i in netIfaces)
        {
            // Kinda questionable, but I don't think there's any better way to do this. ~Homura
            if (!i.Name.Contains("Tailscale")) continue;
            
            var ipProps = i.GetIPProperties();
            foreach (var ip in ipProps.UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    return ip.Address;
            }
        }

        throw new Exception("No Tailscale IPv4 on system.");
    }

    internal static async Task StartServer(
        ILogger<WindowsBackgroundService> logger,
        CancellationToken stoppingToken)
    {
        var tsIp = GetTailscaleIpv4();
        logger.LogInformation("Discovered Tailscale IPv4 address: {ip}", tsIp.ToString());

        var server = new WebServer(o =>
                o.WithUrlPrefix($"http://{tsIp}:6789")
                    .WithMode(HttpListenerMode.EmbedIO))
            .WithWebApi("/", m => m.WithController<SleepController>());
            
        await server.RunAsync(stoppingToken);
        await Task.Delay(-1, stoppingToken);
    }
}