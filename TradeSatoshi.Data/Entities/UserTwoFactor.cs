﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Data.Entities
{
	public class UserTwoFactor
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Index("IX_UserComponent", 1, IsUnique = true)]
		public string UserId { get; set; }

		[Index("IX_UserComponent", 2, IsUnique = true)]
		public TwoFactorComponentType Component { get; set; }

		public TwoFactorType Type { get; set; }

		[MaxLength(256)]
		public string Data { get; set; }

		[MaxLength(256)]
		public string Data2 { get; set; }

		[MaxLength(256)]
		public string Data3 { get; set; }

		[MaxLength(256)]
		public string Data4 { get; set; }

		[MaxLength(256)]
		public string Data5 { get; set; }

		public DateTime Updated { get; set; }

		public DateTime Created { get; set; }

		public bool IsEnabled { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		public void ClearData()
		{
			Data = string.Empty;
			Data2 = string.Empty;
			Data3 = string.Empty;
			Data4 = string.Empty;
			Data5 = string.Empty;
		}
	}

	public enum TwoFactorType : byte
	{
		None = 0,
		[Display(Name = "Email")]
		EmailCode = 1,
		[Display(Name = "Google Authenticator")]
		GoogleCode = 2,
		[Display(Name = "Pin Number")]
		PinCode = 3,
	}

	public enum TwoFactorComponentType : byte
	{
		[Display(Name = "Login")]
		Login = 0,
		[Display(Name = "Withdraw")]
		Withdraw = 1,
	}
}
