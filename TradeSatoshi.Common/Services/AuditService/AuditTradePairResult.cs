namespace TradeSatoshi.Common.Services.AuditService
{
	public class AuditTradePairResult
	{
		public AuditTradePairResult()
		{
		}

		public AuditTradePairResult(bool success)
		{
			Success = success;
		}

	
		public bool Success { get; set; }
		public AuditCurrencyResult Currency { get; set; }
		public AuditCurrencyResult BaseCurrency { get; set; }
	}
}