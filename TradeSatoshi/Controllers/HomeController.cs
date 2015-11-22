using System.Web.Mvc;
using System.Threading.Tasks;
using TradeSatoshi.Models;

namespace TradeSatoshi.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
		}

        public ActionResult Terms()
        {
            return View();
        }

        #region Coin Voting

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Voting()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult SubmitVote()
        {
            return RedirectToAction("Voting");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitVote(int voteItemId)
        {
                return RedirectToAction("Voting");
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateVoteItem()
        {
            return View(new CreateItemModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitVoteItem(CreateItemModel model)
        {
            return RedirectToAction("Voting");
        }

        #endregion
	}
}