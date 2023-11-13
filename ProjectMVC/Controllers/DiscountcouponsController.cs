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
    public class DiscountcouponsController : Controller
    {
        private readonly ModelContext _context;

        public DiscountcouponsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Discountcoupons
        public async Task<IActionResult> Index()
        {
              return _context.Discountcoupons != null ? 
                          View(await _context.Discountcoupons.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Discountcoupons'  is null.");
        }

        // GET: Discountcoupons/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Discountcoupons == null)
            {
                return NotFound();
            }

            var discountcoupon = await _context.Discountcoupons
                .FirstOrDefaultAsync(m => m.Couponid == id);
            if (discountcoupon == null)
            {
                return NotFound();
            }

            return View(discountcoupon);
        }

        // GET: Discountcoupons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discountcoupons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Couponid,Couponcode,Discountpercentage,Expirationdate")] Discountcoupon discountcoupon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discountcoupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discountcoupon);
        }

        // GET: Discountcoupons/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Discountcoupons == null)
            {
                return NotFound();
            }

            var discountcoupon = await _context.Discountcoupons.FindAsync(id);
            if (discountcoupon == null)
            {
                return NotFound();
            }
            return View(discountcoupon);
        }

        // POST: Discountcoupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Couponid,Couponcode,Discountpercentage,Expirationdate")] Discountcoupon discountcoupon)
        {
            if (id != discountcoupon.Couponid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discountcoupon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountcouponExists(discountcoupon.Couponid))
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
            return View(discountcoupon);
        }

        // GET: Discountcoupons/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Discountcoupons == null)
            {
                return NotFound();
            }

            var discountcoupon = await _context.Discountcoupons
                .FirstOrDefaultAsync(m => m.Couponid == id);
            if (discountcoupon == null)
            {
                return NotFound();
            }

            return View(discountcoupon);
        }

        // POST: Discountcoupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Discountcoupons == null)
            {
                return Problem("Entity set 'ModelContext.Discountcoupons'  is null.");
            }
            var discountcoupon = await _context.Discountcoupons.FindAsync(id);
            if (discountcoupon != null)
            {
                _context.Discountcoupons.Remove(discountcoupon);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountcouponExists(decimal id)
        {
          return (_context.Discountcoupons?.Any(e => e.Couponid == id)).GetValueOrDefault();
        }
    }
}
