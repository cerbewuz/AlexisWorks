using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlexisWorks.Web.Data;
using AlexisWorks.Web.Models;

namespace AlexisWorks.Web.Controllers;

public class ToolsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ToolsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Tools
    public async Task<IActionResult> Index()
    {
        var tools = await _context.Tools
            .Include(t => t.Service)
            .OrderBy(t => t.Name)
            .ToListAsync();
        return View(tools);
    }

    // GET: Tools/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Services = new SelectList(await _context.Services.OrderBy(s => s.Name).ToListAsync(), "Id", "Name");
        return View();
    }

    // POST: Tools/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,ServiceId")] Tool tool)
    {
        ModelState.Remove("Service");
        if (ModelState.IsValid)
        {
            _context.Add(tool);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Tool '{tool.Name}' added to inventory.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Services = new SelectList(await _context.Services.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", tool.ServiceId);
        return View(tool);
    }

    // GET: Tools/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var tool = await _context.Tools.FindAsync(id);
        if (tool == null) return NotFound();
        ViewBag.Services = new SelectList(await _context.Services.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", tool.ServiceId);
        return PartialView("_Edit", tool);
    }

    // POST: Tools/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ServiceId")] Tool tool)
    {
        if (id != tool.Id) return NotFound();
        ModelState.Remove("Service");
        if (ModelState.IsValid)
        {
            try {
                _context.Update(tool);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Tool '{tool.Name}' updated successfully.";
            } catch (DbUpdateConcurrencyException) {
                if (!await _context.Tools.AnyAsync(t => t.Id == tool.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Services = new SelectList(await _context.Services.OrderBy(s => s.Name).ToListAsync(), "Id", "Name", tool.ServiceId);
        return PartialView("_Edit", tool);
    }

    // GET: Tools/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var tool = await _context.Tools.Include(t => t.Service).FirstOrDefaultAsync(t => t.Id == id);
        if (tool == null) return NotFound();
        return View(tool);
    }

    // POST: Tools/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var tool = await _context.Tools.FindAsync(id);
        if (tool != null)
        {
            _context.Tools.Remove(tool);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Tool '{tool.Name}' removed from inventory.";
        }
        return RedirectToAction(nameof(Index));
    }
}
