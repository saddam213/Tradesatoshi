using System.ComponentModel.DataAnnotations;

namespace TradeSatoshi.Common.Support
{
	public class UpdateSupportCategoryModel
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(128)]
		public string Name { get; set; }

		public bool IsEnabled { get; set; }
	}
}