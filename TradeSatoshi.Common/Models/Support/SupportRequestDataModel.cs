using System;

namespace TradeSatoshi.Common.Support
{
	public class SupportRequestDataModel
	{
		public int Id { get; set; }
		public string Sender { get; set; }
		public string Title { get; set; }
		public bool Replied { get; set; }
		public DateTime Created { get; set; }
	}
}