using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using ProgPart1.Data;
using ProgPart1.Models;

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
                ViewBag.Error = "Unable to load books";
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
                TempData["Success"] = "Book submitted successfully";
                return RedirectToAction(nameof(Index));


            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error submitting book: " + ex.Message;
                return View(claims);
            }

        }

        public IActionResult Details(int id)
        {
            try
            {
                var book = ClaimData.GetClaimById(id);
                if (book == null)
                {
                    TempData["Error"] = "Book not found.";
                    return View();
                }
                return View(book);
            }
            catch (Exception e)
            {
                TempData["Error"] = "Error loading book";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> DownloadDocument(int bookId, int docId)
        {
            try
            {
                var book = ClaimData.GetClaimById(bookId);
                if (book == null) { return NotFound("Book not found."); }

                var document = book.Documents.FirstOrDefault(doc => doc.Id == docId);
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
    }
}
