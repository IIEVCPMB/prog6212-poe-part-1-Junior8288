using Microsoft.AspNetCore.Mvc;

namespace ProgPart1.Controllers
{
    public class ViewClaimController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Delete() 
        { 
            return View("ClaimDeleted");
        }
    }
}
