using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Core.Helpers;

namespace TradeSatoshi.Core.Admin
{
	public class UserReader : IUserReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		//[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public async Task<DataTablesResponse> GetUserDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
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
						IsTransferEnabled = x.IsTransferEnabled
					});

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		//[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public async Task<UserModel> GetUser(string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = await context.Users
					.Where(x => x.Id == userId)
					.Select(x => new UserModel
					{
						UserId = x.Id,
						UserName = x.UserName,
						Email = x.Email,
						IsLocked = x.LockoutEndDateUtc.HasValue && x.LockoutEndDateUtc > DateTime.UtcNow,
						IsEnabled = x.IsEnabled,
						IsTradeEnabled = x.IsTradeEnabled,
						IsWithdrawEnabled = x.IsWithdrawEnabled,
						IsTransferEnabled = x.IsTransferEnabled
					}).FirstOrDefaultNoLockAsync();

				return query;
			}
		}

		//[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public async Task<UpdateUserModel> GetUserUpdate(string userId)
		{
			using (var context = DataContextFactory.CreateContext())
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
						IsTransferEnabled = x.IsTransferEnabled,
						FirstName = x.Profile.FirstName,
						LastName = x.Profile.LastName,
						Address = x.Profile.Address,
						BirthDate = x.Profile.BirthDate,
						City = x.Profile.City,
						Country = x.Profile.Country,
						PostCode = x.Profile.PostCode,
						State = x.Profile.State
					}).FirstOrDefaultNoLockAsync(x => x.UserId == userId);

				return query;
			}
		}

		//[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public async Task<DataTablesResponse> GetLogonDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.UserLogons
					.Include(u => u.User)
					.Select(x => new LogonModel
					{
						IPAddress = x.IPAddress,
						Timestamp = x.Timestamp,
						UserName = x.User.UserName,
						IsValid = x.IsValid ? "Success" : "Failed"
					});

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}

		//[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public async Task<DataTablesResponse> GetRolesDataTable(DataTablesModel model, SecurityRole securityRole)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var users = context.UserRoles
					.Include(x => x.User)
					.Include(x => x.Role)
					.Where(x => x.Role.Name == securityRole.ToString())
					.Select(x => new RoleModel
					{
						RoleName = x.Role.Name,
						UserName = x.User.UserName
					});

				return await users.GetDataTableResultNoLockAsync(model);
			}
		}
	}
}