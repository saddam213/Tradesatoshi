using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Data.Entities;
using TradeSatoshi.Models;

namespace TradeSatoshi.App_Start
{
	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store)
		{
			this.UserValidator = new UserValidator<ApplicationUser>(this) { AllowOnlyAlphanumericUserNames = false, RequireUniqueEmail = true };
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
			manager.UserValidator = new UserValidator<ApplicationUser>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true,
			};

			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = true,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};

			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(24);
			manager.MaxFailedAccessAttemptsBeforeLockout = 3;

			manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>());


			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ConfirmUser"));
			}
			return manager;
		}
	}
}