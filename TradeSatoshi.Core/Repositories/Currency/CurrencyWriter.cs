using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Validation;
using System.Data.Entity;

namespace TradeSatoshi.Core.Currency
{
	public class CurrencyWriter : ICurrencyWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<WriterResult<bool>> CreateCurrency(string userId, CreateCurrencyModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var existing = await context.Currency.FirstOrDefaultAsync(c => c.Name == model.Name || c.Symbol == model.Symbol);
				if (existing != null)
					return WriterResult<bool>.ErrorResult("Currency with {0} already exists.", existing.Name == model.Name ? string.Format("Name '{0}'", model.Name) : string.Format("Symbol '{0}'", model.Symbol));

				existing = await context.Currency.FirstOrDefaultAsync(c => c.WalletPort == model.WalletPort && c.WalletHost == model.WalletHost);
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
				var currency = await context.Currency.FindAsync(model.Id);
				if (currency == null)
					return WriterResult<bool>.ErrorResult("Currency {0} not found.", model.Id);

				var existing = await context.Currency.FirstOrDefaultAsync(c => c.Id != currency.Id && (c.Name == model.Name || c.Symbol == model.Symbol));
				if (existing != null)
					return WriterResult<bool>.ErrorResult("Currency with {0} already exists.", existing.Name == model.Name ? string.Format("Name '{0}'", model.Name) : string.Format("Symbol '{0}'", model.Symbol));

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
				currency.Symbol = model.Symbol;
				currency.TradeFee = model.TradeFee;
				currency.TransferFee = model.TransferFee;
				currency.WithdrawFee = model.WithdrawFee;
				currency.WithdrawFeeType = model.WithdrawFeeType;

				await context.SaveChangesAsync();
				return WriterResult<bool>.SuccessResult();
			}
		}
	}
}
