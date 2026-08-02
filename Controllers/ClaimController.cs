using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimController : Controller
    {
        // GET: /Claim/Submit
        public IActionResult Submit()
        {
            return View();
        }

        // POST: /Claim/Submit
        // Prototype only - no actual saving/logic yet
        [HttpPost]
        public IActionResult Submit(Claim claim)
        {
            // In Part 2, this will save the claim and redirect to a confirmation.
            return RedirectToAction("Status");
        }

        // GET: /Claim/Status
        public IActionResult Status()
        {
            // Dummy sample data just to visually demonstrate tracking
            var sampleClaims = new List<Claim>
            {
                new Claim { Id = 1, HoursWorked = 20, HourlyRate = 350, Status = ClaimStatus.Pending },
                new Claim { Id = 2, HoursWorked = 15, HourlyRate = 400, Status = ClaimStatus.Approved },
                new Claim { Id = 3, HoursWorked = 10, HourlyRate = 300, Status = ClaimStatus.Rejected }
            };

            return View(sampleClaims);
        }
    }
}