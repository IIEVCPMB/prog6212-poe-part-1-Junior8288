using ProgPart1.Models;
using System.Security.Claims;

namespace ProgPart1.Data
{
    public class ClaimData
    {
        private static List<Claims> _books = new List<Claims>
        {
            new Claims
            {
                Id = 1,
                Month = "January",
                Taught = "Yes",
                Amount = 7500,
                Description = "I have taught 23 classes this month",
                SubmittedDate = DateTime.Now.AddDays(-5),
                Status = ClaimStatus.Pending,
                ReviewedBy = "Jack Fincley",
                SubmittedBy = "John Doe",
                ReviewedDate = DateTime.Now,
                Documents = new List<UploadedDocument>(),
                Reviews = new List<ClaimReview>()

            },
            new Claims
            {
                Id = 2,
                Month = "Februray",
                Taught = "Yes",
                Amount = 10000,
                Description = "I have taught 27 classes this month",
                SubmittedDate = DateTime.Now.AddDays(-8),
                SubmittedBy = "Alice Johnson",
                Status = ClaimStatus.Pending,
                ReviewedBy = "Jack Fincley",
                ReviewedDate = DateTime.Now,
                Documents = new List<UploadedDocument>(),
                Reviews = new List<ClaimReview>()
                {
                    new ClaimReview
                    {
                        Id = 1,
                        ClaimId = 2,
                        ReviewerName = "Admin User",
                        ReviewerRole = "Administrator",
                        ReviewDate = DateTime.Now.AddDays(-11),
                        Decision = ClaimStatus.Approved,
                        Comments = "All documents are in order. Claim approved."
                    }
                }

            },
            new Claims
            {
                Id = 3,
                Month = "September",
                Taught = "Yes",
                Amount = 6000,
                Description = "I have taught 20 classes this month",
                SubmittedDate = DateTime.Now.AddDays(-7),
                Status = ClaimStatus.Pending,
                ReviewedBy = "Jack Fincley",
                SubmittedBy = "Bob Smith",
                ReviewedDate = DateTime.Now,
                Documents = new List<UploadedDocument>(),
                Reviews = new List<ClaimReview>()
                {
                    new ClaimReview
                    {
                        Id = 2,
                        ClaimId = 3,
                        ReviewerName = "Admin User",
                        ReviewerRole = "Administrator",
                        ReviewDate = DateTime.Now.AddDays(-11),
                        Decision = ClaimStatus.Declined,
                        Comments = "Missing proper supporting documentation. Please resubmit with all required documents."
                    }
                }

            }
        };

        private static int _nextId = 4;
        private static int _nextReviewId = 3;

        public static List<Claims> GetAllClaims() => _books.ToList();

        public static Claims? GetClaimById(int id) => _books.FirstOrDefault(b => b.Id == id);

        public static List<Claims> GetClaimByStatus(ClaimStatus status)
            => _books.Where(b => b.Status == status).ToList();

        public static void AddClaim(Claims claim )
        {
            claim.Id = _nextId;
            _nextId++;
            claim.SubmittedDate = DateTime.Now;
            claim.Status = ClaimStatus.Pending;
            _books.Add(claim);
        }

        public static bool UpdateClaimStatus(int id, ClaimStatus newStatus, string reviewedBy, string comments)
        {
            var claim = GetClaimById(id);
            if (claim == null) return false;

            
            var review = new ClaimReview
            {
                Id = _nextReviewId,
                ClaimId = id,
                ReviewerName = reviewedBy,
                ReviewerRole = "Administrator",
                ReviewDate = DateTime.Now,
                Decision = newStatus,
                Comments = comments
            };
            _nextReviewId++;

            claim.Reviews.Add(review);

            
            claim.Status = newStatus;
            claim.ReviewedBy = reviewedBy;
            claim.ReviewedDate = DateTime.Now;

            return true;
        }

        public static int GetPendingCount() => _books.Count(b => b.Status == ClaimStatus.Pending);
        public static int GetApprovedCount() => _books.Count(b => b.Status == ClaimStatus.Approved);
        public static int GetDeclinedCount() => _books.Count(b => b.Status == ClaimStatus.Declined);



    }
}
   