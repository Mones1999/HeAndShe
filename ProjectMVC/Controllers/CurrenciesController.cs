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
    public class CurrenciesController : Controller
    {
        private readonly ModelContext _context;

        public CurrenciesController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SetCurrencyInSession(string currencyCode)
        {
            HttpContext.Session.SetString("UserCurrency", currencyCode);

            // Get user by id
            var id = HttpContext.Session.GetInt32("UserId");

            //if (id != null) 
            //{
            //    var user = _context.Users.Find((decimal)id);

            //    // Get Currency by Currencycode and set it for user
            //    var currency = _context.Currencies.SingleOrDefault(c => c.Currencycode == currencyCode);
            //    //user.Currencyid = currency.Currencyid;
            //    _context.Update(user);
            //    await _context.SaveChangesAsync();
            //}


            // Redirect to the same page the user was on
            var referrer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referrer))
            {
                return Redirect(referrer);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }


        // GET: Currencies
        public async Task<IActionResult> Index()
        {
              return _context.Currencies != null ? 
                          View(await _context.Currencies.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Currencies'  is null.");
        }

        // GET: Currencies/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Currencies == null)
            {
                return NotFound();
            }

            var currency = await _context.Currencies
                .FirstOrDefaultAsync(m => m.Currencyid == id);
            if (currency == null)
            {
                return NotFound();
            }

            return View(currency);
        }

        // GET: Currencies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Currencies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Currencyid,Currencycode,Currencyname,Exchangetobase")] Currency currency)
        {
            if (ModelState.IsValid)
            {
                _context.Add(currency);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(currency);
        }

        // GET: Currencies/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Currencies == null)
            {
                return NotFound();
            }

            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }
            return View(currency);
        }

        // POST: Currencies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Currencyid,Currencycode,Currencyname,Exchangetobase")] Currency currency)
        {
            if (id != currency.Currencyid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(currency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CurrencyExists(currency.Currencyid))
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
            return View(currency);
        }

        // GET: Currencies/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Currencies == null)
            {
                return NotFound();
            }

            var currency = await _context.Currencies
                .FirstOrDefaultAsync(m => m.Currencyid == id);
            if (currency == null)
            {
                return NotFound();
            }

            return View(currency);
        }

        // POST: Currencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Currencies == null)
            {
                return Problem("Entity set 'ModelContext.Currencies'  is null.");
            }
            var currency = await _context.Currencies.FindAsync(id);
            if (currency != null)
            {
                _context.Currencies.Remove(currency);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CurrencyExists(decimal id)
        {
          return (_context.Currencies?.Any(e => e.Currencyid == id)).GetValueOrDefault();
        }
    }
}
