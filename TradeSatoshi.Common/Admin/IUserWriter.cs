using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Common.DataTables;

namespace TradeSatoshi.Core.Admin
{
	public interface IUserWriter
	{
		bool UpdateUser(UpdateUserModel model);
		Task<bool> UpdateUserAsync(UpdateUserModel model);
	}
}
