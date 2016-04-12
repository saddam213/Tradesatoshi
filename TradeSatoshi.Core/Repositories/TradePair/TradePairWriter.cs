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
				var existing = await context.TradePair.Where(t => (t.CurrencyId1 == model.CurrencyId1 && t.CurrencyId2 == model.CurrencyId2) || (t.CurrencyId1 == model.CurrencyId2 && t.CurrencyId2 == model.CurrencyId1)).ToListAsync();
				if (existing.Any())
					return WriterResult<bool>.ErrorResult("{0} already exists", existing.Any(x => x.CurrencyId1 == model.CurrencyId1) ? "TradePair" : "Inverse TradePair");

				var currencies = await context.Currency.Where(x => x.Id == model.CurrencyId1 || x.Id == model.CurrencyId2).ToListAsync();
				if (currencies.Count != 2)
					return WriterResult<bool>.ErrorResult("Currency '{0}' not found", currencies.Any(x => x.Id == model.CurrencyId1) ? model.CurrencyId2 : model.CurrencyId1);

				var entity = new Entity.TradePair
				{
					CurrencyId1 = model.CurrencyId1,
					CurrencyId2 = model.CurrencyId2,
					Status = model.Status,
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

				var tradePair = await context.TradePair.FirstOrDefaultAsync(x => x.Id == model.Id);
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
