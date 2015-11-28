using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Data.Entities;

namespace TradeSatoshi.Data.DataContext
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection")
		{
		}

		public DbSet<UserLogon> UserLogons { get; set; }
		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<UserSettings> UserSettings { get; set; }
		public DbSet<EmailTemplate> EmailTemplates { get; set; }
		public DbSet<UserTwoFactor> UserTwoFactor { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Entity<UserLogon>().HasRequired(p => p.User).WithMany(b => b.Logons).HasForeignKey(p => p.UserId);
			modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.Settings).WithRequiredDependent();
			modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.Profile).WithRequiredDependent();
			modelBuilder.Entity<UserTwoFactor>().HasRequired(p => p.User).WithMany(b => b.TwoFactor).HasForeignKey(p => p.UserId);
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
