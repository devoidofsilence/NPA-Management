using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NPAManagement.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace NPAManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
