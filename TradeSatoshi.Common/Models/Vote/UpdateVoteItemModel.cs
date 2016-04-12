using System.ComponentModel.DataAnnotations;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Common.Vote
{
	public class UpdateVoteItemModel
	{
		public int Id { get; set; }

		[StringLength(50)]
		[Display(Name = "Coin Name")]
		[Required(ErrorMessage = "You must supply a name")]
		public string Name { get; set; }

		[StringLength(5)]
		[Display(Name = "Symbol")]
		[Required(ErrorMessage = "You must supply a symbol")]
		public string Symbol { get; set; }

		[Url]
		[Display(Name = "Official Website")]
		[Required(ErrorMessage = "You must supply a website address")]
		public string Website { get; set; }

		[Url]
		[Display(Name = "Source Code")]
		[Required(ErrorMessage = "You must supply a link to the source code.")]
		public string Source { get; set; }

		[StringLength(265)]
		[Display(Name = "Admin Note")]
		public string Note { get; set; }

		[Display(Name = "Status")]
		public VoteItemStatus Status { get; set; }

		[Display(Name = "Algo Type")]
		public AlgoType AlgoType { get; set; }

		[StringLength(500, MinimumLength = 50)]
		[Display(Name = "Coin Description")]
		[Required(ErrorMessage = "You must supply a description for this coin")]
		public string Description { get; set; }
	}
}