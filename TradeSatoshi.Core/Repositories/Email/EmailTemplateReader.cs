using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Repositories.Email;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Repositories.Email
{
	public class EmailTemplateReader : IEmailTemplateReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<EmailTemplateModel> GetEmailTemplate(EmailType emailType)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.EmailTemplates.Select(e => new EmailTemplateModel
				{
					Type = e.Type,
					Description = e.Description,
					From = e.From,
					IsEnabled = e.IsEnabled,
					IsHtml = e.IsHtml,
					Parameters = e.Parameters,
					Subject = e.Subject,
					Template = e.Template
				}).FirstOrDefaultNoLockAsync(e => e.Type == emailType);
			}
		}

		public async Task<List<EmailTemplateModel>> GetEmailTemplates()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.EmailTemplates.Select(e => new EmailTemplateModel
				{
					Type = e.Type,
					Description = e.Description,
					From = e.From,
					IsEnabled = e.IsEnabled,
					IsHtml = e.IsHtml,
					Parameters = e.Parameters,
					Subject = e.Subject,
					Template = e.Template
				}).ToListNoLockAsync();
			}
		}

	}
}
