﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Services.EncryptionService
{
	public class UserApiModel
	{
		public string Key { get; set; }
		public string Secret { get; set; }
		public bool IsEnabled { get; set; }
	}
}
