using System;
using System.ComponentModel.DataAnnotations;

namespace TradeSatoshi.Common.User
{
	public class UserProfileModel
	{
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