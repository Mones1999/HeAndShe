using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;

namespace ProjectMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ModelContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment; // Declare Var
        public ProductsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Products.Include(p => p.Category).Include(p => p.Coupon);
            return View(await modelContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Coupon)
                .FirstOrDefaultAsync(m => m.Productid == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["Categoryid"] = new SelectList(_context.Categories, "Categoryid", "Categoryid");
            ViewData["Couponid"] = new SelectList(_context.Discountcoupons, "Couponid", "Couponid");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Productid,Productname,Categoryid,Description,Price,Stockquantity,Status,ImageFile,Rating,Numberofreviews,Productsize,Couponid")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {

                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.Imageurl = fileName;

                }


                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categoryid"] = new SelectList(_context.Categories, "Categoryid", "Categoryid", product.Categoryid);
            ViewData["Couponid"] = new SelectList(_context.Discountcoupons, "Couponid", "Couponid", product.Couponid);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["Categoryid"] = new SelectList(_context.Categories, "Categoryid", "Categoryid", product.Categoryid);
            ViewData["Couponid"] = new SelectList(_context.Discountcoupons, "Couponid", "Couponid", product.Couponid);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Productid,Productname,Categoryid,Description,Price,Stockquantity,Status,ImageFile,Rating,Numberofreviews,Productsize,Couponid")] Product product)
        {
            if (id != product.Productid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					if (product.ImageFile != null)
					{

						string wwwRootPath = _webHostEnvironment.WebRootPath;
						string fileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
						string path = Path.Combine(wwwRootPath + "/Images/", fileName);
						using (var fileStream = new FileStream(path, FileMode.Create))
						{
							await product.ImageFile.CopyToAsync(fileStream);
						}
						product.Imageurl = fileName;

					}
					_context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Productid))
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
            ViewData["Categoryid"] = new SelectList(_context.Categories, "Categoryid", "Categoryid", product.Categoryid);
            ViewData["Couponid"] = new SelectList(_context.Discountcoupons, "Couponid", "Couponid", product.Couponid);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Coupon)
                .FirstOrDefaultAsync(m => m.Productid == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ModelContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(decimal id)
        {
          return (_context.Products?.Any(e => e.Productid == id)).GetValueOrDefault();
        }
    }
}
