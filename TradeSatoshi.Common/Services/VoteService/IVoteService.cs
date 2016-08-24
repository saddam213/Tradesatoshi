using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.VoteService
{
	public interface IVoteService
	{
		Task<bool> CheckVoteItems();
	}
}