using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlexisWorks.Web.Data;
using AlexisWorks.Web.ViewModels;
using System.Globalization;

namespace AlexisWorks.Web.Controllers;

public class ReportsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Reports
    public async Task<IActionResult> Index(int? clientId)
    {
        var query = _context.Billings
            .Include(b => b.Booking)
                .ThenInclude(bk => bk.Client)
            .AsQueryable();

        if (clientId.HasValue)
        {
            query = query.Where(b => b.Booking.ClientId == clientId.Value);
        }

        var billings = await query.OrderByDescending(b => b.Booking.ScheduleDate).ToListAsync();

        var vm = new ReportsViewModel
        {
            Billings = billings,
            ClientIdFilter = clientId,
            Clients = await _context.Clients.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync(),
            TotalPaid = billings.Where(b => b.PaymentStatus == "Paid").Sum(b => b.AmountDue),
            TotalUnpaid = billings.Where(b => b.PaymentStatus == "Unpaid").Sum(b => b.AmountDue)
        };

        return View(vm);
    }

    // GET: Reports/WeeklySchedule (Now Monthly Calendar)
    public async Task<IActionResult> WeeklySchedule(DateTime? date)
    {
        var referenceDate = date ?? DateTime.Today;
        var startOfMonth = new DateTime(referenceDate.Year, referenceDate.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        
        // Calculate the weekday offset for the 1st (0 = Sunday, 1 = Monday, etc.)
        int startDayOffset = (int)startOfMonth.DayOfWeek;

        var bookings = await _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.LineItems)
                .ThenInclude(li => li.Service)
            .Where(b => b.ScheduleDate >= startOfMonth && b.ScheduleDate <= endOfMonth)
            .OrderBy(b => b.ScheduleDate)
            .ToListAsync();

        ViewBag.StartOfMonth = startOfMonth;
        ViewBag.EndOfMonth = endOfMonth;
        ViewBag.ReferenceDate = referenceDate;
        ViewBag.StartDayOffset = startDayOffset;

        return View(bookings);
    }
}
