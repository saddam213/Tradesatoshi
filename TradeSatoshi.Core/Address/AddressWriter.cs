using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Address;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Data.DataContext;
using System.Data.Entity;
using TradeSatoshi.Common.Services.WalletService;
using TradeSatoshi.Common.Services.EncryptionService;
using TradeSatoshi.Common.Data;

namespace TradeSatoshi.Core.Address
{
	public class AddressWriter : IAddressWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IWalletService WalletService { get; set; }
		public IEncryptionService EncryptionService { get; set; }

		public IWriterResult<string> GenerateAddress(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var currentAddress = context.Address.FirstOrDefault(x => x.UserId == userId && x.CurrencyId == currencyId && x.IsActive);
				if (currentAddress != null)
				{
					currentAddress.IsActive = false;
				}

				var currency = context.Currency.Find(currencyId);
				if (currency == null)
					return WriterResult<string>.ErrorResult("Currency not found.");

				var newAddress = WalletService.GenerateAddress(userId, currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
				if (newAddress == null)
					return WriterResult<string>.ErrorResult("Failed to generate address for {0}.", currency.Name);

				var addressEntity = new TradeSatoshi.Common.Data.Entities.Address
				{
					AddressHash = newAddress.Address,
					PrivateKey = EncryptionService.EncryptString(newAddress.PrivateKey),
					CurrencyId = currencyId,
					UserId = userId,
					IsActive = true,
				};

				context.Address.Add(addressEntity);
				context.SaveChanges();
				return WriterResult<string>.SuccessResult(newAddress.Address);
			}
		}

		public async Task<IWriterResult<string>> GenerateAddressAsync(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var currentAddress = await context.Address.FirstOrDefaultAsync(x => x.UserId == userId && x.CurrencyId == currencyId && x.IsActive);
				if (currentAddress != null)
				{
					currentAddress.IsActive = false;
				}

				var currency = await context.Currency.FindAsync(currencyId);
				if (currency == null)
					return WriterResult<string>.ErrorResult("Currency not found.");

				var newAddress = await WalletService.GenerateAddressAsync(userId, currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
				if (newAddress == null)
					return WriterResult<string>.ErrorResult("Failed to generate address for {0}.", currency.Name);

				var addressEntity = new TradeSatoshi.Common.Data.Entities.Address
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

	}
}
