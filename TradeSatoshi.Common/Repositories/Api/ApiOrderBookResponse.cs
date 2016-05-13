using System.Collections.Generic;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiOrderBookResponse
	{
		public List<ApiOrderBookItem> Buy { get; set; }
		public List<ApiOrderBookItem> Sell { get; set; }
	}
}