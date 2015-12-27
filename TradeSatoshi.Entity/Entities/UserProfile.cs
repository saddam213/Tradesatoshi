using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Entity
{
	public class UserProfile
	{
		public UserProfile()
		{
			BirthDate = DateTime.UtcNow;
		}

		[Key]
		public string Id { get; set; }

		[MaxLength(50)]
		public string FirstName { get; set; }

		[MaxLength(50)]
		public string LastName { get; set; }

		public DateTime BirthDate { get; set; }

		[MaxLength(256)]
		public string Address { get; set; }

		[MaxLength(256)]
		public string City { get; set; }

		[MaxLength(256)]
		public string State { get; set; }

		[MaxLength(256)]
		public string Country { get; set; }

		[MaxLength(50)]
		public string PostCode { get; set; }

		public bool CanUpdate()
		{
			return string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName);
		}
	}
}
