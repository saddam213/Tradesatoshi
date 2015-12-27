using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Services.EmailService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Data;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	public class TransferController : BaseController
	{
		#region Properties

		public ITransferWriter TransferWriter { get; set; }
		public ITransferReader TransferReader { get; set; }

		#endregion

		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<ActionResult> Create(int currencyId)
		{
			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return Unauthorized();

			var model = await TransferReader.GetCreateTransferAsync(User.Id(), currencyId);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Invalid Request", "An unknown error occured."));

			model.TwoFactorComponentType = TwoFactorComponentType.Transfer;
			model.TwoFactorType = await UserManager.GetUserTwoFactorTypeAsync(user.Id, TwoFactorComponentType.Transfer);

			return View("CreateTransferModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(CreateTransferModel model)
		{
			if (model.TwoFactorType == TwoFactorType.None)
			{
				// If there is no tfa remove the Data field validation.
				ModelState.Remove("Data");
			}

			if (!ModelState.IsValid)
				return View("CreateTransferModal", model);

			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return Unauthorized();

			var recipient = await UserManager.FindByNameAsync(model.Recipient);
			if (recipient == null)
			{
				ModelState.AddModelError("Recipient", string.Format("UserName '{0}' not found.", model.Recipient));
				return View("CreateTransferModal", model);
			}

			// Verify TwoFactor code.
			if (!await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponentType.Transfer, user.Id, model.Data))
			{
				ModelState.AddModelError("Data", "Invalid TwoFactor code.");
				return View("CreateTransferModal", model);
			}

			model.UserId = user.Id;
			model.ToUser = recipient.Id;
			var result = await TransferWriter.CreateTransferAsync(model);
			if (result.HasError)
			{
				ModelState.AddModelError("", result.Error);
				return View("CreateTransferModal", model);
			}

			return ViewMessageModal(new ViewMessageModel(ViewMessageType.Success, "Transfer Success", "Your transfer request has been sucessfully processed."));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UserSearch(string searchTerm)
		{
			var recipient = await UserManager.FindByNameAsync(searchTerm);
			return recipient != null
				? JsonSuccess("Sucessfully found user '{0}'", recipient.UserName)
				: JsonError("UserName '{0}' not found.", searchTerm);
		}
	}
}