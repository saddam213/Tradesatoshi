using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradeSatoshi.Common.DataTables;

namespace TradeSatoshi.ActionResults
{
	public class DataTablesResult : ActionResult
	{
		public DataTablesResponse Data { get; set; }

		public DataTablesResult(DataTablesResponse data)
		{
			this.Data = data;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			HttpResponseBase response = context.HttpContext.Response;
			response.Write(JsonConvert.SerializeObject(this.Data));
		}
	}
}