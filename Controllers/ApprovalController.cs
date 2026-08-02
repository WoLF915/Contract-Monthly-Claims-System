using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly AppDbContext _context;

// Predefined thresholds for automated flagging - claims exceeding these are highlighted for extra scrutiny
private const double MaxReasonableHours = 60;
private const double MaxReasonableRate = 1000;

public ApprovalController(AppDbContext context)
{
    _context = context;
}

        

        // GET: /Approval/Index
        public async Task<IActionResult> Index()
        {
            var pendingClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == ClaimStatus.Pending)
                .OrderBy(c => c.DateSubmitted)
                .ToListAsync();

            return View(pendingClaims);
        }

        // POST: /Approval/Approve/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("Index");
                }

                claim.Status = ClaimStatus.Approved;
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Claim #{id} approved.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error approving claim: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // POST: /Approval/Reject/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("Index");
                }

                claim.Status = ClaimStatus.Rejected;
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Claim #{id} rejected.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error rejecting claim: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}