using System.ComponentModel.DataAnnotations;

namespace TradeSatoshi.Common.Support
{
	public class CreateSupportRequestModel
	{
		[Required]
		[EmailAddress]
		public string Sender { get; set; }

		[Required]
		[MaxLength(256)]
		public string Title { get; set; }

		[Required]
		[MaxLength(4000)]
		public string Description { get; set; }
	}
}