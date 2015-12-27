using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Data.Entities
{
	public class Trade
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		public int TradePairId { get; set; }

		public TradeType TradeType { get; set; }

		public decimal Amount { get; set; }

		public decimal Rate { get; set; }

		public decimal Fee { get; set; }

		public DateTime Timestamp { get; set; }

		public TradeStatus Status { get; set; }

		public decimal Remaining { get; set; }

		public bool IsApi { get; set; }


		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		[ForeignKey("TradePairId")]
		public virtual TradePair TradePair { get; set; }
	}
}
