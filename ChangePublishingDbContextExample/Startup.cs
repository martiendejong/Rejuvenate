using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ChangePublishingDbContextExample.Startup))]
namespace ChangePublishingDbContextExample
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
