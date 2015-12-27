using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		DataTablesResponse AdminGetSupportFaqDataTable(DataTablesModel model);
		DataTablesResponse AdminGetSupportCategoryDataTable(DataTablesModel model);
		DataTablesResponse AdminGetSupportRequestDataTable(DataTablesModel model);
		DataTablesResponse AdminGetSupportTicketDataTable(DataTablesModel model);
	}
}
