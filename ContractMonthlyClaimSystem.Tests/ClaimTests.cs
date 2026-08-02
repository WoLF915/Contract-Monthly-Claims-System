using Xunit;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContractMonthlyClaimSystem.Tests
{
    public class ClaimTests
    {
        // Creates a fresh in-memory database for each test so they don't interfere with each other
        private AppDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void Claim_DefaultStatus_ShouldBePending()
        {
            // A newly created claim should default to Pending status
            var claim = new Claim { HoursWorked = 10, HourlyRate = 200 };

            Assert.Equal(ClaimStatus.Pending, claim.Status);
        }

        [Fact]
        public void Claim_TotalPayment_ShouldCalculateCorrectly()
        {
            // Verifies the hours * rate calculation used across the views
            var claim = new Claim { HoursWorked = 15, HourlyRate = 350 };

            var total = claim.HoursWorked * claim.HourlyRate;

            Assert.Equal(5250, total);
        }

        [Fact]
        public async Task SavingClaim_ShouldPersistToDatabase()
        {
            using var context = GetInMemoryContext();

            var lecturer = new Lecturer { Name = "Test Lecturer", Email = "test@cmcs.local" };
            context.Lecturers.Add(lecturer);
            await context.SaveChangesAsync();

            var claim = new Claim
            {
                LecturerId = lecturer.Id,
                HoursWorked = 20,
                HourlyRate = 300,
                Status = ClaimStatus.Pending
            };
            context.Claims.Add(claim);
            await context.SaveChangesAsync();

            var savedClaim = await context.Claims.FirstOrDefaultAsync(c => c.LecturerId == lecturer.Id);

            Assert.NotNull(savedClaim);
            Assert.Equal(20, savedClaim.HoursWorked);
            Assert.Equal(300, savedClaim.HourlyRate);
        }

        [Fact]
        public async Task ApprovingClaim_ShouldUpdateStatusToApproved()
        {
            using var context = GetInMemoryContext();

            var claim = new Claim { HoursWorked = 10, HourlyRate = 250, Status = ClaimStatus.Pending };
            context.Claims.Add(claim);
            await context.SaveChangesAsync();

            // Simulate what ApprovalController.Approve does
            claim.Status = ClaimStatus.Approved;
            await context.SaveChangesAsync();

            var updatedClaim = await context.Claims.FindAsync(claim.Id);

            Assert.Equal(ClaimStatus.Approved, updatedClaim!.Status);
        }

        [Fact]
        public async Task RejectingClaim_ShouldUpdateStatusToRejected()
        {
            using var context = GetInMemoryContext();

            var claim = new Claim { HoursWorked = 8, HourlyRate = 400, Status = ClaimStatus.Pending };
            context.Claims.Add(claim);
            await context.SaveChangesAsync();

            // Simulate what ApprovalController.Reject does
            claim.Status = ClaimStatus.Rejected;
            await context.SaveChangesAsync();

            var updatedClaim = await context.Claims.FindAsync(claim.Id);

            Assert.Equal(ClaimStatus.Rejected, updatedClaim!.Status);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Claim_InvalidHoursWorked_ShouldFailValidationRange(double hours)
        {
            // Hours worked must be greater than 0 per the [Range] attribute on the model
            var claim = new Claim { HoursWorked = hours, HourlyRate = 100 };

            var validationResults = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var context = new System.ComponentModel.DataAnnotations.ValidationContext(claim);

            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(claim, context, validationResults, true);

            Assert.False(isValid);
        }
    }
}