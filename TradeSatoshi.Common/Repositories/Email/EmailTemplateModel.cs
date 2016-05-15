using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Repositories.Email
{
	public class EmailTemplateModel
	{
		public EmailType Type { get; set; }

		[Required]
		public string From { get; set; }
		
		[Required]
		public string Subject { get; set; }

		[AllowHtml]
		public string Template { get; set; }

		public string Description { get; set; }
		public string Parameters { get; set; }
		public bool IsHtml { get; set; }
		public bool IsEnabled { get; set; }
	}
}