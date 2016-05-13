using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Data
{
	public class Data
	{
		// Placeholder for Dependency Injection resolver
	}

	public static class DataContextExtensions
	{

		public static bool TryRollback(this DbContextTransaction transaction)
		{
			try
			{
				transaction.Rollback();
				return true;
			}
			catch (Exception) { }
			return false;
		}
	}
}
