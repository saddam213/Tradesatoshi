using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Chat
{
	public class ChatUserModel
	{
		public string ChatIcon { get; set; }
		public string UserName { get; set; }
		public DateTime? ChatBanEnd { get; set; }
	}
}
