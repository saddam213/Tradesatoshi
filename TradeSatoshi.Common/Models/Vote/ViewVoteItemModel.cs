using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Vote
{
	public class ViewVoteItemModel
	{
		[Display(Name = "Name:")]
		public string Name { get; set; }

		[Display(Name = "Algorithm:")]
		public AlgoType AlgoType { get; set; }

		[Display(Name = "Github:")]
		public string Source { get; set; }

		[Display(Name = "Description:")]
		public string Description { get; set; }

		[Display(Name = "Ticker:")]
		public string Symbol { get; set; }

		[Display(Name = "Website:")]
		public string Website { get; set; }
	}
}
