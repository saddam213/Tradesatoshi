using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Services;
using TradeSatoshi.Common.Services.EmailService;

namespace TradeSatoshi.Core.Services
{
	public class EmailService : IEmailService
	{
		private readonly string EmailUser = ConfigurationManager.AppSettings["SMTP_User"];
		private readonly string EmailPassword = ConfigurationManager.AppSettings["SMTP_Password"];
		private readonly string EmailServer = ConfigurationManager.AppSettings["SMTP_Server"];
		private readonly string EmailPort = ConfigurationManager.AppSettings["SMTP_Port"];

		public bool Send(EmailTemplate template, params object[] formatParameters)
		{
			return true;
		}

		public Task<bool> SendAsync(EmailTemplate template, params object[] formatParameters)
		{
			return Task.FromResult<bool>(true);
		}
	}
}
