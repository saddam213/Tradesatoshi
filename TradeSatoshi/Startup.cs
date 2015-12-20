using Microsoft.Owin;
using Owin;
using TradeSatoshi.Web;


[assembly: OwinStartup(typeof(Startup))]
namespace TradeSatoshi.Web
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}
