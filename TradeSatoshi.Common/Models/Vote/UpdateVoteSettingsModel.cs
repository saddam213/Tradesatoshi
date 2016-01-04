using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.Common.Vote
{
	public class UpdateVoteSettingsModel
	{
		public DateTime Next { get; set; }

		public int Period { get; set; }
	}
}
