using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Data
{
	public interface IDataContext : IDisposable
	{
		IDataContext CreateContext();
		Database Database { get; }
		int SaveChanges();
		Task<int> SaveChangesAsync();
		List<string> SaveChangesWithLogging();
		Task<List<string>> SaveChangesWithLoggingAsync();
		void LogError(string type, string message);
		Task LogErrorAsync(string type, string message);

		DbSet<Entity.ApplicationUser> Users { get; set; }
		DbSet<IdentityRole> Roles { get; }
		DbSet<Entity.UserLogon> UserLogons { get; set; }
		DbSet<Entity.UserProfile> UserProfiles { get; set; }
		DbSet<Entity.UserSettings> UserSettings { get; set; }
		DbSet<Entity.EmailTemplate> EmailTemplates { get; set; }
		DbSet<Entity.UserTwoFactor> UserTwoFactor { get; set; }
		DbSet<Entity.UserRole> UserRoles { get; set; }

		DbSet<Entity.Currency> Currency { get; set; }
		DbSet<Entity.Deposit> Deposit { get; set; }
		DbSet<Entity.Withdraw> Withdraw { get; set; }
		DbSet<Entity.Balance> Balance { get; set; }
		DbSet<Entity.Address> Address { get; set; }

		DbSet<Entity.TradePair> TradePair { get; set; }
		DbSet<Entity.Trade> Trade { get; set; }
		DbSet<Entity.TradeHistory> TradeHistory { get; set; }

		DbSet<Entity.TransferHistory> TransferHistory { get; set; }

		DbSet<Entity.SupportFaq> SupportFaq { get; set; }
		DbSet<Entity.SupportCategory> SupportCategory { get; set; }
		DbSet<Entity.SupportRequest> SupportRequest { get; set; }
		DbSet<Entity.SupportTicket> SupportTicket { get; set; }
		DbSet<Entity.SupportTicketReply> SupportTicketReply { get; set; }

	
		DbSet<Entity.Vote> Vote { get; set; }
		DbSet<Entity.VoteItem> VoteItem { get; set; }
		DbSet<Entity.VoteSettings> VoteSetting { get; set; }

		DbSet<Entity.Log> Log { get; set; }
	}
}
