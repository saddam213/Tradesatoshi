﻿using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Services.EmailService;
using TradeSatoshi.Common.TwoFactor;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Entity;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	//[Authorize]
	public class TwoFactorController : BaseController
	{
		#region Properties

		public IEmailService EmailService { get; set; }

		#endregion

		[Authorize]
		[ChildActionOnly]
		public ActionResult GetTwoFactor(TwoFactorComponentType component)
		{
			var user = UserManager.FindById(User.Id());
			if (user == null)
				return Unauthorized();

			var twoFactor = user.TwoFactor.FirstOrDefault(x => x.Component == component)
				?? new UserTwoFactor { Type = TwoFactorType.None };
			return PartialView("_ViewPartial", new ViewTwoFactorModel
			{
				Type = twoFactor.Type,
				ComponentType = component
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Remove(TwoFactorComponentType componentType)
		{
			var user = UserManager.FindById(User.Id());
			if (user == null)
				return Unauthorized();

			var twofactor = user.TwoFactor.FirstOrDefault(x => x.Component == componentType) ?? new UserTwoFactor { Type = TwoFactorType.None };
			if (twofactor.Type == TwoFactorType.EmailCode)
			{
				var twofactorCode = await UserManager.GenerateUserTwoFactorCodeAsync(TwoFactorType.EmailCode, user.Id);
				if (!await EmailService.Send(EmailType.TwoFactorUnlockCode, user, Request.GetIPAddress(), new EmailParam("[TFACODE]", twofactorCode), new EmailParam("[TFATYPE]", componentType)))
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, "Email Send Failed!", "An error occured sending twofactor code email, If problems persists please contact <a href='/Support'>Support</a>"));
				}
			}

			return View("Remove", new RemoveTwoFactorModel
			{
				Type = twofactor.Type,
				ComponentType = componentType,
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Remove(RemoveTwoFactorModel model)
		{
			if (!ModelState.IsValid)
				return View("Remove", model);

			var user = UserManager.FindById(User.Id());
			if (user == null)
				return Unauthorized();

			var twofactor = user.TwoFactor.FirstOrDefault(x => x.Component == model.ComponentType && x.Type == model.Type);
			if (twofactor == null)
				return RedirectToRoute("Security");

			if (!await UserManager.VerifyUserTwoFactorCodeAsync(model.ComponentType, user.Id, model.Data))
			{
				// failed to validate last TFA
				ModelState.AddModelError("", "Incorrect TFA code.");
				return View("Remove", model);
			}

			// Delete TFA
			twofactor.ClearData();
			twofactor.Type = TwoFactorType.None;
			twofactor.Updated = DateTime.UtcNow;
			await UserManager.UpdateAsync(user);

			return RedirectToRoute("Security");
		}

		[HttpGet]
		[Authorize]
		public ActionResult Create(TwoFactorComponentType componentType)
		{
			var user = UserManager.FindById(User.Id());
			if (user == null)
				return Unauthorized();

			// If twofactor exists something is dodgy, return unauthorised
			var twofactor = user.TwoFactor.FirstOrDefault(x => x.Component == componentType && x.Type != TwoFactorType.None);
			if (twofactor != null)
				return RedirectToRoute("Security");

			return View(new CreateTwoFactorModel
			{
				ComponentType = componentType,
				GoogleData = GoogleHelper.GetGoogleTwoFactorData(user.UserName)
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(CreateTwoFactorModel model)
		{
			if (!model.IsValid(ModelState))
				return View("Create", model);

			var user = UserManager.FindById(User.Id());
			if (user == null)
				return Unauthorized();

			// If twofactor exists something is dodgy, return unauthorised
			var twofactor = user.TwoFactor.FirstOrDefault(x => x.Component == model.ComponentType);
			if (twofactor != null && twofactor.Type != TwoFactorType.None)
				return RedirectToRoute("Security");

			// If no TFA exists, create and redirect to TFA view partial
			if (twofactor == null)
			{
				user.TwoFactor.Add(new UserTwoFactor
				{
					Type = model.Type,
					Component = model.ComponentType,
					Data = model.Data,
					Data2 = model.GoogleData.PublicKey,
					Created = DateTime.UtcNow,
					Updated = DateTime.UtcNow,
					IsEnabled = true
				});
				await UserManager.UpdateAsync(user);
				return RedirectToRoute("Security");
			}

			twofactor.ClearData();
			twofactor.Type = model.Type;
			twofactor.Data = model.Data;
			twofactor.Data2 = model.GoogleData.PublicKey;
			twofactor.Updated = DateTime.UtcNow;
			await UserManager.UpdateAsync(user);
			return RedirectToRoute("Security");
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> SendEmailCode(TwoFactorComponentType componentType, string dataEmail)
		{
			var user = UserManager.FindById(User.Id());
			if (user == null)
				return JsonError("Unauthorized");

			if (!ValidationHelpers.IsValidEmailAddress(dataEmail))
				return JsonError($"'{dataEmail}' is an invalid email address.");

			var twofactorCode = await UserManager.GenerateUserTwoFactorCodeAsync(TwoFactorType.EmailCode, User.Id());
			if (await EmailService.Send(EmailType.TwoFactorUnlockCode, user, Request.GetIPAddress(), new EmailParam("[TFACODE]", twofactorCode), new EmailParam("[TFATYPE]", componentType)))
			{
				return JsonSuccess();
			}
			return JsonError();
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> VerifyEmailCode(TwoFactorComponentType componentType, string code)
		{
			var user = UserManager.FindById(User.Id());
			if (user == null)
				return JsonError("Unauthorized");

			if (await UserManager.VerifyTwoFactorTokenAsync(user.Id, TwoFactorType.EmailCode.ToString(), code))
			{
				return JsonSuccess();
			}
			return JsonError();
		}

		[HttpPost]
		[Authorize]
		public ActionResult VerifyGoogleCode(string key, string code)
		{
			if (GoogleHelper.VerifyGoogleTwoFactorCode(key, code))
			{
				return JsonSuccess();
			}
			return JsonError();
		}


		[HttpPost]
		public async Task<ActionResult> SendTwoFactorEmailCode(TwoFactorComponentType componentType)
		{
			var twoFactor = await UserManager.GetUserTwoFactorAsync(User.Id(), componentType);
			if (twoFactor == null || twoFactor.Type != TwoFactorType.EmailCode)
				return JsonError("Unauthorized");

			var twofactorCode = await UserManager.GenerateUserTwoFactorCodeAsync(TwoFactorType.EmailCode, User.Id());
			switch (componentType)
			{
				case TwoFactorComponentType.Login:
					if (await EmailService.Send(EmailType.TwoFactorLogin, twoFactor.User, Request.GetIPAddress(), new EmailParam("[TFACODE]", twofactorCode)))
					{
						return JsonSuccess();
					}
					break;
				case TwoFactorComponentType.Withdraw:
					if (await EmailService.Send(EmailType.TwoFactorWithdraw, twoFactor.User, Request.GetIPAddress(), new EmailParam("[TFACODE]", twofactorCode)))
					{
						return JsonSuccess();
					}
					break;
				default:
					break;
			}
			return JsonError();
		}
	}
}