using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Chat;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Web.Attributes;

namespace TradeSatoshi.Web.Hubs
{
	[HubName("Chat")]
	public class ChatHub : Hub
	{
		private static ConcurrentQueue<ChatMessageModel> _messageCache = new ConcurrentQueue<ChatMessageModel>();
		private static ConcurrentDictionary<string, ChatUserModel> _userCache = new ConcurrentDictionary<string, ChatUserModel>();

		public IChatReader ChatReader { get; set; }
		public IChatWriter ChatWriter { get; set; }

		public async Task GetOnlineCount()
		{
			await Clients.Caller.OnlineCount(_userCache.Count, GetTime());
		}

		public async Task GetMessages()
		{
			await GetCache();
			await Clients.Caller.Messages(_messageCache.Where(x => x.IsEnabled).ToArray(), GetTime());
		}

		[AuthorizeSignalRSecurityRole(SecurityRole.Administrator)]
		public async Task RemoveMessage(int id)
		{
			if (await DeleteMessage(id))
			{
				await Clients.All.RemoveMessage(id);
			}
		}

		[AuthorizeSignalRSecurityRole(SecurityRole.Standard)]
		public async Task SendMessage(string message)
		{
			ChatUserModel user = null;
			if (_userCache.TryGetValue(CurrentUserId(), out user))
			{
				if (user.ChatBanEnd.HasValue && user.ChatBanEnd.Value > DateTime.UtcNow)
				{
					return;
				}

				var chatMessage = new ChatMessageModel
				{
					Message = message,
					Timestamp = GetTime(),
					UserName = CurrentUserName(),
					Icon = user.ChatIcon,
					IsEnabled = true,
				};
				await SaveMessage(chatMessage);
				await Clients.All.NewMessage(chatMessage, GetTime());
			}
		}

		public override async Task OnConnected()
		{
			var userId = CurrentUserId();
			if (!string.IsNullOrEmpty(userId))
			{
				if (!_userCache.ContainsKey(userId))
				{
					var chatUser = await GetChatUser(userId);
					if (chatUser != null)
					{
						_userCache.TryAdd(userId, chatUser);
					}
				}
			}
			await base.OnConnected();
		}

		public override async Task OnDisconnected(bool stopCalled)
		{
			if (stopCalled)
			{
				var userId = CurrentUserId();
				if (!string.IsNullOrEmpty(userId))
				{
					ChatUserModel outVal = null;
					_userCache.TryRemove(userId, out outVal);
				}
			}
			await base.OnDisconnected(stopCalled);
		}

		private string CurrentUserId()
		{
			if (Context.User != null && Context.User.Identity != null)
			{
				return Context.User.Identity.GetUserId() ?? string.Empty;
			}
			return string.Empty;
		}

		private string CurrentUserName()
		{
			if (Context.User != null && Context.User.Identity != null)
			{
				return Context.User.Identity.Name;
			}
			return string.Empty;
		}

		private string GetTime()
		{
			return DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");
		}

		private async Task<ChatUserModel> GetChatUser(string userId)
		{
			return await ChatReader.GetChatUser(userId);
		}

		private async Task SaveMessage(ChatMessageModel chatMessage)
		{
			var result = await ChatWriter.CreateChatMessage(CurrentUserId(), chatMessage);
			if (result.HasErrors)
				return;

			chatMessage.Id = result.Data;
			_messageCache.Enqueue(chatMessage);
		}

		private async Task<bool> DeleteMessage(int id)
		{
			var result = await ChatWriter.AdminUpdateChatMessage(CurrentUserId(), new UpdateChatMessageModel { Id = id });
			if (result.HasErrors)
				return false;

			var cacheItem = _messageCache.FirstOrDefault(x => x.Id == id);
			if (cacheItem == null)
				return false;

			cacheItem.IsEnabled = false;
			return true;
		}

		private async Task GetCache()
		{
			var results = await ChatReader.GetChatHistory(CurrentUserId());
			if (results == null)
				return;

			_messageCache = new ConcurrentQueue<ChatMessageModel>(results);
		}
	}
}