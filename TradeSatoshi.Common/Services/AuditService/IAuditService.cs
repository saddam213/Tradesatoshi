using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Entity;

namespace TradeSatoshi.Common.Services.AuditService
{
	public interface IAuditService
	{
		AuditCurrencyResult AuditUserCurrency(IDataContext context, string userId, int currencyId);
		Task<AuditCurrencyResult> AuditUserCurrencyAsync(IDataContext context, string userId, int currencyId);

		AuditTradePairResult AuditUserTradePair(IDataContext context, string userId, TradePair tradepair);
		Task<AuditTradePairResult> AuditUserTradePairAsync(IDataContext context, string userId, TradePair tradepair);
	}
}
