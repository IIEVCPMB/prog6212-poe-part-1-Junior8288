using Xunit;
using ProgPart1.Services;
using ProgPart1.Data;
using ProgPart1.Models;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;

namespace LibraryManagementSystem_Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1_AddClaim_Succuessful()
        {
            //Create a new book
            var initialCount = ClaimData.GetAllClaims().Count;

            var claim = new Claims
            {
                Month = "January",
                Taught = "Yes",
                Amount = 7500,
                Description = "A book about TDD practices",
                SubmittedDate = DateTime.Now,
                SubmittedBy = "Test User",
                ReviewedBy = "Simon Dikks",
                ReviewedDate = DateTime.Now,
                Status = ClaimStatus.Pending
            };

            //Perform the action
            ClaimData.AddClaim(claim);

            //Get the new count
            var newCount = ClaimData.GetAllClaims().Count;
            Assert.Equal(initialCount + 1, newCount);

            Assert.True(claim.Id > 0, "Book should have an ID assigned");

            Assert.Equal(ClaimStatus.Pending, claim.Status);

            //Verify if we can retrieve the book
            var retrievedBook = ClaimData.GetClaimById(claim.Id);

        }

        [Fact]
        public async Task Test2_EncyptionFile_Successful()
        {
            // Create and encrypt the file
            var originalContent = "This is a secret document";
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));
            inputStream.Position = 0; // Reset position to start of the stream

            var tempFile = Path.GetTempFileName();
            var encryptionService = new FileEncryptionService();

            try
            {
                await encryptionService.EncryptFileAsync(inputStream, tempFile);

                // Decrypt
                var decryptedStream = await encryptionService.DecryptFileAsync(tempFile);
                var decryptedContent = Encoding.UTF8.GetString(decryptedStream.ToArray());

                // Verify decrypted content matches the original
                Assert.Equal(originalContent, decryptedContent);
                Assert.Contains(originalContent, decryptedContent);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }


        [Fact]
        public async Task Test3_DeryptFile_Successful()
        {
            //Create and encrypt the file
            var originalContent = "This is a secret document";
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));
            var tempFile = Path.GetTempFileName();
            var encryptionService = new FileEncryptionService();

            try
            {
                await encryptionService.EncryptFileAsync(inputStream, tempFile);

                //Decrypt
                var decryptedStream = await encryptionService.DecryptFileAsync(tempFile);
                var decryptedContent = Encoding.UTF8.GetString(decryptedStream.ToArray());

                //Verfiy decrypted content matched the original
                Assert.Equal(originalContent, decryptedContent);
                Assert.Contains(originalContent, decryptedContent);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }

        }

        [Fact]
        public void Test4_ApproveClaim()
        {
            var claim = new Claims
            {
                Month = "January",
                Taught = "Yes",
                Amount = 7500,
                Description = "A book about TDD practices",
                SubmittedDate = DateTime.Now,
                SubmittedBy = "Test User",
                ReviewedBy = "Simon Dikks",
                ReviewedDate = DateTime.Now,
                Status = ClaimStatus.Pending
            };

            ClaimData.AddClaim(claim);

            //Action : Approve the book
            var success = ClaimData.UpdateClaimStatus(claim.Id, ClaimStatus.Approved, "Admin User", "Looks good");

            Assert.True(success, "Book status update should be successful");

            var updatedBook = ClaimData.GetClaimById(claim.Id);
            Assert.Equal(ClaimStatus.Approved, updatedBook.Status);
            Assert.Equal("Admin User", updatedBook.ReviewedBy);
        }

        [Fact]
        public void Test5_DeclineClaim()
        {
            var claim = new Claims
            {
                Month = "January",
                Taught = "Yes",
                Amount = 7500,
                Description = "A book about TDD practices",
                SubmittedDate = DateTime.Now,
                SubmittedBy = "Test User",
                ReviewedBy = "Simon Dikks",
                ReviewedDate = DateTime.Now,
                Status = ClaimStatus.Pending
            };
            ClaimData.AddClaim(claim);

            var success = ClaimData.UpdateClaimStatus(claim.Id, ClaimStatus.Declined, "Admin User", "Insufficient documentation");

            Assert.True(success, "Book status update should be successful");

            var updatedClaim = ClaimData.GetClaimById(claim.Id);
            Assert.Equal(ClaimStatus.Declined, updatedClaim.Status);
            Assert.Equal("Admin User", updatedClaim.ReviewedBy);
            Assert.NotNull(updatedClaim.ReviewedDate);
        }
    }
}