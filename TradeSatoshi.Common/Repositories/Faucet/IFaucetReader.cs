using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;

namespace TradeSatoshi.Common.Faucet
{
	public interface IFaucetReader
	{
		Task<DataTablesResponse> GetFaucetDataTable(DataTablesModel param, string userId);
	}
}
