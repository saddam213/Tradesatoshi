namespace AdminHax.Results
{
	public class GetAllUserBalancesResult
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; }
        public decimal Unconfirmed { get; set; }
        public decimal HeldForTrades { get; set; }
        public decimal PendingWithdraw { get; set; }
    }
}
