using System.ComponentModel.DataAnnotations;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Withdraw
{
	public class CreateWithdrawModel : ITwoFactorEntry
	{
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }

		public decimal Fee { get; set; }
		public WithdrawFeeType WithdrawFeeType { get; set; }
		public decimal MinWithdraw { get; set; }
		public decimal MaxWithdraw { get; set; }
		public decimal Balance { get; set; }

		[Required]
		[MaxLength(128)]
		public string Address { get; set; }

		[Required]
		public decimal Amount { get; set; }

		public string ConfirmationToken { get; set; }

		#region ITwoFactorEntry

		[Required]
		[MaxLength(128)]
		public string Data { get; set; }
		public TwoFactorType TwoFactorType { get; set; }
		public TwoFactorComponentType TwoFactorComponentType { get; set; }

		#endregion

	}
}
