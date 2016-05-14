using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services.EmailService;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Net;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Enums;
using TradeSatoshi.Entity;

namespace TradeSatoshi.Core.Services
{
	public class EmailService : IEmailService
	{
		private readonly string _emailDisplayName = ConfigurationManager.AppSettings["SMTP_DisplayName"];
		private readonly string _emailUser = ConfigurationManager.AppSettings["SMTP_User"];
		private readonly string _emailPassword = ConfigurationManager.AppSettings["SMTP_Password"];
		private readonly string _emailServer = ConfigurationManager.AppSettings["SMTP_Server"];
		private readonly int _emailPort = int.Parse(ConfigurationManager.AppSettings["SMTP_Port"]);

		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<bool> Send(EmailType template, IdentityUser user, string ipaddress, params object[] formatParameters)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var emailTemplate = await context.EmailTemplates.FirstOrDefaultNoLockAsync(x => x.Type == template && x.IsEnabled);
				if (emailTemplate == null)
					return false;

				return await SendAsync(emailTemplate, user, ipaddress, formatParameters);
			}
		}

		private async Task<bool> SendAsync(EmailTemplate template, IdentityUser user, string ipaddress, params object[] formatParameters)
		{
			using (var email = new MailMessage(new MailAddress(_emailUser, _emailDisplayName), new MailAddress(user.Email)))
			{
				var emailParameters = new List<object> {user.UserName, ipaddress};
				emailParameters.AddRange(formatParameters);
				email.Subject = template.Subject;
				email.Body = string.Format(template.Template, emailParameters.ToArray()).Replace("{{", "{").Replace("}}", "}");
				email.IsBodyHtml = template.IsHtml;
				using (var mailClient = new SmtpClient(_emailServer, _emailPort))
				{
					mailClient.Credentials = new NetworkCredential(_emailUser, _emailPassword);
					mailClient.EnableSsl = true;
					await mailClient.SendMailAsync(email);
					return true;
				}
			}
		}
	}
}