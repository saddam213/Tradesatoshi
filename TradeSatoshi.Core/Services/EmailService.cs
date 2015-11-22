using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Services;
using TradeSatoshi.Common.Services.EmailService;

namespace TradeSatoshi.Core.Services
{
	public class EmailService : IEmailService
	{
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
