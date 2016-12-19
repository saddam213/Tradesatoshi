using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TradeSatoshi.Common.Currency;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Core.Helpers;

namespace TradeSatoshi.Core.Currency
{
	public class CurrencyReader : ICurrencyReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<CurrencyModel> GetCurrency(int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Currency.Select(MapCurrency).FirstOrDefaultNoLockAsync(c => c.Id == currencyId);
			}
		}

		public async Task<UpdateCurrencyModel> GetCurrencyUpdate(int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var currency = await context.Currency.FirstOrDefaultNoLockAsync(c => c.Id == currencyId);
				if (currency == null)
					return null;

				return new UpdateCurrencyModel
				{
					Id = currency.Id,
					IsEnabled = currency.IsEnabled,
					MaxTrade = currency.MaxTrade,
					MaxWithdraw = currency.MaxWithdraw,
					MinBaseTrade = currency.MinBaseTrade,
					MinConfirmations = currency.MinConfirmations,
					MinTrade = currency.MinTrade,
					MinWithdraw = currency.MinWithdraw,
					Name = currency.Name,
					Status = currency.Status,
					StatusMessage = currency.StatusMessage,
					Symbol = currency.Symbol,
					TradeFee = currency.TradeFee,
					TransferFee = currency.TransferFee,
					WithdrawFee = currency.WithdrawFee,
					WithdrawFeeType = currency.WithdrawFeeType,
					ColdBalance = currency.ColdBalance,
					MarketSortOrder = currency.MarketSortOrder,
					Algo = currency.Algo,
					InterfaceType = currency.InterfaceType,
					Type = currency.Type,
					FaucetMax = currency.FaucetMax,
					FaucetPayment = currency.FaucetPayment,
					IsFaucetEnabled = currency.IsFaucetEnabled
				};
			}
		}

		public async Task<List<CurrencyModel>> GetCurrencies()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Currency.Select(MapCurrency).ToListNoLockAsync();
			}
		}

		public async Task<DataTablesResponse> GetCurrencyDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Currency.Select(MapCurrency).GetDataTableResultNoLockAsync(model);
				;
			}
		}

		public async Task<List<CurrencyStatusModel>> GetCurrencyStatus()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Currency.Where(c => c.IsEnabled).Select(MapCurrencyStatus).ToListNoLockAsync();
			}
		}

		private static Expression<Func<Entity.Currency, CurrencyModel>> MapCurrency
		{
			get
			{
				return currency => new CurrencyModel
				{
					Id = currency.Id,
					Name = currency.Name,
					Symbol = currency.Symbol,
					Status = currency.Status,
					Version = currency.Version,
					Balance = currency.Balance,
					Connections = currency.Connections,
					Block = currency.Block,
					Error = currency.Errors,
					IsEnabled = currency.IsEnabled
				};
			}
		}

		private static Expression<Func<Entity.Currency, CurrencyStatusModel>> MapCurrencyStatus
		{
			get
			{
				return currency => new CurrencyStatusModel
				{
					CurrencyId = currency.Id,
					Symbol = currency.Symbol,
					Status = currency.Status,
					StatusMessage = currency.StatusMessage,
					LastBlock = currency.Block,
					Connections = currency.Connections,
					Version = currency.Version
				};
			}
		}
	}
}