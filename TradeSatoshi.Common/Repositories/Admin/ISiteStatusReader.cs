using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;

namespace TradeSatoshi.Common.Repositories.Admin
{
	public interface ISiteStatusReader
	{
		Task<SiteStatusModel> GetSiteStatus();
	}
}
