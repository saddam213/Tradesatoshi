using System;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.VoteService;
using System.Data.Entity;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Services
{
	public class VoteService : IVoteService
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task CheckVoteItems()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var settings = await context.VoteSetting.FirstOrDefaultAsync();
				if (settings == null)
					return;

				if (DateTime.UtcNow >= settings.Next)
				{
					settings.Next = settings.Next.AddDays(settings.Period);
					var votes = await context.VoteItem
						.Include(vi => vi.Votes)
						.Where(vi => vi.Status == VoteItemStatus.Voting)
						.Select(voteItem => new
						{
							voteItem.Id,
							FreeVoteCount = (int?) voteItem.Votes.Where(x => x.Status == VoteStatus.Live && x.Type == VoteType.Free).Sum(x => x.Count) ?? 0,
							PaidVoteCount = (int?) voteItem.Votes.Where(x => x.Status == VoteStatus.Live && x.Type == VoteType.Paid).Sum(x => x.Count) ?? 0
						}).ToListAsync();

					var winningPaid = votes.OrderByDescending(x => x.PaidVoteCount).FirstOrDefault();
					var winningFree = votes.OrderByDescending(x => x.FreeVoteCount).FirstOrDefault();
					if (winningPaid == null || winningFree == null)
						return;

					var paid = await context.VoteItem.FirstOrDefaultAsync(x => x.Id == winningPaid.Id);
					var free = await context.VoteItem.FirstOrDefaultAsync(x => x.Id == winningFree.Id);
					if (paid == null || free == null)
						return;

					paid.Status = VoteItemStatus.Listed;
					settings.LastPaidId = free.Id;

					free.Status = VoteItemStatus.Listed;
					settings.LastFreeId = free.Id;

					foreach (var vote in context.Vote.Where(x => x.Status == VoteStatus.Live).ToList())
					{
						vote.Status = VoteStatus.Archived;
					}

					await context.SaveChangesWithLoggingAsync();
				}
			}
		}
	}
}