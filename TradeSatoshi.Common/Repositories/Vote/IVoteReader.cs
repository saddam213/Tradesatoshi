using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Vote
{
	public interface IVoteReader
	{
		Task<VoteModel> GetVoteSettings();
		Task<ViewVoteItemModel> GetVoteItem(int voteItemId);
		DataTablesResponse GetVoteDataTable(DataTablesModel model, VoteType type);
		DataTablesResponse GetPendingDataTable(DataTablesModel model);
		DataTablesResponse GetRejectedDataTable(DataTablesModel model);

		Task<UpdateVoteItemModel> AdminGetVoteItem(int voteItemId);
		DataTablesResponse AdminGetVoteDataTable(DataTablesModel model);
	}
}