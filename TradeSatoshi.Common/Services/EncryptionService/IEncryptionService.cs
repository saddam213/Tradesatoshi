using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.EncryptionService
{
	public interface IEncryptionService
	{
		string EncryptString(string input);
		string DecryptString(string input);
		string EncryptString(string input, string passphrase);
		string DecryptString(string input, string passphrase);
	}
}
