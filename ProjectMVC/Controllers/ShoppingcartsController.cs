using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;

namespace ProjectMVC.Controllers
{
    public class ShoppingcartsController : Controller
    {
        private readonly ModelContext _context;

        public ShoppingcartsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Shoppingcarts
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Shoppingcarts.Include(s => s.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Shoppingcarts/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Shoppingcarts == null)
            {
                return NotFound();
            }

            var shoppingcart = await _context.Shoppingcarts
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Cartid == id);
            if (shoppingcart == null)
            {
                return NotFound();
            }

            return View(shoppingcart);
        }

        // GET: Shoppingcarts/Create
        public IActionResult Create()
        {
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid");
            return View();
        }

        // POST: Shoppingcarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cartid,Userid,Productid,Quantity")] Shoppingcart shoppingcart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingcart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", shoppingcart.Userid);
            return View(shoppingcart);
        }

        // GET: Shoppingcarts/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Shoppingcarts == null)
            {
                return NotFound();
            }

            var shoppingcart = await _context.Shoppingcarts.FindAsync(id);
            if (shoppingcart == null)
            {
                return NotFound();
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", shoppingcart.Userid);
            return View(shoppingcart);
        }

        // POST: Shoppingcarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Cartid,Userid,Productid,Quantity")] Shoppingcart shoppingcart)
        {
            if (id != shoppingcart.Cartid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingcart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingcartExists(shoppingcart.Cartid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", shoppingcart.Userid);
            return View(shoppingcart);
        }

        // GET: Shoppingcarts/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Shoppingcarts == null)
            {
                return NotFound();
            }

            var shoppingcart = await _context.Shoppingcarts
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Cartid == id);
            if (shoppingcart == null)
            {
                return NotFound();
            }

            return View(shoppingcart);
        }

        // POST: Shoppingcarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Shoppingcarts == null)
            {
                return Problem("Entity set 'ModelContext.Shoppingcarts'  is null.");
            }
            var shoppingcart = await _context.Shoppingcarts.FindAsync(id);
            if (shoppingcart != null)
            {
                _context.Shoppingcarts.Remove(shoppingcart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingcartExists(decimal id)
        {
          return (_context.Shoppingcarts?.Any(e => e.Cartid == id)).GetValueOrDefault();
        }
    }
}
