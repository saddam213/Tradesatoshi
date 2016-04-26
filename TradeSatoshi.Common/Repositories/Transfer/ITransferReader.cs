using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;

namespace TradeSatoshi.Common.Transfer
{
	public interface ITransferReader
	{
		Task<CreateTransferModel> GetCreateTransfer(string userId, int currencyId);
		Task<DataTablesResponse> GetTransferDataTable(DataTablesModel model);
		Task<DataTablesResponse> GetUserTransferDataTable(DataTablesModel model, string userId);
	}
}