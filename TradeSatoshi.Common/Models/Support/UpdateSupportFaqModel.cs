using System.ComponentModel.DataAnnotations;

namespace TradeSatoshi.Common.Support
{
	public class UpdateSupportFaqModel
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(256)]
		public string Question { get; set; }

		[Required]
		[MaxLength(4000)]
		public string Answer { get; set; }

		[Required]
		public int Order { get; set; }

		public bool IsEnabled { get; set; }
	}
}