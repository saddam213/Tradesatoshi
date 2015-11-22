using System.Web.Mvc;
using TradeSatoshi.Common;

namespace TradeSatoshi.Controllers
{
	public class ExchangeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}