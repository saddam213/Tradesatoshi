using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;

namespace TradeSatoshi.Common.Transfer
{
	public interface ITransferReader
	{
		Task<CreateTransferModel> GetCreateTransfer(string userId, int currencyId);
		DataTablesResponse GetTransferDataTable(DataTablesModel model);
		DataTablesResponse GetUserTransferDataTable(DataTablesModel model, string userId);
	}
}