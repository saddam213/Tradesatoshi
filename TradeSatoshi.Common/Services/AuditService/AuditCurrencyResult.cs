namespace TradeSatoshi.Common.Services.AuditService
{
	public class AuditCurrencyResult
	{
		public AuditCurrencyResult()
		{
		}

		public AuditCurrencyResult(bool success)
		{
			Success = success;
		}

		public decimal Avaliable { get; set; }
		public int CurrencyId { get; set; }
		public decimal HeldForTrades { get; set; }
		public decimal PendingWithdraw { get; set; }
		public bool Success { get; set; }
		public string Symbol { get; set; }
		public decimal Total { get; set; }
		public decimal Unconfirmed { get; set; }
		public string UserId { get; set; }
	}
}