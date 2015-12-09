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
		AuditResult AuditUserCurrency(IDataContext context, string userId, int currencyId);
		Task<AuditResult> AuditUserCurrencyAsync(IDataContext context, string userId, int currencyId);
	}
}
