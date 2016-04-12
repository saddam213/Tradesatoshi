using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Common.Support;
using TradeSatoshi.Core.Helpers;

namespace TradeSatoshi.Core.Support
{
	public class SupportReader : ISupportReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<SupportTicketModel> GetSupportTicket(string userId, int ticketId, bool isadmin)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = await context.SupportTicket
				.Include(s => s.Replies)
				.Include(s => s.User)
				.Include(s => s.Category)
				.FirstOrDefaultAsync(x => x.Id == ticketId && (x.UserId == userId || isadmin));
				if (ticket == null)
					return null;

				return new SupportTicketModel
				{
					Id = ticket.Id,
					Category = ticket.Category.Name,
					Description = ticket.Description,
					Status = ticket.Status,
					Title = ticket.Title,
					UserName = ticket.User.UserName,
					Created = ticket.Created,
					LastUpdate = ticket.LastUpdate,
					Replies = ticket.Replies.Where(x => x.IsPublic || isadmin).Select(reply => new SupportTicketReplyModel
					{
						Id = reply.Id,
						IsAdmin = reply.IsAdmin,
						IsPublic = reply.IsPublic,
						Message = reply.Message,
						UserName = reply.User.UserName,
					}).ToList()
				};
			}
		}

		public async Task<List<SupportTicketModel>> GetSupportTickets(string userId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.SupportTicket
					.Include(s => s.Replies)
					.Include(s => s.User)
					.Include(s => s.Category)
					.Where(x => x.UserId == userId)
					.Select(ticket => new SupportTicketModel
					{
						Id = ticket.Id,
						Category = ticket.Category.Name,
						Description = ticket.Description,
						Status = ticket.Status,
						Title = ticket.Title,
						UserName = ticket.User.UserName,
						Created = ticket.Created,
						LastUpdate = ticket.LastUpdate,
						Replies = ticket.Replies.Select(reply => new SupportTicketReplyModel
						{
							Id = reply.Id,
							IsAdmin = reply.IsAdmin,
							IsPublic = reply.IsPublic,
							Message = reply.Message,
							UserName = reply.User.UserName,
						}).ToList()
					}).ToListAsync();
			}
		}

		public async Task<List<SupportCategoryModel>> GetSupportCategories()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.SupportCategory
					.Where(c => c.IsEnabled)
					.OrderBy(x => x.Name)
					.Select(ticket => new SupportCategoryModel
					{
						Id = ticket.Id,
						Name = ticket.Name
					}).ToListAsync();
			}
		}

		public async Task<List<SupportFaqModel>> GetSupportFaq()
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.SupportFaq
					.Where(c => c.IsEnabled)
					.OrderBy(x => x.Order)
					.Select(ticket => new SupportFaqModel
					{
						Id = ticket.Id,
						Question = ticket.Question,
						Answer = ticket.Answer,
						Order = ticket.Order
					}).ToListAsync();
			}
		}

		public DataTablesResponse AdminGetSupportTicketDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.SupportTicket
					.Include(s => s.User)
					.Include(s => s.Category)
					.Select(ticket => new SupportTicketDataModel
					{
						Category = ticket.Category.Name,
						Id = ticket.Id,
						Status = ticket.Status,
						Title = ticket.Title,
						UserName = ticket.User.UserName,
						Created = ticket.Created,
						LastUpdate = ticket.LastUpdate
					});
				return query.GetDataTableResult(model);
			}
		}

		public async Task<SupportRequestModel> AdminGetSupportRequest(int requestId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var request = await context.SupportRequest.FirstOrDefaultAsync(x => x.Id == requestId);
				if (request == null)
					return null;

				return new SupportRequestModel
				{
					Id = request.Id,
					Title = request.Title,
					Sender = request.Sender,
					Created = request.Created,
					Description = request.Description
				};
			}
		}

		public DataTablesResponse AdminGetSupportRequestDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.SupportRequest
					.Select(ticket => new SupportRequestDataModel
					{
						Id = ticket.Id,
						Title = ticket.Title,
						Sender = ticket.Sender,
						Replied = ticket.Replied,
						Created = ticket.Created,
					});
				return query.GetDataTableResult(model);
			}
		}

		public async Task<SupportCategoryModel> AdminGetSupportCategory(int id)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.SupportCategory
					.Select(ticket => new SupportCategoryModel
					{
						Id = ticket.Id,
						Name = ticket.Name,
						IsEnabled = ticket.IsEnabled
					}).FirstOrDefaultAsync(c => c.Id == id);
			}
		}

		public async Task<SupportFaqModel> AdminGetSupportFaq(int id)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				return await context.SupportFaq
					.Select(ticket => new SupportFaqModel
					{
						Id = ticket.Id,
						Answer = ticket.Answer,
						Question = ticket.Question,
						Order = ticket.Order,
						IsEnabled = ticket.IsEnabled
					}).FirstOrDefaultAsync(c => c.Id == id);
			}
		}

		public DataTablesResponse AdminGetSupportFaqDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.SupportFaq
					.Select(ticket => new SupportFaqModel
					{
						Id = ticket.Id,
						Question = ticket.Question,
						Answer = ticket.Answer,
						Order = ticket.Order,
						IsEnabled = ticket.IsEnabled
					});
				return query.GetDataTableResult(model);
			}
		}

		public DataTablesResponse AdminGetSupportCategoryDataTable(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.SupportCategory
					.Select(ticket => new SupportCategoryModel
					{
						Id = ticket.Id,
						Name = ticket.Name,
						IsEnabled = ticket.IsEnabled
					});
				return query.GetDataTableResult(model);
			}
		}
	}
}
