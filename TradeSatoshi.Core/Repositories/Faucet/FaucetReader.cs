using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Faucet;
using TradeSatoshi.Core.Helpers;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Faucet
{
	public class FaucetReader : IFaucetReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetFaucetDataTable(DataTablesModel param, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var lastDate = DateTime.UtcNow.AddHours(-24);
				var query = from currency in context.Currency.Where(x => x.IsFaucetEnabled)
										from balance in context.Balance.Where(x => x.CurrencyId == currency.Id && x.UserId == Constants.SystemFaucetUserId).DefaultIfEmpty()
										from faucetpayment in context.FaucetPayments.Where(x => x.CurrencyId == currency.Id && x.UserId == userId && x.Timestamp > lastDate).DefaultIfEmpty()
										select new
										{
											Id = currency.Id,
											Symbol = currency.Symbol,
											Balance = (decimal?)balance.Total ?? 0,
											Amount = currency.FaucetPayment,
											LastClaim = (DateTime?)faucetpayment.Timestamp ?? lastDate,
											Claimed = faucetpayment != null
										};

				return await query.GetDataTableResultNoLockAsync(param);
			}
		}
	}
}
