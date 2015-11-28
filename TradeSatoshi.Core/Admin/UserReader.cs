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

namespace TradeSatoshi.Core.Admin
{
	public class UserReader : IUserReader
	{
		public IDataContext DataContext { get; set; }

		public DataTablesResponse GetUserDataTable(DataTablesModel model)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.Users
						.Select(x => new UserModel
						{
							UserId = x.Id,
							UserName = x.UserName,
							Email = x.Email,
							IsLocked = x.LockoutEndDateUtc.HasValue && x.LockoutEndDateUtc > DateTime.UtcNow,
							IsEnabled = x.IsEnabled,
							IsTradeEnabled = x.IsTradeEnabled,
							IsWithdrawEnabled = x.IsWithdrawEnabled,
						});

				return query.GetDataTableResult(model);
			}
		}

		public UserModel GetUser(string userId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.Users
						.Select(x => new UserModel
						{
							UserId = x.Id,
							UserName = x.UserName,
							Email = x.Email,
							IsLocked = x.LockoutEndDateUtc.HasValue && x.LockoutEndDateUtc > DateTime.UtcNow,
							IsEnabled = x.IsEnabled,
							IsTradeEnabled = x.IsTradeEnabled,
							IsWithdrawEnabled = x.IsWithdrawEnabled,
						}).FirstOrDefault(x => x.UserId == userId);

				return query;
			}
		}

		public async Task<UserModel> GetUserAsync(string userId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = await context.Users
						.Select(x => new UserModel
						{
							UserId = x.Id,
							UserName = x.UserName,
							Email = x.Email,
							IsLocked = x.LockoutEndDateUtc.HasValue && x.LockoutEndDateUtc > DateTime.UtcNow,
							IsEnabled = x.IsEnabled,
							IsTradeEnabled = x.IsTradeEnabled,
							IsWithdrawEnabled = x.IsWithdrawEnabled,
						}).FirstOrDefaultAsync(x => x.UserId == userId);

				return query;
			}
		}


		public UpdateUserModel GetUserUpdate(string userId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.Users
					.Include(x => x.Profile)
					.Select(x => new UpdateUserModel
					{
						UserId = x.Id,
						UserName = x.UserName,
						Email = x.Email,
						IsLocked = x.LockoutEndDateUtc.HasValue && x.LockoutEndDateUtc > DateTime.UtcNow,
						IsEnabled = x.IsEnabled,
						IsTradeEnabled = x.IsTradeEnabled,
						IsWithdrawEnabled = x.IsWithdrawEnabled,
						FirstName = x.Profile.FirstName,
						LastName = x.Profile.LastName,
						Address = x.Profile.Address,
						BirthDate = x.Profile.BirthDate,
						City = x.Profile.City,
						Country = x.Profile.Country,
						PostCode = x.Profile.PostCode,
						State = x.Profile.State
					}).FirstOrDefault(x => x.UserId == userId);

				return query;
			}
		}

		public async Task<UpdateUserModel> GetUserUpdateAsync(string userId)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = await context.Users
						.Include(x => x.Profile)
						.Select(x => new UpdateUserModel
						{
							UserId = x.Id,
							UserName = x.UserName,
							Email = x.Email,
							IsLocked = x.LockoutEndDateUtc.HasValue && x.LockoutEndDateUtc > DateTime.UtcNow,
							IsEnabled = x.IsEnabled,
							IsTradeEnabled = x.IsTradeEnabled,
							IsWithdrawEnabled = x.IsWithdrawEnabled,
							FirstName = x.Profile.FirstName,
							LastName = x.Profile.LastName,
							Address = x.Profile.Address,
							BirthDate = x.Profile.BirthDate,
							City = x.Profile.City,
							Country = x.Profile.Country,
							PostCode = x.Profile.PostCode,
							State = x.Profile.State
						}).FirstOrDefaultAsync(x => x.UserId == userId);

				return query;
			}
		}
	}
}
