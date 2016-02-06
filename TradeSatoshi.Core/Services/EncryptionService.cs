using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Services.EncryptionService;

namespace TradeSatoshi.Core.Services
{
	public class EncryptionService : IEncryptionService
	{
		private const int keysize = 256;
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
			byte[] cipherTextBytes = null;
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(input);
			using (PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null))
			{
				byte[] keyBytes = password.GetBytes(keysize / 8);
				using (RijndaelManaged symmetricKey = new RijndaelManaged())
				{
					symmetricKey.Mode = CipherMode.CBC;
					using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, InitVectorBytes))
					{
						MemoryStream memoryStream = new MemoryStream();
						using (CryptoStream cryptoStream = new CryptoStream(new MemoryStream(), encryptor, CryptoStreamMode.Write))
						{
							cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
							cryptoStream.FlushFinalBlock();
							cipherTextBytes = memoryStream.ToArray();
							return Convert.ToBase64String(cipherTextBytes);
						}
					}
				}
			}
		}

		public string DecryptString(string input, string passPhrase)
		{
			byte[] cipherTextBytes = Convert.FromBase64String(input);
			using (PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null))
			{
				byte[] keyBytes = password.GetBytes(keysize / 8);
				using (RijndaelManaged symmetricKey = new RijndaelManaged())
				{
					symmetricKey.Mode = CipherMode.CBC;
					using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, InitVectorBytes))
					{
						MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
						{
							byte[] plainTextBytes = new byte[cipherTextBytes.Length];
							int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
							return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
						}
					}
				}
			}
		}
	}
}
