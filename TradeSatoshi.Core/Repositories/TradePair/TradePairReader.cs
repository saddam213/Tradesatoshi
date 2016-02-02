using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Data.DataContext;
using System.Data.Entity;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Core.Helpers;
using TradeSatoshi.Common;
using System.Threading;
using System.Security.Claims;
using System.Security.Permissions;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Deposit;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.TradePair;
using System.Linq.Expressions;

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
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.Select(MapTradePair)
					.FirstOrDefaultAsync(t => t.Id == tradePairId);
			}
		}

		public async Task<UpdateTradePairModel> GetTradePairUpdate(int tradePairId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var tradepair = await context.TradePair
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.FirstOrDefaultAsync(t => t.Id == tradePairId);
				if (tradepair == null)
					return null;

				return new UpdateTradePairModel
				{
					Id = tradepair.Id,
					Name = tradepair.Currency1.Symbol + "/" + tradepair.Currency2.Symbol,
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
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.Select(MapTradePair)
					.ToListAsync();
			}
		}

		public DataTablesResponse GetTradePairDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return context.TradePair
					.Include(t => t.Currency1)
					.Include(t => t.Currency2)
					.Select(MapTradePair)
					.GetDataTableResult(model);
			}
		}

		private Expression<Func<Entity.TradePair, TradePairModel>> MapTradePair
		{
			get
			{
				return (tradepair) => new TradePairModel
				{
					Id = tradepair.Id,
					Name = tradepair.Currency1.Symbol + "/" + tradepair.Currency2.Symbol,
					CurrencyId = tradepair.CurrencyId1,
					BaseCurrencyId = tradepair.CurrencyId2,
					Status = tradepair.Status
				};
			}
		}


	}
}
