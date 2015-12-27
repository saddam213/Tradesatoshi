using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;
using TradeSatoshi.Common.Data.Entities;
using TradeSatoshi.Common.Support;
using TradeSatoshi.Common.Validation;
using System.Data.Entity;

namespace TradeSatoshi.Core.Support
{
	public class SupportWriter : ISupportWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult<int>> CreateSupportTicket(string userId, CreateSupportTicketModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = new SupportTicket
				{
					CategoryId = model.CategoryId,
					Description = model.Description,
					Status = Common.SupportTicketStatus.New,
					Title = model.Title,
					UserId = userId,
					LastUpdate = DateTime.UtcNow,
					Created = DateTime.UtcNow
				};
				context.SupportTicket.Add(ticket);
				await context.SaveChangesAsync();

				return WriterResult<int>.SuccessResult(ticket.Id);
			}
		}

		public async Task<IWriterResult<int>> CreateSupportTicketReply(string userId, CreateSupportTicketReplyModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = await context.SupportTicket.Include(x => x.Replies).FirstOrDefaultAsync(x => x.Id == model.TicketId);
				if (ticket == null)
					return WriterResult<int>.ErrorResult("Support ticket #{0} not found", model.TicketId);

				var reply = new SupportTicketReply
				{
					Message = model.Message,
					UserId = userId,
					TicketId = ticket.Id,
					IsPublic = true,
					IsAdmin = false,
					Created = DateTime.UtcNow
				};
				ticket.Replies.Add(reply);
				ticket.Status = Common.SupportTicketStatus.UserReply;
				ticket.LastUpdate = DateTime.UtcNow;
				await context.SaveChangesAsync();

				return WriterResult<int>.SuccessResult(reply.Id);
			}
		}



		public async Task<IWriterResult<bool>> UpdateSupportTicketStatus(string userId, UpdateSupportTicketStatusModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = await context.SupportTicket.Include(x => x.Replies).FirstOrDefaultAsync(x => x.Id == model.TicketId);
				if (ticket == null)
					return WriterResult<bool>.ErrorResult("Support ticket #{0} not found", model.TicketId);

				ticket.Status = model.Status;
				ticket.LastUpdate = DateTime.UtcNow;
				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}

		public async Task<IWriterResult<bool>> CreateSupportRequest(CreateSupportRequestModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var request = new SupportRequest
				{
					Title = model.Title,
					Sender = model.Sender,
					Description = model.Description,
					Replied = false,
					Created = DateTime.UtcNow
				};
				context.SupportRequest.Add(request);
				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}

		public async Task<IWriterResult<int>> AdminCreateSupportTicketReply(string userId, CreateSupportTicketReplyModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = await context.SupportTicket.Include(x => x.Replies).FirstOrDefaultAsync(x => x.Id == model.TicketId);
				if (ticket == null)
					return WriterResult<int>.ErrorResult("Support ticket #{0} not found", model.TicketId);

				var reply = new SupportTicketReply
				{
					Message = model.Message,
					UserId = userId,
					TicketId = ticket.Id,
					IsPublic = model.IsPublic,
					IsAdmin = true,
					Created = DateTime.UtcNow
				};
				ticket.Replies.Add(reply);
				ticket.Status = Common.SupportTicketStatus.AdminReply;
				ticket.LastUpdate = DateTime.UtcNow;
				await context.SaveChangesAsync();

				return WriterResult<int>.SuccessResult(reply.Id);
			}
		}

		public async Task<IWriterResult<bool>> AdminCreateSupportRequestReply(string userId, CreateSupportRequestReplyModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = await context.SupportRequest.FirstOrDefaultAsync(x => x.Id == model.RequestId);
				if (ticket == null)
					return WriterResult<bool>.ErrorResult("Support request #{0} not found", model.RequestId);

				ticket.Replied = true;
				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}

		public async Task<IWriterResult<bool>> AdminUpdateSupportReplyStatus(string userId, UpdateSupportReplyStatusModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var reply = await context.SupportTicketReply
					.Include(r => r.Ticket)
					.FirstOrDefaultAsync(x => x.Id == model.ReplyId);
				if (reply == null)
					return WriterResult<bool>.ErrorResult("Support reply #{0} not found", model.ReplyId);

				reply.IsPublic = model.IsPublic;
				reply.Ticket.LastUpdate = DateTime.UtcNow;
				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}


		public async Task<IWriterResult<bool>> AdminCreateSupportCategory(string userId, CreateSupportCategoryModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var category = await context.SupportCategory.FirstOrDefaultAsync(x => x.Name == model.Name);
				if (category != null)
					return WriterResult<bool>.ErrorResult("Category with name '{0}' already exists.", model.Name);

				category = context.SupportCategory.Add(new SupportCategory
				{
					Name = model.Name,
					IsEnabled = true
				});
				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}

		public async Task<IWriterResult<bool>> AdminUpdateSupportCategory(string userId, UpdateSupportCategoryModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var category = await context.SupportCategory.FirstOrDefaultAsync(x => x.Id == model.Id);
				if (category == null)
					return WriterResult<bool>.ErrorResult("Category with id '{0}' not found.", model.Id);

				category.Name = model.Name;
				category.IsEnabled = model.IsEnabled;
				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}

		public async Task<IWriterResult<bool>> AdminCreateSupportFaq(string userId, CreateSupportFaqModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var faq = await context.SupportFaq.FirstOrDefaultAsync(x => x.Question == model.Question);
				if (faq != null)
					return WriterResult<bool>.ErrorResult("FAQ with question '{0}' already exists.", model.Question);

				int order = context.SupportFaq.Count() + 1;
				faq = context.SupportFaq.Add(new SupportFaq
				{
					Question = model.Question,
					Answer = model.Answer,
					Order = order,
					IsEnabled = true
				});

				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}

		public async Task<IWriterResult<bool>> AdminUpdateSupportFaq(string userId, UpdateSupportFaqModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var faq = await context.SupportFaq.FirstOrDefaultAsync(x => x.Id == model.Id);
				if (faq == null)
					return WriterResult<bool>.ErrorResult("FAQ with id '{0}' not found.", model.Id);

				if (faq.Order != model.Order)
				{
					var order = Math.Max(Math.Min(context.SupportFaq.Count() - 1, model.Order), 0);
					foreach (var item in context.SupportFaq.Where(x => x.Order >= order).ToList())
					{
						item.Order++;
					}
					faq.Order = order;
				}

				faq.Question = model.Question;
				faq.Answer = model.Answer;
				faq.IsEnabled = model.IsEnabled;

				await context.SaveChangesAsync();

				return WriterResult<bool>.SuccessResult();
			}
		}
	}
}
