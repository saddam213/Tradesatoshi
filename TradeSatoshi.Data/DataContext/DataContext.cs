using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Entity;

namespace TradeSatoshi.Data.DataContext
{
	public class DataContext : DbContext, IDataContext
	{
		public DataContext()
			: base("DefaultConnection")
		{
			Database.Log = (e) => Debug.WriteLine(e);
		}

		public DbSet<User> Users { get; set; }
		public DbSet<IdentityRole> Roles { get; set; }
		public DbSet<UserLogon> UserLogons { get; set; }
		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<UserSettings> UserSettings { get; set; }
		public DbSet<EmailTemplate> EmailTemplates { get; set; }
		public DbSet<UserTwoFactor> UserTwoFactor { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }

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


		public DbSet<Vote> Vote { get; set; }
		public DbSet<VoteItem> VoteItem { get; set; }
		public DbSet<VoteSettings> VoteSetting { get; set; }

		public DbSet<ChatMessage> ChatMessage { get; set; }

		public DbSet<Log> Log { get; set; }
		public DbSet<FaucetPayment> FaucetPayments { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(38, 8));

			modelBuilder.Entity<User>().ToTable("AspNetUsers");
			modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
			modelBuilder.Entity<UserRole>().ToTable("AspNetUserRoles").HasKey(r => new { r.RoleId, r.UserId }).HasRequired(p => p.User);
			modelBuilder.Entity<UserRole>().ToTable("AspNetUserRoles").HasRequired(p => p.Role);

			modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
			modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
			modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
			modelBuilder.Entity<UserLogon>().HasRequired(p => p.User).WithMany(b => b.Logons).HasForeignKey(p => p.UserId);

			modelBuilder.Entity<User>().HasRequired(p => p.Settings).WithRequiredDependent();
			modelBuilder.Entity<User>().HasRequired(p => p.Profile).WithRequiredDependent();
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
			modelBuilder.Entity<TradeHistory>().Property(x => x.Amount).HasPrecision(38, 8);
			modelBuilder.Entity<TradeHistory>().Property(x => x.Rate).HasPrecision(38, 8);
			modelBuilder.Entity<Trade>().HasRequired(p => p.User).WithMany(b => b.Trade).HasForeignKey(p => p.UserId);

			modelBuilder.Entity<TransferHistory>().HasRequired(p => p.User);
			modelBuilder.Entity<TransferHistory>().HasRequired(p => p.ToUser);
			modelBuilder.Entity<TransferHistory>().HasRequired(p => p.Currency);

			modelBuilder.Entity<SupportTicket>().HasRequired(p => p.User).WithMany(b => b.SupportTickets).HasForeignKey(p => p.UserId);
			modelBuilder.Entity<SupportTicket>().HasRequired(p => p.Category).WithMany(b => b.SupportTickets).HasForeignKey(p => p.CategoryId);
			modelBuilder.Entity<SupportTicketReply>().HasRequired(p => p.User);
			modelBuilder.Entity<SupportTicketReply>().HasRequired(p => p.Ticket).WithMany(b => b.Replies).HasForeignKey(p => p.TicketId);

			modelBuilder.Entity<VoteItem>().HasRequired(p => p.User);
			modelBuilder.Entity<Vote>().HasRequired(p => p.User);
			modelBuilder.Entity<Vote>().HasRequired(p => p.VoteItem).WithMany(v => v.Votes).HasForeignKey(v => v.VoteItemId);
			modelBuilder.Entity<VoteSettings>().HasOptional(p => p.LastFree);
			modelBuilder.Entity<VoteSettings>().HasOptional(p => p.LastPaid);

			modelBuilder.Entity<ChatMessage>().HasRequired(p => p.User);


			modelBuilder.Entity<FaucetPayment>().HasRequired(p => p.User);
			modelBuilder.Entity<FaucetPayment>().HasRequired(p => p.Currency);

			base.OnModelCreating(modelBuilder);
		}

		public IDataContext CreateContext()
		{
			return new DataContext();
		}

		public List<string> SaveChangesWithLogging()
		{
			var errorMessages = new List<string>();
			try
			{
				SaveChanges();
			}
			catch (DbEntityValidationException ex)
			{
				errorMessages = ex.EntityValidationErrors
					.SelectMany(x => x.ValidationErrors)
					.Select(x => x.ErrorMessage)
					.ToList();

				LogError("DbEntityValidationException", string.Join(Environment.NewLine, errorMessages));
			}
			catch (DbUpdateException ex)
			{
				var updateException = (UpdateException)ex.InnerException;
				var sqlException = (SqlException)updateException.InnerException;
				errorMessages = sqlException.Errors.OfType<SqlError>()
					.Select(x => x.Message)
					.ToList();

				LogError("DbUpdateException", string.Join(Environment.NewLine, errorMessages));
			}
			return errorMessages;
		}

		public async Task<List<string>> SaveChangesWithLoggingAsync()
		{
			var errorMessages = new List<string>();
			try
			{
				await SaveChangesAsync();
			}
			catch (DbEntityValidationException ex)
			{
				errorMessages = ex.EntityValidationErrors
					.SelectMany(x => x.ValidationErrors)
					.Select(x => x.ErrorMessage)
					.ToList();

				LogError("DbEntityValidationException", string.Join(Environment.NewLine, errorMessages));
			}
			catch (DbUpdateException ex)
			{
				var updateException = (UpdateException)ex.InnerException;
				var sqlException = (SqlException)updateException.InnerException;
				errorMessages = sqlException.Errors.OfType<SqlError>()
					.Select(x => x.Message)
					.ToList();
			
				LogError("DbUpdateException", string.Join(Environment.NewLine, errorMessages));
			}
			return errorMessages;
		}

		public void LogError(string type, string message)
		{
			try
			{
				Log.Add(new Log
				{
					Component = "DbContext",
					Type = type,
					Message = message,
					Timestamp = DateTime.UtcNow
				});
				SaveChanges();
			}
			catch (Exception)
			{
			}
		}

		public async Task LogErrorAsync(string type, string message)
		{
			try
			{
				Log.Add(new Log
				{
					Component = "DbContext",
					Type = type,
					Message = message,
					Timestamp = DateTime.UtcNow
				});
				await SaveChangesAsync();
			}
			catch (Exception)
			{
			}
		}
	}
}
