using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Entity
{
	public class Withdraw
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		[Index("IX_UserId")]
		public string UserId { get; set; }

		[Index("IX_CurrencyId")]
		public int CurrencyId { get; set; }

		[MaxLength(256)]
		public string Address { get; set; }

		public decimal Amount { get; set; }

		public decimal Fee { get; set; }

		public int Confirmations { get; set; }

		[MaxLength(256)]
		[Index("IX_TxId")]
		public string Txid { get; set; }

		public WithdrawType WithdrawType { get; set; }

		[Index("IX_Status")]
		public WithdrawStatus WithdrawStatus { get; set; }

		[MaxLength(1024)]
		public string TwoFactorToken { get; set; }

		public bool IsApi { get; set; }

		public DateTime TimeStamp { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
