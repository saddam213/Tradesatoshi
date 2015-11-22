using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using TradeSatoshi.App_Start;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Models;

namespace TradeSatoshi
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

			// Enable the application to use a cookie to store information for the signed in user
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login")
			});
			// Use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
		}
	}
}