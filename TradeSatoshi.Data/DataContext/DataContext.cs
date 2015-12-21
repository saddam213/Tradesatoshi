using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Data.Entities;

namespace TradeSatoshi.Data.DataContext
{
	public class DataContext : DbContext, IDataContext
	{
		public DataContext()
			: base("DefaultConnection")
		{
			Database.Log = (e) => Debug.WriteLine(e);
		}

		public DbSet<ApplicationUser> Users { get; set; }
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

		public DbSet<Log> Log { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(38, 8));

			modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
			modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
			modelBuilder.Entity<UserRole>().ToTable("AspNetUserRoles").HasKey(r => new { r.RoleId, r.UserId }).HasRequired(p => p.User);
			modelBuilder.Entity<UserRole>().ToTable("AspNetUserRoles").HasRequired(p => p.Role);

			modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
			modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
			modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
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
			modelBuilder.Entity<TransferHistory>().HasRequired(p => p.User).WithMany(b => b.Transfer).HasForeignKey(p => p.UserId);

			base.OnModelCreating(modelBuilder);
		}

		public IDataContext CreateContext()
		{
			return new DataContext();
		}
	}
}
