using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Withdraw
{
	public class WithdrawModel
	{
		public int Id { get; set; }
		public string Currency { get; set; }
		public string Symbol { get; set; }
		public decimal Amount { get; set; }
		public decimal Fee { get; set; }
		public decimal NetAmount
		{
			get { return Amount - Fee; }
		}
		public WithdrawStatus WithdrawStatus { get; set; }
		public int Confirmations { get; set; }
		public string Txid { get; set; }
		public string Address { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
