using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.TradePair;
using TradeSatoshi.Common.Validation;

namespace TradeSatoshi.Core.TradePair
{
	public class TradePairWriter : ITradePairWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<WriterResult<bool>> CreateTradePair(string userId, CreateTradePairModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var existing = await context.TradePair.Where(t => (t.CurrencyId1 == model.CurrencyId1 && t.CurrencyId2 == model.CurrencyId2) || (t.CurrencyId1 == model.CurrencyId2 && t.CurrencyId2 == model.CurrencyId1)).ToListNoLockAsync();
				if (existing.Any())
					return WriterResult<bool>.ErrorResult("{0} already exists", existing.Any(x => x.CurrencyId1 == model.CurrencyId1) ? "TradePair" : "Inverse TradePair");

				var currency = await context.Currency.Where(x => x.Id == model.CurrencyId1).FirstOrDefaultNoLockAsync();
				if (currency == null)
					return WriterResult<bool>.ErrorResult("Currency '{0}' not found", model.CurrencyId1);

				var baseCurrency = await context.Currency.Where(x => x.Id == model.CurrencyId2).FirstOrDefaultNoLockAsync();
				if (baseCurrency == null)
					return WriterResult<bool>.ErrorResult("Currency '{0}' not found", model.CurrencyId2);

				var entity = new Entity.TradePair
				{
					CurrencyId1 = model.CurrencyId1,
					CurrencyId2 = model.CurrencyId2,
					Status = model.Status,
					Name = $"{currency.Symbol}_{baseCurrency.Symbol}"
				};

				context.TradePair.Add(entity);
				await context.SaveChangesAsync();
				return WriterResult<bool>.SuccessResult();
			}
		}

		public async Task<WriterResult<bool>> UpdateTradePair(string userId, UpdateTradePairModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var tradePair = await context.TradePair.FirstOrDefaultNoLockAsync(x => x.Id == model.Id);
				if (tradePair == null)
					return WriterResult<bool>.ErrorResult("TradePair '{0}' not found");

				tradePair.Status = model.Status;
				tradePair.StatusMessage = model.StatusMessage;

				await context.SaveChangesAsync();
				return WriterResult<bool>.SuccessResult();
			}
		}
	}
}