using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        public int Id { get; set; }

        public int LecturerId { get; set; }

        [ValidateNever]
        public Lecturer Lecturer { get; set; } = null!;

        [Required(ErrorMessage = "Hours worked is required.")]
        [Range(0.1, 300, ErrorMessage = "Hours worked must be between 0.1 and 300.")]
        public double HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required.")]
        [Range(0.1, 10000, ErrorMessage = "Hourly rate must be a positive value.")]
        public double HourlyRate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string Notes { get; set; } = string.Empty;

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

        [ValidateNever]
        public List<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
    }
}