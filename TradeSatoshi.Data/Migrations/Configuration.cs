namespace TradeSatoshi.Data.Migrations
{
	using Microsoft.AspNet.Identity.EntityFramework;
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;
	using TradeSatoshi.Common.Data.Entities;
	using TradeSatoshi.Common.Security;

	internal sealed class Configuration : DbMigrationsConfiguration<TradeSatoshi.Data.DataContext.ApplicationDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(TradeSatoshi.Data.DataContext.ApplicationDbContext context)
		{
			// Add or update the roles
			foreach (SecurityRole role in Enum.GetValues(typeof(SecurityRole)))
			{
				context.Roles.AddOrUpdate(e => e.Name, new IdentityRole(role.ToString()));
			}
			context.SaveChanges();

			// Add or update the roles
			foreach (Common.EmailType template in Enum.GetValues(typeof(Common.EmailType)))
			{
				context.EmailTemplates.AddOrUpdate(e => e.Type, new EmailTemplate
				{
					IsEnabled = true,
					IsHtml = true,
					Subject = template.ToString(),
					Template = "User: {0}, IpAddress: {1}",
					Type = template
				});
			}

			var adminUser = new ApplicationUser
			{
				Id = "4a6347c2-2c93-46e9-80d3-cbe064cb8491",
				Email = "admin@admin.com",
				UserName = "Admin",
				PasswordHash = "AKkENJo+TaEed4we8iBt81GjHM/Wu+4CCM2EKz/KmeGW4Il5JTDZTjFEwaepKY/3SQ==",
				SecurityStamp = "8b03ec82-5ea3-406e-bfd6-97036e7fa3ba",
				EmailConfirmed = true,
				IsTradeEnabled = true,
				IsWithdrawEnabled = true,
				IsEnabled = true,
				Profile = new UserProfile { Id = "4a6347c2-2c93-46e9-80d3-cbe064cb8491" },
				Settings = new UserSettings { Id = "4a6347c2-2c93-46e9-80d3-cbe064cb8491" },
			};

			adminUser.Roles.Add(new IdentityUserRole(){ UserId = "4a6347c2-2c93-46e9-80d3-cbe064cb8491", RoleId = context.Roles.FirstOrDefault(x => x.Name == SecurityRoles.Administrator).Id});
			context.Users.AddOrUpdate(u => u.UserName, adminUser);


		}
	}
}
