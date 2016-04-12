namespace TradeSatoshi.Common.Balance
{
	public class BalanceModel
	{
		public int CurrencyId { get; set; }
		public string Currency { get; set; }
		public string Symbol { get; set; }
		public string Address { get; set; }
		public decimal Total { get; set; }

		public decimal Avaliable
		{
			get { return Total - (Unconfirmed + HeldForTrades + PendingWithdraw); }
		}

		public decimal Unconfirmed { get; set; }
		public decimal HeldForTrades { get; set; }
		public decimal PendingWithdraw { get; set; }
	}
}