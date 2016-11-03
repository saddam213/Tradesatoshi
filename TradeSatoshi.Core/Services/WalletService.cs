using Cryptopia.WalletAPI.Base;
using System;
using System.Threading.Tasks;
using TradeSatoshi.Common.Address;
using TradeSatoshi.Common.Services.WalletService;

namespace TradeSatoshi.Core.Services
{
	public class WalletService : IWalletService
	{
		public async Task<AddressModel> GenerateAddress(string userId, string ipAddress, int port, string username, string password)
		{
			try
			{
				var wallerConnector = new WalletConnector(ipAddress, port, username, password);
				var address = wallerConnector.GenerateAddress(userId, true);
				var privateKey = await wallerConnector.DumpPrivKeyAsync(address);
				return new AddressModel
				{
					Address = address,
					PrivateKey = privateKey
				};
			}
			catch (Exception)
			{
				return null;
			}
		}

		public async Task<bool> ValidateAddress(string address, string ipAddress, int port, string username, string password)
		{
			try
			{
				var wallerConnector = new WalletConnector(ipAddress, port, username, password);
				return await wallerConnector.ValidateAddressAsync(address);
			}
			catch (Exception)
			{
				return true;
			}
		}
	}
}