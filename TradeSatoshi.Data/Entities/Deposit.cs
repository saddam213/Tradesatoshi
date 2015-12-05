using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;

namespace TradeSatoshi.Data.Entities
{
	public class Deposit
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		public int CurrencyId { get; set; }

		public decimal Amount { get; set; }

		[MaxLength(256)]
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
