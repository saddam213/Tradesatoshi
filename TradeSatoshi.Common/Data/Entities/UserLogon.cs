using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Data.Entities
{
	public class UserLogon
	{
		public UserLogon()
		{
			Timestamp = DateTime.UtcNow;
		}

		public UserLogon(string ipAddress, bool isValid)
		{
			IsValid = isValid;
			IPAddress = ipAddress;
			Timestamp = DateTime.UtcNow;
		}

		[Key]
		public int Id { get; set; }
		public string UserId { get; set; }
		public string IPAddress { get; set; }
		public bool IsValid { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}
