using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Services.EmailService;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Data;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	public class TradePairController : BaseController
	{
		#region Properties

		public ITradePairWriter TradePairWriter { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }

		#endregion

		[HttpGet]
		public async Task<ActionResult> Create()
		{
			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return Unauthorized();

			var currencies = await CurrencyReader.GetCurrencies();

			return View("CreateTradePairModal", new CreateTradePairModel
			{
				Currencies = new List<CurrencyModel>(currencies)
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(CreateTradePairModel model)
		{
			if (!ModelState.IsValid)
				return View("CreateTradePairModal", model);

			var result = await TradePairWriter.CreateTradePair(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("CreateTradePairModal", model);

			return CloseModal();
		}


		[HttpGet]
		public async Task<ActionResult> Update(int tradePairId)
		{
			var user = await UserManager.FindByIdAsync(User.Id());
			if (user == null)
				return Unauthorized();

			var model = await TradePairReader.GetTradePairUpdate(tradePairId);
			if (model == null)
				return ViewMessageModal(ViewMessageModel.Error("Not Found!", "TradePair '{0}' not found.", tradePairId));

			return View("UpdateTradePairModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Update(UpdateTradePairModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateCTradePairModal", model);

			var result = await TradePairWriter.UpdateTradePair(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateTradePairModal", model);

			return CloseModal();
		}
	}
}