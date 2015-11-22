using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Data.Entities;

namespace TradeSatoshi.Data.DataContext
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
	{
		public ApplicationDbContext()
			: base("DefaultConnection")
		{
		}
		
		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<UserSettings> UserSettings { get; set; }
		public DbSet<EmailTemplate> EmailTemplates { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.Settings).WithRequiredDependent();
			modelBuilder.Entity<ApplicationUser>().HasRequired(p => p.Profile).WithRequiredDependent();
			base.OnModelCreating(modelBuilder);
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		public IApplicationDbContext CreateContext()
		{
			return new ApplicationDbContext();
		}
	}
}
