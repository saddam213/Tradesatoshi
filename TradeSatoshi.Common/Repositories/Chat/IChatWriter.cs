using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Chat
{
	public interface IChatWriter
	{
		Task<WriterResult<int>> CreateChatMessage(string userId, ChatMessageModel model);
		Task<WriterResult<bool>> AdminUpdateChatMessage(string userId, UpdateChatMessageModel model);
	}
}
