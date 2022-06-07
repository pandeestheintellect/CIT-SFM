using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BuildInspect.App_Start.Startup))]
namespace BuildInspect.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
    
        }
    }
}
