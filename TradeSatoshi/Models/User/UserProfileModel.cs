using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TradeSatoshi.Models.User
{
	public class UserProfileModel
	{
		public UserProfileModel()
		{

		}

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required]
		public DateTime BirthDate { get; set; }

		[Required]
		public string Address { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
		public string State { get; set; }

		[Required]
		public string Country { get; set; }

		[Required]
		public string PostCode { get; set; }

		public bool CanUpdate { get; set; }
	}
}