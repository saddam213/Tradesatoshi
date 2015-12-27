using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Enums
{
	public enum SupportTicketStatus
	{
		New = 0,
		UserReply = 1,
		AdminReply = 2,
		AdminClosed = 3,
		UserClosed = 4
	}
}
