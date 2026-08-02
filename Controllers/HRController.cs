using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class HRController : Controller
    {
        private readonly AppDbContext _context;

        public HRController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /HR/Index - shows approved claims ready for payment processing
        public async Task<IActionResult> Index()
        {
            var approvedClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == ClaimStatus.Approved)
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();

            return View(approvedClaims);
        }

        // GET: /HR/Lecturers - manage lecturer data
        public async Task<IActionResult> Lecturers()
        {
            var lecturers = await _context.Lecturers.ToListAsync();
            return View(lecturers);
        }

        // GET: /HR/EditLecturer/1
        public async Task<IActionResult> EditLecturer(int id)
        {
            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer == null)
            {
                TempData["Error"] = "Lecturer not found.";
                return RedirectToAction("Lecturers");
            }
            return View(lecturer);
        }

        // POST: /HR/EditLecturer/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLecturer(Lecturer lecturer)
        {
            if (!ModelState.IsValid)
            {
                return View(lecturer);
            }

            try
            {
                _context.Lecturers.Update(lecturer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Lecturer details updated successfully.";
                return RedirectToAction("Lecturers");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating lecturer: {ex.Message}");
                return View(lecturer);
            }
        }
    }
}