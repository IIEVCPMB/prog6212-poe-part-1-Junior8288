using Microsoft.AspNetCore.Mvc;

namespace ProgPart1.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
