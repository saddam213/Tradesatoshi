using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Data.Entities
{
	public class ApplicationUser : IdentityUser
	{

		public virtual UserSettings Settings { get; set; }
		public virtual UserProfile Profile { get; set; }
	}
}
