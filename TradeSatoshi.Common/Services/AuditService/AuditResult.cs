using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.AuditService
{
	public class AuditResult
	{
		public AuditResult()
		{
		}
		public AuditResult(bool success)
		{
			Success = success;
		}
		public bool Success { get; set; }
	}
}
