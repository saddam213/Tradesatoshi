using System.Threading.Tasks;
using TradeSatoshi.Common.Address;

namespace TradeSatoshi.Common.Services.WalletService
{
	public interface IWalletService
	{
		Task<AddressModel> GenerateAddress(string userId, string ipAddress, int port, string username, string password);
	}
}