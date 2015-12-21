using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Data.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public bool IsEnabled { get; set; }
		public bool IsTradeEnabled { get; set; }
		public bool IsWithdrawEnabled { get; set; }
		
		public virtual UserSettings Settings { get; set; }
		public virtual UserProfile Profile { get; set; }
		public virtual ICollection<UserLogon> Logons { get; set; }
		public virtual ICollection<UserTwoFactor> TwoFactor { get; set; }

		public virtual ICollection<Deposit> Deposit { get; set; }
		public virtual ICollection<Withdraw> Withdraw { get; set; }
		public virtual ICollection<Balance> Balance { get; set; }
		public virtual ICollection<Address> Address { get; set; }
		public virtual ICollection<Trade> Trade { get; set; }
		public virtual ICollection<TransferHistory> Transfer { get; set; }


		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}

	}
}
