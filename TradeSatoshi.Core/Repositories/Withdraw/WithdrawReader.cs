using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Core.Helpers;

namespace TradeSatoshi.Core.Withdraw
{
	public class WithdrawReader : IWithdrawReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<List<WithdrawModel>> GetWithdrawals(string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Withdraw
					.Where(x => x.UserId == userId && x.Currency.IsEnabled)
					.Select(withdraw => new WithdrawModel
					{
						Address = withdraw.Address,
						Amount = withdraw.Amount,
						Confirmations = withdraw.Confirmations,
						Currency = withdraw.Currency.Name,
						Fee = withdraw.Fee,
						Id = withdraw.Id,
						Symbol = withdraw.Currency.Symbol,
						TimeStamp = withdraw.TimeStamp,
						Txid = withdraw.Txid,
						WithdrawStatus = withdraw.WithdrawStatus
					});
				return await query.ToListAsync();
			}
		}

		public async Task<List<WithdrawModel>> GetWithdrawals(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Withdraw
					.Where(x => x.UserId == userId && x.CurrencyId == currencyId && x.Currency.IsEnabled)
					.Select(withdraw => new WithdrawModel
					{
						Address = withdraw.Address,
						Amount = withdraw.Amount,
						Confirmations = withdraw.Confirmations,
						Currency = withdraw.Currency.Name,
						Fee = withdraw.Fee,
						Id = withdraw.Id,
						Symbol = withdraw.Currency.Symbol,
						TimeStamp = withdraw.TimeStamp,
						Txid = withdraw.Txid,
						WithdrawStatus = withdraw.WithdrawStatus
					});
				return await query.ToListAsync();
			}
		}

		public async Task<DataTablesResponse> GetWithdrawDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Withdraw
					.Where(x => x.Currency.IsEnabled)
					.Select(withdraw => new WithdrawModel
					{
						Address = withdraw.Address,
						Amount = withdraw.Amount,
						Confirmations = withdraw.Confirmations,
						Currency = withdraw.Currency.Name,
						Fee = withdraw.Fee,
						Id = withdraw.Id,
						Symbol = withdraw.Currency.Symbol,
						TimeStamp = withdraw.TimeStamp,
						Txid = withdraw.Txid,
						WithdrawStatus = withdraw.WithdrawStatus
					});
				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetUserWithdrawDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Withdraw
					.Where(x => x.UserId == userId && x.Currency.IsEnabled)
					.Select(withdraw => new WithdrawModel
					{
						Address = withdraw.Address,
						Amount = withdraw.Amount,
						Confirmations = withdraw.Confirmations,
						Currency = withdraw.Currency.Name,
						Fee = withdraw.Fee,
						Id = withdraw.Id,
						Symbol = withdraw.Currency.Symbol,
						TimeStamp = withdraw.TimeStamp,
						Txid = withdraw.Txid,
						WithdrawStatus = withdraw.WithdrawStatus
					});
				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<CreateWithdrawModel> GetCreateWithdraw(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var userInfo = await context.Currency
					.Where(c => c.Id == currencyId && c.IsEnabled)
					.Select(x => new CreateWithdrawModel
					{
						CurrencyId = x.Id,
						Fee = x.WithdrawFee,
						MaxWithdraw = x.MaxWithdraw,
						MinWithdraw = x.MinWithdraw,
						Symbol = x.Symbol,
						WithdrawFeeType = x.WithdrawFeeType
					}).FirstOrDefaultAsync();

				var balance = await context.Balance.FirstOrDefaultAsync(c => c.UserId == userId && c.CurrencyId == currencyId);
				if (balance != null)
				{
					userInfo.Balance = balance.Avaliable;
				}

				return userInfo;
			}
		}
	}
}