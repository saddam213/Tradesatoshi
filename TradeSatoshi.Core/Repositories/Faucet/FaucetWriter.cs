using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Faucet;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Core.Faucet
{
	public class FaucetWriter : IFaucetWriter
	{
		public ITradeService TradeService { get; set; }

		public async Task<WriterResult<bool>> Claim(string userId, string ipaddress, int currencyId)
		{
			var result = await TradeService.QueueFaucetPayment(new CreateFaucetPaymentModel
			{
				UserId = userId,
				IPAddress = ipaddress,
				CurrencyId = currencyId,
				IsApi = false
			});
			if (result.HasError)
				return WriterResult<bool>.ErrorResult(result.Error);

			return WriterResult<bool>.SuccessResult(result.Message);
		}
	}
}
