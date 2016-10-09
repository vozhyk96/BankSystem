using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BankSystem.Startup))]
namespace BankSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
