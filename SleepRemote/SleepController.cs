using System.Text;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace SleepRemote;

public class SleepController : WebApiController
{
    [Route(HttpVerbs.Post, "/sleep")]
    public async Task SleepSystem()
    {
        // I'm going to launch a thread here so I give the system time to respond to the HTTP request.
        // This still isn't an amazing solution, but it should at least work out. ~Homura
        new Thread(() =>
        {
            Thread.Sleep(1000); // Should be enough time.
            Native.SuspendSystem(Native.SuspendMode.Sleep);
        }).Start();
        
        await HttpContext.SendStringAsync("done", "text/html", Encoding.UTF8);
    }
    
    [Route(HttpVerbs.Post, "/shutdown")]
    public async Task ShutdownSystem()
    {
        new Thread(() =>
        {
            Thread.Sleep(1000);
            Native.ShutdownSystem(Native.ShutdownMode.Shutdown);
        }).Start();
        
        await HttpContext.SendStringAsync("done", "text/html", Encoding.UTF8);
    }
    
    [Route(HttpVerbs.Post, "/reboot")]
    public async Task RebootSystem()
    {
        new Thread(() =>
        {
            Thread.Sleep(1000);
            Native.ShutdownSystem(Native.ShutdownMode.Reboot);
        }).Start();
        
        await HttpContext.SendStringAsync("done", "text/html", Encoding.UTF8);
    }
}