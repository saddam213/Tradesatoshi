using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;

namespace TradeSatoshi.Common.Support
{
	public interface ISupportReader
	{
		Task<SupportTicketModel> GetSupportTicket(string userId, int ticketId, bool isadmin);
		Task<List<SupportTicketModel>> GetSupportTickets(string userId);
		Task<List<SupportCategoryModel>> GetSupportCategories();
		Task<List<SupportFaqModel>> GetSupportFaq();


		Task<SupportFaqModel> AdminGetSupportFaq(int id);
		Task<SupportRequestModel> AdminGetSupportRequest(int requestId);
		Task<SupportCategoryModel> AdminGetSupportCategory(int id);
		Task<DataTablesResponse> AdminGetSupportFaqDataTable(DataTablesModel model);
		Task<DataTablesResponse> AdminGetSupportCategoryDataTable(DataTablesModel model);
		Task<DataTablesResponse> AdminGetSupportRequestDataTable(DataTablesModel model);
		Task<DataTablesResponse> AdminGetSupportTicketDataTable(DataTablesModel model);
	}
}