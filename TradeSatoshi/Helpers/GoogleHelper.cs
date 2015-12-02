﻿using Base32;
using OtpSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeSatoshi.Helpers
{
	public class GoogleHelper
	{
		private const string Issuer = "TradeSatoshi";

		public static GoogleTwoFactorData GetGoogleTwoFactorData(string userName)
		{
			try
			{
				var secretKey = KeyGeneration.GenerateRandomKey(20);
				string barcodeUrl = string.Format("{0}&issuer={1}", KeyUrl.GetTotpUrl(secretKey, userName), Issuer);
				return new GoogleTwoFactorData
				{
					PrivateKey = Base32Encoder.Encode(secretKey),
					PublicKey = HttpUtility.UrlEncode(barcodeUrl)
				};
			}
			catch { }
			return new GoogleTwoFactorData();
		}

		public static bool VerifyGoogleTwoFactorCode(string key, string code)
		{
			try
			{
				byte[] secretKey = Base32Encoder.Decode(key);
				long timeStepMatched = 0;
				var otp = new Totp(secretKey);
				return otp.VerifyTotp(code, out timeStepMatched, new VerificationWindow(5, 5));
			}
			catch {}
			return false;
		}
	}

	public class GoogleTwoFactorData
	{
		public string PrivateKey { get; set; }
		public string PublicKey { get; set; }
		public bool IsValid
		{
			get
			{ return !string.IsNullOrEmpty(PublicKey) 
				&& !string.IsNullOrEmpty(PrivateKey);
			}
		}
	}
}