using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeSatoshi.Common.TradePair;

namespace TradeSatoshi.Web.Helpers
{
	public static class ModelExtensions
	{
		public static string MarketUrl(this TradePairModel model)
		{
			if (model == null)
				return string.Empty;

			return model.Name.Replace('/', '_');
		}
	}
}