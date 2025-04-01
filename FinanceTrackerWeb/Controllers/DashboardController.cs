using System.Web.Mvc;

namespace FinanceTrackerWeb.Controllers
{
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
