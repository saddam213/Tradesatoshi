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
	public class Deposit
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		[Index("IX_UserTxId", 1, IsUnique = true)]
		public string UserId { get; set; }

		public int CurrencyId { get; set; }

		public decimal Amount { get; set; }

		[MaxLength(256)]
		[Index("IX_UserTxId", 2, IsUnique = true)]
		public string Txid { get; set; }
	
		public int Confirmations { get; set; }

		public DepositType DepositType { get; set; }

		public DepositStatus DepositStatus { get; set; }
		
		public DateTime TimeStamp { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}
