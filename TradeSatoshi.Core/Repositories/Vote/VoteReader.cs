﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Vote;
using TradeSatoshi.Enums;
using System.Data.Entity;
using TradeSatoshi.Core.Helpers;
using TradeSatoshi.Common.Services.VoteService;

namespace TradeSatoshi.Core.Vote
{
	public class VoteReader : IVoteReader
	{
		public IVoteService VoteService { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public DataTablesResponse GetVoteDataTable(DataTablesModel model, VoteType type)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.VoteItem
				.Include(vi => vi.Votes)
				.Where(vi => vi.Status == VoteItemStatus.Voting)
				.Select(voteItem => new VoteItemModel
				{
					Id = voteItem.Id,
					Name = voteItem.Name,
					VoteType = type,
					VoteCount = (int?)voteItem.Votes.Where(x => x.Status == VoteStatus.Live && x.Type == type).Sum(x => x.Count) ?? 0
				});
				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse GetPendingDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.VoteItem
					.Where(v => v.Status == VoteItemStatus.Pending)
					.Select(voteItem => new VotePendingModel
					{
						Name = voteItem.Name,
						Status = VoteItemStatus.Pending
					})
					.OrderBy(x => x.Name);
				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse GetRejectedDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.VoteItem
					.Where(v => v.Status == VoteItemStatus.Rejected)
					.Select(voteItem => new VoteRejectedModel
					{
						Name = voteItem.Name,
						Reason = voteItem.AdminNote
					})
					.OrderBy(x => x.Name);
				return query.GetDataTableResult(model);
			}
		}

		public async Task<ViewVoteItemModel> GetVoteItem(int voteItemId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = await context.VoteItem.Where(x => x.Id == voteItemId)
					.Select(voteItem => new ViewVoteItemModel
					{
						Name = voteItem.Name,
						AlgoType = voteItem.AlgoType,
						Source = voteItem.Source,
						Description = voteItem.Description,
						Symbol = voteItem.Symbol,
						Website = voteItem.Website,
					}).FirstOrDefaultAsync();
				return query;
			}
		}

		public async Task<UpdateVoteItemModel> AdminGetVoteItem(int voteItemId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = await context.VoteItem
					.Select(voteItem => new UpdateVoteItemModel
					{
						Id = voteItem.Id,
						Name = voteItem.Name,
						AlgoType = voteItem.AlgoType,
						Source = voteItem.Source,
						Description = voteItem.Description,
						Status = voteItem.Status,
						Symbol = voteItem.Symbol,
						Website = voteItem.Website,
						Note = voteItem.AdminNote
					}).FirstOrDefaultAsync(x => x.Id == voteItemId);
				return query;
			}
		}

		public DataTablesResponse AdminGetVoteDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.VoteItem
					.Include(vi => vi.User)
					.Include(vi => vi.Votes)
					.Where(vi => vi.Status != VoteItemStatus.Listed)
					.Select(voteItem => new AdminVoteItemModel
					{
						Id = voteItem.Id,
						Name = voteItem.Name,
						Status = voteItem.Status,
						CreatedBy = voteItem.User.UserName,
						Created = voteItem.Created,
						VoteCountFree = (int?)voteItem.Votes.Where(x => x.Status == VoteStatus.Live && x.Type == VoteType.Free).Sum(x => x.Count) ?? 0,
						VoteCountPaid = (int?)voteItem.Votes.Where(x => x.Status == VoteStatus.Live && x.Type == VoteType.Paid).Sum(x => x.Count) ?? 0
					});
				return query.GetDataTableResult(model);
			}
		}

		public async Task<VoteModel> GetVoteSettings()
		{
			await VoteService.CheckVoteItems();

			using (var context = DataContextFactory.CreateContext())
			{
				var settings = await context.VoteSetting
					.Include(v => v.LastFree).DefaultIfEmpty()
					.Include(v => v.LastPaid).DefaultIfEmpty()
					.FirstOrDefaultAsync();
				return new VoteModel
				{
					NextVote = settings.Next,
					VotePeriod = settings.Period,
					LastFreeId = settings.LastFreeId,
					LastPaidId = settings.LastPaidId,
					LastFree = settings.LastFree != null
						? string.Format("{0}({1})", settings.LastFree.Name, settings.LastFree.Symbol)
						: string.Empty,
					LastPaid = settings.LastPaid != null
						? string.Format("{0}({1})", settings.LastPaid.Name, settings.LastPaid.Symbol)
						: string.Empty
				};
			}
		}
	}
}