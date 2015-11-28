using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;

namespace TradeSatoshi.Core.Admin
{
	public interface ILogonReader
	{
		DataTablesResponse GetLogonDataTable(DataTablesModel model);
	}
}
