using System.Web.Mvc;

namespace ClassBooksWebApp.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public void SetSession(string id)
        {
            Session["UserId"] = id;
        }
    }
}