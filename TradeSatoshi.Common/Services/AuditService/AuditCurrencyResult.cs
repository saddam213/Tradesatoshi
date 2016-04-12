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

		public AuditCurrencyResult(string symbol, decimal available)
		{
			Success = true;
			Symbol = symbol;
			Available = available;
		}

		public bool Success { get; set; }
		public string Symbol { get; set; }
		public decimal Available { get; set; }
	}
}