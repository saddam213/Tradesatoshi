﻿using System;
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
						UserName = withdraw.User.UserName,
						Fee = withdraw.Fee,
						Id = withdraw.Id,
						Symbol = withdraw.Currency.Symbol,
						TimeStamp = withdraw.TimeStamp,
						Txid = withdraw.Txid,
						WithdrawStatus = withdraw.WithdrawStatus
					});
				return await query.ToListNoLockAsync();
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
						UserName = withdraw.User.UserName,
						Fee = withdraw.Fee,
						Id = withdraw.Id,
						Symbol = withdraw.Currency.Symbol,
						TimeStamp = withdraw.TimeStamp,
						Txid = withdraw.Txid,
						WithdrawStatus = withdraw.WithdrawStatus
					});
				return await query.ToListNoLockAsync();
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
						UserName = withdraw.User.UserName,
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
						UserName = withdraw.User.UserName,
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
					}).FirstOrDefaultNoLockAsync();

				var balance = await context.Balance.Where(c => c.UserId == userId && c.CurrencyId == currencyId).FirstOrDefaultNoLockAsync();
				if (balance != null)
				{
					userInfo.Balance = balance.Avaliable;
				}

				return userInfo;
			}
		}

		public async Task<string> GetWithdrawalToken(string userId, int withdrawId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.Withdraw
					.Where(x => x.Id == withdrawId && x.UserId == userId)
					.Select(withdraw => withdraw.TwoFactorToken)
					.FirstOrDefaultNoLockAsync();
			}
		}
	}
}