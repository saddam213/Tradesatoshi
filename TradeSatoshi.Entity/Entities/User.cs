using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Entity
{
	public class User : IdentityUser
	{
		public bool IsEnabled { get; set; }
		public bool IsTradeEnabled { get; set; }
		public bool IsWithdrawEnabled { get; set; }
		public bool IsTransferEnabled { get; set; }

		public string ChatIcon { get; set; }
		public DateTime? ChatBanEnd { get; set; }

		[MaxLength(128)]
		public string ApiKey { get; set; }

		[MaxLength(256)]
		public string ApiSecret { get; set; }
		public bool IsApiEnabled { get; set; }
		public DateTime RegisterDate { get; set; }

		public virtual UserSettings Settings { get; set; }
		public virtual UserProfile Profile { get; set; }
		public virtual ICollection<UserLogon> Logons { get; set; }
		public virtual ICollection<UserTwoFactor> TwoFactor { get; set; }

		public virtual ICollection<Deposit> Deposit { get; set; }
		public virtual ICollection<Withdraw> Withdraw { get; set; }
		public virtual ICollection<Balance> Balance { get; set; }
		public virtual ICollection<Address> Address { get; set; }
		public virtual ICollection<Trade> Trade { get; set; }
		public virtual ICollection<SupportTicket> SupportTickets { get; set; }
		
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}


		
	}
}
