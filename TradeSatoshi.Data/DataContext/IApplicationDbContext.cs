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
	public interface IApplicationDbContext : IDisposable
	{
		IApplicationDbContext CreateContext();
		//DbSet<ApplicationUser> Users { get; set; }
		//IDbSet<IdentityRole> ApplicationRoles { get; }
		DbSet<UserProfile> UserProfiles { get; set; }
		DbSet<UserSettings> UserSettings { get; set; }
		DbSet<EmailTemplate> EmailTemplates { get; set; }
		int SaveChanges();
		Task<int> SaveChangesAsync();
	}
}
