using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TradeSatoshi.WalletService.Implementation
{
	public interface IWalletTracker
	{
		bool Running { get; }
		void Stop();
		void Start(CancellationToken token);
	}
}
