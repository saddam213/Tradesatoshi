using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Data.DataContext;
using System.Data.Entity;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Core.Heplers;
using TradeSatoshi.Common;
using System.Threading;
using System.Security.Claims;
using System.Security.Permissions;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Withdraw;

namespace TradeSatoshi.Core.Withdraw
{
	public class WithdrawReader : IWithdrawReader
	{
		public IDataContext DataContext { get; set; }

		public List<WithdrawModel> GetWithdrawals(string userId)
		{
			using (var context = DataContext.CreateContext())
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
			using (var context = DataContext.CreateContext())
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
			using (var context = DataContext.CreateContext())
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
			using (var context = DataContext.CreateContext())
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
			using (var context = DataContext.CreateContext())
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
			using (var context = DataContext.CreateContext())
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
		
	}
}
