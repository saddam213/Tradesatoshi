using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.Wallet
{
	public interface IWalletService
	{
		AddressModel GenerateAddress(string userId, string ipAddress, int port, string username, string password);
		Task<AddressModel> GenerateAddressAsync(string userId, string ipAddress, int port, string username, string password);
	}
}
