using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Admin;
using TradeSatoshi.Data.DataContext;
using System.Data.Entity;
using TradeSatoshi.Common.DataTables;
using TradeSatoshi.Core.Heplers;
using TradeSatoshi.Data.Entities;
using System.Linq.Expressions;

namespace TradeSatoshi.Core.Admin
{
	public class LogonReader : ILogonReader
	{
		public IDataContext DataContext { get; set; }

		public DataTablesResponse GetLogonDataTable(DataTablesModel model)
		{
			using (var context = DataContext.CreateContext())
			{
				var query = context.UserLogons
						.Include(u => u.User)
						.Select(x => new LogonModel
						{
							IPAddress = x.IPAddress,
							Timestamp = x.Timestamp,
							UserName = x.User.UserName,
							IsValid = x.IsValid ? "Success" : "Failed"
						});

				return query.GetDataTableResult(model);
			}
		}
	}
}
