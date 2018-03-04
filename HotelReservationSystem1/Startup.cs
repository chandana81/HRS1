using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HotelReservationSystem1.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace HotelReservationSystem1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
