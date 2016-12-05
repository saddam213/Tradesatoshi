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

	//internal sealed class Configuration : DbMigrationsConfiguration<TradeSatoshi.Data.DataContext.ApplicationDbContext>
	//{
	//	public Configuration()
	//	{
	//		AutomaticMigrationsEnabled = false;
	//	}

	//	protected override void Seed(TradeSatoshi.Data.DataContext.ApplicationDbContext context)
	//	{
	//		//context.Currency.AddOrUpdate(s => s.Id, new TradeSatoshi.Entity.Currency
	//		//{
	//		//	Algo = "sha256",
	//		//	Balance = 0,
	//		//	Block = 0,
	//		//	ColdBalance = 0,
	//		//	Connections = 0,
	//		//	Errors = string.Empty,
	//		//	InterfaceType = CurrencyInterfaceType.Bitcoin,
	//		//	IsEnabled = true,
	//		//	LastBlockHash = string.Empty,
	//		//	LastWithdrawBlockHash = string.Empty,
	//		//	MarketSortOrder = 0,
	//		//	MaxTrade = 10000,
	//		//	MaxWithdraw = 5,
	//		//	MinBaseTrade = 0.00002000m,
	//		//	MinConfirmations = 2,
	//		//	MinTrade = 0.00002000m,
	//		//	MinWithdraw = 0.00002000m,
	//		//	Name = "Bitcoin",
	//		//	Status = CurrencyStatus.OK,
	//		//	StatusMessage = "",
	//		//	Symbol = "BTC",
	//		//	TradeFee = 0.2m,
	//		//	TransferFee = 0,
	//		//	Type = CurrencyType.Bitcoin,
	//		//	Version = "",
	//		//	WalletHost = "",
	//		//	WalletPass = "",
	//		//	WalletPort = 1000,
	//		//	WalletUser = "",
	//		//	WithdrawFee = 0.00002000m,
	//		//	WithdrawFeeType = WithdrawFeeType.Normal
	//		//});
	//		//context.SaveChanges();

	//		//context.VoteSetting.AddOrUpdate(s => s.Id, new TradeSatoshi.Entity.VoteSettings
	//		//{
	//		//	Id = 1,
	//		//	Next = DateTime.UtcNow.AddDays(14),
	//		//	CurrencyId = 1,
	//		//	IsFreeEnabled = false,
	//		//	IsPaidEnabled = false,
	//		//	Price = 0.00000001m,
	//		//	Period = 14
	//		//});

	//		//// Add or update the roles
	//		//foreach (SecurityRole role in Enum.GetValues(typeof(SecurityRole)))
	//		//{
	//		//	context.Roles.AddOrUpdate(e => e.Name, new IdentityRole(role.ToString()));
	//		//}
	//		//context.SaveChanges();


	//		//var adminUser = new User
	//		//{
	//		//	Id = "4a6347c2-2c93-46e9-80d3-cbe064cb8491",
	//		//	Email = "admin@admin.com",
	//		//	UserName = "Admin",
	//		//	PasswordHash = "AKkENJo+TaEed4we8iBt81GjHM/Wu+4CCM2EKz/KmeGW4Il5JTDZTjFEwaepKY/3SQ==",
	//		//	SecurityStamp = "8b03ec82-5ea3-406e-bfd6-97036e7fa3ba",
	//		//	EmailConfirmed = true,
	//		//	IsTradeEnabled = true,
	//		//	IsWithdrawEnabled = true,
	//		//	IsTransferEnabled = true,
	//		//	IsEnabled = true,
	//		//	RegisterDate = DateTime.UtcNow,
	//		//	Profile = new UserProfile { Id = "4a6347c2-2c93-46e9-80d3-cbe064cb8491" },
	//		//	Settings = new UserSettings { Id = "4a6347c2-2c93-46e9-80d3-cbe064cb8491" },
	//		//};

	//		//adminUser.Roles.Add(new IdentityUserRole() { UserId = "4a6347c2-2c93-46e9-80d3-cbe064cb8491", RoleId = context.Roles.FirstOrDefault(x => x.Name == SecurityRoles.Standard).Id });
	//		//adminUser.Roles.Add(new IdentityUserRole() { UserId = "4a6347c2-2c93-46e9-80d3-cbe064cb8491", RoleId = context.Roles.FirstOrDefault(x => x.Name == SecurityRoles.Administrator).Id });
	//		//context.Users.AddOrUpdate(u => u.UserName, adminUser);

	//		////var moderator1User = new User
	//		////{
	//		////	Id = "033CE02D-A6FD-4FC3-8C2C-16B7D9B48D5F",
	//		////	Email = "vote@admin.com",
	//		////	UserName = "Vote",
	//		////	PasswordHash = "AKkENJo+TaEed4we8iBt81GjHM/Wu+4CCM2EKz/KmeGW4Il5JTDZTjFEwaepKY/3SQ==",
	//		////	SecurityStamp = "8b03ec82-5ea3-406e-bfd6-97036e7fa3ba",
	//		////	EmailConfirmed = true,
	//		////	IsTradeEnabled = false,
	//		////	IsWithdrawEnabled = false,
	//		////	IsTransferEnabled = true,
	//		////	IsEnabled = true,
	//		////	RegisterDate = DateTime.UtcNow,
	//		////	Profile = new UserProfile { Id = "033CE02D-A6FD-4FC3-8C2C-16B7D9B48D5F" },
	//		////	Settings = new UserSettings { Id = "033CE02D-A6FD-4FC3-8C2C-16B7D9B48D5F" },
	//		////};

	//		////moderator1User.Roles.Add(new IdentityUserRole() { UserId = "033CE02D-A6FD-4FC3-8C2C-16B7D9B48D5F", RoleId = context.Roles.FirstOrDefault(x => x.Name == SecurityRoles.Standard).Id });
	//		////context.Users.AddOrUpdate(u => u.UserName, moderator1User);

	//		////var moderator2Id = Guid.NewGuid();
	//		////var moderator2User = new User
	//		////{
	//		////	Id = moderator2Id.ToString(),
	//		////	Email = "moderator2@admin.com",
	//		////	UserName = "Moderator2",
	//		////	PasswordHash = "AKkENJo+TaEed4we8iBt81GjHM/Wu+4CCM2EKz/KmeGW4Il5JTDZTjFEwaepKY/3SQ==",
	//		////	SecurityStamp = "8b03ec82-5ea3-406e-bfd6-97036e7fa3ba",
	//		////	EmailConfirmed = true,
	//		////	IsTradeEnabled = true,
	//		////	IsWithdrawEnabled = true,
	//		////	IsEnabled = true,
	//		////	 RegisterDate = DateTime.UtcNow,
	//		////	Profile = new UserProfile { Id = moderator2Id.ToString() },
	//		////	Settings = new UserSettings { Id = moderator2Id.ToString() },
	//		////};

	//		////moderator2User.Roles.Add(new IdentityUserRole() { UserId = moderator2Id.ToString(), RoleId = context.Roles.FirstOrDefault(x => x.Name == SecurityRoles.Standard).Id });
	//		//////moderator2User.Roles.Add(new IdentityUserRole() { UserId = moderator2Id.ToString(), RoleId = context.Roles.FirstOrDefault(x => x.Name == SecurityRoles.Moderator2).Id });
	//		////context.Users.AddOrUpdate(u => u.UserName, moderator2User);

	//		//var voteUser = new User
	//		//{
	//		//	Id = "033CE02D-A6FD-4FC3-8C2C-16B7D9B48D5F",
	//		//	Email = "vote@vote.com",
	//		//	UserName = "Vote",
	//		//	PasswordHash = "AKkENJo+TaEed4we8iBt81GjHM/Wu+4CCM2EKz/KmeGW4Il5JTDZTjFEwaepKY/3SQ==",
	//		//	SecurityStamp = "8b03ec82-5ea3-406e-bfd6-97036e7fa3ba",
	//		//	EmailConfirmed = false,
	//		//	IsTradeEnabled = false,
	//		//	IsWithdrawEnabled = true,
	//		//	IsEnabled = true,
	//		//	RegisterDate = DateTime.UtcNow,
	//		//	Profile = new UserProfile { Id = "033CE02D-A6FD-4FC3-8C2C-16B7D9B48D5F" },
	//		//	Settings = new UserSettings { Id = "033CE02D-A6FD-4FC3-8C2C-16B7D9B48D5F" },
	//		//};
	//		//context.Users.AddOrUpdate(u => u.UserName, voteUser);

	//		//context.Currency.AddOrUpdate(c => c.Id,
	//		//	new TradeSatoshi.Entity.Currency
	//		//	{
	//		//		Id = 1,
	//		//		Symbol = "BTC",
	//		//		Name = "Bitcoin",
	//		//		IsEnabled = true,
	//		//		TradeFee = 0.2m,
	//		//	}
	//		//);

	//		//foreach (var item in new[] { "General", "Deposit", "Withdraw", "Account", "Chat" })
	//		//{
	//		//	context.SupportCategory.AddOrUpdate(x => x.Name, new SupportCategory { Name = item, IsEnabled = true });
	//		//}
	//	}
	//}
}
