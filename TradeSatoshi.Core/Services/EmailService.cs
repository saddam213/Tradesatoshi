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
		private readonly string _emailPassword = ConfigurationManager.AppSettings["SMTP_Password"];
		private readonly string _emailServer = ConfigurationManager.AppSettings["SMTP_Server"];
		private readonly int _emailPort = int.Parse(ConfigurationManager.AppSettings["SMTP_Port"]);

		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<bool> Send(EmailType template, IdentityUser user, string ipaddress, params EmailParam[] formatParameters)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var emailTemplate = await context.EmailTemplates.FirstOrDefaultNoLockAsync(x => x.Type == template && x.IsEnabled);
					if (emailTemplate == null)
						return false;

					return await SendAsync(emailTemplate, user, ipaddress, formatParameters);
				}
			}
			catch (System.Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendRaw(string subject, string body, string toAddress)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var emailTemplate = await context.EmailTemplates.FirstOrDefaultNoLockAsync(x => x.IsEnabled && !string.IsNullOrEmpty(x.From));
					if (emailTemplate == null)
						return false;

					using (var email = new MailMessage(new MailAddress(emailTemplate.From, _emailDisplayName), new MailAddress(toAddress)))
					{
						email.Body = body;
						email.Subject = subject;
						email.IsBodyHtml = false;
						using (var mailClient = new SmtpClient(_emailServer, _emailPort))
						{
							mailClient.Credentials = new NetworkCredential(emailTemplate.From, _emailPassword);
							mailClient.EnableSsl = true;
							await mailClient.SendMailAsync(email);
							return true;
						}
					}
				}
			}
			catch (System.Exception)
			{
				return false;
			}
		}

		private async Task<bool> SendAsync(EmailTemplate template, IdentityUser user, string ipaddress, params EmailParam[] formatParameters)
		{
			using (var email = new MailMessage(new MailAddress(template.From, _emailDisplayName), new MailAddress(user.Email)))
			{
				var emailParameters = new List<EmailParam>
				{
					new EmailParam("[USERNAME]", user.UserName),
					new EmailParam("[IPADDRESS]", ipaddress)
				};
				emailParameters.AddRange(formatParameters);

				var body = template.Template;
				foreach (var parameter in emailParameters)
				{
					body = body.Replace(parameter.Name, parameter.Value);
				}

				email.Body = body;
				email.Subject = template.Subject;
				email.IsBodyHtml = template.IsHtml;
				using (var mailClient = new SmtpClient(_emailServer, _emailPort))
				{
					mailClient.Credentials = new NetworkCredential(template.From, _emailPassword);
					mailClient.EnableSsl = true;
					await mailClient.SendMailAsync(email);
					return true;
				}
			}
		}
	}
}