using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Entity
{
	public class FaucetPayment
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		public int CurrencyId { get; set; }

		public decimal Amount { get; set; }

		[MaxLength(128)]
		public string IPAddress { get; set; }

		public DateTime Timestamp { get; set; }


		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
