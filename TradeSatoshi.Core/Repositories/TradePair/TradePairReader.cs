using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Core.Helpers;

namespace TradeSatoshi.Core.TradePair
{
	public class TradePairReader : ITradePairReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<TradePairModel> GetTradePair(int tradePairId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.TradePair
					.Where(t => t.Currency1.IsEnabled && t.Currency2.IsEnabled)
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.Select(MapTradePair)
					.FirstOrDefaultNoLockAsync(t => t.Id == tradePairId);
			}
		}

		public async Task<UpdateTradePairModel> GetTradePairUpdate(int tradePairId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var tradepair = await context.TradePair
					.Where(t => t.Currency1.IsEnabled && t.Currency2.IsEnabled)
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.FirstOrDefaultNoLockAsync(t => t.Id == tradePairId);
				if (tradepair == null)
					return null;

				return new UpdateTradePairModel
				{
					Id = tradepair.Id,
					Name = tradepair.Name,
					Status = tradepair.Status,
					StatusMessage = tradepair.StatusMessage
				};
			}
		}

		public async Task<List<TradePairModel>> GetTradePairs()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.TradePair
					.Where(t => t.Currency1.IsEnabled && t.Currency2.IsEnabled)
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.Select(MapTradePair)
					.OrderBy(t => t.Name)
					.ToListNoLockAsync();
			}
		}

		public async Task<DataTablesResponse> GetTradePairDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.TradePair
					.Where(t => t.Currency1.IsEnabled && t.Currency2.IsEnabled)
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.Select(MapTradePair)
					.GetDataTableResultNoLockAsync(model);
			}
		}

		private Expression<Func<Entity.TradePair, TradePairModel>> MapTradePair
		{
			get
			{
				return tradepair => new TradePairModel
				{
					Id = tradepair.Id,
					Change = tradepair.Change,
					LastTrade = tradepair.LastTrade,
					Name = tradepair.Name,
					CurrencyId = tradepair.CurrencyId1,
					BaseCurrencyId = tradepair.CurrencyId2,
					Status = tradepair.Status,
					BaseSymbol = tradepair.Currency2.Symbol,
					Symbol = tradepair.Currency1.Symbol
				};
			}
		}
	}
}