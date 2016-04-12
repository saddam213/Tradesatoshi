using System.ComponentModel.DataAnnotations;

namespace TradeSatoshi.Common.Support
{
	public class CreateSupportCategoryModel
	{
		[Required]
		[MaxLength(128)]
		public string Name { get; set; }
	}
}