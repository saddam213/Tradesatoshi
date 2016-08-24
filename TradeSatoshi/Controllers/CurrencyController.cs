using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Services.EmailService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Data;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Attributes;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	[AuthorizeSecurityRole(SecurityRole.Administrator)]
	public class CurrencyController : BaseController
	{
		#region Properties

		public ICurrencyWriter CurrencyWriter { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }

		#endregion

		[HttpGet]
		public async Task<ActionResult> Create()
		{
			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return Unauthorized();

			return View("CreateCurrencyModal", new CreateCurrencyModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(CreateCurrencyModel model)
		{
			if (!ModelState.IsValid)
				return View("CreateCurrencyModal", model);

			var result = await CurrencyWriter.CreateCurrency(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("CreateCurrencyModal", model);

			return CloseModal();
		}


		[HttpGet]
		public async Task<ActionResult> Update(int currencyId)
		{
			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return Unauthorized();

			var model = await CurrencyReader.GetCurrencyUpdate(currencyId);
			if (model == null)
				return ViewMessageModal(ViewMessageModel.Error("Not Found!", "Currency '{0}' not found.", currencyId));

			return View("UpdateCurrencyModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Update(UpdateCurrencyModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateCurrencyModal", model);

			var result = await CurrencyWriter.UpdateCurrency(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateCurrencyModal", model);

			return CloseModal();
		}
	}
}