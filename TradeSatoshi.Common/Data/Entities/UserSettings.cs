using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Data.Entities
{
	public class UserSettings
	{
		[Key]
		public string Id { get; set; }
	}
}
