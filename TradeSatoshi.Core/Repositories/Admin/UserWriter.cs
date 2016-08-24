using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Entity;

namespace TradeSatoshi.Core.Admin
{
	public class UserWriter : IUserWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public async Task<IWriterResult<bool>> UpdateUser(UpdateUserModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var existinguser = await context.Users.FirstOrDefaultNoLockAsync(x => (x.Email == model.Email && x.Id != model.UserId) || (x.UserName == model.UserName && x.Id != model.UserId));
				if (existinguser != null)
				{
					return WriterResult<bool>.ErrorResult(model.UserName == existinguser.UserName ? "Username already in use." : "Email already in use.");
				}
				var user = await context.Users
					.Include(x => x.Profile)
					.FirstOrDefaultNoLockAsync(x => x.Id == model.UserId);
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
		public async Task<IWriterResult<bool>> AddUserRole(UserRoleModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == model.UserName);
				if (user == null)
					return WriterResult<bool>.ErrorResult("User {0} not found.", model.UserName);

				var role = await context.Roles.FirstOrDefaultNoLockAsync(x => x.Name == model.SecurityRole.ToString());
				if (role == null)
					return WriterResult<bool>.ErrorResult("{0} role does not exist", model.SecurityRole);

				var exists = await context.UserRoles.FirstOrDefaultNoLockAsync(x => x.User.UserName == model.UserName && x.Role.Name == model.SecurityRole.ToString());
				if (exists != null)
					return WriterResult<bool>.ErrorResult("{0} is already assigned to {1} role.", model.UserName, model.SecurityRole);

				context.UserRoles.Add(new UserRole {UserId = user.Id, RoleId = role.Id});
				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityRoles.Administrator)]
		public async Task<IWriterResult<bool>> RemoveUserRole(UserRoleModel model)
		{
			if (model.SecurityRole == SecurityRole.Standard)
				return WriterResult<bool>.ErrorResult("The {0} role cannot be remove from users.", SecurityRole.Standard);

			using (var context = DataContextFactory.CreateContext())
			{
				var role = await context.UserRoles.FirstOrDefaultNoLockAsync(x => x.User.UserName == model.UserName && x.Role.Name == model.SecurityRole.ToString());
				if (role == null)
					return WriterResult<bool>.ErrorResult("{0} in not assigned to {1} role.", model.UserName, model.SecurityRole);

				context.UserRoles.Remove(role);
				await context.SaveChangesAsync();
				return WriterResult<bool>.SuccessResult();
			}
		}
	}
}