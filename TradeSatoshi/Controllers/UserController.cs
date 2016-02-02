using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Web.Helpers;
using TradeSatoshi.Common.User;
using TradeSatoshi.Web.Attributes;
using TradeSatoshi.Common.TradePair;
using System.Collections.Generic;

namespace TradeSatoshi.Web.Controllers
{
	[AuthorizeSecurityRole(SecurityRole.Standard)]
	public class UserController : BaseController
	{
		public IBalanceReader BalanceReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{

			var tradePairs = await TradePairReader.GetTradePairs();
			var balances = await BalanceReader.GetBalances(User.Id());

			return View(new UserSettingsModel
			{
				SecurityModel = new UserSecurityModel(),
				UserProfile = new UserProfileModel(),
				TradePairs = new List<TradePairModel>(tradePairs),
				Balances = new List<BalanceModel>(balances)
			});
		}

		#region Profile

		[HttpGet]
		public async Task<ActionResult> UserProfile()
		{
			var user = await UserManager.FindByIdAsync(User.Id());
			var model = new UserProfileModel
			{
				BirthDate = user.Profile.BirthDate,
				City = user.Profile.City,
				Country = user.Profile.Country,
				Address = user.Profile.Address,
				FirstName = user.Profile.FirstName,
				LastName = user.Profile.LastName,
				PostCode = user.Profile.PostCode,
				State = user.Profile.State,
				CanUpdate = user.Profile.CanUpdate()
			};
			return PartialView("_ProfilePartial", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UserProfile(UserProfileModel model)
		{
			if (!ModelState.IsValid)
				return PartialView("_ProfilePartial", model);

			var user = await UserManager.FindByIdAsync(User.Id());
			if (user.Profile.CanUpdate())
			{
				user.Profile.Address = model.Address;
				user.Profile.BirthDate = model.BirthDate;
				user.Profile.City = model.City;
				user.Profile.Country = model.Country;
				user.Profile.FirstName = model.FirstName;
				user.Profile.LastName = model.LastName;
				user.Profile.PostCode = model.PostCode;
				user.Profile.State = model.State;

				await UserManager.UpdateAsync(user);
				model.CanUpdate = user.Profile.CanUpdate();
			}

			return PartialView("_ProfilePartial", model);
		}

		#endregion

		#region Security

		[HttpPost]
		public async Task<ActionResult> ChangePassword(UserSecurityModel model)
		{
			if (!ModelState.IsValid)
				return PartialView("_PasswordPartial", model);

			var result = await UserManager.ChangePasswordAsync(User.Id(), model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				return PartialView("_PasswordPartial", model);
			}

			AddErrors(result);
			return PartialView("_PasswordPartial", model);
		}

		#endregion
	}
}