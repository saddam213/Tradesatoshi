using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data.Entities;

namespace TradeSatoshi.Common.Data
{
	public interface IDataContext : IDisposable
	{
		IDataContext CreateContext();
		int SaveChanges();
		Task<int> SaveChangesAsync();

		DbSet<UserLogon> UserLogons { get; set; }
		DbSet<ApplicationUser> Users { get; set; }
		DbSet<IdentityRole> Roles { get; }
		DbSet<UserProfile> UserProfiles { get; set; }
		DbSet<UserSettings> UserSettings { get; set; }
		DbSet<EmailTemplate> EmailTemplates { get; set; }
		DbSet<UserTwoFactor> UserTwoFactor { get; set; }
		DbSet<UserRole> UserRoles { get; set; }

		DbSet<Currency> Currency { get; set; }
		DbSet<Entities.Deposit> Deposit { get; set; }
		DbSet<Entities.Withdraw> Withdraw { get; set; }
		DbSet<Entities.Balance> Balance { get; set; }
		DbSet<Entities.Address> Address { get; set; }
	}
}
