using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Entity
{
	public class VoteItem
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		[MaxLength(128)]
		public string Name { get; set; }

		[MaxLength(265)]
		public string AdminNote { get; set; }

		[MaxLength(10)]
		public string Symbol { get; set; }

		[MaxLength(128)]
		public string Website { get; set; }

		[MaxLength(128)]
		public string Source { get; set; }

		[MaxLength(500)]
		public string Description { get; set; }

		public AlgoType AlgoType { get; set; }

		public VoteItemStatus Status { get; set; }

		public DateTime Created { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User {get;set;}

		public virtual ICollection<Vote> Votes { get; set; }

	}
}
