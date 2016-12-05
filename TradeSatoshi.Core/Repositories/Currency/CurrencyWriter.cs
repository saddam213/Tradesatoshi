using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Core.Currency
{
	public class CurrencyWriter : ICurrencyWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<WriterResult<bool>> CreateCurrency(string userId, CreateCurrencyModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var existing = await context.Currency.FirstOrDefaultNoLockAsync(c => c.Name == model.Name || c.Symbol == model.Symbol);
				if (existing != null)
					return WriterResult<bool>.ErrorResult("Currency with {0} already exists.", existing.Name == model.Name ? $"Name '{model.Name}'" : $"Symbol '{model.Symbol}'");

				existing = await context.Currency.FirstOrDefaultNoLockAsync(c => c.WalletPort == model.WalletPort && c.WalletHost == model.WalletHost);
				if (existing != null)
					return WriterResult<bool>.ErrorResult("Wallet with RPC settings {0}:{1} already exists.", model.WalletHost, model.WalletPort);

				var entity = new Entity.Currency
				{
					IsEnabled = model.IsEnabled,
					MaxTrade = model.MaxTrade,
					MaxWithdraw = model.MaxWithdraw,
					MinBaseTrade = model.MinBaseTrade,
					MinConfirmations = model.MinConfirmations,
					MinTrade = model.MinTrade,
					MinWithdraw = model.MinWithdraw,
					Name = model.Name,
					Status = model.Status,
					StatusMessage = model.StatusMessage,
					Symbol = model.Symbol,
					TradeFee = model.TradeFee,
					TransferFee = model.TransferFee,
					WithdrawFee = model.WithdrawFee,
					WithdrawFeeType = model.WithdrawFeeType,
					WalletHost = model.WalletHost,
					WalletPass = model.WalletPass,
					WalletPort = model.WalletPort,
					WalletUser = model.WalletUser,
					ColdBalance = 0m,

					MarketSortOrder = model.MarketSortOrder,
					Algo = model.Algo,
					InterfaceType = model.InterfaceType,
					Type = model.Type,
					IsFaucetEnabled = model.IsFaucetEnabled,
					FaucetMax = model.FaucetMax,
					FaucetPayment = model.FaucetPayment
				};

				context.Currency.Add(entity);
				await context.SaveChangesAsync();
				return WriterResult<bool>.SuccessResult();
			}
		}

		public async Task<WriterResult<bool>> UpdateCurrency(string userId, UpdateCurrencyModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var currency = await context.Currency.FirstOrDefaultNoLockAsync(c => c.Id == model.Id);
				if (currency == null)
					return WriterResult<bool>.ErrorResult("Currency {0} not found.", model.Id);

				var existing = await context.Currency.FirstOrDefaultNoLockAsync(c => c.Id != currency.Id && (c.Name == model.Name || c.Symbol == model.Symbol));
				if (existing != null)
					return WriterResult<bool>.ErrorResult("Currency with {0} already exists.", existing.Name == model.Name ? $"Name '{model.Name}'" : $"Symbol '{model.Symbol}'");

				currency.IsEnabled = model.IsEnabled;
				currency.MaxTrade = model.MaxTrade;
				currency.MaxWithdraw = model.MaxWithdraw;
				currency.MinBaseTrade = model.MinBaseTrade;
				currency.MinConfirmations = model.MinConfirmations;
				currency.MinTrade = model.MinTrade;
				currency.MinWithdraw = model.MinWithdraw;
				currency.Name = model.Name;
				currency.Status = model.Status;
				currency.StatusMessage = model.StatusMessage;
				currency.TradeFee = model.TradeFee;
				currency.TransferFee = model.TransferFee;
				currency.WithdrawFee = model.WithdrawFee;
				currency.WithdrawFeeType = model.WithdrawFeeType;
				currency.ColdBalance = model.ColdBalance;
				currency.MarketSortOrder = model.MarketSortOrder;
				currency.Algo = model.Algo;
				currency.InterfaceType = model.InterfaceType;
				currency.Type = model.Type;
				currency.IsFaucetEnabled = model.IsFaucetEnabled;
				currency.FaucetPayment = model.FaucetPayment;
				currency.FaucetMax = model.FaucetMax;

				// Id the symbol has changed update the tradepair names
				if (currency.Symbol != model.Symbol)
				{
					currency.Symbol = model.Symbol;
					var tradePairs = await context.TradePair
						.Include(testc => testc.Currency1)
						.Include(testc => testc.Currency2)
						.Where(t => t.CurrencyId1 == model.Id || t.CurrencyId2 == model.Id)
						.ToListNoLockAsync();
					foreach (var tradePair in tradePairs)
					{
						tradePair.Name = tradePair.CurrencyId1 == model.Id
							? $"{currency.Symbol}_{tradePair.Currency2.Symbol}"
							: $"{tradePair.Currency1.Symbol}_{currency.Symbol}";
					}
				}

				await context.SaveChangesAsync();
				return WriterResult<bool>.SuccessResult();
			}
		}
	}
}