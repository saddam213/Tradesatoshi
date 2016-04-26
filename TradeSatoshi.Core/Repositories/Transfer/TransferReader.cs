using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Core.Helpers;

namespace TradeSatoshi.Core.Transfer
{
	public class TransferReader : ITransferReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetTransferDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TransferHistory
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
				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<DataTablesResponse> GetUserTransferDataTable(DataTablesModel model, string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.TransferHistory
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
				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		public async Task<CreateTransferModel> GetCreateTransfer(string userId, int currencyId)
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