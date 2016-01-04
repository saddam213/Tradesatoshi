using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Validation;
using TradeSatoshi.Enums;
using TradeSatoshi.Common.Vote;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Entity;
using TradeSatoshi.Common.Services.TradeService;
using TradeSatoshi.Common.Transfer;
using System.Data.Entity;
using TradeSatoshi.Common.Services.VoteService;

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
				var voteItem = context.VoteItem.FirstOrDefault(x => x.Name == model.Name);
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
			await VoteService.CheckVoteItems();
			using (var context = DataContextFactory.CreateContext())
			{
				var voteItem = await context.VoteItem.FirstOrDefaultAsync(x => x.Id == model.VoteItemId);
				if (voteItem == null)
					return WriterResult<bool>.ErrorResult("VoteItem not found.");

				var lastDate = DateTime.UtcNow.AddDays(-1);
				var vote = await context.Vote.FirstOrDefaultAsync(x => x.UserId == userId && x.VoteItemId == model.VoteItemId && x.Created > lastDate);
				if (vote != null)
					return WriterResult<bool>.ErrorResult("You have already voted for this coin today.");

				vote = new Entity.Vote
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
			await VoteService.CheckVoteItems();
			using (var context = DataContextFactory.CreateContext())
			{
				var voteItem = await context.VoteItem.FirstOrDefaultAsync(x => x.Id == model.VoteItemId);
				if (voteItem == null)
					return WriterResult<bool>.ErrorResult("VoteItem not found.");

				var transferResult = await TradeService.QueueTradeItem(new CreateTransferModel
				{
					UserId = userId,
					ToUser = Constants.SystemVoteUserId,
					CurrencyId = Constants.SystemCurrencyId,
					Amount = model.VoteCount,
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
				Entity.VoteItem voteItem = context.VoteItem.FirstOrDefault(x => x.Id == model.Id);
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
				var votesettings = await context.VoteSetting.FirstOrDefaultAsync();
				if (votesettings == null)
				{
					votesettings = new VoteSettings();
					context.VoteSetting.Add(votesettings);
					await context.SaveChangesAsync();
				}

				votesettings.Next = model.Next;
				votesettings.Period = model.Period;

				var contextResults = await context.SaveChangesWithLoggingAsync();
				return WriterResult<bool>.ContextResult(contextResults);
			}
		}
	}
}
