using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using ProgPart1.Data;
using ProgPart1.Models;

namespace ProgPart1.Controllers
{
    public class ManagerController : Controller
    {
    public IActionResult Index(string filter = "all")
    {
            try
            {
                var claims = ClaimData.GetAllClaims();
                ViewBag.Filter = filter;

                claims = filter.ToLower() switch
                {
                    "pending" => ClaimData.GetClaimByStatus(ClaimStatus.Pending),
                    "approved" => ClaimData.GetClaimByStatus(ClaimStatus.Approved),
                    "declined" => ClaimData.GetClaimByStatus(ClaimStatus.Declined),
                    _ => claims
                };

                ViewBag.PendingCount = ClaimData.GetPendingCount();
                ViewBag.ApprovedCount = ClaimData.GetApprovedCount();
                ViewBag.DeclinedCount = ClaimData.GetDeclinedCount();

                return View(claims);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load books.";
                return View(new List<Claims>());
            }
    }

    public IActionResult Details(int id)
    {
            try
            {
                var claims = ClaimData.GetClaimById(id);
                if (claims == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(claims);
            }
            catch (Exception)
            {
                TempData["Error"] = "Error loading claim details.";
                return RedirectToAction(nameof(Index));
            }
    }
        // POST: /Admin/Approve - CREATES REVIEW RECORD
        [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Approve(int id, string? comments)
    {
        try
        {
            string reviewedBy = "Admin User";
            string reviewComments = string.IsNullOrWhiteSpace(comments)
                ? "Approved for library collection"
                : comments;

            var success = ClaimData.UpdateClaimStatus(id, ClaimStatus.Approved, reviewedBy, reviewComments);

            if (success)
            {
                TempData["Success"] = "Book approved successfully!";
            }
            else
            {
                TempData["Error"] = "Book not found.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error approving book.";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: /Admin/Decline - CREATES REVIEW RECORD
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Decline(int id, string? comments)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(comments))
            {
                TempData["Error"] = "Please provide a reason for declining.";
                return RedirectToAction(nameof(Details), new { id });
            }

            string reviewedBy = "Admin User";
            var success = ClaimData.UpdateClaimStatus(id, ClaimStatus.Declined, reviewedBy, comments);

            if (success)
            {
                TempData["Success"] = "Book declined.";
            }
            else
            {
                TempData["Error"] = "Book not found.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error declining book.";
            return RedirectToAction(nameof(Index));
        }
    }
        public IActionResult DownloadFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", fileName);
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var originalFileName = Path.GetFileName(fileName);

            return File(fileBytes, "application/octet-stream", originalFileName);
        }


    }
}
