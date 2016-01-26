using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ColdCaller.Startup))]
namespace ColdCaller
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
