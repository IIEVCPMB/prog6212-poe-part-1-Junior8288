using Microsoft.AspNetCore.Mvc;

namespace ProgPart1.Controllers
{
    public class AddClaimController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
           
            return View("ClaimSubmitted");
        }

       
    }
}
