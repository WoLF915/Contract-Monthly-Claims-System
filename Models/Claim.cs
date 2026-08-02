using System;
using System.Collections.Generic;

namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public int LecturerId { get; set; }
        public Lecturer Lecturer { get; set; } = null!;

        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public string Notes { get; set; } = string.Empty;

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        public List<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
    }
}