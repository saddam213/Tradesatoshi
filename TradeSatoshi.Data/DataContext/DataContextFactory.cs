using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data;

namespace TradeSatoshi.Data.DataContext
{
	public class DataContextFactory : IDataContextFactory
	{
		public IDataContext CreateContext()
		{
			return new DataContext();
		}
	}
}
