using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlexisWorks.Web.Data;
using AlexisWorks.Web.ViewModels;

namespace AlexisWorks.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel
        {
            TotalClients = await _context.Clients.CountAsync(),
            TotalBookings = await _context.Bookings.CountAsync(),
            UnpaidBillings = await _context.Billings.CountAsync(b => b.PaymentStatus == "Unpaid"),
            TotalRevenue = await _context.Billings
                .Where(b => b.PaymentStatus == "Paid")
                .SumAsync(b => b.AmountDue),
            RecentBookings = await _context.Bookings
                .Include(b => b.Client)
                .OrderByDescending(b => b.ScheduleDate)
                .Take(5)
                .ToListAsync(),
            UnpaidBills = await _context.Billings
                .Include(b => b.Booking)
                    .ThenInclude(bk => bk.Client)
                .Where(b => b.PaymentStatus == "Unpaid")
                .OrderByDescending(b => b.Id)
                .Take(5)
                .ToListAsync()
        };

        return View(vm);
    }

    public IActionResult Error()
    {
        return View();
    }
}
