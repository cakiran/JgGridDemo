using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DemoGrids.Startup))]
namespace DemoGrids
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
