using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Vote
{
	public interface IVoteWriter
	{
		Task<IWriterResult<bool>> CreateFreeVote(string userId, CreateFreeVoteModel model);
		Task<IWriterResult<bool>> CreatePaidVote(string userId, CreatePaidVoteModel model);
		Task<IWriterResult<bool>> CreateVoteItem(string userId, CreateVoteItemModel model);

		Task<IWriterResult<bool>> AdminUpdateVoteItem(string userId, UpdateVoteItemModel model);
		Task<IWriterResult<bool>> AdminUpdateVoteSettings(string userId, UpdateVoteSettingsModel model);
	}
}
