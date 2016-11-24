using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Services.VoteService;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Common.Vote;
using TradeSatoshi.Entity;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Core.Vote
{
	public class VoteWriter : IVoteWriter
	{
		public IVoteService VoteService { get; set; }
		public ITradeService TradeService { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult<bool>> CreateVoteItem(string userId, CreateVoteItemModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var voteItem = await context.VoteItem.FirstOrDefaultNoLockAsync(x => x.Name == model.Name);
				if (voteItem != null)
					return WriterResult<bool>.ErrorResult("VoteItem {0} already exists.", model.Name);

				voteItem = new VoteItem
				{
					Name = model.Name,
					Symbol = model.Symbol,
					Website = model.Website,
					Source = model.Source,
					AlgoType = model.AlgoType,
					Created = DateTime.UtcNow,
					UserId = userId,
					Status = VoteItemStatus.Pending,
					Description = model.Description
				};

				context.VoteItem.Add(voteItem);

				var contextResults = await context.SaveChangesWithLoggingAsync();
				return WriterResult<bool>.ContextResult(contextResults, "Successfully submitted coin for review");
			}
		}

		public async Task<IWriterResult<bool>> CreateFreeVote(string userId, CreateFreeVoteModel model)
		{
			if (!await VoteService.CheckVoteItems())
				return WriterResult<bool>.ErrorResult("The current vote round has ended.");

			using (var context = DataContextFactory.CreateContext())
			{
				var voteItem = await context.VoteItem.FirstOrDefaultNoLockAsync(x => x.Id == model.VoteItemId);
				if (voteItem == null)
					return WriterResult<bool>.ErrorResult("VoteItem not found.");

				var lastDate = DateTime.UtcNow.AddDays(-1);
				if (await context.Vote.AnyAsync(x => x.UserId == userId && x.Created > lastDate))
					return WriterResult<bool>.ErrorResult("You have already voted today.");

				var vote = new Entity.Vote
				{
					Created = DateTime.UtcNow,
					Count = 1,
					Type = VoteType.Free,
					Status = VoteStatus.Live,
					UserId = userId,
					VoteItemId = model.VoteItemId
				};

				context.Vote.Add(vote);

				var contextResults = await context.SaveChangesWithLoggingAsync();
				return WriterResult<bool>.ContextResult(contextResults, "Successfully added {0} free vote(s) for {1}", 1, voteItem.Name);
			}
		}

		public async Task<IWriterResult<bool>> CreatePaidVote(string userId, CreatePaidVoteModel model)
		{
			if (!await VoteService.CheckVoteItems())
				return WriterResult<bool>.ErrorResult("The current vote round has ended.");

			using (var context = DataContextFactory.CreateContext())
			{
				var settings = await context.VoteSetting.FirstOrDefaultNoLockAsync();
				if (settings == null)
					return WriterResult<bool>.ErrorResult("VoteItem not found.");

				if (model.VoteCount <= 0 || (settings.Price * model.VoteCount) <= 0)
					return WriterResult<bool>.ErrorResult("Invalid vote amount.");

				var voteItem = await context.VoteItem.FirstOrDefaultNoLockAsync(x => x.Id == model.VoteItemId);
				if (voteItem == null)
					return WriterResult<bool>.ErrorResult("VoteItem not found.");

				var transferResult = await TradeService.QueueTransfer(new CreateTransferModel
				{
					UserId = userId,
					ToUser = Constants.SystemVoteUserId,
					CurrencyId = settings.CurrencyId,
					Amount = model.VoteCount * settings.Price,
					TransferType = TransferType.Vote
				});

				if (transferResult.HasError)
					return WriterResult<bool>.ErrorResult(transferResult.Error);

				var vote = new Entity.Vote
				{
					Created = DateTime.UtcNow,
					Count = model.VoteCount,
					Type = VoteType.Paid,
					Status = VoteStatus.Live,
					UserId = userId,
					VoteItemId = model.VoteItemId
				};

				context.Vote.Add(vote);

				var contextResults = await context.SaveChangesWithLoggingAsync();
				return WriterResult<bool>.ContextResult(contextResults, "Successfully added {0} vote(s) for {1}", model.VoteCount, voteItem.Name);
			}
		}

		public async Task<IWriterResult<bool>> AdminUpdateVoteItem(string userId, UpdateVoteItemModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var voteItem = await context.VoteItem.FirstOrDefaultNoLockAsync(x => x.Id == model.Id);
				if (voteItem == null)
					return WriterResult<bool>.ErrorResult("VoteItem {0} not found.", model.Id);

				if (model.Status == VoteItemStatus.Deleted)
				{
					context.VoteItem.Remove(voteItem);
				}
				else
				{
					voteItem.AdminNote = model.Note;
					voteItem.Name = model.Name;
					voteItem.Source = model.Source;
					voteItem.Status = model.Status;
					voteItem.Symbol = model.Symbol;
					voteItem.Website = model.Website;
					voteItem.AlgoType = model.AlgoType;
					voteItem.Description = model.Description;
				}

				var contextResults = await context.SaveChangesWithLoggingAsync();
				return WriterResult<bool>.ContextResult(contextResults);
			}
		}

		public async Task<IWriterResult<bool>> AdminUpdateVoteSettings(string userId, UpdateVoteSettingsModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var votesettings = await context.VoteSetting.FirstOrDefaultNoLockAsync();
				if (votesettings == null)
				{
					votesettings = new VoteSettings();
					context.VoteSetting.Add(votesettings);
					await context.SaveChangesAsync();
				}

				votesettings.Next = model.Next;
				votesettings.IsPaidEnabled = model.IsPaidEnabled;
				votesettings.IsFreeEnabled = model.IsFreeEnabled;
				votesettings.CurrencyId = model.CurrencyId;

				var contextResults = await context.SaveChangesWithLoggingAsync();
				return WriterResult<bool>.ContextResult(contextResults);
			}
		}
	}
}