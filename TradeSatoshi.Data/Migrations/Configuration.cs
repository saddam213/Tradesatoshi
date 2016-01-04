namespace TradeSatoshi.Data.Migrations
{
	using Microsoft.AspNet.Identity.EntityFramework;
	using System;
	using System.Collections.Generic;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;
	using TradeSatoshi.Common.Security;
	using TradeSatoshi.Entity;
	using TradeSatoshi.Enums;

	internal sealed class Configuration : DbMigrationsConfiguration<TradeSatoshi.Data.DataContext.ApplicationDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(TradeSatoshi.Data.DataContext.ApplicationDbContext context)
		{
			context.VoteSetting.AddOrUpdate(s => s.Id, new TradeSatoshi.Entity.VoteSettings
			{
				Id = 1,
				Next = DateTime.UtcNow.AddDays(14),
				Period = 14
			});

			// Add or update the roles
			foreach (SecurityRole role in Enum.GetValues(typeof(SecurityRole)))
			{
				context.Roles.AddOrUpdate(e => e.Name, new IdentityRole(role.ToString()));
			}
			context.SaveChanges();

			// Add or update the roles
			foreach (EmailType template in Enum.GetValues(typeof(EmailType)))
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

			adminUser.Roles.Add(new IdentityUserRole() { UserId = "4a6347c2-2c93-46e9-80d3-cbe064cb8491", RoleId = context.Roles.FirstOrDefault(x => x.Name == SecurityRoles.Standard).Id });
			adminUser.Roles.Add(new IdentityUserRole() { UserId = "4a6347c2-2c93-46e9-80d3-cbe064cb8491", RoleId = context.Roles.FirstOrDefault(x => x.Name == SecurityRoles.Administrator).Id });
			context.Users.AddOrUpdate(u => u.UserName, adminUser);

			var voteUser = new ApplicationUser
			{
				Id = "033CE02D-A6FD-4FC3-8C2C-16B7D9B48D5F",
				Email = "vote@vote.com",
				UserName = "Vote",
				PasswordHash = "AKkENJo+TaEed4we8iBt81GjHM/Wu+4CCM2EKz/KmeGW4Il5JTDZTjFEwaepKY/3SQ==",
				SecurityStamp = "8b03ec82-5ea3-406e-bfd6-97036e7fa3ba",
				EmailConfirmed = true,
				IsTradeEnabled = true,
				IsWithdrawEnabled = true,
				IsEnabled = true,
				Profile = new UserProfile { Id = "033CE02D-A6FD-4FC3-8C2C-16B7D9B48D5F" },
				Settings = new UserSettings { Id = "033CE02D-A6FD-4FC3-8C2C-16B7D9B48D5F" },
			};
			context.Users.AddOrUpdate(u => u.UserName, voteUser);

			context.Currency.AddOrUpdate(c => c.Id,
				new TradeSatoshi.Entity.Currency
				{
					Id = 1,
					Symbol = "BTC",
					Name = "Bitcoin",
					IsEnabled = true,
					TradeFee = 0.2m,
				},
				new TradeSatoshi.Entity.Currency
				{
					Id = 2,
					Symbol = "DOT",
					Name = "Dotcoin",
					IsEnabled = true,
					TradeFee = 0.2m,
				}
			);

			context.TradePair.Add(new TradePair { CurrencyId1 = 2, CurrencyId2 = 1 });

			foreach (var item in new[] { "General", "Deposit", "Withdraw", "Account", "Chat" })
			{
				context.SupportCategory.AddOrUpdate(x => x.Name, new SupportCategory { Name = item, IsEnabled = true });
			}
		}
	}
}
