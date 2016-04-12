using System;

namespace TradeSatoshi.Common.Chat
{
	public class ChatMessageModel
	{
		public int Id { get; set; }
		public string Icon { get; set; }
		public string UserName { get; set; }
		public string Message { get; set; }
		public string Timestamp { get; set; }
		public DateTime RawTimestamp { get; set; }
		public bool IsEnabled { get; set; }
	}
}