using TradeSatoshi;
using Microsoft.Owin;
using Owin;
using TradeSatoshi.Common;
using TradeSatoshi.Core;
using System.Web.Mvc;


[assembly: OwinStartup(typeof(Startup))]
namespace TradeSatoshi
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}
