using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Data.Entities
{
	public class Address
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }
		
		public int CurrencyId { get; set; }

		[MaxLength(256)]
		public string AddressHash { get; set; }
		
		[MaxLength(512)]
		public string PrivateKey { get; set; }
	
		public bool IsActive { get; set; }

		[ForeignKey("CurrencyId")]
		public virtual Currency Currency { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}
