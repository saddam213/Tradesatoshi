using System;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.VoteService;
using System.Data.Entity;
using TradeSatoshi.Enums;
using TradeSatoshi.Base.Queueing;
using TradeSatoshi.Data.DataContext;

namespace TradeSatoshi.Core.Services
{
	public class VoteService : IVoteService
	{
		private static readonly ProcessorQueueVoid<bool> VoteProcessor = new ProcessorQueueVoid<bool>(VoteService.Processor());
		private static Func<Task<bool>> Processor()
		{
			var service = new VoteService();
			return service.CalculateVoteStatus;
		}

		public VoteService()
		{
			DataContextFactory = new DataContextFactory();
		}

		public async Task<bool> CheckVoteItems()
		{
			return await VoteProcessor.QueueItem().ConfigureAwait(false);
		}

		public IDataContextFactory DataContextFactory { get; set; }

		private async Task<bool> CalculateVoteStatus()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var settings = await context.VoteSetting.FirstOrDefaultNoLockAsync();
				if (settings == null)
					return false;

				if (!settings.IsFreeEnabled && !settings.IsPaidEnabled)
					return false;

				if (DateTime.UtcNow >= settings.Next && (settings.IsFreeEnabled || settings.IsPaidEnabled))
				{
					var roundVotes = await context.Vote.Where(x => x.VoteItem.Status == VoteItemStatus.Voting && x.Status == VoteStatus.Live && x.Created <= settings.Next).ToListNoLockAsync();
					var voteGroups = roundVotes
					.GroupBy(v => v.VoteItemId)
					.Select(votes => new
					{
						Id = votes.Key,
						FreeVoteCount = (int?)votes.Where(x => x.Type == VoteType.Free).Sum(x => x.Count) ?? 0,
						PaidVoteCount = (int?)votes.Where(x => x.Type == VoteType.Paid).Sum(x => x.Count) ?? 0
					}).ToList();

					if (settings.IsFreeEnabled)
					{
						var winningFree = voteGroups.OrderByDescending(x => x.FreeVoteCount).FirstOrDefault();
						if (winningFree != null)
						{
							var free = await context.VoteItem.FirstOrDefaultAsync(x => x.Id == winningFree.Id);
							if (free != null)
							{
								settings.LastFreeId = free.Id;
								free.Status = VoteItemStatus.Listed;
							}
						}
					}

					if (settings.IsPaidEnabled)
					{
						var winningPaid = voteGroups.OrderByDescending(x => x.PaidVoteCount).FirstOrDefault();
						if (winningPaid != null)
						{
							var paid = await context.VoteItem.FirstOrDefaultAsync(x => x.Id == winningPaid.Id);
							if (paid != null)
							{
								settings.LastPaidId = paid.Id;
								paid.Status = VoteItemStatus.Listed;
							}
						}
					}

					settings.IsFreeEnabled = false;
					settings.IsPaidEnabled = false;
					await context.SaveChangesWithLoggingAsync();
					return false;
				}
			}
			return true;
		}
	}
}