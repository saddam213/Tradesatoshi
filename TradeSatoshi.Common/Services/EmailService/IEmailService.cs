using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Services.EmailService
{
	public interface IEmailService
	{
		Task<bool> SendRaw(string subject, string body, string toAddress);
		Task<bool> Send(EmailType template, IdentityUser user, string ipaddress, params EmailParam[] formatParameters);
	}
}