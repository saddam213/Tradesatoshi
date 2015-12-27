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
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Common.Security;
using System.Data.Entity.Validation;
using System.Security.Permissions;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Entity;

namespace TradeSatoshi.Core.Admin
{
	public class UserWriter : IUserWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public IWriterResult<bool> UpdateUser(UpdateUserModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var existinguser = context.Users.FirstOrDefault(x => (x.Email == model.Email && x.Id != model.UserId) || (x.UserName == model.UserName && x.Id != model.UserId));
				if (existinguser != null)
				{
					return WriterResult<bool>.ErrorResult(model.UserName == existinguser.UserName ? "Username already in use." : "Email already in use.");
				}
				var user = context.Users
					.Include(x => x.Profile)
					.FirstOrDefault(x => x.Id == model.UserId);
				if (user == null)
					return WriterResult<bool>.ErrorResult("User {0} not found.", model.UserName);

				user.UserName = model.UserName;
				user.Email = model.Email;
				user.LockoutEndDateUtc = model.IsLocked ? DateTime.UtcNow.AddYears(10) : DateTime.UtcNow;
				user.IsEnabled = model.IsEnabled;
				user.IsTradeEnabled = model.IsTradeEnabled;
				user.IsWithdrawEnabled = model.IsWithdrawEnabled;
				user.IsTransferEnabled = model.IsTransferEnabled;
				user.Profile.FirstName = model.FirstName;
				user.Profile.LastName = model.LastName;
				user.Profile.Address = model.Address;
				user.Profile.BirthDate = model.BirthDate;
				user.Profile.City = model.City;
				user.Profile.Country = model.Country;
				user.Profile.PostCode = model.PostCode;
				user.Profile.State = model.State;

				context.SaveChanges();

				return WriterResult<bool>.SuccessResult();
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public async Task<IWriterResult<bool>> UpdateUserAsync(UpdateUserModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var existinguser = await context.Users.FirstOrDefaultAsync(x => (x.Email == model.Email && x.Id != model.UserId) || (x.UserName == model.UserName && x.Id != model.UserId));
				if (existinguser != null)
				{
					return WriterResult<bool>.ErrorResult(model.UserName == existinguser.UserName ? "Username already in use." : "Email already in use.");
				}
				var user = await context.Users
					.Include(x => x.Profile)
					.FirstOrDefaultAsync(x => x.Id == model.UserId);
				if (user == null)
					return WriterResult<bool>.ErrorResult("User {0} not found.", model.UserName);

				user.UserName = model.UserName;
				user.Email = model.Email;
				user.LockoutEndDateUtc = model.IsLocked ? DateTime.UtcNow.AddYears(10) : DateTime.UtcNow;
				user.IsEnabled = model.IsEnabled;
				user.IsTradeEnabled = model.IsTradeEnabled;
				user.IsWithdrawEnabled = model.IsWithdrawEnabled;
				user.IsTransferEnabled = model.IsTransferEnabled;
				user.Profile.FirstName = model.FirstName;
				user.Profile.LastName = model.LastName;
				user.Profile.Address = model.Address;
				user.Profile.BirthDate = model.BirthDate;
				user.Profile.City = model.City;
				user.Profile.Country = model.Country;
				user.Profile.PostCode = model.PostCode;
				user.Profile.State = model.State;

				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public IWriterResult<bool> AddUserRole(UserRoleModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = context.Users.FirstOrDefault(x => x.UserName == model.UserName);
				if (user == null)
					return WriterResult<bool>.ErrorResult("User {0} not found.", model.UserName);

				var role = context.Roles.FirstOrDefault(x => x.Name == model.SecurityRole.ToString());
				if (role == null)
					return WriterResult<bool>.ErrorResult("{0} role does not exist", model.SecurityRole);

				var exists = role.Users.FirstOrDefault(x => x.UserId == user.Id);
				if (exists != null)
					return WriterResult<bool>.ErrorResult("{0} is already assigned to {1} role.", model.UserName, model.SecurityRole);

				context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
				context.SaveChanges();
				return WriterResult<bool>.SuccessResult();
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public async Task<IWriterResult<bool>> AddUserRoleAsync(UserRoleModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
				if (user == null)
					return WriterResult<bool>.ErrorResult("User {0} not found.", model.UserName);

				var role = await context.Roles.FirstOrDefaultAsync(x => x.Name == model.SecurityRole.ToString());
				if (role == null)
					return WriterResult<bool>.ErrorResult("{0} role does not exist", model.SecurityRole);

				var exists = context.UserRoles.FirstOrDefault(x => x.User.UserName == model.UserName && x.Role.Name == model.SecurityRole.ToString());
				if (exists != null)
					return WriterResult<bool>.ErrorResult("{0} is already assigned to {1} role.", model.UserName, model.SecurityRole);

				context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public IWriterResult<bool> RemoveUserRole(UserRoleModel model)
		{
			if (model.SecurityRole == SecurityRole.Standard)
				return WriterResult<bool>.ErrorResult("The {0} role cannot be remove from users.", SecurityRole.Standard);

			using (var context = DataContextFactory.CreateContext())
			{
				var role = context.UserRoles.FirstOrDefault(x => x.User.UserName == model.UserName && x.Role.Name == model.SecurityRole.ToString());
				if (role == null)
					return WriterResult<bool>.ErrorResult("{0} in not assigned to {1} role.", model.UserName, model.SecurityRole);

				context.UserRoles.Remove(role);
				context.SaveChanges();
				return WriterResult<bool>.SuccessResult();
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public async Task<IWriterResult<bool>> RemoveUserRoleAsync(UserRoleModel model)
		{
			if (model.SecurityRole == SecurityRole.Standard)
				return WriterResult<bool>.ErrorResult("The {0} role cannot be remove from users.", SecurityRole.Standard);

			using (var context = DataContextFactory.CreateContext())
			{
				var role = await context.UserRoles.FirstOrDefaultAsync(x => x.User.UserName == model.UserName && x.Role.Name == model.SecurityRole.ToString());
				if (role == null)
					return WriterResult<bool>.ErrorResult("{0} in not assigned to {1} role.", model.UserName, model.SecurityRole);

				context.UserRoles.Remove(role);
				await context.SaveChangesAsync();
				return WriterResult<bool>.SuccessResult();
			}
		}
	}
}
