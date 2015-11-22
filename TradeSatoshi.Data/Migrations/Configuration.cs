namespace TradeSatoshi.Data.Migrations
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;
	using TradeSatoshi.Data.DataContext;

	internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(ApplicationDbContext context)
		{
			//  This method will be called after migrating to the latest version.
			context.EmailTemplates.AddOrUpdate(e => e.Type, new Entities.EmailTemplate
			{
				IsEnabled = true,
				IsHtml = true,
				Subject = "FailedLogon",
				Template = "Hi {0}, Failed logon attempt from Ip Address: {1}",
				Type = Common.Services.EmailService.EmailType.FailedLogon
			},
			new Entities.EmailTemplate
			{
				IsEnabled = true,
				IsHtml = true,
				Subject = "Logon",
				Template = "Hi {0}, Successfull logon attempt from Ip Address: {1}",
				Type = Common.Services.EmailService.EmailType.Logon
			},
			new Entities.EmailTemplate
			{
				IsEnabled = true,
				IsHtml = true,
				Subject = "Registration",
				Template = "Hi {0}, Please click the link to activate your account: <a href='{2}'>Activate</a>",
				Type = Common.Services.EmailService.EmailType.Registration
			},
			new Entities.EmailTemplate
			{
				IsEnabled = true,
				IsHtml = true,
				Subject = "Lockout",
				Template = "Hi {0}, Your account has been locked due to 3 incorrect password attempts",
				Type = Common.Services.EmailService.EmailType.PasswordLockout
			},
			new Entities.EmailTemplate
			{
				IsEnabled = true,
				IsHtml = true,
				Subject = "Lockout",
				Template = "Hi {0}, Your account has been locked at your request",
				Type = Common.Services.EmailService.EmailType.UserLockout
			});
		
		}
	}
}
