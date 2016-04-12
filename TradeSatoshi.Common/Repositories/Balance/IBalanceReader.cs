using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Balance
{
	public interface IBalanceReader
	{
		Task<BalanceModel> GetBalance(string userId, int currencyId);
		Task<List<BalanceModel>> GetBalances(string userId);
	}
}