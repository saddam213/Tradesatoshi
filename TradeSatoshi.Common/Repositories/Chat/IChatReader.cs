using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Chat
{
	public interface IChatReader
	{
		Task<ChatUserModel> GetChatUser(string userId);
		Task<List<ChatMessageModel>> GetChatHistory(string userId);
	}
}
