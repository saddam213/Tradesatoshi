using System.Collections.Generic;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.TradePair;

namespace TradeSatoshi.Common.Faucet
{
	public class FaucetViewModel : ITradeSidebarModel
	{
		public FaucetViewModel()
		{
			Balances = new List<BalanceModel>();
			TradePairs = new List<TradePairModel>();
			//CurrencyStatus = new List<CurrencyStatusModel>();
		}
	
		public List<BalanceModel> Balances { get; set; }
		//public List<CurrencyStatusModel> CurrencyStatus { get; set; }
		public List<TradePairModel> TradePairs { get; set; }
	}
}