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

namespace TradeSatoshi.Core.Transfer
{
	public class TransferReader : ITransferReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public DataTablesResponse GetTransferDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TransferHistory
					.Include(t => t.Currency)
					.Include(t => t.User)
					.Include(t => t.ToUser)
					.Select(transfer => new TransferModel
					{
						Id = transfer.Id,
						Symbol = transfer.Currency.Symbol,
						Amount = transfer.Amount,
						Fee = transfer.Fee,
						TransferType = transfer.TransferType,
						Sender = transfer.User.UserName,
						Receiver = transfer.ToUser.UserName,
						TimeStamp = transfer.Timestamp
					});
				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse GetUserTransferDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TransferHistory
					.Include(t => t.Currency)
					.Include(t => t.User)
					.Include(t => t.ToUser)
					.Where(x => x.UserId == userId || x.ToUserId == userId)
					.Select(transfer => new TransferModel
					{
						Id = transfer.Id,
						Symbol = transfer.Currency.Symbol,
						Amount = transfer.Amount,
						Fee = transfer.Fee,
						TransferType = transfer.TransferType,
						Sender = transfer.User.UserName,
						Receiver = transfer.ToUser.UserName,
						TimeStamp = transfer.Timestamp
					});
				return query.GetDataTableResult(model);
			}
		}

		public async Task<CreateTransferModel> GetCreateTransferAsync(string userId, int currencyId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var userInfo = await context.Currency
					.Where(c => c.Id == currencyId)
					.Select(x => new CreateTransferModel
					{
						CurrencyId = x.Id,
						Fee = x.TransferFee,
						Symbol = x.Symbol,
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
