using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TradeSatoshi.Models.Vote
{
    public class VotingModel
    {
        public VotingModel()
        {
            VoteItems = new List<VoteItemModel>();
            PendingVoteItems = new List<VoteItemModel>();
            RejectedVoteItems = new List<VoteItemModel>();
        }

        public List<VoteItemModel> VoteItems { get; set; }
        public List<VoteItemModel> PendingVoteItems { get; set; }
        public List<VoteItemModel> RejectedVoteItems { get; set; }
    }

    public class VoteItemModel
    {
        public int VoteItemId { get; set; }
        public string Name { get; set; }
        public int UserVotes { get; set; }
        public int CoinVotes { get; set; }
        public int PreviousVotes { get; set; }
        public int TotalVotes { get; set; }
        public string Status { get; set; }
    }

    public class CreateItemModel
    {
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
        public string ProjectWebsite { get; set; }

        [Url]
        [Display(Name = "Source Code")]
        [Required(ErrorMessage = "You must supply a link to the source code.")]
        public string SourceControl { get; set; }

        [StringLength(500, MinimumLength = 50)]
        [Display(Name = "Coin Description")]
        [Required(ErrorMessage = "You must supply a description for this coin")]
        public string Note { get; set; }
    }
}