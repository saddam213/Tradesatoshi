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
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Common.Data;

namespace TradeSatoshi.Core.Withdraw
{
	public class WithdrawReader : IWithdrawReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public List<WithdrawModel> GetWithdrawals(string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Withdraw
							.Where(x => x.UserId == userId)
							.Select(withdraw =>
							new WithdrawModel
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
				return query.ToList();
			}
		}

		public List<WithdrawModel> GetWithdrawals(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Withdraw
							.Where(x => x.UserId == userId && x.CurrencyId == currencyId)
							.Select(withdraw =>
							new WithdrawModel
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
				return query.ToList();
			}
		}

		public async Task<List<WithdrawModel>> GetWithdrawalsAsync(string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Withdraw
							.Where(x => x.UserId == userId)
							.Select(withdraw =>
							new WithdrawModel
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

		public async Task<List<WithdrawModel>> GetWithdrawalsAsync(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Withdraw
							.Where(x => x.UserId == userId && x.CurrencyId == currencyId)
							.Select(withdraw =>
							new WithdrawModel
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

		public DataTablesResponse GetWithdrawDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Withdraw
							.Select(withdraw =>
							new WithdrawModel
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
				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse GetUserWithdrawDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.Withdraw
							.Where(x => x.UserId == userId)
							.Select(withdraw =>
							new WithdrawModel
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
				return query.GetDataTableResult(model);
			}
		}


		public CreateWithdrawModel GetCreateWithdraw(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var userInfo = context.Currency
						.Where(c => c.Id == currencyId)
						.Select(x => new CreateWithdrawModel
						{
							CurrencyId = x.Id,
							Fee = x.WithdrawFee,
							MaxWithdraw = x.MaxWithdraw,
							MinWithdraw = x.MinWithdraw,
							Symbol = x.Symbol,
							WithdrawFeeType = x.WithdrawFeeType
						}).FirstOrDefault();

				var balance = context.Balance.FirstOrDefault(c => c.UserId == userId && c.CurrencyId == currencyId);
				if (balance != null)
				{
					userInfo.Balance = balance.Avaliable;
				}

				return userInfo;
			}
		}

		public async Task<CreateWithdrawModel> GetCreateWithdrawAsync(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var userInfo = await context.Currency
						.Where(c => c.Id == currencyId)
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
