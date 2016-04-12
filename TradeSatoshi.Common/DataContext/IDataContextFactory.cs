namespace TradeSatoshi.Common.Data
{
	public interface IDataContextFactory
	{
		IDataContext CreateContext();
	}
}