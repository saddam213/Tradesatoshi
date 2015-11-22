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
using Microsoft.AspNet.Identity.EntityFramework;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Data.Entities;
using System.Data.Entity;
using System.Net;

namespace TradeSatoshi.Core.Services
{
	public class EmailService : IEmailService
	{
		private readonly string EmailDisplayName = ConfigurationManager.AppSettings["SMTP_DisplayName"];
		private readonly string EmailUser = ConfigurationManager.AppSettings["SMTP_User"];
		private readonly string EmailPassword = ConfigurationManager.AppSettings["SMTP_Password"];
		private readonly string EmailServer = ConfigurationManager.AppSettings["SMTP_Server"];
		private readonly int EmailPort = int.Parse(ConfigurationManager.AppSettings["SMTP_Port"]);

		public IApplicationDbContext ApplicationDbContext { get; set; }
		
		public bool Send(EmailType template, IdentityUser user, string ipaddress, params object[] formatParameters)
		{
			using (var context = ApplicationDbContext.CreateContext())
			{
				var emailTemplate = context.EmailTemplates.FirstOrDefault(x => x.Type == template && x.IsEnabled);
				if (emailTemplate == null)
					return false;

				return Send(emailTemplate, user, ipaddress, formatParameters);
			}
		}

		public async Task<bool> SendAsync(EmailType template, IdentityUser user, string ipaddress, params object[] formatParameters)
		{
			using (var context = ApplicationDbContext.CreateContext())
			{
				var emailTemplate = await context.EmailTemplates.FirstOrDefaultAsync(x => x.Type == template && x.IsEnabled);
				if (emailTemplate == null)
					return false;

				return await SendAsync(emailTemplate, user, ipaddress, formatParameters);
			}
		}

		private bool Send(EmailTemplate template, IdentityUser user, string ipaddress, params object[] formatParameters)
		{
			using (var email = new MailMessage(new MailAddress(EmailUser, EmailDisplayName), new MailAddress(user.Email)))
			{
				var emailParameters = new List<object>();
				emailParameters.Add(user.UserName);
				emailParameters.Add(ipaddress);
				emailParameters.AddRange(formatParameters);

				email.Subject = template.Subject;
				email.Body = string.Format(template.Template, emailParameters.ToArray()).Replace("{{", "{").Replace("}}", "}");
				email.IsBodyHtml = template.IsHtml;
				using (var mailClient = new SmtpClient(EmailServer, EmailPort))
				{
					mailClient.Credentials = new NetworkCredential(EmailUser, EmailPassword);
					mailClient.EnableSsl = true;
					mailClient.Send(email);
					return true;
				}
			}
		}

		private async Task<bool> SendAsync(EmailTemplate template, IdentityUser user, string ipaddress, params object[] formatParameters)
		{
			using (var email = new MailMessage(new MailAddress(EmailUser, EmailDisplayName), new MailAddress(user.Email)))
			{
				var emailParameters = new List<object>();
				emailParameters.Add(user.UserName);
				emailParameters.Add(ipaddress);
				emailParameters.AddRange(formatParameters);

				email.Subject = template.Subject;
				email.Body = string.Format(template.Template, emailParameters.ToArray()).Replace("{{", "{").Replace("}}", "}");
				email.IsBodyHtml = template.IsHtml;
				using (var mailClient = new SmtpClient(EmailServer, EmailPort))
				{
					mailClient.Credentials = new NetworkCredential(EmailUser, EmailPassword);
					mailClient.EnableSsl = true;
					await mailClient.SendMailAsync(email);
					return true;
				}
			}
		}
	}
}
