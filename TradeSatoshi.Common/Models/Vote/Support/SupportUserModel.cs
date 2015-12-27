using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Support
{
	public class SupportUserModel
	{
		public SupportUserModel()
		{
			Tickets = new List<SupportTicketModel>();
			SupportFaq = new List<SupportFaqModel>();
		}

		public List<SupportFaqModel> SupportFaq { get; set; }
		public List<SupportTicketModel> Tickets { get; set; }
	}
}
