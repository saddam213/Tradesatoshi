using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Enums;

namespace TradeSatoshi.Entity
{
	public class Currency
	{
		[Key]
		public int Id { get; set; }

		[MaxLength(128)]
		public string Name { get; set; }

		[MaxLength(128)]
		[Index("IX_Symbol", IsUnique = true)]
		public string Symbol { get; set; }

		[MaxLength(128)]
		public string Algo { get; set; }

		public decimal Balance { get; set; }

		public decimal ColdBalance { get; set; }

		[MaxLength(128)]
		public string WalletUser { get; set; }

		[MaxLength(128)]
		public string WalletPass { get; set; }

		public int WalletPort { get; set; }

		[MaxLength(128)]
		public string WalletHost { get; set; }

		public decimal TradeFee { get; set; }

		public decimal MinTrade { get; set; }

		public decimal MaxTrade { get; set; }

		public decimal MinBaseTrade { get; set; }

		public decimal WithdrawFee { get; set; }

		public WithdrawFeeType WithdrawFeeType { get; set; }

		public decimal MinWithdraw { get; set; }

		public decimal MaxWithdraw { get; set; }

		public int MinConfirmations { get; set; }

		[Index("IX_Status")]
		public CurrencyStatus Status { get; set; }

		[MaxLength(1024)]
		public string StatusMessage { get; set; }

		[MaxLength(256)]
		public string LastBlockHash { get; set; }

		[MaxLength(256)]
		public string LastWithdrawBlockHash { get; set; }

		public int Block { get; set; }

		[MaxLength(128)]
		public string Version { get; set; }

		public int Connections { get; set; }

		[MaxLength(1024)]
		public string Errors { get; set; }

		public bool IsEnabled { get; set; }

		public decimal TransferFee { get; set; }

		public CurrencyType Type { get; set; }

		public CurrencyInterfaceType InterfaceType { get; set; }

		public int MarketSortOrder { get; set; }
		public decimal FaucetPayment { get; set; }
		public decimal FaucetMax { get; set; }
		public bool IsFaucetEnabled { get; set; }
	}

}
