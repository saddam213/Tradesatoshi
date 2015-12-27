using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TradeSatoshi.Common.Support
{
	public class CreateSupportRequestReplyModel
	{
		public int RequestId { get; set; }

		[Required]
		[MaxLength(4000)]
		public string Message { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
