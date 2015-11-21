using System.Web.Mvc;
using TradeSatoshi.Common;

namespace TradeSatoshi.Controllers
{
	public class ExchangeController : Controller
	{
		public ITestInjection TestInjection { get; set; }

		public ActionResult Index()
		{
			TestInjection.Trade(2);
			return View();
		}
	}
}