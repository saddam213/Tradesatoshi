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
using TradeSatoshi.Base.Logging;
using TradeSatoshi.WalletService.Implementation;

namespace TradeSatoshi.WalletService
{
	public partial class WalletService : ServiceBase
	{
		private IWalletTracker _walletTracker;
		private CancellationTokenSource _cancellationTokenSource;
		private readonly Log Log = LoggingManager.GetLog(typeof(WalletService));
		
		public WalletService(IWalletTracker walletTracker)
		{
			InitializeComponent();
			_walletTracker = walletTracker;
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				Log.Message(LogLevel.Info, "WalletService starting...");
				_cancellationTokenSource = new CancellationTokenSource();
				var cancelToken = _cancellationTokenSource.Token;
				_walletTracker.Start(_cancellationTokenSource.Token);
				Log.Message(LogLevel.Info, "WalletService started.");
			}
			catch (Exception ex)
			{
				Log.Exception("An exception occured starting WalletService.", ex);
				throw;
			}
		}

		protected override void OnStop()
		{
			Log.Message(LogLevel.Info, "WalletService stopping...");
			_walletTracker.Stop();
			_cancellationTokenSource.Cancel();
			while (_walletTracker.Running)
			{
				Log.Message(LogLevel.Info, "WalletService tracker stil running, waiting 5000ms...");
				Task.Delay(5000).Wait();
				RequestAdditionalTime(5000);
			}
			Log.Message(LogLevel.Info, "WalletService stopoped.");
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
