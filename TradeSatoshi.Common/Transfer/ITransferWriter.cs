using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Transfer
{
	public interface ITransferWriter
	{
		Task<IWriterResult<bool>> CreateTransferAsync(CreateTransferModel tradeItem);
	}
}
