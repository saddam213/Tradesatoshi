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
		Database Database { get; }
		int SaveChanges();
		Task<int> SaveChangesAsync();

		DbSet<ApplicationUser> Users { get; set; }
		DbSet<IdentityRole> Roles { get; }
		DbSet<Entities.UserLogon> UserLogons { get; set; }
		DbSet<Entities.UserProfile> UserProfiles { get; set; }
		DbSet<Entities.UserSettings> UserSettings { get; set; }
		DbSet<Entities.EmailTemplate> EmailTemplates { get; set; }
		DbSet<Entities.UserTwoFactor> UserTwoFactor { get; set; }
		DbSet<Entities.UserRole> UserRoles { get; set; }

		DbSet<Entities.Currency> Currency { get; set; }
		DbSet<Entities.Deposit> Deposit { get; set; }
		DbSet<Entities.Withdraw> Withdraw { get; set; }
		DbSet<Entities.Balance> Balance { get; set; }
		DbSet<Entities.Address> Address { get; set; }

		DbSet<Entities.TradePair> TradePair { get; set; }
		DbSet<Entities.Trade> Trade { get; set; }
		DbSet<Entities.TradeHistory> TradeHistory { get; set; }

		DbSet<Entities.TransferHistory> TransferHistory { get; set; }

		DbSet<Entities.Log> Log { get; set; }
	}
}
