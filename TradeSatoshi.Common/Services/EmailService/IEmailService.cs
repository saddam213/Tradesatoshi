using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Services.EmailService
{
	public interface IEmailService
	{
		bool Send(EmailType template, IdentityUser user, string ipaddress, params object[] formatParameters);
		Task<bool> SendAsync(EmailType template, IdentityUser user, string ipaddress, params object[] formatParameters);
	}
}
