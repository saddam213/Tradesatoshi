using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TradeSatoshi.WalletService
{
	public partial class WalletService : ServiceBase
	{
		public WalletService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
		}

		protected override void OnStop()
		{
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
