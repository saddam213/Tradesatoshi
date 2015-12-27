using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Entity;

namespace TradeSatoshi.Data.DataContext
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

	{
		public ApplicationDbContext()
			: base("DefaultConnection")
		{
			Database.Log = (e) => Debug.WriteLine(e);
		}

		public DbSet<UserLogon> UserLogons { get; set; }
		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<UserSettings> UserSettings { get; set; }
		public DbSet<EmailTemplate> EmailTemplates { get; set; }
		public DbSet<UserTwoFactor> UserTwoFactor { get; set; }

		public DbSet<Currency> Currency { get; set; }
		public DbSet<Deposit> Deposit { get; set; }
		public DbSet<Withdraw> Withdraw { get; set; }
		public DbSet<Balance> Balance { get; set; }
		public DbSet<Address> Address { get; set; }
		public DbSet<TradePair> TradePair { get; set; }
		public DbSet<Trade> Trade { get; set; }
		public DbSet<TradeHistory> TradeHistory { get; set; }
		public DbSet<TransferHistory> TransferHistory { get; set; }

		public DbSet<SupportFaq> SupportFaq { get; set; }
		public DbSet<SupportCategory> SupportCategory { get; set; }
		public DbSet<SupportTicket> SupportTicket { get; set; }
		public DbSet<SupportTicketReply> SupportTicketReply { get; set; }
		public DbSet<SupportRequest> SupportRequest { get; set; }

		public DbSet<Log> Log { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(38, 8));

			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Entity<UserLogon>().HasRequired(p => p.User).WithMany(b => b.Logons).HasForeignKey(p => p.UserId);
			modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.Settings).WithRequiredDependent();
			modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.Profile).WithRequiredDependent();
			modelBuilder.Entity<UserTwoFactor>().HasRequired(p => p.User).WithMany(b => b.TwoFactor).HasForeignKey(p => p.UserId);

			modelBuilder.Entity<Deposit>().HasRequired(p => p.User).WithMany(b => b.Deposit).HasForeignKey(p => p.UserId);
			modelBuilder.Entity<Withdraw>().HasRequired(p => p.User).WithMany(b => b.Withdraw).HasForeignKey(p => p.UserId);
			modelBuilder.Entity<Balance>().HasRequired(p => p.User).WithMany(b => b.Balance).HasForeignKey(p => p.UserId);
			modelBuilder.Entity<Address>().HasRequired(p => p.User).WithMany(b => b.Address).HasForeignKey(p => p.UserId);

			modelBuilder.Entity<TradePair>().HasRequired(p => p.Currency1);
			modelBuilder.Entity<TradePair>().HasRequired(p => p.Currency2);
			modelBuilder.Entity<TradeHistory>().HasRequired(p => p.User);
			modelBuilder.Entity<TradeHistory>().HasRequired(p => p.ToUser);
			modelBuilder.Entity<TradeHistory>().HasRequired(p => p.TradePair);
			modelBuilder.Entity<TradeHistory>().HasRequired(p => p.Currency);
			modelBuilder.Entity<Trade>().HasRequired(p => p.User).WithMany(b => b.Trade).HasForeignKey(p => p.UserId);

			modelBuilder.Entity<TransferHistory>().HasRequired(p => p.User);
			modelBuilder.Entity<TransferHistory>().HasRequired(p => p.ToUser);
			modelBuilder.Entity<TransferHistory>().HasRequired(p => p.Currency);

			modelBuilder.Entity<SupportTicket>().HasRequired(p => p.User).WithMany(b => b.SupportTickets).HasForeignKey(p => p.UserId);
			modelBuilder.Entity<SupportTicket>().HasRequired(p => p.Category).WithMany(b => b.SupportTickets).HasForeignKey(p => p.CategoryId);
			modelBuilder.Entity<SupportTicketReply>().HasRequired(p => p.User);
			modelBuilder.Entity<SupportTicketReply>().HasRequired(p => p.Ticket).WithMany(b => b.Replies).HasForeignKey(p => p.TicketId);

			base.OnModelCreating(modelBuilder);
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		public ApplicationDbContext CreateContext()
		{
			return new ApplicationDbContext();
		}
	}
}
