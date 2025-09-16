using Microsoft.AspNetCore.Mvc;

namespace ProgPart1.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            return View();

        }
    }
}
