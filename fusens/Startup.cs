using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(fusens.Startup))]
namespace fusens
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
