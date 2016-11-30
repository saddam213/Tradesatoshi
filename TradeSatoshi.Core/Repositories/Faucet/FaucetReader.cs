using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Faucet;

namespace TradeSatoshi.Core.Faucet
{
	public class FaucetReader : IFaucetReader
	{
		public Task<DataTablesResponse> GetFaucetDataTable(DataTablesModel param, string userId)
		{
			throw new NotImplementedException();
		}
	}
}
