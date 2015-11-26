using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TradeSatoshi.Data;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Data.Entities;
using TradeSatoshi.Helpers;
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

		/// <summary>
		/// Generates unique hashed token based on the users security stamp to be sent via email.
		/// </summary>
		/// <param name="manager">The usermanager.</param>
		/// <param name="tokenType">Type of the token.</param>
		/// <param name="userid">The userid.</param>
		public async Task<string> GenerateUserTwoFactorTokenAsync(TwoFactorTokenType tokenType, string userid)
		{
			return await GenerateUserTokenAsync(tokenType.ToString(), userid);
		}

		/// <summary>
		/// Verifies the unique hashed token sent to the user
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="tokenType">Type of the token.</param>
		/// <param name="userid">The userid.</param>
		/// <param name="code">The code.</param>
		/// <param name="updateSecurityStamp">if set to <c>true</c> update the users security stamp. This will invalidate any other active tokens</param>
		public async Task<bool> VerifyUserTwoFactorTokenAsync(TwoFactorTokenType tokenType, string userid, string token, bool updateSecurityStamp = false)
		{
			if (await FindByIdAsync(userid) != null)
			{
				if (await VerifyUserTokenAsync(userid, tokenType.ToString(), token))
				{
					// update security stamp, this will invalidate any other tokens
					if (updateSecurityStamp)
					{
						await UpdateSecurityStampAsync(userid);
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Generates a short security code for the user to manually enter.
		/// </summary>
		/// <param name="manager">The usermanager.</param>
		/// <param name="codeType">Type of the code.</param>
		/// <param name="userid">The userid.</param>
		/// <returns>A numeric code</returns>
		public async Task<string> GenerateUserTwoFactorCodeAsync(TwoFactorType codeType, string userid)
		{
			if (codeType == TwoFactorType.EmailCode)
			{
				return await GenerateTwoFactorTokenAsync(userid, codeType.ToString());
			}
			return string.Empty;
		}

		/// <summary>
		/// Verifies the users short code.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="codeType">Type of the code.</param>
		/// <param name="userid">The userid.</param>
		/// <param name="code">The code.</param>
		public async Task<bool> VerifyUserTwoFactorCodeAsync(TwoFactorComponentType component, string userid, string data)
		{
			var user = await FindByIdAsync(userid);
			if (user == null)
				return false;

			var twofactorMethod = user.TwoFactor.FirstOrDefault(x => x.Component == component);
			if (twofactorMethod == null || twofactorMethod.Type == TwoFactorType.None)
				return true;

			if (twofactorMethod.Type == TwoFactorType.PinCode)
				return twofactorMethod.Data == data;

			if (twofactorMethod.Type == TwoFactorType.EmailCode)
				return await VerifyTwoFactorTokenAsync(userid, twofactorMethod.Type.ToString(), data);

			if (twofactorMethod.Type == TwoFactorType.GoogleCode)
			{
				return GoogleHelper.VerifyGoogleTwoFactorCode(twofactorMethod.Data, data);
			}

			return false;
		}

		public async Task<TwoFactorType> GetUserTwoFactorTypeAsync(string userId, TwoFactorComponentType component)
		{
			var user = await FindByIdAsync(userId);
			if (user == null)
				throw new UnauthorizedAccessException();

			var twofactorMethod = user.TwoFactor.FirstOrDefault(x => x.Component == component);
			return twofactorMethod == null
				? TwoFactorType.None
				: twofactorMethod.Type;
		}
	}


}