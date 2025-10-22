namespace ProgPart1.Models
{
    public class Claims
    {
        public int Id { get; set; }
        public string Month { get; set; }
        public string Taught { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string SubmittedBy { get; set; }
        public ClaimStatus Status { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime ReviewedDate { get; set; }
        public List<UploadedDocument> Documents { get; set; } = new List<UploadedDocument>();
        public List<ClaimReview> Reviews { get; set; } = new List<ClaimReview>();


    }
}
