using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TradeSatoshi.Common.Services.EncryptionService;

namespace TradeSatoshi.Core.Services
{
	public class EncryptionService : IEncryptionService
	{
		private const int Keysize = 256;
		private static readonly byte[] InitVectorBytes = Encoding.ASCII.GetBytes("tu89geji340t89u2");
		private static readonly string InternalPassPhrase = "1CsDkCRtYBiS5DgdCd9Qzr263mFVSapR8s";

		public string EncryptString(string input)
		{
			return EncryptString(input, InternalPassPhrase);
		}

		public string DecryptString(string input)
		{
			return DecryptString(input, InternalPassPhrase);
		}

		public string EncryptString(string input, string passPhrase)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(input);
			using (var password = new PasswordDeriveBytes(passPhrase, null))
			{
				var keyBytes = password.GetBytes(Keysize / 8);
				using (var symmetricKey = new RijndaelManaged())
				{
					symmetricKey.Mode = CipherMode.CBC;
					using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, InitVectorBytes))
					{
						using (var memoryStream = new MemoryStream())
						using (var cryptoStream = new CryptoStream(new MemoryStream(), encryptor, CryptoStreamMode.Write))
						{
							cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
							cryptoStream.FlushFinalBlock();
							var cipherTextBytes = memoryStream.ToArray();
							return Convert.ToBase64String(cipherTextBytes);
						}
					}
				}
			}
		}

		public string DecryptString(string input, string passPhrase)
		{
			var cipherTextBytes = Convert.FromBase64String(input);
			using (var password = new PasswordDeriveBytes(passPhrase, null))
			{
				var keyBytes = password.GetBytes(Keysize / 8);
				using (var symmetricKey = new RijndaelManaged())
				{
					symmetricKey.Mode = CipherMode.CBC;
					using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, InitVectorBytes))
					{
						using (var cryptoStream = new CryptoStream(new MemoryStream(cipherTextBytes), decryptor, CryptoStreamMode.Read))
						{
							var plainTextBytes = new byte[cipherTextBytes.Length];
							var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
							return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
						}
					}
				}
			}
		}


		public EncryptionKeyPair GenerateEncryptionKeyPair()
		{
			using (var cryptoProvider = new RNGCryptoServiceProvider())
			{
				var key = Guid.NewGuid().ToString("N");
				byte[] secretKeyByteArray = new byte[32]; //256 bit
				cryptoProvider.GetBytes(secretKeyByteArray);
				var secret = Convert.ToBase64String(secretKeyByteArray);
				var result = new EncryptionKeyPair
				{
					PublicKey = key,
					PrivateKey = secret
				};

				return result;
			}
		}
	}


}