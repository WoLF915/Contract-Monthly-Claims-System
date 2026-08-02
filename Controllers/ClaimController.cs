using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        private readonly AppDbContext _context;

        public ClaimController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Claim/Submit
        public IActionResult Submit()
        {
            return View();
        }

        // POST: /Claim/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Claim claim, IFormFile? document)
        {
            if (!ModelState.IsValid)
            {
                return View(claim);
            }

            try
            {
                // Temporary: attach to a hardcoded lecturer until authentication is added.
                // Ensures at least one lecturer exists to satisfy the foreign key.
                var lecturer = await _context.Lecturers.FirstOrDefaultAsync();
                if (lecturer == null)
                {
                    lecturer = new Lecturer { Name = "Demo Lecturer", Email = "demo@cmcs.local" };
                    _context.Lecturers.Add(lecturer);
                    await _context.SaveChangesAsync();
                }

                claim.LecturerId = lecturer.Id;
                claim.Status = ClaimStatus.Pending;
                claim.DateSubmitted = DateTime.Now;

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                if (document != null && document.Length > 0)
                {
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var extension = Path.GetExtension(document.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Error"] = "Invalid file type. Only .pdf, .docx, and .xlsx are allowed.";
                        return RedirectToAction("Status");
                    }

                     const long maxFileSize = 5 * 1024 * 1024; // 5MB
    if (document.Length > maxFileSize)
    {
        TempData["Error"] = "File is too large. Maximum allowed size is 5MB.";
        return RedirectToAction("Status");
    }

                    var uploadsFolder = Path.Combine("wwwroot", "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    var supportingDoc = new SupportingDocument
                    {
                        ClaimId = claim.Id,
                        FileName = document.FileName,
                        FilePath = $"/uploads/{uniqueFileName}"
                    };

                    _context.SupportingDocuments.Add(supportingDoc);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction("Status");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while submitting your claim: {ex.Message}");
                return View(claim);
            }
        }

        // GET: /Claim/Status
        public async Task<IActionResult> Status()
        {
            var claims = await _context.Claims
                .Include(c => c.SupportingDocuments)
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();

            return View(claims);
        }
    }
}