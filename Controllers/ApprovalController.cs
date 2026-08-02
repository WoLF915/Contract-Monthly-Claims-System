using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ApprovalController : Controller
    {
        // GET: /Approval/Index
        public IActionResult Index()
        {
            // Dummy sample data for prototype display
            var pendingClaims = new List<Claim>
            {
                new Claim { Id = 1, Lecturer = new Lecturer { Name = "T. Ndlovu" }, HoursWorked = 20, HourlyRate = 350, Status = ClaimStatus.Pending },
                new Claim { Id = 2, Lecturer = new Lecturer { Name = "S. Mokoena" }, HoursWorked = 18, HourlyRate = 375, Status = ClaimStatus.Pending }
            };

            return View(pendingClaims);
        }

        // POST: /Approval/Approve/1
        // Prototype only - no real logic yet
        [HttpPost]
        public IActionResult Approve(int id)
        {
            return RedirectToAction("Index");
        }

        // POST: /Approval/Reject/1
        [HttpPost]
        public IActionResult Reject(int id)
        {
            return RedirectToAction("Index");
        }
    }
}