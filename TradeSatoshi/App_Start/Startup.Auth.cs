using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Web;
using TradeSatoshi.App_Start;
using TradeSatoshi.Common.Data.Entities;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Hubs;
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
				Provider = new CookieAuthenticationProvider
				{
					// Enables the application to validate the security stamp when the user logs in.
					// This is a security feature which is used when you change a password or add an external login to your account.  
					OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
											 validateInterval: TimeSpan.FromMinutes(30),
											 regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),
					OnApplyRedirect = ctx => { if (!IsApiRequest(ctx.Request)) { ctx.Response.Redirect(ctx.RedirectUri); } }
				},
				LoginPath = new PathString("/Account/Login")
			});
			// Use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

			//Signalr
			var hubConfiguration = new HubConfiguration
			{
#if DEBUG
				EnableDetailedErrors = true,
#else
				EnableDetailedErrors = false
#endif
			};
			GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new HubUserIdProvider());
			app.MapSignalR(hubConfiguration);
		}

		private static bool IsApiRequest(IOwinRequest request)
		{
			string apiPath = VirtualPathUtility.ToAbsolute("~/Api/");
			return request.Uri.LocalPath.StartsWith(apiPath);
		}
	}
}