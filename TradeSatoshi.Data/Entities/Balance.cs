using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Data.Entities
{
	public class Balance
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }
		
		public int CurrencyId { get; set; }

		public decimal Total { get; set; }
		
		public decimal Unconfirmed { get; set; }

		public decimal HeldForTrades { get; set; }

		public decimal PendingWithdraw { get; set; }

		[NotMapped]
		public decimal Avaliable
		{
			get { return Total - (Unconfirmed + HeldForTrades + PendingWithdraw); }
		}

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}
