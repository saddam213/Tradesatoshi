using System.ComponentModel.DataAnnotations;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Transfer
{
	public class CreateTransferModel : ITwoFactorEntry, ITradeItem
	{
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
		public decimal Fee { get; set; }
		public decimal Balance { get; set; }
		public TransferType TransferType { get; set; }

		[Required]
		[MaxLength(128)]
		public string Recipient { get; set; }

		[Required]
		[Range(typeof (decimal), "0.00000001", "1000000000")]
		public decimal Amount { get; set; }

		public string ToUser { get; set; }

		#region ITradeItem

		public string UserId { get; set; }
		public bool IsApi { get; set; }

		#endregion

		#region ITwoFactorEntry

		[Required]
		[MaxLength(128)]
		public string Data { get; set; }

		public TwoFactorType TwoFactorType { get; set; }
		public TwoFactorComponentType TwoFactorComponentType { get; set; }

		#endregion
	}
}