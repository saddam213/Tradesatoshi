using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Repositories.Api
{
	public class ApiSubmitOrderResponse
	{
		public int? OrderId { get; set; }
		public List<int> Filled { get; set; }
	}
}
