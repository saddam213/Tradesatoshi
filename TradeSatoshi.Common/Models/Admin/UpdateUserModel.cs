using Mvc.JQuery.Datatables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Admin
{
	public class UpdateUserModel
	{
		public string UserId { get; set; }

		[Required]
		[RegularExpression(@"^\w+$", ErrorMessage = @"UserName can only contain letters, digits and underscore")]
		public string UserName { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }
		public bool IsLocked { get; set; }
		public bool IsEnabled { get; set; }
		public bool IsTradeEnabled { get; set; }
		public bool IsWithdrawEnabled { get; set; }
		public bool IsTransferEnabled { get; set; }


		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime BirthDate { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
		public string PostCode { get; set; }
	}
}
