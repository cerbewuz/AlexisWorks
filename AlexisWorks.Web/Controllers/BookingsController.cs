using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlexisWorks.Web.Data;
using AlexisWorks.Web.Models;
using AlexisWorks.Web.ViewModels;

namespace AlexisWorks.Web.Controllers;

public class BookingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Bookings
    public async Task<IActionResult> Index()
    {
        var bookings = await _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Billing)
            .OrderByDescending(b => b.ScheduleDate)
            .ToListAsync();
        return View(bookings);
    }

    // GET: Bookings/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var booking = await _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.LineItems)
                .ThenInclude(li => li.Service)
            .Include(b => b.Billing)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return NotFound();
        return View(booking);
    }

    // GET: Bookings/Create
    public async Task<IActionResult> Create()
    {
        var vm = new BookingCreateViewModel
        {
            Clients = (await _context.Clients.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList(),
            Services = (await _context.Services.OrderBy(s => s.Name).ToListAsync())
                .Select(s => new SelectListItem($"{s.Name} (₱{s.HourlyRate}/hr)", s.Id.ToString())).ToList()
        };
        return View(vm);
    }

    // POST: Bookings/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingCreateViewModel vm)
    {
        // Remove navigation-property validation
        ModelState.Remove("Clients");
        ModelState.Remove("Services");

        if (vm.LineItems == null || !vm.LineItems.Any())
        {
            ModelState.AddModelError("", "You must add at least one service to create a booking.");
        }

        if (!ModelState.IsValid)
        {
            vm.Clients = (await _context.Clients.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
            vm.Services = (await _context.Services.OrderBy(s => s.Name).ToListAsync())
                .Select(s => new SelectListItem($"{s.Name} (₱{s.HourlyRate}/hr)", s.Id.ToString())).ToList();
            return View(vm);
        }

        // Check for scheduling conflicts (same hour block)
        var conflictExists = await _context.Bookings.AnyAsync(b =>
            b.ScheduleDate.Date == vm.ScheduleDate.Date &&
            b.ScheduleDate.Hour == vm.ScheduleDate.Hour);

        if (conflictExists)
        {
            ModelState.AddModelError("ScheduleDate", "A booking already exists for this time slot. Please choose another time.");
            vm.Clients = (await _context.Clients.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync())
                .Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
            vm.Services = (await _context.Services.OrderBy(s => s.Name).ToListAsync())
                .Select(s => new SelectListItem($"{s.Name} (₱{s.HourlyRate}/hr)", s.Id.ToString())).ToList();
            return View(vm);
        }

        // Build booking with server-side computation
        var booking = new Booking
        {
            ClientId = vm.ClientId,
            ScheduleDate = vm.ScheduleDate,
            Status = "Confirmed"
        };

        decimal totalAmount = 0;
        foreach (var item in vm.LineItems)
        {
            var service = await _context.Services.FindAsync(item.ServiceId);
            if (service == null) continue;

            var lineTotal = item.HoursRendered * service.HourlyRate;
            totalAmount += lineTotal;

            booking.LineItems.Add(new BookingLineItem
            {
                ServiceId = item.ServiceId,
                HoursRendered = item.HoursRendered,
                LineTotal = lineTotal
            });
        }

        booking.TotalAmount = totalAmount;

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Auto-generate billing record
        var billing = new Billing
        {
            BookingId = booking.Id,
            AmountDue = totalAmount,
            PaymentStatus = "Unpaid"
        };
        _context.Billings.Add(billing);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Booking created successfully! Total: ₱{totalAmount:N2}. Invoice generated.";
        return RedirectToAction(nameof(Details), new { id = booking.Id });
    }

    // GET: Bookings/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var booking = await _context.Bookings
            .Include(b => b.Client)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (booking == null) return NotFound();
        return View(booking);
    }

    // POST: Bookings/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Billing)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Booking deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }
}
