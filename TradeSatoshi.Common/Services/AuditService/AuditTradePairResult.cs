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

		public AuditTradePairResult(string symbol, string baseSymbol, decimal available, decimal baseAvailable)
		{
			Success = true;
			Symbol = symbol;
			Available = available;
			BaseSymbol = baseSymbol;
			BaseAvailable = baseAvailable;
		}

		public bool Success { get; set; }
		public string Symbol { get; set; }
		public decimal Available { get; set; }
		public string BaseSymbol { get; set; }
		public decimal BaseAvailable { get; set; }
	}
}