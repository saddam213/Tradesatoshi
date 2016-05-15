using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Common.Repositories.Email
{
	public interface IEmailTemplateWriter
	{
		Task<IWriterResult<bool>> UpdateEmailTemplate(EmailTemplateModel model);
	}
}
