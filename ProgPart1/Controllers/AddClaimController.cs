using ProgPart1.Services;
using Microsoft.AspNetCore.Mvc;
using ProgPart1.ClaimStore;
using ProgPart1.Data;
using ProgPart1.Models;
using System.Security.Claims;

namespace ProgPart1.Controllers
{
    public class AddClaimController : Controller
    {
        public readonly IWebHostEnvironment _environment;
        public readonly FileEncryptionService _encryptionService;

        public AddClaimController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _encryptionService = new FileEncryptionService();
        }

        public IActionResult Index()
        {
            try
            {
                var books = ClaimData.GetAllClaims();
                return View(books);
            }
            catch (Exception e)
            {
                ViewBag.Error = "Unable to load claims";
                return View(new List<Claims>());
            }

        }

        public IActionResult Add()
        {
            return View();
        }

        //POST: /Books/Add - Add form data to the datastore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(List<IFormFile> documents, Claims claims)
        {
            try
            {
                if (string.IsNullOrEmpty(claims.Month))
                {
                    ViewBag.Error = "Month is required";
                    return View(claims);
                }

                
                if (claims.Amount <= 0)
                {
                    ViewBag.Error = "The amount is required";
                    return View(claims);
                }

                claims.SubmittedBy = "John Librarian";

                if (documents != null && documents.Count > 0)
                {
                    foreach (var file in documents)
                    {
                        if (file.Length > 0)
                        {
                            var allowedExtensions = new[] { ".pdf", ".docx", ".txt", ".xlsx" };
                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(extension))
                            {
                                ViewBag.Error = $"File extension {extension} not allowed";
                                return View(claims);
                            }

                            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                            Directory.CreateDirectory(uploadsFolder);

                            var uniqueFileName = Guid.NewGuid().ToString() + ".encrypted";
                            var encryptedFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = file.OpenReadStream())
                            {
                                await _encryptionService.EncryptFileAsync(fileStream, encryptedFilePath);
                            }

                            claims.Documents.Add(new UploadedDocument
                            {
                                FileName = file.FileName,
                                FilePath = "/uploads/" + uniqueFileName,
                                FileSize = file.Length,
                                IsEncrypted = true
                            });

                        }
                    }
                }

                ClaimData.AddClaim(claims);
                TempData["Success"] = "claims submitted successfully";
                return RedirectToAction(nameof(Index));


            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error submitting claims: " + ex.Message;
                return View(claims);
            }

        }

        public IActionResult Details(int id)
        {
            try
            {
                var claims = ClaimData.GetClaimById(id);
                if (claims == null)
                {
                    TempData["Error"] = "claim not found.";
                    return View();
                }
                return View(claims);
            }
            catch (Exception e)
            {
                TempData["Error"] = "Error loading claim";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> DownloadDocument(int claimsId, int docId)
        {
            try
            {
                var claims = ClaimData.GetClaimById(claimsId);
                if (claims == null) { return NotFound("claim not found."); }

                var document = claims.Documents.FirstOrDefault(doc => doc.Id == docId);
                if (document == null) { return NotFound("Document not found."); }

                var encryptedFilePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
                if (!System.IO.File.Exists(encryptedFilePath)) return NotFound("File not found;");

                var decryptedStream = await _encryptionService.DecryptFileAsync(encryptedFilePath);

                var contentType = Path.GetExtension(document.FileName).ToLower()
                    switch
                {
                    ".pdf" => "application/pdf",
                    ".txt" => "application/txt",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    _ => "application/octet-stream"
                };

                return File(decryptedStream, contentType, document.FileName);

            }
            catch (Exception ex)
            {
                return BadRequest("Error downloading file: " + ex.Message);
            }
        }
          

            // Show claim submission form
            public IActionResult SubmitClaim()
            {
                return View();
            }

            // Handle submission
            [HttpPost]
            public IActionResult SubmitClaim(Claims claim, IFormFile file)
            {
          
            claim.Id = ClaimsDataStore.Claims.Count > 0 ? ClaimsDataStore.Claims.Max(c => c.Id) + 1 : 1;
                claim.CoordinatorStatus = "Pending";
                claim.ManagerStatus = "Pending";
                claim.SubmittedDate = DateTime.Now;

                if (file != null && file.Length > 0)
                {
                    var fileName = FileEncryptionService.SaveFile(file);
                    claim.FileName = fileName;
                }

                ClaimsDataStore.AddClaim(claim); // Persist in JSON
                return RedirectToAction("ClaimSuccess");
            }

            // Claim submitted successfully
            public IActionResult ClaimSuccess()
            {
                return View();
            }

            // View claim details
            public IActionResult ViewClaims(int id)
            {
                var claim = ClaimsDataStore.Claims.FirstOrDefault(c => c.Id == id);
                if (claim == null) return NotFound();

                return View(claim);
            }

            // Download uploaded document
            public IActionResult DownloadFile(string fileName)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", fileName);

                if (!System.IO.File.Exists(filePath))
                    return NotFound();

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/octet-stream", fileName.Substring(fileName.IndexOf('_') + 1));
            }
        }
    }
