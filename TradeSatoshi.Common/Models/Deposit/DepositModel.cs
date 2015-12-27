using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Deposit
{
	public class DepositModel
	{
		public int Id { get; set; }
		public string Currency { get; set; }
		public string Symbol { get; set; }
		public decimal Amount { get; set; }
		public DepositStatus DepositStatus { get; set; }
		public int Confirmations { get; set; }
		public string Txid { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
