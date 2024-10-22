﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Entity
{
	public class TradeHistory
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		[MaxLength(128)]
		public string ToUserId { get; set; }

		public int TradePairId { get; set; }

		public int CurrencyId { get; set; }

		[Index("IX_Type")]
		public TradeType TradeHistoryType { get; set; }

		public decimal Amount { get; set; }

		public decimal Rate { get; set; }

		public decimal Fee { get; set; }

		public DateTime Timestamp { get; set; }

		public bool IsApi { get; set; }


		[ForeignKey("UserId")]
		public virtual User User { get; set; }

		[ForeignKey("ToUserId")]
		public virtual User ToUser { get; set; }

		[ForeignKey("TradePairId")]
		public virtual TradePair TradePair { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }
	}
}
