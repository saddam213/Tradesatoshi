using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Address;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.EncryptionService;
using TradeSatoshi.Common.Services.WalletService;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Core.Address
{
	public class AddressWriter : IAddressWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IWalletService WalletService { get; set; }
		public IEncryptionService EncryptionService { get; set; }

		public async Task<IWriterResult<string>> GenerateAddress(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var currency = await context.Currency.FirstOrDefaultNoLockAsync(x => x.Id == currencyId && x.IsEnabled);
				if (currency == null)
					return WriterResult<string>.ErrorResult("Currency not found.");

				var newAddress = await WalletService.GenerateAddress(userId, currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
				if (newAddress == null)
					return WriterResult<string>.ErrorResult("Failed to generate address for {0}.", currency.Name);

				var currentAddresses = await context.Address.Where(x => x.UserId == userId && x.CurrencyId == currencyId && x.IsActive).ToListNoLockAsync();
				foreach (var currentAddress in currentAddresses)
				{
					currentAddress.IsActive = false;
				}

				var addressEntity = new Entity.Address
				{
					AddressHash = newAddress.Address,
					PrivateKey = newAddress.PrivateKey,
					CurrencyId = currencyId,
					UserId = userId,
					IsActive = true,
				};

				context.Address.Add(addressEntity);
				await context.SaveChangesAsync();
				return WriterResult<string>.SuccessResult(newAddress.Address);
			}
		}

		public async Task<IWriterResult<string>> GenerateAddress(string userId, string currency)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var currencyEntity = await context.Currency.FirstOrDefaultNoLockAsync(x => x.Symbol == currency && x.IsEnabled);
				if (currencyEntity == null)
					return WriterResult<string>.ErrorResult("Currency not found.");

				var newAddress = await WalletService.GenerateAddress(userId, currencyEntity.WalletHost, currencyEntity.WalletPort, currencyEntity.WalletUser, currencyEntity.WalletPass);
				if (newAddress == null)
					return WriterResult<string>.ErrorResult("Failed to generate address for {0}.", currencyEntity.Name);

				var currentAddresses = await context.Address.Where(x => x.UserId == userId && x.CurrencyId == currencyEntity.Id && x.IsActive).ToListNoLockAsync();
				foreach (var currentAddress in currentAddresses)
				{
					currentAddress.IsActive = false;
				}

				var addressEntity = new Entity.Address
				{
					AddressHash = newAddress.Address,
					PrivateKey = newAddress.PrivateKey,
					CurrencyId = currencyEntity.Id,
					UserId = userId,
					IsActive = true,
				};

				context.Address.Add(addressEntity);
				await context.SaveChangesAsync();
				return WriterResult<string>.SuccessResult(newAddress.Address);
			}
		}
	}
}