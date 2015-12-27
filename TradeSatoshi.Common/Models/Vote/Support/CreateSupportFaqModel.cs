﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TradeSatoshi.Common.Support
{
	public class CreateSupportFaqModel
	{
		[Required]
		[MaxLength(256)]
		public string Question { get; set; }

		[Required]
		[MaxLength(4000)]
		public string Answer { get; set; }
	}
}
