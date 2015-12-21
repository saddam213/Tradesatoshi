using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Data.Entities
{
	public class TransferHistory
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		[MaxLength(128)]
		public string ToUserId { get; set; }

		public int CurrencyId { get; set; }

		public TransferType TransferType { get; set; }

		public decimal Amount { get; set; }

		public decimal Fee { get; set; }

		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		[ForeignKey("ToUserId")]
		public virtual ApplicationUser ToUser { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }
	}
}
