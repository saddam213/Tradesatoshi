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
using TradeSatoshi.Common.Deposit;
using TradeSatoshi.Common.Data;

namespace TradeSatoshi.Core.Balance
{
	public class DepositReader : IDepositReader
	{
		public IDataContext DataContext { get; set; }

		public List<DepositModel> GetDeposits(string userId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.Deposit
							.Where(x => x.UserId == userId)
							.Select(deposit =>
							new DepositModel
							{
								Amount = deposit.Amount,
								Confirmations = deposit.Confirmations,
								Currency = deposit.Currency.Name,
								Id = deposit.Id,
								Symbol = deposit.Currency.Symbol,
								TimeStamp = deposit.TimeStamp,
								Txid = deposit.Txid,
								DepositStatus = deposit.DepositStatus
							});
				return query.ToList();
			}
		}

		public List<DepositModel> GetDeposits(string userId, int currencyId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.Deposit
							.Where(x => x.UserId == userId)
							.Select(deposit =>
							new DepositModel
							{
								Amount = deposit.Amount,
								Confirmations = deposit.Confirmations,
								Currency = deposit.Currency.Name,
								Id = deposit.Id,
								Symbol = deposit.Currency.Symbol,
								TimeStamp = deposit.TimeStamp,
								Txid = deposit.Txid,
								DepositStatus = deposit.DepositStatus
							});
				return query.ToList();
			}
		}

		public async Task<List<DepositModel>> GetDepositsAsync(string userId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.Deposit
							.Where(x => x.UserId == userId)
							.Select(deposit =>
							new DepositModel
							{
								Amount = deposit.Amount,
								Confirmations = deposit.Confirmations,
								Currency = deposit.Currency.Name,
								Id = deposit.Id,
								Symbol = deposit.Currency.Symbol,
								TimeStamp = deposit.TimeStamp,
								Txid = deposit.Txid,
								DepositStatus = deposit.DepositStatus
							});
				return await query.ToListAsync();
			}
		}

		public async Task<List<DepositModel>> GetDepositsAsync(string userId, int currencyId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.Deposit
							.Where(x => x.UserId == userId && x.CurrencyId == currencyId)
							.Select(deposit =>
							new DepositModel
							{
								Amount = deposit.Amount,
								Confirmations = deposit.Confirmations,
								Currency = deposit.Currency.Name,
								Id = deposit.Id,
								Symbol = deposit.Currency.Symbol,
								TimeStamp = deposit.TimeStamp,
								Txid = deposit.Txid,
								DepositStatus = deposit.DepositStatus
							});
				return await query.ToListAsync();
			}
		}

		public DataTablesResponse GetDepositDataTable(DataTablesModel model)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.Deposit
							.Select(deposit =>
							new DepositModel
							{
								Amount = deposit.Amount,
								Confirmations = deposit.Confirmations,
								Currency = deposit.Currency.Name,
								Id = deposit.Id,
								Symbol = deposit.Currency.Symbol,
								TimeStamp = deposit.TimeStamp,
								Txid = deposit.Txid,
								DepositStatus = deposit.DepositStatus
							});
				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse GetUserDepositDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.Deposit
							.Where(x => x.UserId == userId)
							.Select(deposit =>
							new DepositModel
							{
								Amount = deposit.Amount,
								Confirmations = deposit.Confirmations,
								Currency = deposit.Currency.Name,
								Id = deposit.Id,
								Symbol = deposit.Currency.Symbol,
								TimeStamp = deposit.TimeStamp,
								Txid = deposit.Txid,
								DepositStatus = deposit.DepositStatus
							});
				return query.GetDataTableResult(model);
			}
		}
	}
}
