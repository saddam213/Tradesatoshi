using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TradeSatoshi.Common.Support
{
	public class CreateSupportTicketModel
	{
		public CreateSupportTicketModel()
		{
			Categories = new List<SupportCategoryModel>();
		}

		[Required]
		[MaxLength(256)]
		public string Title { get; set; }

		[Required]
		[MaxLength(4000)]
		public string Description { get; set; }

		public int CategoryId { get; set; }
		public List<SupportCategoryModel> Categories { get; set; }
	}
}
