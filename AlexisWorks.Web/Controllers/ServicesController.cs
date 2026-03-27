using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlexisWorks.Web.Data;
using AlexisWorks.Web.Models;

namespace AlexisWorks.Web.Controllers;

public class ServicesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Services
    public async Task<IActionResult> Index()
    {
        var services = await _context.Services.OrderBy(s => s.Name).ToListAsync();
        return View(services);
    }

    // GET: Services/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Services/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,HourlyRate")] Service service)
    {
        if (ModelState.IsValid)
        {
            _context.Add(service);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Service '{service.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(service);
    }

    // GET: Services/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var service = await _context.Services.FindAsync(id);
        if (service == null) return NotFound();
        return View(service);
    }

    // POST: Services/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,HourlyRate")] Service service)
    {
        if (id != service.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(service);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Service '{service.Name}' updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Services.AnyAsync(s => s.Id == service.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(service);
    }

    // GET: Services/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
        if (service == null) return NotFound();
        return View(service);
    }

    // POST: Services/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service != null)
        {
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Service '{service.Name}' deleted successfully.";
        }
        return RedirectToAction(nameof(Index));
    }
}
