using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LaserArt.Startup))]
namespace LaserArt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
