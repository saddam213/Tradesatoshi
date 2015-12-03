using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Core.Admin;
using TradeSatoshi.Helpers;
using TradeSatoshi.Models.User;

namespace TradeSatoshi.Controllers
{
	[AuthorizeSecurityRole(SecurityRole.Standard)]
	public class UserController : BaseController
	{
		public IUserReader UserReader { get; set; }
		public IUserWriter UserWriter { get; set; }

		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		#region Profile

		[HttpGet]
		public async Task<ActionResult> UserProfile()
		{
			var user = await UserManager.FindByIdAsync(User.Id());
			var model =new UserProfileModel
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

		[HttpGet]
		public ActionResult Security()
		{
			return PartialView("_SecurityPartial", new UserSecurityModel());
		}

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

		#region Balances

		[HttpGet]
		public ActionResult Balances()
		{
			return PartialView("_BalancesPartial", new UserBalancesModel());
		}

		#endregion

	}
}