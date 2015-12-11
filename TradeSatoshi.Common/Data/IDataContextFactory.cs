using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.Data.Entities;

namespace TradeSatoshi.Common.Data
{
	public interface IDataContextFactory
	{
		IDataContext CreateContext();
	}
}
