using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Entity
{
	public class UserRole
	{
		[Key]
		public string UserId { get; set; }

		[Key]
		public string RoleId { get; set; }

		[ForeignKey("RoleId")]
		public virtual IdentityRole Role { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
