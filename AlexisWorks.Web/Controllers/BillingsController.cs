using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlexisWorks.Web.Data;
using AlexisWorks.Web.ViewModels;

namespace AlexisWorks.Web.Controllers;

public class BillingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BillingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Billings
    public async Task<IActionResult> Index(string? status)
    {
        var query = _context.Billings
            .Include(b => b.Booking)
                .ThenInclude(bk => bk.Client)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(b => b.PaymentStatus == status);
        }

        var vm = new BillingListViewModel
        {
            Billings = await query.OrderByDescending(b => b.Id).ToListAsync(),
            StatusFilter = status,
            Clients = await _context.Clients.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync()
        };

        return View(vm);
    }

    // GET: Billings/ProcessPaymentSummary/5 (Partial)
    public async Task<IActionResult> ProcessPaymentSummary(int id)
    {
        var billing = await _context.Billings
            .Include(b => b.Booking)
                .ThenInclude(bk => bk.Client)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (billing == null) return NotFound();

        return PartialView("_ProcessPayment", billing);
    }

    // POST: Billings/ProcessPayment/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessPayment(int id)
    {
        var billing = await _context.Billings
            .Include(b => b.Booking)
                .ThenInclude(bk => bk.Client)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (billing == null) return NotFound();

        billing.PaymentStatus = "Paid";
        billing.PaymentDate = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = $"Payment of ₱{billing.AmountDue:N2} for {billing.Booking.Client.Name} processed successfully.";
        return RedirectToAction(nameof(Index));
    }
}
