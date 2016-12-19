using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Faucet;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Attributes;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	
	public class FaucetController : BaseController
	{
		public IBalanceReader BalanceReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public IFaucetReader FaucetReader { get; set; }
		public IFaucetWriter FaucetWriter { get; set; }
		public ITransferReader TransferReader { get; set; }
		public ITransferWriter TransferWriter { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var tradePairs = await TradePairReader.GetTradePairs();
			var balances = await BalanceReader.GetBalances(User.Id());
			return View(new FaucetViewModel
			{
				TradePairs = new List<TradePairModel>(tradePairs),
				Balances = new List<BalanceModel>(balances),
			});
		}

		[HttpPost]
		public async Task<ActionResult> GetFaucets(DataTablesModel param)
		{
			return DataTable(await FaucetReader.GetFaucetDataTable(param, User.Id()));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> Claim(int id)
		{
			var result = await FaucetWriter.Claim(User.Id(), Request.GetIPAddress(), id);
			if (!ModelState.IsWriterResultValid(result))
				return JsonError(result.FirstError, "Faucet Claim Failed", AlertType.Warning);

			return JsonSuccess(result.Message, "Faucet Claim Success");
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> Donate(int currencyId)
		{
			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return Unauthorized();

			var model = await TransferReader.GetCreateTransfer(User.Id(), currencyId);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Invalid Request", "An unknown error occured."));

			model.Recipient = "Faucet";
			model.TwoFactorComponentType = TwoFactorComponentType.Transfer;
			model.TwoFactorType = await UserManager.GetUserTwoFactorTypeAsync(user.Id, TwoFactorComponentType.Transfer);

			return View("DonateModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Standard)]
		public async Task<ActionResult> Donate(CreateTransferModel model)
		{
			if (model.TwoFactorType == TwoFactorType.None)
			{
				// If there is no tfa remove the Data field validation.
				ModelState.Remove("Data");
			}

			if (!ModelState.IsValid)
				return View("DonateModal", model);

			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return Unauthorized();

			// Verify TwoFactor code.
			if (!await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponentType.Transfer, user.Id, model.Data))
			{
				ModelState.AddModelError("Data", "Invalid TwoFactor code.");
				return View("DonateModal", model);
			}

			model.UserId = user.Id;
			model.ToUser = Constants.SystemFaucetUserId;
			model.TransferType = TransferType.Faucet;
			var result = await TransferWriter.CreateTransfer(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("DonateModal", model);

			return ViewMessageModal(new ViewMessageModel(ViewMessageType.Success, "Donation Success", "Your donation has been sucessfully processed."));
		}
	}
}