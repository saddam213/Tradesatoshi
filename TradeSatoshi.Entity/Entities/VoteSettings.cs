﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Entity
{
	public class VoteSettings
	{
		[Key]
		public int Id { get; set; }

		public int Period { get; set; }
		public DateTime Next { get; set; }

		public int? LastFreeId { get; set; }
		public int? LastPaidId { get; set; }

		[ForeignKey("LastFreeId")]
		public virtual VoteItem LastFree { get; set; }

		[ForeignKey("LastPaidId")]
		public virtual VoteItem LastPaid { get; set; }
	}
}