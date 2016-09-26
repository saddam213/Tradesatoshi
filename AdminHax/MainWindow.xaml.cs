using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Cryptopia.WalletAPI.DataObjects;
using Cryptopia.WalletAPI.Base;
using AdminHax.Common.ExtensionManagerUI.Common;
using TradeSatoshi.Data.DataContext;
using TradeSatoshi.Entity;
using System.Data.Entity;
using Cryptopia.WalletAPI.Helpers;

namespace AdminHax
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private TransactionDataType _selectedTransactionType;
		private ObservableCollection<Wallet> _currencies = new ObservableCollection<Wallet>();
		private ObservableCollection<WalletTransaction> _walletTransactions = new ObservableCollection<WalletTransaction>();
		private ObservableCollection<string> _walletBackupRestoreResults = new ObservableCollection<string>();

		public MainWindow()
		{
			LoadWalletTransactionsCommand = new RelayCommand<Wallet>(async x => await LoadWalletTransactions(x), x => CanLoadWalletTransactionsExecute(x));
			WalletBackupCommand = new RelayCommand<Wallet>(async x => await WalletBackup(x), WalletBackupRestoreCommandCanExecute);
			WalletRestoreCommand = new RelayCommand<Wallet>(async x => await WalletRestore(x), WalletBackupRestoreCommandCanExecute);
			InitializeComponent();
			Initialize();
		}

	




		public ObservableCollection<Wallet> Currencies
		{
			get { return _currencies; }
			set { _currencies = value; }
		}


		

	

		private async void Initialize()
		{
			using (var context = new DataContext())
			//using (var poolRepo = new Repository<Pool>(Connection.CryptopiaPool))
			// using (var explorerRepo = new Repository<ExplorerWallet>(Connection.CryptopiaExplorer))
			{
				//var pools = new List<Pool>(await poolRepo.GetAllAsync());
				// var explorers = new List<ExplorerWallet>(await explorerRepo.GetAllAsync());
				var currencies = await context.Currency.ToListAsync();
				foreach (var currency in currencies)
				{
					Currencies.Add(new Wallet
					{
						Name = string.Format("{0}({1}) - Exchange", currency.Name, currency.Symbol),
						Host = currency.WalletHost,
						Port = currency.WalletPort,
						Pass = currency.WalletPass,
						User = currency.WalletUser
					});

					//var pool = pools.FirstOrDefault(x => x.CurrencyId == currency.Id);
					//if (pool != null)
					//{
					//	Currencies.Add(new Wallet
					//	{
					//		Name = string.Format("{0}({1}) - Pool", currency.Name, currency.Symbol),
					//		Host = pool.WalletHost,
					//		Port = pool.WalletPort,
					//		Pass = pool.WalletPass,
					//		User = pool.WalletUser
					//	});
					//}

					//var explorer = explorers.FirstOrDefault(x => x.CurrencyId == currency.Id);
					//if (explorer != null)
					//{
					//    Currencies.Add(new Wallet
					//    {
					//        Name = string.Format("{0}({1}) - Explorer", currency.Name, currency.Symbol),
					//        Host = explorer.WalletHost,
					//        Port = explorer.WalletPort,
					//        Pass = explorer.WalletPass,
					//        User = explorer.WalletUser
					//    });
					//}
				}
			}
		}

		#region Transactions

		public ICommand LoadWalletTransactionsCommand { get; private set; }

		public ObservableCollection<WalletTransaction> WalletTransactions
		{
			get { return _walletTransactions; }
			set { _walletTransactions = value; }
		}

		public TransactionDataType SelectedTransactionType
		{
			get { return _selectedTransactionType; }
			set { _selectedTransactionType = value; }
		}

		private bool CanLoadWalletTransactionsExecute(object obj)
		{
			return obj != null;
		}

		private async Task LoadWalletTransactions(Wallet wallet)
		{
			WalletTransactions.Clear();
			try
			{
				var walletConnection = new WalletConnector(wallet.Host, wallet.Port, wallet.User, wallet.Pass, 60000 * 4);
				var wallettransactions = await walletConnection.GetTransactionsAsync("", _selectedTransactionType);
				var transactions = wallettransactions.Select(x => new WalletTransaction
				{
					Timestamp = x.Time.ToDateTime(),
					Account = x.Account,
					Amount = Math.Abs(x.Amount),
					Txid = x.Txid,
					Type = Cryptopia.WalletAPI.Helpers.Extensions.ToTransactionType(x.Category),
					Address = x.Address,
					Confirmations = x.Confirmations
				})
				.OrderByDescending(x => x.Timestamp);

				int refesh = 10;
				foreach (var item in transactions)
				{
					WalletTransactions.Add(item);
					refesh--;
					if (refesh < 0)
					{
						refesh = 10;
						await Task.Delay(1);
					}
				}
			}
			catch (Exception ex)
			{

				MessageBox.Show("Something went wrong \n" + ex.ToString());
			}
		}
		#endregion


		#region BackupRestore

		public ICommand WalletBackupCommand { get; private set; }
		public ICommand WalletRestoreCommand { get; private set; }

		public ObservableCollection<string> WalletBackupRestoreResults
		{
			get { return _walletBackupRestoreResults; }
			set { _walletBackupRestoreResults = value; }
		}

		private bool WalletBackupRestoreCommandCanExecute(object obj)
		{
			return obj != null;
		}

		private async Task WalletRestore(Wallet wallet)
		{
			throw new NotImplementedException();
		}

		private async Task WalletBackup(Wallet wallet)
		{
			throw new NotImplementedException();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var user = username.Text;
				var walletConnection = new WalletConnector(host.Text, int.Parse(port.Text), username.Text, password.Text);
				var result = walletConnection.GenerateAddress("adam");
				MessageBox.Show(result);

				var result2 = walletConnection.ValidateAddress(result);
				MessageBox.Show(result2.ToString());

				var result3 = walletConnection.GetTransactions();
				MessageBox.Show(result3.Count().ToString());

				var result4 = walletConnection.GetTransactions("d5325c49c3c11a1907cf431f31b1295bf092406fd442d8a1119e43f4ea6b5cc6");
				MessageBox.Show(result4.Count().ToString());

				var result5 = walletConnection.GetBalance();
				MessageBox.Show(result5.ToString("F8"));
			}
			catch (Exception ex)
			{


				MessageBox.Show(ex.ToString());
			}
		}



		#endregion

		//private async void Button_Click(object sender, RoutedEventArgs e)
		//{
		//    allUserBalances.ItemsSource = await DBCalls.GetAllUserBalances();
		//    allExchangeWalletBalances.ItemsSource = WalletCalls.GetAllExchangeWalletBalances();
		//    allWithdrawalFees.ItemsSource = await DBCalls.GetAllWithdrawalFees();//returns coin names, but not fees, works in SQL Management Studio
		//}
	}

	public class WalletTransaction
	{
		public string Account { get; set; }
		public TransactionDataType Type { get; set; }
		public decimal Amount { get; set; }
		public string Txid { get; set; }
		public DateTime Timestamp { get; set; }

		public string Address { get; set; }

		public int Confirmations { get; set; }
	}

	public class Wallet
	{
		public string Name { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public string User { get; set; }
		public string Pass { get; set; }
	}

	

}
