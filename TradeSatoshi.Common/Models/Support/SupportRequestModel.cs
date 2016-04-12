using System;

namespace TradeSatoshi.Common.Support
{
	public class SupportRequestModel
	{
		public DateTime Created { get; set; }
		public string Description { get; set; }
		public int Id { get; set; }
		public string Sender { get; set; }
		public string Title { get; set; }
	}
}