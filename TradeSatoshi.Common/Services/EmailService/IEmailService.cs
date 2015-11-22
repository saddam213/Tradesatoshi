using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.EmailService
{
	public interface IEmailService
	{
		bool Send(EmailTemplate template, params object[] formatParameters);
		Task<bool> SendAsync(EmailTemplate template, params object[] formatParameters);
	}
}
