using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradeSatoshi.WalletService.Implementation;

namespace TradeSatoshi.WalletService
{
	public partial class WalletService : ServiceBase
	{
		private WalletTracker _walletTracker;
		private CancellationTokenSource _cancellationTokenSource;

		public WalletService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				_cancellationTokenSource = new CancellationTokenSource();
				var cancelToken = _cancellationTokenSource.Token;
				_walletTracker = new WalletTracker(cancelToken);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		protected override void OnStop()
		{
			_cancellationTokenSource.Cancel();
			while (_walletTracker.Running)
			{
				Task.Delay(5000).Wait();
				RequestAdditionalTime(5000);
			}
		}

		internal void StartService()
		{
			OnStart(null);
		}

		internal void StopService()
		{
			OnStop();
		}
	}
}
