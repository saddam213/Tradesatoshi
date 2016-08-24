using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Common.Vote;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Attributes;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	public class VoteController : BaseController
	{
		public IVoteReader VoteReader { get; set; }
		public IVoteWriter VoteWriter { get; set; }
		public IBalanceReader BalanceReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }

		public async Task<ActionResult> Index()
		{
			var model = await VoteReader.GetVoteSettings();
			var tradePairs = await TradePairReader.GetTradePairs();
			var balances = await BalanceReader.GetBalances(User.Id());
			model.TradePairs = new List<TradePairModel>(tradePairs);
			model.Balances = new List<BalanceModel>(balances);
			return View(model);
		}

		[HttpPost]
		public async Task<ActionResult> GetPaidVotes(DataTablesModel param)
		{
			return DataTable(await VoteReader.GetVoteDataTable(param, VoteType.Paid));
		}

		[HttpPost]
		public async Task<ActionResult> GetFreeVotes(DataTablesModel param)
		{
			return DataTable(await VoteReader.GetVoteDataTable(param, VoteType.Free));
		}

		[HttpPost]
		public async Task<ActionResult> GetRejects(DataTablesModel param)
		{
			return DataTable(await VoteReader.GetRejectedDataTable(param));
		}

		[HttpPost]
		public async Task<ActionResult> GetPending(DataTablesModel param)
		{
			return DataTable(await VoteReader.GetPendingDataTable(param));
		}


		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> CreatePaidVote(int voteItemId, string voteItem)
		{
			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return UnauthorizedModal();

			var voteSettings = await VoteReader.GetVoteSettings();
			if (voteSettings == null)
				return ViewMessageModal(ViewMessageModel.Error("Invalid Request", "An unknown error occured."));

			var balance = await BalanceReader.GetBalance(User.Id(), voteSettings.CurrencyId);
			if (balance == null)
				return ViewMessageModal(ViewMessageModel.Error("Invalid Request", "An unknown error occured."));

			var model = new CreatePaidVoteModel
			{
				VoteItemId = voteItemId,
				VoteItem = voteItem,
				Price = voteSettings.Price,
				Balance = balance.Avaliable,
				Symbol = balance.Symbol,
				TwoFactorComponentType = TwoFactorComponentType.Transfer,
				TwoFactorType = await UserManager.GetUserTwoFactorTypeAsync(user.Id, TwoFactorComponentType.Transfer)
			};

			return View("CreatePaidVoteModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> CreatePaidVote(CreatePaidVoteModel model)
		{
			if (model.TwoFactorType == TwoFactorType.None)
				ModelState.Remove("Data");

			if (!ModelState.IsValid)
				return View("CreatePaidVoteModal", model);

			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return UnauthorizedModal();

			// Verify TwoFactor code.
			if (!await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponentType.Transfer, user.Id, model.Data))
			{
				ModelState.AddModelError("Data", "Invalid TwoFactor code.");
				return View("CreatePaidVoteModal", model);
			}

			var result = await VoteWriter.CreatePaidVote(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("CreatePaidVoteModal", model);

			return CloseModalSuccess(result.Message, "Vote Success");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> CreateFreeVote(CreateFreeVoteModel model)
		{
			if (!ModelState.IsValid)
				return JsonError();

			var result = await VoteWriter.CreateFreeVote(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return JsonError(result.FirstError, "Vote Failed", AlertType.Warning);

			return JsonSuccess(result.Message, "Vote Success");
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public ActionResult CreateVoteItem()
		{
			return View("CreateVoteItemModal", new CreateVoteItemModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> CreateVoteItem(CreateVoteItemModel model)
		{
			if (!ModelState.IsValid)
				return View("CreateVoteItemModal", model);

			var result = await VoteWriter.CreateVoteItem(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("CreateVoteItemModal", model);

			return CloseModalSuccess(result.Message, "Successfully Submitted");
		}


		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
		public async Task<ActionResult> AdminUpdateVoteItem(int voteItemId)
		{
			var model = await VoteReader.AdminGetVoteItem(voteItemId);
			if (model == null)
				return ViewMessageModal(ViewMessageModel.Error("Not Found", "VoteItem {0} not found.", voteItemId));

			return View("AdminUpdateVoteItemModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
		public async Task<ActionResult> AdminUpdateVoteItem(UpdateVoteItemModel model)
		{
			if (!ModelState.IsValid)
				return View("AdminUpdateVoteItemModal", model);

			var result = await VoteWriter.AdminUpdateVoteItem(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("AdminUpdateVoteItemModal", model);

			return CloseModal();
		}

		[HttpGet]

		public async Task<ActionResult> ViewVoteItem(int voteItemId)
		{
			var model = await VoteReader.GetVoteItem(voteItemId);
			if (model == null)
				return ViewMessageModal(ViewMessageModel.Error("Not Found!", "VoteItem not found."));

			return View("ViewVoteItemModal", model);
		}


		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
		public async Task<ActionResult> AdminUpdateVoteSettings()
		{
			var model = await VoteReader.GetVoteSettings();
			if (model == null)
				return ViewMessageModal(ViewMessageModel.Error("Not Found", "Vote settings not found."));

			return View("AdminUpdateVoteSettingsModal", new UpdateVoteSettingsModel
			{
				Next = model.NextVote,
				Price = model.Price,
				IsFreeEnabled = model.IsFreeEnabled,
				IsPaidEnabled = model.IsPaidEnabled,
				CurrencyId = model.CurrencyId,
				Currencies = await CurrencyReader.GetCurrencies()
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Administrator, SecurityRole.Moderator1)]
		public async Task<ActionResult> AdminUpdateVoteSettings(UpdateVoteSettingsModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Currencies = await CurrencyReader.GetCurrencies();
				return View("AdminUpdateVoteSettingsModal", model);
			}

			var result = await VoteWriter.AdminUpdateVoteSettings(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Currencies = await CurrencyReader.GetCurrencies();
				return View("AdminUpdateVoteSettingsModal", model);
			}

			return CloseModal();
		}
	}
}