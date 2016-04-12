using System;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Transfer
{
	public class TransferModel
	{
		public int Id { get; set; }

		public string Symbol { get; set; }

		public decimal Amount { get; set; }

		public decimal Fee { get; set; }

		public TransferType TransferType { get; set; }

		public string Sender { get; set; }

		public string Receiver { get; set; }

		public DateTime TimeStamp { get; set; }
	}
}
