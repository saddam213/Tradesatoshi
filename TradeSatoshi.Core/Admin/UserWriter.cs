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
	public class UserWriter : IUserWriter
	{
		public IDataContext DataContext { get; set; }

		public bool UpdateUser(UpdateUserModel model)
		{
			using (var context = DataContext.CreateContext())
			{
				var user = context.Users
					.Include(x => x.Profile)
					.FirstOrDefault(x => x.Id == model.UserId);
				if (user == null)
					return false;

				user.UserName = model.UserName;
				user.Email = model.Email;
				user.LockoutEndDateUtc = model.IsLocked ? DateTime.UtcNow.AddYears(10) : DateTime.UtcNow;
				user.IsEnabled = model.IsEnabled;
				user.IsTradeEnabled = model.IsTradeEnabled;
				user.IsWithdrawEnabled = model.IsWithdrawEnabled;
				user.Profile.FirstName = model.FirstName;
				user.Profile.LastName = model.LastName;
				user.Profile.Address = model.Address;
				user.Profile.BirthDate = model.BirthDate;
				user.Profile.City = model.City;
				user.Profile.Country = model.Country;
				user.Profile.PostCode = model.PostCode;
				user.Profile.State = model.State;
				context.SaveChanges();

				return true;
			}
		}

		public async Task<bool> UpdateUserAsync(UpdateUserModel model)
		{
			using (var context = DataContext.CreateContext())
			{
				var user = await context.Users
					.Include(x => x.Profile)
					.FirstOrDefaultAsync(x => x.Id == model.UserId);
				if (user == null)
					return false;

				user.UserName = model.UserName;
				user.Email = model.Email;
				user.LockoutEndDateUtc = model.IsLocked ? DateTime.UtcNow.AddYears(10) : DateTime.UtcNow;
				user.IsEnabled = model.IsEnabled;
				user.IsTradeEnabled = model.IsTradeEnabled;
				user.IsWithdrawEnabled = model.IsWithdrawEnabled;
				user.Profile.FirstName = model.FirstName;
				user.Profile.LastName = model.LastName;
				user.Profile.Address = model.Address;
				user.Profile.BirthDate = model.BirthDate;
				user.Profile.City = model.City;
				user.Profile.Country = model.Country;
				user.Profile.PostCode = model.PostCode;
				user.Profile.State = model.State;
				await context.SaveChangesAsync();

				return true;
			}
		}
	}
}
