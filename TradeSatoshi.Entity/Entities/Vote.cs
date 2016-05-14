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
	public class Vote
	{
		[Key]
		public int Id { get; set; }

		public int VoteItemId { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }

		[Index("IX_Type")]
		public VoteType Type { get; set; }

		public int Count { get; set; }

		[Index("IX_Status")]
		public VoteStatus Status { get; set; }

		public DateTime Created { get; set; }

		[ForeignKey("VoteItemId")]
		public virtual VoteItem VoteItem { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
