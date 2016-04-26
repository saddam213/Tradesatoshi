using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Vote
{
	public interface IVoteReader
	{
		Task<VoteModel> GetVoteSettings();
		Task<ViewVoteItemModel> GetVoteItem(int voteItemId);
		Task<DataTablesResponse> GetVoteDataTable(DataTablesModel model, VoteType type);
		Task<DataTablesResponse> GetPendingDataTable(DataTablesModel model);
		Task<DataTablesResponse> GetRejectedDataTable(DataTablesModel model);

		Task<UpdateVoteItemModel> AdminGetVoteItem(int voteItemId);
		Task<DataTablesResponse> AdminGetVoteDataTable(DataTablesModel model);
	}
}