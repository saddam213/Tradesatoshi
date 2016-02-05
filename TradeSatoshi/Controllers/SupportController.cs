using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TradeSatoshi.Common;
using TradeSatoshi.Common.Balance;
using TradeSatoshi.Common.Modal;
using TradeSatoshi.Common.Security;
using TradeSatoshi.Common.Services.EmailService;
using TradeSatoshi.Common.Support;
using TradeSatoshi.Common.Trade;
using TradeSatoshi.Common.Transfer;
using TradeSatoshi.Common.Withdraw;
using TradeSatoshi.Data;
using TradeSatoshi.Enums;
using TradeSatoshi.Web.Attributes;
using TradeSatoshi.Web.Helpers;

namespace TradeSatoshi.Web.Controllers
{
	public class SupportController : BaseController
	{
		#region Properties

		public ISupportReader SupportReader { get; set; }
		public ISupportWriter SupportWriter { get; set; }

		#endregion

		#region SupportRequest

		[HttpGet]
		[AllowAnonymous]
		public ActionResult Index()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Support");
			}
			return View(new CreateSupportRequestModel());
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SupportRequest(CreateSupportRequestModel model)
		{
			if (!ModelState.IsValid)
				return View("Index", model);

			var result = await SupportWriter.CreateSupportRequest(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("Index", model);

			return ViewMessage(ViewMessageModel.Success("Success", "Successfully submitted support request, a support person will be in touch shortly."));
		}

		#endregion

		#region UserSupport

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Support()
		{
			var faq = await SupportReader.GetSupportFaq();
			var tickets = await SupportReader.GetSupportTickets(User.Id());
			return View(new SupportUserModel
			{
				SupportFaq = new List<SupportFaqModel>(faq),
				Tickets = new List<SupportTicketModel>(tickets)
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> ViewTicket(int ticketId)
		{
			var model = await SupportReader.GetSupportTicket(User.Id(), ticketId, false);
			if (model == null)
				return ViewMessage(ViewMessageModel.Warning("Not Found", "Support ticket #{0} not found.", ticketId));

			return View(model);
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> CreateTicket()
		{
			var categories = await SupportReader.GetSupportCategories();
			return View("CreateTicketModal", new CreateSupportTicketModel
			{
				Categories = new List<SupportCategoryModel>(categories)
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateTicket(CreateSupportTicketModel model)
		{
			if (!ModelState.IsValid)
				return View("CreateTicketModal", model);

			var result = await SupportWriter.CreateSupportTicket(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("CreateTicketModal", model);

			return CloseModalRedirect(Url.Action("ViewTicket", new { ticketId = result.Data }));
		}

		[HttpGet]
		[Authorize]
		public ActionResult ReplyTicket(int ticketId)
		{
			return View("ReplyTicketModal", new CreateSupportTicketReplyModel { TicketId = ticketId });
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ReplyTicket(CreateSupportTicketReplyModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var result = await SupportWriter.CreateSupportTicketReply(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View(model);

			return CloseModalRedirect(Url.Action("ViewTicket", new { ticketId = model.TicketId }));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CloseTicket(int ticketId)
		{
			var result = await SupportWriter.UpdateSupportTicketStatus(User.Id(), new UpdateSupportTicketStatusModel
			{
				TicketId = ticketId,
				Status = SupportTicketStatus.UserClosed
			});
			if (result.HasErrors)
			{
				return ViewMessage(ViewMessageModel.Error("Error", result.FirstError));
			}

			return RedirectToAction("ViewTicket", new { ticketId = ticketId });
		}

		#endregion

		#region AdminSupport

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminViewTicket(int ticketId)
		{
			var model = await SupportReader.GetSupportTicket(User.Id(), ticketId, true);
			if (model == null)
				return ViewMessage(ViewMessageModel.Warning("Not Found", "Support ticket #{0} not found.", ticketId));

			return View(model);
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminViewRequest(int requestId)
		{
			var model = await SupportReader.AdminGetSupportRequest(requestId);
			if (model == null)
				return ViewMessage(ViewMessageModel.Warning("Not Found", "Support request #{0} not found.", requestId));

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminUpdateTicketStatus(int ticketId, SupportTicketStatus status)
		{
			var result = await SupportWriter.UpdateSupportTicketStatus(User.Id(), new UpdateSupportTicketStatusModel
			{
				TicketId = ticketId,
				Status = status
			});
			if (result.HasErrors)
			{
				return ViewMessage(ViewMessageModel.Error("Error", result.FlattenErrors));
			}

			return RedirectToAction("AdminViewTicket", new { ticketId = ticketId });
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult AdminReplyTicket(int ticketId, string userName)
		{
			return View("ReplyTicketModal", new CreateSupportTicketReplyModel { IsAdmin = true, TicketId = ticketId, Message = string.Format("Hi {0},\r\n\r\n", userName) });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminReplyTicket(CreateSupportTicketReplyModel model)
		{
			if (!ModelState.IsValid)
				return View("ReplyTicketModal", model);

			var result = await SupportWriter.AdminCreateSupportTicketReply(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("ReplyTicketModal", model);

			return CloseModalRedirect(Url.Action("AdminViewTicket", new { ticketId = model.TicketId }));
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminUpdateReplyStatus(UpdateSupportReplyStatusModel model)
		{
			var result = await SupportWriter.AdminUpdateSupportReplyStatus(User.Id(), model);
			return RedirectToAction("AdminViewTicket", new { ticketId = model.TicketId });
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult AdminReplyRequest(int requestId, string email)
		{
			return View("AdminReplyRequestModal", new CreateSupportRequestReplyModel { RequestId = requestId, Email = email });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminReplyRequest(CreateSupportRequestReplyModel model)
		{
			if (!ModelState.IsValid)
				return View("AdminReplyRequestModal", model);

			var result = await SupportWriter.AdminCreateSupportRequestReply(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("AdminReplyRequestModal", model);

			return CloseModalRedirect(Url.Action("Index", "Admin") + "#Support");
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult AdminCreateCategory()
		{
			return View("AdminCreateCategoryModal", new CreateSupportCategoryModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminCreateCategory(CreateSupportCategoryModel model)
		{
			if (!ModelState.IsValid)
				return View("AdminCreateCategoryModal", model);

			var result = await SupportWriter.AdminCreateSupportCategory(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("AdminCreateCategoryModal", model);

			return CloseModalRedirect(Url.Action("Index", "Admin"));
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminUpdateCategory(int categoryId)
		{
			var category = await SupportReader.AdminGetSupportCategory(categoryId);
			if (category == null)
				return ViewMessageModal(ViewMessageModel.Error("Not Found", "Category with id {0} not found", categoryId));

			return View("AdminUpdateCategoryModal", new UpdateSupportCategoryModel
			{
				Id = category.Id,
				Name = category.Name,
				IsEnabled = category.IsEnabled
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminUpdateCategory(UpdateSupportCategoryModel model)
		{
			if (!ModelState.IsValid)
				return View("AdminUpdateCategoryModal", model);

			var result = await SupportWriter.AdminUpdateSupportCategory(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("AdminUpdateCategoryModal", model);

			return CloseModalRedirect(Url.Action("Index", "Admin"));
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public ActionResult AdminCreateFaq()
		{
			return View("AdminCreateFaqModal", new CreateSupportFaqModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminCreateFaq(CreateSupportFaqModel model)
		{
			if (!ModelState.IsValid)
				return View("AdminCreateFaqModal", model);

			var result = await SupportWriter.AdminCreateSupportFaq(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("AdminCreateFaqModal", model);

			return CloseModalRedirect(Url.Action("Index", "Admin"));
		}

		[HttpGet]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminUpdateFaq(int faqId)
		{
			var faq = await SupportReader.AdminGetSupportFaq(faqId);
			if (faq == null)
				return ViewMessageModal(ViewMessageModel.Error("Not Found", "FAQ with id {0} not found", faqId));

			return View("AdminUpdateFaqModal", new UpdateSupportFaqModel
			{
				Id = faq.Id,
				Question = faq.Question,
				Answer = faq.Answer,
				Order = faq.Order,
				IsEnabled = faq.IsEnabled
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeSecurityRole(SecurityRole.Administrator)]
		public async Task<ActionResult> AdminUpdateFaq(UpdateSupportFaqModel model)
		{
			if (!ModelState.IsValid)
				return View("AdminUpdateFaqModal", model);

			var result = await SupportWriter.AdminUpdateSupportFaq(User.Id(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("AdminUpdateFaqModal", model);

			return CloseModalRedirect(Url.Action("Index", "Admin"));
		}

		#endregion
	}
}