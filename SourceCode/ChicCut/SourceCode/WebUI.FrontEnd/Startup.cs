using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebUI.FrontEnd.Startup))]
namespace WebUI.FrontEnd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
