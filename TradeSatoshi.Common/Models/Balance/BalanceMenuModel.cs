using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvc.JQuery.Datatables;

namespace TradeSatoshi.Common.Balance
{
	public class BalanceMenuModel
	{
		public string Symbol { get; set; }
		public decimal Balance { get; set; }
	}
}
