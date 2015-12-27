using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Data
{
	public interface IDataContextFactory
	{
		IDataContext CreateContext();
	}
}
