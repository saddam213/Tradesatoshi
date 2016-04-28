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
	public class TradePair
	{
		public int Id { get; set; }

		public int CurrencyId1 { get; set; }

		public int CurrencyId2 { get; set; }

		public decimal LastTrade { get; set; }

		public double Change { get; set; }

		public TradePairStatus Status { get; set; }

		[MaxLength(4000)]
		public string StatusMessage { get; set; }

		[MaxLength(50)]
		public string Name { get; set; }


		[ForeignKey("CurrencyId1")]
		public virtual Currency Currency1 { get; set; }

		[ForeignKey("CurrencyId2")]
		public virtual Currency Currency2 { get; set; }
	}
}