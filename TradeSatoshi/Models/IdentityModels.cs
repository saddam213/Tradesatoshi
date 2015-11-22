using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace TradeSatoshi.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser
	{

		public virtual UserSettings Settings { get; set; }
		public virtual UserProfile Profile { get; set; }
	}


	public class UserSettings
	{
		[Key]
		public string Id { get; set; }
	}

	public class UserProfile
	{

		public UserProfile()
		{
			BirthDate = DateTime.UtcNow;
		}

		[Key]
		public string Id { get; set; }

		[MaxLength(50)]
		public string FirstName { get; set; }

		[MaxLength(50)]
		public string LastName { get; set; }

		public DateTime BirthDate { get; set; }

		[MaxLength(256)]
		public string Address { get; set; }

		[MaxLength(256)]
		public string City { get; set; }

		[MaxLength(256)]
		public string State { get; set; }

		[MaxLength(256)]
		public string Country { get; set; }

		[MaxLength(50)]
		public string PostCode { get; set; }

		public bool CanUpdate()
		{
			return string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName);
		}
	}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection")
		{
		}

		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<UserSettings> UserSettings { get; set; }

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
	}
}