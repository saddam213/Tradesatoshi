using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Vote
{
	public class CreatePaidVoteModel : ITwoFactorEntry
	{
		public decimal Balance { get; set; }
		public int VoteCount { get; set; }
		public int VoteItemId { get; set; }
		public string Symbol { get; set; }
		public string VoteItem { get; set; }

		#region ITwoFactorEntry

		[Required]
		[MaxLength(128)]
		public string Data { get; set; }
		public TwoFactorType TwoFactorType { get; set; }
		public TwoFactorComponentType TwoFactorComponentType { get; set; }

		#endregion
	}
}
