using Microsoft.Owin;
using Owin;
using System.Net;
using System.Net.Sockets;
using System.Text;

[assembly: OwinStartupAttribute(typeof(WebSite.Startup))]
namespace WebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           
            ConfigureAuth(app);
            var endPoint = new IPEndPoint(Dns.GetHostAddresses("73d09dd5.carbon.hostedgraphite.com")[0], 2003);
            var bytes = Encoding.ASCII.GetBytes("045a9911-4ed9-41a6-bc20-cf8ff37a358e.test.testing 1.2\n");
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) { Blocking = false };
            sock.SendTo(bytes, endPoint);
        }
    }
}
