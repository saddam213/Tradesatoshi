namespace TradeSatoshi.Common.Admin
{
	public class UserModel
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public bool IsLocked { get; set; }
		public bool IsEnabled { get; set; }
		public bool IsTradeEnabled { get; set; }
		public bool IsWithdrawEnabled { get; set; }
		public bool IsTransferEnabled { get; set; }
	}
}