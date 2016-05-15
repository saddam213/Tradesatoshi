using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.EmailService
{
	public class EmailParam
	{
		public EmailParam() { }
		public EmailParam(string name, object value)
		{
			Name = name;
			Value = value?.ToString();
		}
		public string Name { get; set; }
		public string Value { get; set; }
	}
}
