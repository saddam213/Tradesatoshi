using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Repositories.Email;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Core.Repositories.Email
{
	public class EmailTemplateWriter : IEmailTemplateWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult<bool>> UpdateEmailTemplate(EmailTemplateModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var template = await context.EmailTemplates.FirstOrDefaultNoLockAsync(x => x.Type == model.Type);
				if(template == null)
				return WriterResult<bool>.ErrorResult($"Email template '{model.Type}' not found.");

				template.Subject = model.Subject;
				template.Template = model.Template;
				template.IsHtml = model.IsHtml;
				template.IsEnabled = model.IsEnabled;

				await context.SaveChangesAsync();
				return WriterResult<bool>.SuccessResult($"Successfully update email template.");
			}
		}
	}
}
