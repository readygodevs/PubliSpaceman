using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PlanogramaGen.Startup))]
namespace PlanogramaGen
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
