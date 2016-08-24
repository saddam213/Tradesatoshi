using System.ComponentModel.DataAnnotations;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Currency
{
	public class UpdateCurrencyModel
	{
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Symbol { get; set; }

		public CurrencyStatus Status { get; set; }
		public string StatusMessage { get; set; }
		public bool IsEnabled { get; set; }

		public int MinConfirmations { get; set; }
		public decimal TransferFee { get; set; }

		public decimal TradeFee { get; set; }
		public decimal MinTrade { get; set; }
		public decimal MaxTrade { get; set; }
		public decimal MinBaseTrade { get; set; }


		public decimal WithdrawFee { get; set; }
		public decimal MinWithdraw { get; set; }
		public decimal MaxWithdraw { get; set; }
		public WithdrawFeeType WithdrawFeeType { get; set; }
		public decimal ColdBalance { get; set; }
		public int MarketSortOrder { get; set; }
		public string Algo { get; set; }
		public CurrencyInterfaceType InterfaceType { get; set; }
		public CurrencyType Type { get; set; }
	}
}