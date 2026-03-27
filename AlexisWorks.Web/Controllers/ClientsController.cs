using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlexisWorks.Web.Data;
using AlexisWorks.Web.Models;

namespace AlexisWorks.Web.Controllers;

public class ClientsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Clients
    public async Task<IActionResult> Index()
    {
        var clients = await _context.Clients
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();
        return View(clients);
    }

    // GET: Clients/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var client = await _context.Clients
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (client == null) return NotFound();
        return View(client);
    }

    // GET: Clients/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,MiddleName,LastName,Suffix,StreetVillageBarangay,City,Province,Phone")] Client client)
    {
        if (ModelState.IsValid)
        {
            client.Address = $"{client.StreetVillageBarangay}|{client.City}|{client.Province}";
            _context.Add(client);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Client '{client.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(client);
    }

    // GET: Clients/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return NotFound();
        // Split merged address for UI fields
        if (!string.IsNullOrEmpty(client.Address))
        {
            var parts = client.Address.Split('|');
            if (parts.Length >= 3)
            {
                client.StreetVillageBarangay = parts[0];
                client.City = parts[1];
                client.Province = parts[2];
            }
        }
        
        return View(client);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,MiddleName,LastName,Suffix,StreetVillageBarangay,City,Province,Phone")] Client client)
    {
        if (id != client.Id) return NotFound();
        if (ModelState.IsValid)
        {
            try {
                client.Address = $"{client.StreetVillageBarangay}|{client.City}|{client.Province}";
                _context.Update(client);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Client '{client.Name}' updated successfully.";
            } catch (DbUpdateConcurrencyException) {
                if (!await _context.Clients.AnyAsync(c => c.Id == client.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(client);
    }

    // GET: Clients/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var client = await _context.Clients
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (client == null) return NotFound();
        return View(client);
    }

    // POST: Clients/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = await _context.Clients
            .Include(c => c.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id);
            
        if (client != null)
        {
            if (client.Bookings.Any())
            {
                TempData["Error"] = $"Cannot delete client '{client.Name}' because they have {client.Bookings.Count} booking(s). Delete the bookings first.";
                return RedirectToAction(nameof(Index));
            }
            
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Client '{client.Name}' deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }
}
