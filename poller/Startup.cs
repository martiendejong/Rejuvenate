using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RejuvenatingExample.Startup))]
namespace RejuvenatingExample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
