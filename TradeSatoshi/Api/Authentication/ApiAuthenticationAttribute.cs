using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace TradeSatoshi.Web.Api.Authentication
{
	public class ApiAuthenticationAttribute : Attribute, IAuthenticationFilter
	{
		private readonly UInt64 _requestMaxAgeInSeconds = 120; //2 hours

		public ApiAuthenticationAttribute()
		{
		}

		/// <summary>
		/// Authenticates the request.
		/// </summary>
		/// <param name="context">The authentication context.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>
		/// A Task that will perform authentication.
		/// </returns>
		public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			await context.Request.Content.LoadIntoBufferAsync();
			var request = context.Request;
			if (request.Headers.Authorization != null)
			{
				var rawAuthzHeader = request.Headers.Authorization.Parameter;
				var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);
				if (autherizationHeaderArray != null)
				{
					var apiKey = autherizationHeaderArray[0];
					var incomingBase64Signature = autherizationHeaderArray[1];
					var nonce = autherizationHeaderArray[2];

					var apiAuthKey = ApiKeyStore.GetApiAuthKey(apiKey);
					if (apiAuthKey != null && await IsValidRequest(request, apiAuthKey, incomingBase64Signature, nonce))
					{
						var currentPrincipal = new GenericPrincipal(new GenericIdentity(apiAuthKey.UserId), null);
						context.Principal = currentPrincipal;
					}
					else
					{
						context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
					}
				}
				else
				{
					context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
				}
			}
			else
			{
				context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
			}
		}

		/// <summary>
		/// Challenges the asynchronous.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			context.Result = new ApiResultWithChallenge(context.Result);
			return Task.FromResult(0);
		}

		/// <summary>
		/// Gets or sets a value indicating whether more than one instance of the indicated attribute can be specified for a single program element.
		/// </summary>
		/// <returns>true if more than one instance is allowed to be specified; otherwise, false. The default is false.</returns>
		public bool AllowMultiple
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the autherization header values.
		/// </summary>
		/// <param name="rawAuthzHeader">The raw auth header.</param>
		/// <returns></returns>
		private string[] GetAutherizationHeaderValues(string rawAuthHeader)
		{
			var credArray = rawAuthHeader.Split(':');
			if (credArray.Length == 3)
			{
				return credArray;
			}
			return null;
		}

		/// <summary>
		/// Determines whether ther specified request is valid
		/// </summary>
		/// <param name="request">The req.</param>
		/// <param name="apiKey">The API key.</param>
		/// <param name="incomingBase64Signature">The incoming base64 signature.</param>
		/// <param name="nonce">The nonce.</param>
		/// <param name="requestTimeStamp">The request time stamp.</param>
		/// <returns></returns>
		private async Task<bool> IsValidRequest(HttpRequestMessage request, UserApiAuthKey apiAuthKey, string incomingBase64Signature, string nonce)
		{
			if (!apiAuthKey.IsValid)
			{
				return false;
			}

			if (IsReplayRequest(apiAuthKey.Key, nonce))
			{
				return false;
			}

			string requestContentBase64String = "";
			string requestUri = HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());
			string requestHttpMethod = request.Method.Method;
			byte[] hash = await ComputeHash(request.Content);
			if (hash != null)
			{
				requestContentBase64String = Convert.ToBase64String(hash);
			}

			string data = String.Format("{0}{1}{2}{3}{4}", apiAuthKey.Key, requestHttpMethod, requestUri, nonce, requestContentBase64String);
			var secretKeyBytes = Convert.FromBase64String(apiAuthKey.Secret);
			byte[] signature = Encoding.UTF8.GetBytes(data);
			using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
			{
				byte[] signatureBytes = hmac.ComputeHash(signature);
				return (incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal));
			}
		}

		/// <summary>
		/// Determines whether the request is a replay request/attack
		/// </summary>
		/// <param name="nonce">The nonce.</param>
		/// <param name="requestTimeStamp">The request time stamp.</param>
		/// <returns></returns>
		private bool IsReplayRequest(string key, string nonce)
		{
			if (MemoryCache.Default.Contains(key + nonce))
			{
				return true;
			}
			MemoryCache.Default.Add(key + nonce, "", DateTimeOffset.UtcNow.AddMinutes(_requestMaxAgeInSeconds));
			return false;
		}

		/// <summary>
		/// Computes the hash.
		/// </summary>
		/// <param name="httpContent">Content of the HTTP.</param>
		/// <returns></returns>
		private static async Task<byte[]> ComputeHash(HttpContent httpContent)
		{
			using (var md5 = MD5.Create())
			{
				byte[] hash = null;
				var content = await httpContent.ReadAsByteArrayAsync();
				if (content.Length != 0)
				{
					hash = md5.ComputeHash(content);
				}
				return hash;
			}
		}
	}
}