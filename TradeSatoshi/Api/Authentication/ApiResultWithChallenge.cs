using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace TradeSatoshi.Web.Api.Authentication
{
	public class ApiResultWithChallenge : IHttpActionResult
	{
		private readonly string _authenticationScheme = "amx";
		private readonly IHttpActionResult _next;

		public ApiResultWithChallenge(IHttpActionResult next)
		{
			this._next = next;
		}

		public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			var response = await _next.ExecuteAsync(cancellationToken);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(_authenticationScheme));
			}
			return response;
		}
	}
}