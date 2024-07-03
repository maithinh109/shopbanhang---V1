using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Shopbanhang.Startup))]
namespace Shopbanhang
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
