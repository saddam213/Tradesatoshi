using System;
using System.Threading.Tasks;
using TradeSatoshi.Common.Chat;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Core.Repositories.Chat
{
	public class ChatWriter : IChatWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<WriterResult<int>> CreateChatMessage(string userId, ChatMessageModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var chatEntity = new Entity.ChatMessage
				{
					IsEnabled = true,
					Message = model.Message,
					Timestamp = DateTime.UtcNow,
					UserId = userId
				};
				context.ChatMessage.Add(chatEntity);
				await context.SaveChangesAsync();
				return WriterResult<int>.SuccessResult(chatEntity.Id);
			}
		}

		public async Task<WriterResult<bool>> AdminUpdateChatMessage(string userId, UpdateChatMessageModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var chatEntity = await context.ChatMessage.FirstOrDefaultNoLockAsync(x => x.Id == model.Id);
				if (chatEntity == null)
					return WriterResult<bool>.ErrorResult();

				chatEntity.IsEnabled = model.IsEnabled;
				await context.SaveChangesAsync();
				return WriterResult<bool>.SuccessResult();
			}
		}
	}
}