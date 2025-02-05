﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;

namespace TradeSatoshi.Common.Transfer
{
	public interface ITransferReader
	{
		Task<CreateTransferModel> GetCreateTransferAsync(string userId, int currencyId);
		DataTablesResponse GetTransferDataTable(DataTablesModel model);
		DataTablesResponse GetUserTransferDataTable(DataTablesModel model, string userId);
	}
}
