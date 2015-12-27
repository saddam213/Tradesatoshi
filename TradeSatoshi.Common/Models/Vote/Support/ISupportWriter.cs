using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Support
{
	public interface ISupportWriter
	{
		Task<IWriterResult<bool>> CreateSupportRequest(CreateSupportRequestModel model);
		Task<IWriterResult<int>> CreateSupportTicket(string userId, CreateSupportTicketModel model);
		Task<IWriterResult<int>> CreateSupportTicketReply(string userId, CreateSupportTicketReplyModel model);
		Task<IWriterResult<bool>> UpdateSupportTicketStatus(string userId, UpdateSupportTicketStatusModel model);

		Task<IWriterResult<int>> AdminCreateSupportTicketReply(string userId, CreateSupportTicketReplyModel model);
		Task<IWriterResult<bool>> AdminUpdateSupportReplyStatus(string userId, UpdateSupportReplyStatusModel model);
		Task<IWriterResult<bool>> AdminCreateSupportRequestReply(string userId, CreateSupportRequestReplyModel model);
		Task<IWriterResult<bool>> AdminCreateSupportCategory(string userId, CreateSupportCategoryModel model);
		Task<IWriterResult<bool>> AdminUpdateSupportCategory(string userId, UpdateSupportCategoryModel model);
		Task<IWriterResult<bool>> AdminCreateSupportFaq(string userId, CreateSupportFaqModel model);
		Task<IWriterResult<bool>> AdminUpdateSupportFaq(string userId, UpdateSupportFaqModel model);
	}
}
