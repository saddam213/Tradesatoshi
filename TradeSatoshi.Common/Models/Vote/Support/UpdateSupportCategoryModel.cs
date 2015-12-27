using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

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
