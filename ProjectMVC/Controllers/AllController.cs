using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;
using ProjectMVC.Services;
using System.Linq;

namespace ProjectMVC.Controllers
{
    public class AllController : Controller
    {
        private readonly ModelContext _context;
        private readonly CurrencyService _currencyService;

        public AllController(ModelContext context, CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }
        private IQueryable<Product> GetProducts(decimal? gender = null)
        {
            if (gender != null)
            {
                // Get All Products By Gender
                var products = _context.Products.Include(p => p.Category).Where(p => p.Category.Genderid == gender);
                return products;
            }
            else
            {
                // Get All Products By Category
                var products = _context.Products;
                return products;
            }
        }
        public IActionResult Index(decimal? gender = null, int pg = 1)
        {
            var AllProducts = GetProducts(gender);

            if (gender != null) 
            { 
                ViewBag.Genderid = gender;
            }

            // Set Currency 
            AllProducts = _currencyService.ConvertToSelectedCurrency(AllProducts);
            ViewBag.CurrencyCode = _currencyService.GetCurrentCurrencyCode();

            // Pagination
            const int pageSize = 9;
            if (pg < 1)
                pg = 1;

            int proCount = AllProducts.Count();
            var pager = new Pager(proCount, pg, pageSize);
            int proSkip = (pg - 1) * pageSize;
            var data = AllProducts.Skip(proSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;

            var cat = _context.Categories.ToList();
            var ProductsCat = Tuple.Create(data, cat);

            return View(ProductsCat);
        }

        public async Task<IActionResult> FilterAllProducts(decimal? priceTo = 500, List<string>? sizes = null, List<decimal?>? category = null,string? searchInput = null, int pg = 1) 
        {

            var Products = (IQueryable<Product>)_context.Products;

            Products = _currencyService.ConvertToSelectedCurrency(Products);
            ViewBag.CurrencyCode = _currencyService.GetCurrentCurrencyCode();

            // Filter by name if any
            if (searchInput != null && searchInput.Any())
            {
                Products = Products.Where(p => p.Productname.Contains(searchInput));
            }

            // Filter by selected category if any
            if (category != null && category.Any())
            {
                Products = Products.Where(p => category.Contains(p.Categoryid)); 
            }

            // Filter by selected sizes if any
            if (sizes != null && sizes.Any())
            {
                Products = Products.Where(p => sizes.Contains(p.Productsize)); 
            }

            // Filter by price
            if (priceTo.HasValue)
            {
                Products = Products.Where(p => p.Price <= priceTo.Value);
            }

            // Pagination
            const int pageSize = 9;
            if (pg < 1)
                pg = 1;

            int proCount = Products.Count();
            var pager = new Pager(proCount, pg, pageSize);
            int proSkip = (pg - 1) * pageSize;
            var data = Products.Skip(proSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;

            var cat = _context.Categories.Include(c=> c.Products).ToList();
            var ProductsCat = Tuple.Create(data, cat);
            return View(ProductsCat);
        }
    }
}
