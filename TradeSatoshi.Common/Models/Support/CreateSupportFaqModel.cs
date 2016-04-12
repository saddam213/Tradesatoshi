using System.ComponentModel.DataAnnotations;

namespace TradeSatoshi.Common.Support
{
	public class CreateSupportFaqModel
	{
		[Required]
		[MaxLength(256)]
		public string Question { get; set; }

		[Required]
		[MaxLength(4000)]
		public string Answer { get; set; }
	}
}