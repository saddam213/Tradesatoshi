using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Repositories.Email
{
	public interface IEmailTemplateReader
	{
		Task<EmailTemplateModel> GetEmailTemplate(EmailType emailType);
		Task<List<EmailTemplateModel>> GetEmailTemplates();
	}
}
