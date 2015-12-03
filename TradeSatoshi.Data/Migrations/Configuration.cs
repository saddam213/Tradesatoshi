namespace TradeSatoshi.Data.Migrations
{
	using Microsoft.AspNet.Identity.EntityFramework;
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;
	using TradeSatoshi.Common.Security;

	internal sealed class Configuration : DbMigrationsConfiguration<TradeSatoshi.Data.DataContext.ApplicationDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(TradeSatoshi.Data.DataContext.ApplicationDbContext context)
		{
			//// Add or update the roles
			//foreach (SecurityRole role in Enum.GetValues(typeof(SecurityRole)))
			//{
			//	context.Roles.AddOrUpdate(e => e.Name, new IdentityRole(role.ToString()));
			//}

			//// Add or update the roles
			//foreach (Common.Services.EmailService.EmailType template in Enum.GetValues(typeof(Common.Services.EmailService.EmailType)))
			//{
			//	context.EmailTemplates.AddOrUpdate(e => e.Type, new Entities.EmailTemplate
			//	{
			//		IsEnabled = true,
			//		IsHtml = true,
			//		Subject = template.ToString(),
			//		Template = "User: {0}, IpAddress: {1}",
			//		Type = template
			//	});
			//}
		}
	}
}
