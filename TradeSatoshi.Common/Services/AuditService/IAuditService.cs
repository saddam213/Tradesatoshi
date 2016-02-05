using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;

namespace TradeSatoshi.Common.Services.AuditService
{
	public interface IAuditService
	{
		Task<AuditCurrencyResult> AuditUserCurrency(IDataContext context, string userId, int currencyId);

		Task<AuditTradePairResult> AuditUserTradePair(IDataContext context, string userId, Entity.TradePair tradepair);
	}
}
