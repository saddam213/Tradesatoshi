using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Chat;
using TradeSatoshi.Common.Data;

namespace TradeSatoshi.Core.Repositories.Chat
{
	public class ChatReader : IChatReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<List<ChatMessageModel>> GetChatHistory(string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var messages = await context.ChatMessage
					.Include(x => x.User)
					.Where(x => x.IsEnabled)
					.OrderByDescending(x => x.Id)
					.Take(500)
					.Select(x => new ChatMessageModel
					{
						Icon = x.User.ChatIcon,
						Id = x.Id,
						IsEnabled = x.IsEnabled,
						Message = x.Message,
						RawTimestamp = x.Timestamp,
						UserName = x.User.UserName
					}).ToListAsync();

				messages.ForEach(x => x.Timestamp = x.RawTimestamp.ToString("MM/dd/yyyy HH:mm:ss"));
				return messages.OrderBy(x => x.Id).ToList();
			}
		}

		public async Task<ChatUserModel> GetChatUser(string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FindAsync(userId);
				if (user == null)
					return null;

				return new ChatUserModel
				{
					UserName = user.UserName,
					ChatBanEnd = user.ChatBanEnd,
					ChatIcon = user.ChatIcon
				};
			}
		}
	}
}
